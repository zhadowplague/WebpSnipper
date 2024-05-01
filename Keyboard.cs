using SharpHook;
using System.Diagnostics;

namespace AnimatedImageMaker
{
	internal class Keyboard : IDisposable
	{
		TaskPoolGlobalHook _hook;
		bool _disposed;

		public event EscapePressedEvent EscapePressed;
		public delegate void EscapePressedEvent();

		public Keyboard() 
		{
			_hook = new TaskPoolGlobalHook();
			_hook.KeyPressed += OnKeyPressed;
			_hook.RunAsync().ContinueWith((e) => Debug.WriteLine("hook exited"));
		}

		public void Dispose()
		{
			if (_disposed)
				return;
			_hook.KeyPressed -= OnKeyPressed;
			try
			{
				_hook.Dispose();
			} catch (Exception ex) { }
			_disposed = true;
		}

		private void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
		{
			if (e.Data.KeyCode != SharpHook.Native.KeyCode.VcEscape)
				return;
			EscapePressed?.Invoke();
		}
	}
}
