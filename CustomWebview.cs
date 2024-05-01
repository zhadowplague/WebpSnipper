using Microsoft.AspNetCore.Components.WebView.Wpf;
using Microsoft.Extensions.FileProviders;

namespace AnimatedImageMaker
{
	public class CustomWebView : BlazorWebView
	{
		public override IFileProvider CreateFileProvider(string contentRootDir)
		{
			var lPhysicalFiles = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory);
			return new CompositeFileProvider(lPhysicalFiles, base.CreateFileProvider(contentRootDir));
		}
	}
}
