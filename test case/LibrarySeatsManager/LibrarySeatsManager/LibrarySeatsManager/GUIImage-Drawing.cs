using System;
using System.Drawing;
using System.Drawing.Drawing2D;

public class DisplayImage : BasicImagingInterface
{
	public override int Width {
		get {
			return (bm.Width);
		}
	}
	public override int Height {
		get {
			return (bm.Height);
		}
	}

	private Bitmap bm;

	private DisplayImage ()
	{
	}

	public DisplayImage (string filename)
	{
		bm = new Bitmap (filename);
	}

	private DisplayImage (Bitmap bm)
	{
		this.bm = bm;
	}

	public override double ScaleWithin (int dim)
	{
        //dimֱ������С��ͼƬ�Ŀ�Ⱥ͸߶�
		if (Width <= dim && Height <= dim)
			return (1.0);

		float xScale = ((float) dim / Width);
		float yScale = ((float) dim / Height);
        //ȡ��С�ı���
		float smallestScale = xScale <= yScale ? xScale : yScale;

		// Scale image.
		// XXX: under Mono this produces ugly nearest-neighbour sampled
		// versions, so stay away from using Mono with this code (yet).
        
        //����һ��λͼ���ߴ�����С��ĳߴ�
		Bitmap bmScaled = new Bitmap ((int) (Width * smallestScale + 0.5F),
			(int) (Height * smallestScale + 0.5F));
		Graphics gr = Graphics.FromImage (bmScaled);
        //���ò岹ģʽΪ������˫���β�ֵ��
		gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
		gr.DrawImage (bm, new RectangleF (0.0F, 0.0F,
			Width * smallestScale, Height * smallestScale));
        //�޸ĳ�Ա���������ͼ��ĳ�Ա����
		bm = bmScaled;

		return (smallestScale);
	}

	/** Carve a part out of the image, where (x1, y1) is the lower top
	 * rectangle with the given width and height.
	 *
	 * Return the carved copy of the image.
	 */
	public DisplayImage Carve (int x1, int y1, int width, int height)
	{
		Rectangle cr = new Rectangle (x1, y1, width, height);

		Bitmap carved = bm.Clone (cr, bm.PixelFormat);
		//carved.Save ("test.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

		return (new DisplayImage (carved));
	}

	public void Save (string filename)
	{
		bm.Save (filename, System.Drawing.Imaging.ImageFormat.Jpeg);
	}

	public override ImageMap ConvertToImageMap (IPixelConverter pconv)
	{
		if (pconv == null)
			pconv = new CanonicalPixelConverter ();

		ImageMap res = new ImageMap (Width, Height);

		// This code is quite slow, but I found no quick way to convert and
		// access the raw image data at always the same format. So, fix it or
		// stick with it. At least it looks elegant ;-)
        //��ȡͼƬ��ÿһ������
		for (int y = 0 ; y < Height ; ++y) {
			for (int x = 0 ; x < Width ; ++x) {
				Color col = bm.GetPixel (x, y);
                //ÿһ������ȡ����������res[x,y]��
				res[x, y] = pconv.Convert (col.R, col.G, col.B);
			}
		}

		return (res);
	}
}


