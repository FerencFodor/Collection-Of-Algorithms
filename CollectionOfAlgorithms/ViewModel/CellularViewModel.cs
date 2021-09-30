using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPF_App1.Model;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;
using Size = System.Windows.Size;


namespace WPF_App1.ViewModel
{
    public class CellularViewModel
    {
        private Random _random;
        
        private ICommand _command;
        public ICommand Command
        {
            get => _command ??= new Command();

            set => _command = value;
        }

        public Action OnGenerate => Generate;
        public Action OnRefine => () => Refine(false);
        public Action OnAutoRefine => () => Refine(true);

        public CellularModel CellularModel { get; set; }
        
        public CellularViewModel()
        {
            _random = new Random();
            CellularModel = new CellularModel
            {
                Height = 10,
                Width = 10,
                Probabilities = new List<int>(new []{10,18,30,10})
            };
            
        }

        public void Generate()
        {
            // if (CellularModel.Width == CellularModel.WindowWidth &&
            //     CellularModel.Height == CellularModel.WindowHeight)
            // {
            //     var bitmap = new WriteableBitmap(
            //         CellularModel.WindowWidth,
            //         CellularModel.WindowHeight,
            //         96,
            //         96,
            //         PixelFormats.Bgr32,
            //         null);
            //     
            // }

            var drawingGroup = new DrawingGroup();

            var xSize = CellularModel.WindowWidth / CellularModel.Width;
            var ySize = CellularModel.WindowHeight / CellularModel.Height;


            var cellVals = new ConcurrentBag<int>();

            Parallel.For(0, CellularModel.Width * CellularModel.Height, i =>
                cellVals.Add(Misc.RandomWeightedValue(CellularModel.Probabilities.ToArray(), _random))
            );

            var completeCells = cellVals.ToArray();

            for (var x = 0; x < CellularModel.Width; x++)
            {
                for (var y = 0; y < CellularModel.Height; y++)
                {
                    drawingGroup.Children.Add(new GeometryDrawing(
                        new SolidColorBrush(CellularModel.Colors[completeCells[y*CellularModel.Width+x]]),
                        null,
                        new RectangleGeometry(new Rect(x*xSize, y*ySize, xSize, ySize))));
                }
            }

            var image = new DrawingImage(drawingGroup);
            image.Freeze();

            CellularModel.Output = image;
        }

        public void Refine(bool auto)
        {
            Debug.WriteLine(auto ? "auto" : "non auto");
        }
    }

    public class Command : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ((Action)parameter).Invoke();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}