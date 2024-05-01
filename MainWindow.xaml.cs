using ABI.Windows.ApplicationModel.Activation;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace AnimatedImageMaker
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		CancellationTokenSource _source;
		Keyboard _keyBoardListener;
		Task _saveTask;
		bool _disposed;
		bool _exited;
		bool _posSet;
		bool _sizeSet;
		double x;
		double y;
		double width;
		double height;

		public MainWindow()
		{
			InitializeComponent();

			_source = new();
			_keyBoardListener = new();

			Width = SystemParameters.PrimaryScreenWidth;
			Height = SystemParameters.PrimaryScreenHeight;

			Cursor = System.Windows.Input.Cursors.Cross;
			MouseDown += OnMouseClick;
			MouseMove += OnMouseMove;
			Closed += OnClosed;
			Deactivated += OnDeactivated;
			_keyBoardListener.EscapePressed += OnEscape;
			progressBar.Visibility = Visibility.Collapsed;
			regionSize.Width = 0;
			regionSize.Height = 0;
		}

		private void OnDeactivated(object? sender, EventArgs e)
		{
			if (!_posSet || !_sizeSet)
				System.Windows.Application.Current.Shutdown();
		}

		private void OnClosed(object? sender, EventArgs e)
		{
			Dispose();
		}

		private void OnEscape()
		{
			Dispatcher.Invoke(Exit);
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			var pos = e.GetPosition(this);
			if (_posSet && !_sizeSet)
			{
				if (pos.X < x)
				{
					Canvas.SetLeft(regionPos, pos.X);
					width = regionSize.Width = Math.Max(x - Canvas.GetLeft(regionPos), 0);
				}
				else
				{
					Canvas.SetLeft(regionPos, x);
					width = regionSize.Width = Math.Max(pos.X - Canvas.GetLeft(regionPos), 0);
				}

				if (pos.Y < y)
				{
					Canvas.SetTop(regionPos, pos.Y);
					height = regionSize.Height = Math.Max(y - Canvas.GetTop(regionPos), 0);
				}
				else
				{
					Canvas.SetTop(regionPos, y);
					height = regionSize.Height = Math.Max(pos.Y - Canvas.GetTop(regionPos), 0);
				}
			}
		}

		private void OnMouseClick(object sender, MouseEventArgs e)
		{
			var pos = e.GetPosition(this);
			if (!_posSet)
			{
				y = pos.Y;
				x = pos.X;
				_posSet = true;
			}
			else if (!_sizeSet && width > 0 && height > 0)
			{
				_saveTask = Task.Run(DoRecord, _source.Token);
				_sizeSet = true;
				this.Width = 200;
				this.Height = 100;
				this.Left = 0;
				this.Top = 0;
				regionPos.Visibility = Visibility.Collapsed;
				BorderThickness = new Thickness(0);
				progressBar.Visibility = Visibility.Visible;
				progressBar.Value = 0;
			}
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
			_keyBoardListener.Dispose();
		}

		private async Task DoRecord()
		{
			var progress = 0.0;
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			const int msPerFrame = (int)((1.0 / Constants.DefaultFPS) * 1000);
			const double maxTimeMs = 10000.0;
			while (!_source.IsCancellationRequested && progress < maxTimeMs)
			{
				ScreenRecorder.RecordScreen((int)x, (int)y, (int)width, (int)height);
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
					this.progressBar.Value = progress / maxTimeMs;
					this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
					this.TaskbarItemInfo.ProgressValue = progress / maxTimeMs;
				}));
			}
			stopWatch.Stop();
			await ScreenRecorder.Finish();
			OnEscape();
		}
	}
}