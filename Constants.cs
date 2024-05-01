namespace AnimatedImageMaker;

public static class Constants
{
    public const int DefaultCompressionQuality = 3;
	public const int DefaultFPS = 10;
	public const double MaxLengthMs = 20000;
	public static readonly string Img2WebpPath = $"{AppDomain.CurrentDomain.BaseDirectory}img2webp.exe";
	public static readonly string InPath = $"{AppDomain.CurrentDomain.BaseDirectory}in";
	public static readonly string OutPath = $"{AppDomain.CurrentDomain.BaseDirectory}out";
}
