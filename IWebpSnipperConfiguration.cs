using System.ComponentModel;

namespace WebpSnipper;

public interface IWebpSnipperConfiguration
{
	[DefaultValue(3)]
	public int DefaultCompressionQuality { get; set; }
	[DefaultValue(10)]
	public int DefaultFPS { get; set; }
	[DefaultValue(20000.0)]
	public double MaxLengthMs { get; set; }
	[DefaultValue(false)]
	public bool DrawCursor { get; set; }
}
