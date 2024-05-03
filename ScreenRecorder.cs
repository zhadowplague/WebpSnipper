using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace WebpSnipper;

public static class ScreenRecorder
{
	static List<string> _files = new();
	static readonly string _inPath = $"{AppDomain.CurrentDomain.BaseDirectory}in";
	static readonly string _outPath = $"{AppDomain.CurrentDomain.BaseDirectory}out";
	static bool _started = false;
	static bool _stopped = false;
	static Icon _cursorIcon;

	static public int FrameCount => _files.Count;

	static ScreenRecorder()
	{
		if (Directory.Exists(_inPath))
		{
			Directory.Delete(_inPath, true);
		}

		Directory.CreateDirectory(_inPath);

		_cursorIcon = new Icon("cursor.ico");
	}

	public static void RecordScreen(int x, int y, int width, int height)
	{
		if (_stopped)
			return;
		using var bitmap = new Bitmap(width, height);
		using var g = Graphics.FromImage(bitmap);
		g.CopyFromScreen(x, y, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
		if (Constants.DrawCursor)
		{
			g.DrawIcon(_cursorIcon, Cursor.Position.X - x, Cursor.Position.Y - y);
		}
		var fileName = $"{_inPath}{Path.DirectorySeparatorChar}{_files.Count}.jpg";
		bitmap.Save(fileName, ImageFormat.Jpeg);
		_files.Add(fileName);
		_started = true;
	}

	public static async Task Finish()
	{
		if (!_started)
			return;
		_stopped = true;
		if (Directory.Exists(_outPath))
		{
			Directory.Delete(_outPath, true);
		}
		Directory.CreateDirectory(_outPath);

		await Process(0, 0, Constants.DefaultFPS, Constants.DefaultCompressionQuality);
	} 

	public static async Task Process(int startFrameOffset, int endFrameOffset, int framerate, int compressionQuality)
	{
		var outputFileName = $"{_outPath}{Path.DirectorySeparatorChar}output.webp";
		var msPerFrame = (int)((1.0 / framerate) * 1000);

		StringBuilder arguments = new("-loop 0 -mixed ");
		foreach (var file in _files.Skip(startFrameOffset).SkipLast(endFrameOffset))
		{
			arguments.Append($"{file} -d {msPerFrame} -m {compressionQuality} ");
		}
		arguments.Append($"-o {outputFileName}");

		//Delete outputfile if it exists
		if (File.Exists(outputFileName))
		{
			File.Delete(outputFileName);
		}

		// Create process start info
		ProcessStartInfo startInfo = new()
		{
			FileName = Constants.Img2WebpPath, 
			Arguments = arguments.ToString(),
			UseShellExecute = false, 
			CreateNoWindow = true
		};

		// Start the process
		using Process process = new Process();
		process.StartInfo = startInfo;
		process.Start();

		// Wait for the process to exit
		await process.WaitForExitAsync();
	
		Debug.WriteLine($"Processing exited with: {process.ExitCode}");
	
	}

	public static void OpenOutputFolder()
	{
		System.Diagnostics.Process.Start("explorer.exe", _outPath);
	}

}
