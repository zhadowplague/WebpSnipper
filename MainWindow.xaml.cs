using System.Diagnostics;
using System.IO;
using System.Windows;
using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using MessageBox = System.Windows.MessageBox;


namespace AnimatedImageMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
		List<RegionSelectionWindow> _windows;
		CancellationTokenSource _source;
		Keyboard? _keyBoardListener;
		Task? _saveTask;

		bool _disposed;
		bool _exited;

		double _x;
		double _y;
		double _width;
		double _height;

		public MainWindow()
		{
			InitializeComponent();

			_source = new();
			_windows = new();
			if (!File.Exists(Constants.Img2WebpPath))
			{
				MessageBox.Show("Missing img2webp.exe");
                Application.Current.Shutdown();
                return;
			}

			foreach (var screen in Screen.AllScreens)
			{
				var subWindow = new RegionSelectionWindow(this, screen);
				_windows.Add(subWindow);
				subWindow.Show();
			}

			_keyBoardListener = new();
			Cursor = Cursors.Cross;
			Closed += OnClosed;
			_keyBoardListener.EscapePressed += OnEscape;
			progressBar.Visibility = Visibility.Collapsed;
		}

		private void OnClosed(object? sender, EventArgs e)
		{
			Dispose();
		}

		private void OnEscape()
		{
			Dispatcher.Invoke(Exit);
		}

		private async Task Exit()
		{
			if (_exited)
				return;
			_exited = true;
			_source.Cancel();
			if (_saveTask != null)
			{
				await _saveTask;
			}
			var resultWindow = new ResultWindow();
			resultWindow.Show();
			resultWindow.Focus();
			resultWindow.BringIntoView();
			Visibility = Visibility.Collapsed;
		}

		private void Dispose()
		{
			if (_disposed)
				return;
			_disposed = true;
			_source.Cancel();
			_keyBoardListener?.Dispose();
		}

		private async Task DoRecord()
		{
			var progress = 0.0;
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			const int msPerFrame = (int)((1.0 / Constants.DefaultFPS) * 1000);
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                TaskbarItemInfo.ProgressValue = 0;
            }));
            while (!_source.IsCancellationRequested && progress < Constants.MaxLengthMs)
			{
				ScreenRecorder.RecordScreen((int)_x, (int)_y, (int)_width, (int)_height);
				var elapsed = msPerFrame - (int)stopWatch.ElapsedMilliseconds;
				if (elapsed > 0)
				{
					await Task.Delay(elapsed);
				}
				else
				{
					Debug.WriteLine("Slow");
				}
				progress += stopWatch.ElapsedMilliseconds;
				stopWatch.Restart();
				await Dispatcher.InvokeAsync(new Action(() =>
				{
					progressBar.Value = progress / Constants.MaxLengthMs;
					TaskbarItemInfo.ProgressValue = progress / Constants.MaxLengthMs;
				}));
			}
			stopWatch.Stop();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                progressBar.Visibility = Visibility.Collapsed;
                infoLabel.Visibility = Visibility.Visible;
            }));
			await ScreenRecorder.Finish();
			OnEscape();
		}

        internal void StartRecording(Screen screen, double x, double y, double width, double height)
        {
			foreach (var window in _windows)
			{
				window.Close();
			}
			_x = x + screen.Bounds.Left;
			_y = y + screen.Bounds.Top;
			_width = width;
			_height = height;
			Top = _y + height;
			Left = _x + width;
			progressBar.Visibility = Visibility.Visible;
			_saveTask = Task.Run(DoRecord);
        }
    }
}