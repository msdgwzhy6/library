using System;
using System.Collections;


public class LoweFeatureDetector
{
	OctavePyramid pyr;
	public OctavePyramid Pyr {
		get {
			return (pyr);
		}
	}

	double octaveSigma = 1.6;

	double preprocSigma = 1.5;

	int minimumRequiredPixelsize = 32;

	int scaleSpaceLevels = 3;

	bool printWarning = true;

	public bool PrintWarning {
		set {
			printWarning = value;
		}
	}

	bool verbose = true;
	public bool Verbose {
		set {
			verbose = value;
		}
	}

	double dogThresh = 0.0075;

	double dValueLowThresh = 0.008;

	double maximumEdgeRatio = 20.0;

	double scaleAdjustThresh = 0.50;

	int relocationMaximum = 4;

	ArrayList globalKeypoints;
	public ArrayList GlobalKeypoints {
		get {
			return (globalKeypoints);
		}
	}

	ArrayList globalNaturalKeypoints = null;
	public ArrayList GlobalNaturalKeypoints {
		get {
			if (globalNaturalKeypoints != null)
				return (globalNaturalKeypoints);

			if (globalKeypoints == null)
				throw (new ArgumentException ("No keypoints generated yet."));

			globalNaturalKeypoints = new ArrayList ();
			foreach (Keypoint kp in globalKeypoints)
				globalNaturalKeypoints.Add (new KeypointN (kp));

			return (globalNaturalKeypoints);
		}
	}

	public LoweFeatureDetector ()
	{
	}

	public int DetectFeatures (ImageMap img)
	{
		return (DetectFeaturesDownscaled (img, -1, 1.0));
	}

	public int DetectFeaturesDownscaled (ImageMap img, int bothDimHi,
		double startScale)
	{
		globalKeypoints = globalNaturalKeypoints = null;

		if (printWarning) {
			Console.Error.WriteLine ("");
			Console.Error.WriteLine ("===============================================================================");
			Console.Error.WriteLine ("The use of this software is restricted by certain conditions.");
			Console.Error.WriteLine ("See the \"LICENSE\" file distributed with the program for details.");
			Console.Error.WriteLine ("");
			Console.Error.WriteLine ("The University of British Columbia has applied for a patent on the SIFT");
			Console.Error.WriteLine ("algorithm in the United States.  Commercial applications of this software may");
			Console.Error.WriteLine ("require a license from the University of British Columbia.");
			Console.Error.WriteLine ("===============================================================================");
			Console.Error.WriteLine ("");
		}

		if (bothDimHi < 0) {
			img = img.ScaleDouble ();
			startScale *= 0.5;
		} else if (bothDimHi > 0) {
			while (img.XDim > bothDimHi || img.YDim > bothDimHi) {
				img = img.ScaleHalf ();//pass
				startScale *= 2.0;
			}
		}

		if (preprocSigma > 0.0) {
			GaussianConvolution gaussianPre =
				new GaussianConvolution (preprocSigma);
			img = gaussianPre.Convolve (img);
		}

		pyr = new OctavePyramid ();
		pyr.Verbose = verbose;
		pyr.BuildOctaves (img, startScale, scaleSpaceLevels,
			octaveSigma, minimumRequiredPixelsize);

		globalKeypoints = new ArrayList ();

		for (int on = 0 ; on < pyr.Count ; ++on) {
			DScaleSpace dsp = pyr[on];

			ArrayList peaks = dsp.FindPeaks (dogThresh);
			if (verbose)
				Console.WriteLine ("Octave {0} has {1} raw peaks",
					on, peaks.Count);

			int oldCount = peaks.Count;
			ArrayList peaksFilt = dsp.FilterAndLocalizePeaks (peaks,
				maximumEdgeRatio, dValueLowThresh, scaleAdjustThresh,
				relocationMaximum);

			if (verbose) {
				Console.WriteLine ("  filtered: {0} remaining from {1}, thats % {2:N2}",
					peaksFilt.Count, oldCount, (100.0 * peaksFilt.Count) / oldCount);

				Console.WriteLine ("generating keypoints from peaks");
			}

			dsp.GenerateMagnitudeAndDirectionMaps ();
			ArrayList keypoints = dsp.GenerateKeypoints (peaksFilt,
				scaleSpaceLevels, octaveSigma);
			dsp.ClearMagnitudeAndDirectionMaps ();

			globalKeypoints.AddRange (keypoints);
		}

		return (globalKeypoints.Count);
	}
}


