using System;
using System.IO;

public class ImageMap : ICloneable
{
	private ImageMap ()
	{
	}

	public object Clone ()
	{
		ImageMap cl = new ImageMap (xDim, yDim);

		for (int y = 0 ; y < yDim ; ++y)
			for (int x = 0 ; x < xDim ; ++x)
				cl[x, y] = this[x, y];

		return (cl);
	}

	public void Save (string filename, string extraComment)
	{
		StreamWriter sr = new StreamWriter (filename);

		sr.WriteLine ("# {0} {1}", XDim, YDim);
		if (extraComment != null)
			sr.WriteLine ("# {0}", extraComment);

		for (int y = 0 ; y < yDim ; ++y) {
			for (int x = 0 ; x < xDim ; ++x) {
				sr.WriteLine ("{0} {1} {2}", y, x, this[x, y]);
			}
			sr.WriteLine ();
		}
		sr.Close ();
	}

	int yDim, xDim;
	double[,] valArr;
	public int YDim {
		get {
			return (yDim);
		}
	}
	public int XDim {
		get {
			return (xDim);
		}
	}

	public double MaxElem {
		get {
			double max = 0.0;

			for (int y = 0 ; y < yDim ; ++y) {
				for (int x = 0 ; x < xDim ; ++x) {
					if (this[x, y] > max)
						max = this[x, y];
				}
			}
			return (max);
		}
	}

	public double this[int x, int y] {
		get {
			return (valArr[y, x]);
		}
		set {
			valArr[y, x] = value;
		}
	}

	public ImageMap (int xDim, int yDim)
	{
		this.xDim = xDim;
		this.yDim = yDim;

        //���Ǳ�֤�ڴ��е����ݴ洢��ʵ�ʵ�ͼ���ʾ��ʽ��һ�µ�
		valArr = new double[yDim, xDim];
	}

	public ImageMap ScaleHalf ()
	{
		if ((xDim / 2) == 0 || (yDim / 2) == 0)
			return (null);

		ImageMap res = new ImageMap (xDim / 2, yDim / 2);
        //��ԭ����ֵ���Ϊ��������
		for (int y = 0 ; y < res.yDim ; ++y) {
			for (int x = 0 ; x < res.xDim ; ++x) {
				res[x, y] = this[2 * x, 2 * y];
			}
		}

		return (res);
	}

	// Double the size of an imagemap using linear interpolation.
	// It is not a real doubling as the last line is omitted (the image size
	// would always be odd otherwise and we have no second line for
	// interpolation).
	public ImageMap ScaleDouble ()
	{
		// Doubling an image with x/y dimensions less or equal than 2 will
		// result in an image with just (2, 2) dims, so its useless.
        //������ȵ�ͼ��
		if (xDim <= 2 || yDim <= 2)
			return (null);
        //����ǳ����2��-2
		ImageMap res = new ImageMap (xDim * 2 - 2, yDim * 2 - 2);

		// fill four pixels per step, except for the last line/col, which will
		// be omitted
        //�����������Ŵ��һ�ֲ�ֵ����
        //ʹͼƬ��ÿһ�����ض����
		for (int y = 0 ; y < (yDim - 1) ; ++y) {
			for (int x = 0 ; x < (xDim - 1) ; ++x) {
				// pixel layout:
				// A B
				// C D

				// A��ԭ����ֵ
				res[2 * x + 0, 2 * y + 0] = this[x, y];
				// B��ԭ����ֵ/2
				res[2 * x + 1, 2 * y + 0] =
					(this[x, y] + this[x + 1, y]) / 2.0;
                // C��Bֵ/2
				res[2 * x + 0, 2 * y + 1] =
					(this[x, y] + this[x, y + 1]) / 2.0;
                // D��ԭ����ֵ+B��ԭ����+C��Bֵ��/4
				res[2 * x + 1, 2 * y + 1] = (this[x, y] + this[x + 1, y] +
					this[x, y + 1] + this[x + 1, y + 1]) / 4.0;
			}
		}

		return (res);
	}
    //�������������ͼ��
	static public ImageMap operator* (ImageMap f1, ImageMap f2)
	{
		if (f1.xDim != f2.xDim || f1.yDim != f2.yDim) {
			throw (new ArgumentException ("Mismatching dimensions"));
		}

		ImageMap resultMap = new ImageMap (f1.xDim, f1.yDim);

		for (int y = 0 ; y < f1.yDim ; ++y) {
			for (int x = 0 ; x < f1.xDim ; ++x) {
				resultMap[x, y] = f1[x, y] * f2[x, y];
			}
		}

		return (resultMap);
	}

	static public ImageMap operator+ (ImageMap f1, ImageMap f2)
	{
		if (f1.xDim != f2.xDim || f1.yDim != f2.yDim) {
			throw (new ArgumentException ("Mismatching dimensions"));
		}

		ImageMap resultMap = new ImageMap (f1.xDim, f1.yDim);

		for (int y = 0 ; y < f1.yDim ; ++y) {
			for (int x = 0 ; x < f1.xDim ; ++x) {
				resultMap[x, y] = f1[x, y] + f2[x, y];
			}
		}

		return (resultMap);
	}

	static public ImageMap operator- (ImageMap f1, ImageMap f2)
	{
		if (f1.xDim != f2.xDim || f1.yDim != f2.yDim) {
			throw (new ArgumentException ("Mismatching dimensions"));
		}

		ImageMap resultMap = new ImageMap (f1.xDim, f1.yDim);

		for (int y = 0 ; y < f1.yDim ; ++y) {
			for (int x = 0 ; x < f1.xDim ; ++x) {
				resultMap[x, y] = f1[x, y] - f2[x, y];
			}
		}

		return (resultMap);
	}

	// Normalize: Find the minimum to maximum range, then stretch and limit
	// those to exactly 0.0 to 1.0. If both the minimum and maximum values are
	// equal, no normalization takes place.
	public void Normalize ()
	{
		double min = 0.0;
		double max = 0.0;

		for (int y = 0 ; y < yDim ; ++y) {
			for (int x = 0 ; x < xDim ; ++x) {
				if (min > this[x, y])
					min = this[x, y];

				if (max < this[x, y])
					max = this[x, y];
			}
		}
		if (min == max)
			return;

		double diff = max - min;

		for (int y = 0 ; y < yDim ; ++y) {
			for (int x = 0 ; x < xDim ; ++x) {
				this[x, y] = (this[x, y] - min) / diff;
			}
		}
	}
}


