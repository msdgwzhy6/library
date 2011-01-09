using System;
using System.Collections;


public class OctavePyramid
{
	public OctavePyramid ()
	{
	}

	bool verbose = true;
	public bool Verbose {
		set {
			verbose = value;
		}
	}

	ArrayList octaves;
	public int Count {
		get {
			return (octaves.Count);
		}
	}
	public DScaleSpace this[int idx] {
		get {
			return ((DScaleSpace) octaves[idx]);
		}
	}

	public int BuildOctaves (ImageMap source, double scale,
		int levelsPerOctave, double octaveSigma, int minSize)
	{
		octaves = new ArrayList ();

		DScaleSpace downSpace = null;
		ImageMap prev = source;

		while (prev != null && prev.XDim >= minSize && prev.YDim >= minSize) {
			DScaleSpace dsp = new DScaleSpace ();
			dsp.Verbose = verbose;

			if (verbose)
				Console.WriteLine ("Building octave, ({0}, {1})", prev.XDim, prev.YDim);

			dsp.BuildGaussianMaps (prev, scale, levelsPerOctave, octaveSigma);
			dsp.BuildDiffMaps ();

			octaves.Add (dsp);

			prev = dsp.LastGaussianMap.ScaleHalf ();

			if (downSpace != null)
				downSpace.Up = dsp;

			dsp.Down = downSpace;
			downSpace = dsp;

			scale *= 2.0;
		}

		return (octaves.Count);
	}
}

public class DScaleSpace
{
	bool verbose = true;
	public bool Verbose {
		set {
			verbose = value;
		}
	}

	DScaleSpace down = null;
	DScaleSpace up = null;
	internal DScaleSpace Down {
		get {
			return (down);
		}
		set {
			down = value;
		}
	}
	internal DScaleSpace Up {
		get {
			return (up);
		}
		set {
			up = value;
		}
	}

	ImageMap baseImg;
	double basePixScale;

	ImageMap[] imgScaled;
	public ImageMap GetGaussianMap (int idx)
	{
		return (imgScaled[idx]);
	}

	private ImageMap[] magnitudes;
	private ImageMap[] directions;

	public ImageMap LastGaussianMap {
		get {
			if (imgScaled.Length < 2)
				throw (new Exception ("bu keneng: too few gaussian maps"));

			return (imgScaled[imgScaled.Length - 2]);
		}
	}

	ImageMap[] spaces;

	public DScaleSpace ()
	{
	}

	public int Count {
		get {
			return (spaces.Length);
		}
	}

	public ImageMap this[int idx] {
		get {
			return (spaces[idx]);
		}
	}

	public ArrayList GenerateKeypoints (ArrayList localizedPeaks,
		int scaleCount, double octaveSigma)
	{
		ArrayList keypoints = new ArrayList ();

		foreach (ScalePoint sp in localizedPeaks) {
			ArrayList thisPointKeys = GenerateKeypointSingle (basePixScale,
				sp, 36, 0.8, scaleCount, octaveSigma);

			thisPointKeys = CreateDescriptors (thisPointKeys,
				magnitudes[sp.Level], directions[sp.Level], 2.0, 4, 8, 0.2);

			foreach (Keypoint kp in thisPointKeys) {
				if (kp.HasFV == false)
					throw (new Exception ("should not happen"));

				kp.X *= kp.ImgScale;
				kp.Y *= kp.ImgScale;
				kp.Scale *= kp.ImgScale;

				keypoints.Add (kp);
			}
		}

		return (keypoints);
	}

	private ArrayList GenerateKeypointSingle (double imgScale, ScalePoint point,
		int binCount, double peakRelThresh, int scaleCount,
		double octaveSigma)
	{
		double kpScale = octaveSigma *
			Math.Pow (2.0, (point.Level + point.Local.ScaleAdjust) / scaleCount);

		double sigma = 3.0 * kpScale;
		int radius = (int) (3.0 * sigma / 2.0 + 0.5);
		int radiusSq = radius * radius;

		ImageMap magnitude = magnitudes[point.Level];
		ImageMap direction = directions[point.Level];

		int xMin = Math.Max (point.X - radius, 1);
		int xMax = Math.Min (point.X + radius, magnitude.XDim - 1);
		int yMin = Math.Max (point.Y - radius, 1);
		int yMax = Math.Min (point.Y + radius, magnitude.YDim - 1);

		double gaussianSigmaFactor = 2.0 * sigma * sigma;

		double[] bins = new double[binCount];

		for (int y = yMin ; y < yMax ; ++y) {
			for (int x = xMin ; x < xMax ; ++x) {
				int relX = x - point.X;
				int relY = y - point.Y;
				if (IsInCircle (relX, relY, radiusSq) == false)
					continue;

				double gaussianWeight = Math.Exp
					(- ((relX * relX + relY * relY) / gaussianSigmaFactor));

				int binIdx = FindClosestRotationBin (binCount, direction[x, y]);
				bins[binIdx] += magnitude[x, y] * gaussianWeight;
			}
		}


		AverageWeakBins (bins, binCount);

		double maxGrad = 0.0;
		int maxBin = 0;
		for (int b = 0 ; b < binCount ; ++b) {
			if (bins[b] > maxGrad) {
				maxGrad = bins[b];
				maxBin = b;
			}
		}

		double maxPeakValue, maxDegreeCorrection;
		InterpolateOrientation (bins[maxBin == 0 ? (binCount - 1) : (maxBin - 1)],
			bins[maxBin], bins[(maxBin + 1) % binCount],
			out maxDegreeCorrection, out maxPeakValue);

		bool[] binIsKeypoint = new bool[binCount];
		for (int b = 0 ; b < binCount ; ++b) {
			binIsKeypoint[b] = false;

			if (b == maxBin) {
				binIsKeypoint[b] = true;
				continue;
			}

			if (bins[b] < (peakRelThresh * maxPeakValue))
				continue;

			int leftI = (b == 0) ? (binCount - 1) : (b - 1);
			int rightI = (b + 1) % binCount;
			if (bins[b] <= bins[leftI] || bins[b] <= bins[rightI])
				continue;	

			binIsKeypoint[b] = true;
		}

		ArrayList keypoints = new ArrayList ();

		double oneBinRad = (2.0 * Math.PI) / binCount;

		for (int b = 0 ; b < binCount ; ++b) {
			if (binIsKeypoint[b] == false)
				continue;

			int bLeft = (b == 0) ? (binCount - 1) : (b - 1);
			int bRight = (b + 1) % binCount;

			double peakValue;
			double degreeCorrection;

			if (InterpolateOrientation (bins[bLeft], bins[b], bins[bRight],
				out degreeCorrection, out peakValue) == false)
			{
				throw (new InvalidOperationException ("BUG: Parabola fitting broken"));
			}


			double degree = (b + degreeCorrection) * oneBinRad - Math.PI;

			if (degree < -Math.PI)
				degree += 2.0 * Math.PI;
			else if (degree > Math.PI)
				degree -= 2.0 * Math.PI;

			Keypoint kp = new Keypoint (imgScaled[point.Level],
				point.X + point.Local.FineX,
				point.Y + point.Local.FineY,
				imgScale, kpScale, degree);
			keypoints.Add (kp);
		}

		return (keypoints);
	}


	private bool InterpolateOrientation (double left, double middle,
		double right, out double degreeCorrection, out double peakValue)
	{
		double a = ((left + right) - 2.0 * middle) / 2.0;
		degreeCorrection = peakValue = Double.NaN;

		if (a == 0.0)
			return (false);

		double c = (((left - middle) / a) - 1.0) / 2.0;
		double b = middle - c * c * a;

		if (c < -0.5 || c > 0.5)
			throw (new InvalidOperationException
				("InterpolateOrientation: off peak ]-0.5 ; 0.5["));

		degreeCorrection = c;
		peakValue = b;

		return (true);
	}

	private int FindClosestRotationBin (int binCount, double angle)
	{
		angle += Math.PI;
		angle /= 2.0 * Math.PI;

		angle *= binCount;

		int idx = (int) angle;
		if (idx == binCount)
			idx = 0;

		return (idx);
	}

	private void AverageWeakBins (double[] bins, int binCount)
	{
		for (int sn = 0 ; sn < 4 ; ++sn) {
			double firstE = bins[0];
			double last = bins[binCount - 1];

			for (int sw = 0 ; sw < binCount ; ++sw) {
				double cur = bins[sw];
				double next = (sw == (binCount - 1)) ?
					firstE : bins[(sw + 1) % binCount];

				bins[sw] = (last + cur + next) / 3.0;
				last = cur;
			}
		}
	}

	private ArrayList CreateDescriptors (ArrayList keypoints,
		ImageMap magnitude, ImageMap direction,
		double considerScaleFactor, int descDim, int directionCount,
		double fvGradHicap)
	{
		if (keypoints.Count <= 0)
			return (keypoints);

		considerScaleFactor *= ((Keypoint) keypoints[0]).Scale;
		double dDim05 = ((double) descDim) / 2.0;

		int radius = (int) (((descDim + 1.0) / 2) *
			Math.Sqrt (2.0) * considerScaleFactor + 0.5);

		ArrayList survivors = new ArrayList ();

		double sigma2Sq = 2.0 * dDim05 * dDim05;

		foreach (Keypoint kp in keypoints)
		{
			double angle = -kp.Orientation;

			kp.CreateVector (descDim, descDim, directionCount);
			for (int y = -radius ; y < radius ; ++y) {
				for (int x = -radius ; x < radius ; ++x) {
					double yR = Math.Sin (angle) * x +
						Math.Cos (angle) * y;
					double xR = Math.Cos (angle) * x -
						Math.Sin (angle) * y;
                    
					yR /= considerScaleFactor; 
					xR /= considerScaleFactor; 

					if (yR >= (dDim05 + 0.5) || xR >= (dDim05 + 0.5) ||
						xR <= -(dDim05 + 0.5) || yR <= -(dDim05 + 0.5))
						continue;
					int currentX = (int) (x + kp.X + 0.5);
					int currentY = (int) (y + kp.Y + 0.5);
					if (currentX < 1 || currentX >= (magnitude.XDim - 1) ||
						currentY < 1 || currentY >= (magnitude.YDim - 1))
						continue;

					double magW = Math.Exp (-(xR * xR + yR * yR) / sigma2Sq) *
						magnitude[currentX, currentY];

					yR += dDim05 - 0.5;
					xR += dDim05 - 0.5;

					int[] xIdx = new int[2];
					int[] yIdx = new int[2];
					int[] dirIdx = new int[2];
					double[] xWeight = new double[2];
					double[] yWeight = new double[2];
					double[] dirWeight = new double[2];

					if (xR >= 0) {
						xIdx[0] = (int) xR;
						xWeight[0] = (1.0 - (xR - xIdx[0]));
					}
					if (yR >= 0) {
						yIdx[0] = (int) yR;
						yWeight[0] = (1.0 - (yR - yIdx[0]));
					}

					if (xR < (descDim - 1)) {
						xIdx[1] = (int) (xR + 1.0);
						xWeight[1] = xR - xIdx[1] + 1.0;
					}
					if (yR < (descDim - 1)) {
						yIdx[1] = (int) (yR + 1.0);
						yWeight[1] = yR - yIdx[1] + 1.0;
					}

					double dir = direction[currentX, currentY] - kp.Orientation;
					if (dir <= -Math.PI)
						dir += Math.PI;
					if (dir > Math.PI)
						dir -= Math.PI;

					double idxDir = (dir * directionCount) /
						(2.0 * Math.PI);
					if (idxDir < 0.0)
						idxDir += directionCount;

					dirIdx[0] = (int) idxDir;
					dirIdx[1] = (dirIdx[0] + 1) % directionCount; 
					dirWeight[0] = 1.0 - (idxDir - dirIdx[0]);   
					dirWeight[1] = idxDir - dirIdx[0];           

					for (int iy = 0 ; iy < 2 ; ++iy) {
						for (int ix = 0 ; ix < 2 ; ++ix) {
							for (int id = 0 ; id < 2 ; ++id) {
								kp.FVSet (xIdx[ix], yIdx[iy], dirIdx[id],
									kp.FVGet (xIdx[ix], yIdx[iy], dirIdx[id]) +
									xWeight[ix] * yWeight[iy] * dirWeight[id] * magW);
							}
						}
					}
				}
			}

			CapAndNormalizeFV (kp, fvGradHicap);

			survivors.Add (kp);
		}

		return (survivors);
	}

	private void CapAndNormalizeFV (Keypoint kp, double fvGradHicap)
	{
		double norm = 0.0;
		for (int n = 0 ; n < kp.FVLinearDim ; ++n)
			norm += Math.Pow (kp.FVLinearGet (n), 2.0);

		norm = Math.Sqrt (norm);
		if (norm == 0.0)
			throw (new InvalidOperationException
				("CapAndNormalizeFV cannot normalize with norm = 0.0"));

		for (int n = 0 ; n < kp.FVLinearDim ; ++n)
			kp.FVLinearSet (n, kp.FVLinearGet (n) / norm);

		for (int n = 0 ; n < kp.FVLinearDim ; ++n) {
			if (kp.FVLinearGet (n) > fvGradHicap) {
				kp.FVLinearSet (n, fvGradHicap);
			}
		}

		norm = 0.0;
		for (int n = 0 ; n < kp.FVLinearDim ; ++n)
			norm += Math.Pow (kp.FVLinearGet (n), 2.0);

		norm = Math.Sqrt (norm);

		for (int n = 0 ; n < kp.FVLinearDim ; ++n)
			kp.FVLinearSet (n, kp.FVLinearGet (n) / norm);
	}


	private bool IsInCircle (int rX, int rY, int radiusSq)
	{
		rX *= rX;
		rY *= rY;
		if ((rX + rY) <= radiusSq)
			return (true);

		return (false);
	}

	public ArrayList FilterAndLocalizePeaks (ArrayList peaks, double edgeRatio,
		double dValueLoThresh, double scaleAdjustThresh, int relocationMaximum)
	{
		ArrayList filtered = new ArrayList ();

		int[,] processedMap = new int[spaces[0].XDim, spaces[0].YDim];

		foreach (ScalePoint peak in peaks) {
			if (IsTooEdgelike (spaces[peak.Level], peak.X, peak.Y, edgeRatio))
				continue;

			if (LocalizeIsWeak (peak, relocationMaximum, processedMap))
				continue;

			if (Math.Abs (peak.Local.ScaleAdjust) > scaleAdjustThresh)
				continue;

			if (Math.Abs (peak.Local.DValue) <= dValueLoThresh)
				continue;

			filtered.Add (peak);
		}

		return (filtered);
	}

	private bool LocalizeIsWeak (ScalePoint point, int steps, int[,] processed)
	{
		bool needToAdjust = true;
		int adjusted = steps;

		while (needToAdjust) {
			int x = point.X;
			int y = point.Y;

			if (point.Level <= 0 || point.Level >= (spaces.Length - 1))
				return (true);

			ImageMap space = spaces[point.Level];
			if (x <= 0 || x >= (space.XDim - 1))
				return (true);
			if (y <= 0 || y >= (space.YDim - 1))
				return (true);

			double dp;
			SimpleMatrix adj = GetAdjustment (point, point.Level, x, y, out dp);

			double adjS = adj[0, 0];
			double adjY = adj[1, 0];
			double adjX = adj[2, 0];
			if (Math.Abs (adjX) > 0.5 || Math.Abs (adjY) > 0.5) {
				if (adjusted == 0) {
					return (true);
				}

				adjusted -= 1;

				double distSq = adjX * adjX + adjY * adjY;
				if (distSq > 2.0)
					return (true);

				point.X = (int) (point.X + adjX + 0.5);
				point.Y = (int) (point.Y + adjY + 0.5);

				continue;
			}

			if (processed[point.X, point.Y] != 0)
				return (true);

			processed[point.X, point.Y] = 1;

			PointLocalInformation local = new PointLocalInformation (adjS, adjX, adjY);

			local.DValue = space[point.X, point.Y] + 0.5 * dp;
			point.Local = local;

			needToAdjust = false;
		}

		return (false);
	}

	private bool IsTooEdgelike (ImageMap space, int x, int y, double r)
	{
		double D_xx, D_yy, D_xy;

        D_xx = space[x + 1, y] + space[x - 1, y] - 2.0 * space[x, y];
		D_yy = space[x, y + 1] + space[x, y - 1] - 2.0 * space[x, y];
		D_xy = 0.25 * ((space[x + 1, y + 1] - space[x + 1, y - 1]) -
			(space[x - 1, y + 1] - space[x - 1, y - 1]));

		double TrHsq = D_xx + D_yy;
		TrHsq *= TrHsq;
		double DetH = D_xx * D_yy - (D_xy * D_xy);

		double r1sq = (r + 1.0);
		r1sq *= r1sq;

		if ((TrHsq / DetH) < (r1sq / r)) {

			return (false);
		}

		return (true);
	}

	private SimpleMatrix GetAdjustment (ScalePoint point,
		int level, int x, int y, out double dp)
	{
		dp = 0.0;
		if (point.Level <= 0 || point.Level >= (spaces.Length - 1))
			throw (new ArgumentException ("point.Level is not within [bottom-1;top-1] range"));

		ImageMap below = spaces[level - 1];
		ImageMap current = spaces[level];
		ImageMap above = spaces[level + 1];

		SimpleMatrix H = new SimpleMatrix (3, 3);

        H[0, 0] = below[x, y] - 2 * current[x, y] + above[x, y];
		H[0, 1] = H[1, 0] = 0.25 * (above[x, y + 1] - above[x, y - 1] -
			(below[x, y + 1] - below[x, y - 1]));
		H[0, 2] = H[2, 0] = 0.25 * (above[x + 1, y] - above[x - 1, y] -
			(below[x + 1, y] - below[x - 1, y]));
		H[1, 1] = current[x, y - 1] - 2 * current[x, y] + current[x, y + 1];
		H[1, 2] = H[2, 1] = 0.25 * (current[x + 1, y + 1] - current[x - 1, y + 1] -
			(current[x + 1, y - 1] - current[x - 1, y - 1]));
		H[2, 2] = current[x - 1, y] - 2 * current[x, y] + current[x + 1, y];

		SimpleMatrix d = new SimpleMatrix (3, 1);

        d[0, 0] = 0.5 * (above[x, y] - below[x, y]);
		d[1, 0] = 0.5 * (current[x, y + 1] - current[x, y - 1]);
		d[2, 0] = 0.5 * (current[x + 1, y] - current[x - 1, y]);

		SimpleMatrix b = (SimpleMatrix) d.Clone ();
		b.Negate ();

		H.SolveLinear (b);

		dp = b.Dot (d);

		return (b);
	}

	public ArrayList FindPeaks (double dogThresh)
	{
		if (verbose)
			Console.WriteLine ("  FindPeaks: scale {0:N2}, testing {1} levels",
				basePixScale, Count - 2);

		ArrayList peaks = new ArrayList ();

		ImageMap current, above, below;

		for (int level = 1 ; level < (Count - 1) ; ++level)
		{
			current = this[level];
			below = this[level - 1];
			above = this[level + 1];

			Console.WriteLine ("peak-search at level {0}", level);

			peaks.AddRange (FindPeaksThreeLevel (below, current, above,
				level, dogThresh));
			below = current;
		}

		return (peaks);
	}

	private ArrayList FindPeaksThreeLevel (ImageMap below, ImageMap current,
		ImageMap above, int curLev, double dogThresh)
	{
		ArrayList peaks = new ArrayList ();

		for (int y = 1 ; y < (current.YDim - 1) ; ++y) {
			for (int x = 1 ; x < (current.XDim - 1) ; ++x) {
				bool cIsMax = true;
				bool cIsMin = true;

				double c = current[x, y];	

				if (Math.Abs (c) <= dogThresh)
					continue;

				CheckMinMax (current, c, x, y, ref cIsMin, ref cIsMax, true);
				CheckMinMax (below, c, x, y, ref cIsMin, ref cIsMax, false);
				CheckMinMax (above, c, x, y, ref cIsMin, ref cIsMax, false);
				if (cIsMin == false && cIsMax == false)
					continue;

				peaks.Add (new ScalePoint (x, y, curLev));
			}
		}

		return (peaks);
	}

	private void CheckMinMax (ImageMap layer, double c, int x, int y,
		ref bool IsMin, ref bool IsMax, bool cLayer)
	{
		if (layer == null)
			return;

		if (IsMin == true) {
			if (layer[x - 1, y - 1] <= c ||
				layer[x, y - 1] <= c ||
				layer[x + 1, y - 1] <= c ||
				layer[x - 1, y] <= c ||
				(cLayer ? false : (layer[x, y] < c)) ||
				layer[x + 1, y] <= c ||
				layer[x - 1, y + 1] <= c ||
				layer[x, y + 1] <= c ||
				layer[x + 1, y + 1] <= c)
				IsMin = false;
		}
		if (IsMax == true) {
			if (layer[x - 1, y - 1] >= c ||
				layer[x, y - 1] >= c ||
				layer[x + 1, y - 1] >= c ||
				layer[x - 1, y] >= c ||
				(cLayer ? false : (layer[x, y] > c)) ||
				layer[x + 1, y] >= c ||
				layer[x - 1, y + 1] >= c ||
				layer[x, y + 1] >= c ||
				layer[x + 1, y + 1] >= c)
				IsMax = false;
		}
	}

	static public double SToK (int s)
	{
		return (Math.Pow (2.0, 1.0 / s));
	}

	public void GenerateMagnitudeAndDirectionMaps ()
	{

		magnitudes = new ImageMap[Count - 1];
		directions = new ImageMap[Count - 1];

		for (int s = 1 ; s < (Count - 1) ; ++s) {
			magnitudes[s] = new ImageMap (imgScaled[s].XDim, imgScaled[s].YDim);
			directions[s] = new ImageMap (imgScaled[s].XDim, imgScaled[s].YDim);

			for (int y = 1 ; y < (imgScaled[s].YDim - 1) ; ++y) {
				for (int x = 1 ; x < (imgScaled[s].XDim - 1) ; ++x) {
					magnitudes[s][x, y] = Math.Sqrt (
						Math.Pow (imgScaled[s][x + 1, y] -
							imgScaled[s][x - 1, y], 2.0) +
						Math.Pow (imgScaled[s][x, y + 1] -
							imgScaled[s][x, y - 1], 2.0));

					directions[s][x, y] = Math.Atan2
						(imgScaled[s][x, y + 1] - imgScaled[s][x, y - 1],
						imgScaled[s][x + 1, y] - imgScaled[s][x - 1, y]);
				}
			}
		}
	}

	public void ClearMagnitudeAndDirectionMaps ()
	{
		magnitudes = directions = null;
	}

	public void BuildDiffMaps ()
	{
		spaces = new ImageMap[imgScaled.Length - 1];

		for (int sn = 0 ; sn < spaces.Length ; ++sn) {
			spaces[sn] = imgScaled[sn + 1] - imgScaled[sn];
		}
	}

	public void BuildGaussianMaps (ImageMap first, double firstScale,
		int scales, double sigma)
	{
		imgScaled = new ImageMap[scales + 1 + 1 + 1];
		this.basePixScale = firstScale;

		ImageMap prev = first;
		imgScaled[0] = first;

		double w = sigma;
		double kTerm = Math.Sqrt (Math.Pow (SToK (scales), 2.0) - 1.0);
		for (int scI = 1 ; scI < imgScaled.Length ; ++scI) {
			GaussianConvolution gauss = new GaussianConvolution (w * kTerm);
			prev = imgScaled[scI] = gauss.Convolve (prev);
			w *= SToK (scales);
		}

#if false
		for (int scI = 0 ; scI < imgScaled.Length ; ++scI) {
			imgScaled[scI].Save ("scale-" + scI + ".dat", null);
			if (scI < (imgScaled.Length - 1))
				(imgScaled[scI+1]-imgScaled[scI]).Save
					("scale-diff-" + scI + ".dat", null);
		}
		Console.WriteLine ("DEBUG EXIT");
#endif
	}

#if false
	// Incrementally blur the input image first so it reaches the next octave.
	public void BuildGaussianMaps (ImageMap first, double firstScale,
		int scales, double sigma)
	{
		// We need one more gaussian blurred image than the number of DoG
		// maps. But for the minima/maxima pixel search, we need two more. See
		// BuildDiffMaps.
		imgScaled = new ImageMap[scales + 1 + 1 + 1];
		this.basePixScale = firstScale;

		// Convolve first image with the octaveSigma. Previously we got this
		// wrong, but thanks to Alexandre Jenny, this got fixed. Thanks!
		GaussianConvolution gauss = new GaussianConvolution (sigma);
		ImageMap prev = imgScaled[0] = gauss.Convolve (first);

		// Ln1(x, y, k^{p+1}) = G(x, y, k) * Ln0(x, y, k^p).
		for (int scI = 1 ; scI < imgScaled.Length ; ++scI) {
			gauss = new GaussianConvolution (sigma);

			// TODO: real fix, "first" is the correct behaviour, however, a
			// large sigma leads to a large convolution kernel -> slow
			// better: incremental convolution with smaller sigma.
			prev = imgScaled[scI] = gauss.Convolve (first);
			sigma *= SToK (scales);
		}
	}
#endif

	

}


internal class ScalePoint
{
	int x, y;
	int level;
	public int X {
		get {
			return (x);
		}
		set {
			x = value;
		}
	}
	public int Y {
		get {
			return (y);
		}
		set {
			y = value;
		}
	}
	public int Level {
		get {
			return (level);
		}
		set {
			level = value;
		}
	}

	PointLocalInformation local;
	public PointLocalInformation Local {
		get {
			return (local);
		}
		set {
			local = value;
		}
	}

	private ScalePoint ()
	{
	}

	public ScalePoint (int x, int y, int level)
	{
		this.x = x;
		this.y = y;
		this.level = level;
	}
}

internal class PointLocalInformation
{
	double fineX, fineY;
	public double FineX {
		get {
			return (fineX);
		}
	}
	public double FineY {
		get {
			return (fineY);
		}
	}

	double scaleAdjust;
	public double ScaleAdjust {
		get {
			return (scaleAdjust);
		}
		set {
			scaleAdjust = value;
		}
	}

	double dValue;
	public double DValue {
		get {
			return (dValue);
		}
		set {
			dValue = value;
		}
	}

	private PointLocalInformation ()
	{
	}

	public PointLocalInformation (double fineS, double fineX, double fineY)
	{
		this.fineX = fineX;
		this.fineY = fineY;
		this.scaleAdjust = fineS;
	}
}





