using System;

public class GaussianConvolution
{
    ConvLinearMask mask;

	public GaussianConvolution (double sigma)
		: this (sigma, 1 + 2 * ((int) (3.0 * sigma)))
	{
	}


	public GaussianConvolution (double sigma, int dim)
	{
		dim |= 1;

		mask = new ConvLinearMask (dim);

		double sigma2sq = 2 * sigma * sigma;

		double normalizeFactor = 1.0 / (Math.Sqrt (2.0 * Math.PI) * sigma);

		for (int n = 0 ; n < dim ; ++n) {
			int relPos = n - mask.Middle;

			double G = (relPos * relPos) / sigma2sq;
			G = Math.Exp (-G);
			G *= normalizeFactor;
			mask[n] = G;
			mask.MaskSum += G;
		}
	}

	public ImageMap Convolve (ImageMap img)
	{
		return (ConvolutionFilter.Convolve (img, mask));
	}

}

public class ConvolutionFilter
{
	public static ImageMap Convolve (ImageMap img, ConvLinearMask mask)
	{
		ImageMap res = new ImageMap (img.XDim, img.YDim);
		ImageMap res2 = new ImageMap (img.XDim, img.YDim);

		Convolve1D (res, mask, img, Direction.Vertical);
		Convolve1D (res2, mask, res, Direction.Horizontal);

		return (res2);
	}

	public enum Direction {
		Vertical,
		Horizontal,
	};

	public static void Convolve1D (ImageMap dest, ConvLinearMask mask,
		ImageMap src, Direction dir)
	{
		int maxN;
		int maxP;

		if (dir == Direction.Vertical) {
			maxN = src.XDim;
			maxP = src.YDim;
		} else if (dir == Direction.Horizontal) {
			maxN = src.YDim;
			maxP = src.XDim;
		} else
			throw (new Exception ("TODO: invalid direction"));

		for (int n = 0 ; n < maxN ; ++n) {
			for (int p = 0 ; p < maxP ; ++p) {
				double val = ConvolutionFilter.CalculateConvolutionValue1D (src, mask,
					n, p, maxN, maxP, dir);

				if (dir == Direction.Vertical)
					dest[n, p] = val;
				else
					dest[p, n] = val;
			}
		}
	}

	internal static double CalculateConvolutionValue1D (ImageMap src,
		ConvLinearMask mask, int n, int p, int maxN, int maxP, Direction dir)
	{
		double sum = 0.0;

		bool isOut = false;
		double outBound = 0.0;	

		for (int xw = 0 ; xw < mask.Count ; ++xw) {
			int curAbsP = xw - mask.Middle + p;

			if (curAbsP < 0 || curAbsP >= maxP) {
				isOut = true;
				outBound += mask[xw];

				continue;
			}

			if (dir == Direction.Vertical)
				sum += mask[xw] * src[n, curAbsP];
			else
				sum += mask[xw] * src[curAbsP, n];
		}

		if (isOut)
			sum *= 1.0 / (1.0 - outBound);

		return (sum);
	}
}

public class ConvLinearMask
{
	int dim;
	public int Count {
		get {
			return (dim);
		}
	}

	int middle;
	public int Middle {
		get {
			return (middle);
		}
	}
	double[] mask;
	public double this[int idx] {
		get {
			return (mask[idx]);
		}
		set {
			mask[idx] = value;
		}
	}
	double maskSum;
	public double MaskSum {
		get {
			return (maskSum);
		}
		set {
			maskSum = value;
		}
	}

	private ConvLinearMask ()
	{
	}

	public ConvLinearMask (int dim)
	{
		mask = new double[dim];
		this.dim = dim;
		this.middle = dim / 2;
	}
}


