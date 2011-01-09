
public abstract class BasicImagingInterface
{
	public abstract int Width {
		get;
	}
	public abstract int Height {
		get;
	}

	public abstract double ScaleWithin (int downsizeResolution);

	public abstract ImageMap ConvertToImageMap (IPixelConverter pconv);
}

public interface IPixelConverter
{
	double Convert (byte r, byte g, byte b);
}

internal class CanonicalPixelConverter : IPixelConverter
{
	public double Convert (byte r, byte g, byte b)
	{
		return ((r + g + b) / (255.0 * 3.0));
	}
}

