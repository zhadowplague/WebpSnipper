using Config.Net;
using System.ComponentModel;
using System.Reflection;

namespace WebpSnipper;

public static class Constants
{
    public static int DefaultCompressionQuality { get; private set; }
	public static int DefaultFPS { get; private set; }
	public static double MaxLengthMs { get; private set; }
	public static bool DrawCursor { get; private set; }

	public static readonly string Img2WebpPath = $"{AppDomain.CurrentDomain.BaseDirectory}img2webp.exe";
	public static readonly string InPath = $"{AppDomain.CurrentDomain.BaseDirectory}in";
	public static readonly string OutPath = $"{AppDomain.CurrentDomain.BaseDirectory}out";
	
	static Constants()
	{
		var configuration = new ConfigurationBuilder<IWebpSnipperConfiguration>().UseIniFile("appsettings.ini").Build();
		DefaultCompressionQuality = configuration.DefaultCompressionQuality;
		DefaultFPS = configuration.DefaultFPS;
		MaxLengthMs = configuration.MaxLengthMs;
		DrawCursor = configuration.DrawCursor;
		WriteDefaultsToConfigFile(configuration);
	}

	static void WriteDefaultsToConfigFile(IWebpSnipperConfiguration configuration)
	{
		foreach (var property in typeof(IWebpSnipperConfiguration).GetProperties())
		{
			var defaultValueAttr = property.GetCustomAttribute<DefaultValueAttribute>(true);
			if (defaultValueAttr != null && property.GetValue(configuration)?.Equals(defaultValueAttr.Value) == true)
			{
				property.SetValue(configuration, defaultValueAttr.Value);
			}
		}
	}
}
