using System.Windows;
using System.Windows.Controls;

namespace AnimatedImageMaker
{
    /// <summary>
    /// Interaction logic for RegionSelectionWindow.xaml
    /// </summary>
    public partial class RegionSelectionWindow : Window
    {
        MainWindow _main;
        Screen _screen;

        bool _posSet;
        bool _sizeSet;
        double x;
        double y;
        double width;
        double height;

        public RegionSelectionWindow(MainWindow main, Screen screen)
        {
            InitializeComponent();

            _main = main;
            _screen = screen;

            Left = screen.Bounds.Left;
            Top = screen.Bounds.Top;
            Width = screen.Bounds.Width;
            Height = screen.Bounds.Height;

            Cursor = System.Windows.Input.Cursors.Cross;
            MouseDown += OnMouseClick;
            MouseMove += OnMouseMove;
            regionSize.Width = 0;
            regionSize.Height = 0;
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
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

        private void OnMouseClick(object sender, System.Windows.Input.MouseEventArgs e)
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
                _sizeSet = true;
                _main.StartRecording(_screen, x, y, width, height);
            }
        }

    }
}
