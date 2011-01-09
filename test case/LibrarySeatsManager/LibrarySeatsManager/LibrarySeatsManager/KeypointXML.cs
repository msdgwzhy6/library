using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;

using ICSharpCode.SharpZipLib.GZip;


public class KeypointXMLReader
{
	private KeypointXMLReader ()
	{
	}

	public static KeypointXMLList ReadComplete (string filename)
	{
		return (ReadComplete (filename,
			String.Compare (Path.GetExtension (filename).ToLower (), ".gz") == 0));
	}

	public static KeypointXMLList ReadComplete (string filename, bool compressed)
	{
		XmlSerializer xs = new XmlSerializer (typeof (KeypointXMLList));

		TextReader reader;
		if (compressed) {
			reader = new StreamReader (new GZipInputStream
				(File.OpenRead (filename)));
		} else {
			reader = new StreamReader (filename);
		}

		KeypointXMLList kl = null;
		try {
			kl = (KeypointXMLList) xs.Deserialize (reader);
		} catch (Exception ex) {
			Console.WriteLine ("ex: {0}", ex);
		}
		reader.Close ();

		return (kl);
	}
}


[Serializable]
public class KeypointXMLList
{
	public KeypointXMLList ()
	{
	}

	public KeypointXMLList (string imageFile, int x, int y,
		ArrayList list)
	{
		this.imageFile = imageFile;
		this.array = new KeypointN[list.Count];
		this.xDim = x;
		this.yDim = y;

		for (int n = 0 ; n < list.Count ; ++n)
			array[n] = (KeypointN) list[n];
	}

	public KeypointXMLList (ArrayList list, int maxX, int maxY)
	{
		this.imageFile = "-memory-";
		this.array = (KeypointN[]) list.ToArray (typeof (KeypointN));
		this.xDim = maxX;
		this.yDim = maxY;
	}

	private int xDim;
	public int XDim {
		get {
			return (xDim);
		}
		set {
			xDim = value;
		}
	}
	private int yDim;
	public int YDim {
		get {
			return (yDim);
		}
		set {
			yDim = value;
		}
	}

	private string imageFile;
	public string ImageFile {
		get {
			return (imageFile);
		}
		set {
			imageFile = value;
		}
	}

	private KeypointN[] array;
	public KeypointN[] Arr {
		get {
			return (array);
		}
		set {
			array = value;
		}
	}
}

public class KeypointXMLWriter
{
	private KeypointXMLWriter ()
	{
	}

	public static void WriteComplete (string imageFile, int x, int y,
		string filename, ArrayList list)
	{
		WriteComplete (imageFile, x, y, filename, list,
			String.Compare (Path.GetExtension (filename).ToLower (), ".gz") == 0);
	}

	public static void WriteComplete (string imageFile, int x, int y,
		string filename, ArrayList list, bool compressed)
	{
		KeypointXMLList xl = new KeypointXMLList ();
		xl.ImageFile = imageFile;
		xl.Arr = new KeypointN[list.Count];
		xl.XDim = x;
		xl.YDim = y;

		for (int n = 0 ; n < list.Count ; ++n)
			xl.Arr[n] = (KeypointN) list[n];

		XmlSerializer xs = new XmlSerializer (typeof (KeypointXMLList));
		TextWriter writer;
		if (compressed) {
			writer = new StreamWriter (new GZipOutputStream
				(File.Create (filename)));
		} else {
			writer = new StreamWriter (filename);
		}
		xs.Serialize (writer, xl);
		writer.Close ();
	}
}


[Serializable]
public class KeypointN : KDTree.IKDTreeDomain, ICloneable
{
	double x, y;
	double scale;
	double orientation;

	int dim;
	int[] descriptor;

	public object Clone ()
	{
		KeypointN kc = new KeypointN ();

		kc.x = x;
		kc.y = y;
		kc.scale = scale;
		kc.orientation = orientation;
		kc.dim = dim;
		kc.descriptor = (int[]) descriptor.Clone ();

		return (kc);
	}

	public double X {
		get {
			return (x);
		}
		set {
			x = value;
		}
	}
	public double Y {
		get {
			return (y);
		}
		set {
			y = value;
		}
	}
	public double Scale {
		get {
			return (scale);
		}
		set {
			scale = value;
		}
	}
	public double Orientation {
		get {
			return (orientation);
		}
		set {
			orientation = value;
		}
	}
	public int Dim {
		get {
			return (dim);
		}
		set {
			dim = value;
		}
	}
	public int[] Descriptor {
		get {
			return (descriptor);
		}
		set {
			descriptor = value;
		}
	}

	public KeypointN ()
	{
	}

	public KeypointN (Keypoint kp)
	{
		if (kp.HasFV != true)
			throw (new ArgumentException ("While trying to generate integer " +
				"vector: source keypoint has no feature vector yet"));

		x = kp.X;
		y = kp.Y;
		scale = kp.Scale;
		orientation = kp.Orientation;

		dim = kp.FVLinearDim;
		descriptor = new int[kp.FVLinearDim];

		for (int d = 0 ; d < kp.FVLinearDim ; ++d) {
			descriptor[d] = (int) (255.0 * kp.FVLinearGet (d));
			if (descriptor[d] < 0 || descriptor[d] > 255) {
				throw (new ArgumentOutOfRangeException
					("Resulting integer descriptor k is not 0 <= k <= 255"));
			}
		}
	}

	public int DimensionCount {
		get {
			return (dim);
		}
	}

	public int GetDimensionElement (int n)
	{
		return (descriptor[n]);
	}
}

public class Keypoint
{
    public Keypoint()
    {
    }

    ImageMap image;
    public ImageMap Image
    {
        get
        {
            return (image);
        }
    }

    double x, y;
    double imgScale;	
    double scale;
    double orientation;
    public double X
    {
        get
        {
            return (x);
        }
        set
        {
            x = value;
        }
    }
    public double Y
    {
        get
        {
            return (y);
        }
        set
        {
            y = value;
        }
    }
    public double ImgScale
    {
        get
        {
            return (imgScale);
        }
        set
        {
            imgScale = value;
        }
    }
    public double Scale
    {
        get
        {
            return (scale);
        }
        set
        {
            scale = value;
        }
    }
    public double Orientation
    {
        get
        {
            return (orientation);
        }
        set
        {
            orientation = value;
        }
    }

    bool hasFV = false;
    public bool HasFV
    {
        get
        {
            return (hasFV);
        }
        set
        {
            hasFV = value;
        }
    }

    double[] featureVector;
    public double[] FV
    {
        get
        {
            return (featureVector);
        }
        set
        {
            featureVector = value;
        }
    }

    public double FVGet(int xI, int yI, int oI)
    {
        return (featureVector[(xI * yDim * oDim) + (yI * oDim) + oI]);
    }
    public void FVSet(int xI, int yI, int oI, double value)
    {
        featureVector[(xI * yDim * oDim) + (yI * oDim) + oI] = value;
    }

    public int FVLinearDim
    {
        get
        {
            return (featureVector.Length);
        }
    }

    public double FVLinearGet(int idx)
    {
        return (featureVector[idx]);
    }

    public void FVLinearSet(int idx, double value)
    {
        featureVector[idx] = value;
    }

    public void CreateLinearVector(int dim)
    {
        featureVector = new double[dim];
    }

    private int xDim, yDim, oDim;
    public void CreateVector(int xDim, int yDim, int oDim)
    {
        hasFV = true;
        this.xDim = xDim;
        this.yDim = yDim;
        this.oDim = oDim;
        featureVector = new double[yDim * xDim * oDim];
    }

    public Keypoint(ImageMap image, double x, double y, double imgScale,
        double kpScale, double orientation)
    {
        this.image = image;
        this.x = x;
        this.y = y;
        this.imgScale = imgScale;
        this.scale = kpScale;
        this.orientation = orientation;
    }

    public int DimensionCount
    {
        get
        {
            return (FVLinearDim);
        }
    }

    public double GetDimensionElement(int dim)
    {
        return (FVLinearGet(dim));
    }
}

