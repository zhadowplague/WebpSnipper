using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace WebpSnipper
{
	/// <summary>
	/// Interaction logic for ResultWindow.xaml
	/// </summary>
	public partial class ResultWindow : Window
	{
		public ResultWindow()
		{
			InitializeComponent();

			var serviceCollection = new ServiceCollection();
			serviceCollection.AddWpfBlazorWebView();
			serviceCollection.AddBlazorWebViewDeveloperTools();
			Resources.Add("services", serviceCollection.BuildServiceProvider());
			Closed += OnWindowClose;
		}

		private void OnWindowClose(object? sender, EventArgs e)
		{
			ScreenRecorder.OpenOutputFolder();
			System.Windows.Application.Current.Shutdown();
		}
	}
}
