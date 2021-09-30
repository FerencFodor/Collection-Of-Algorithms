using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Numerics;

namespace WPF_App1
{

    
    public class Mandelbrot
    {
        private const int Width = 300;
        private const int Height = 300;
        private const int MaxCycles = 1000;

        private readonly Bitmap _bitmap;
        // private  Bitmap _hiResBitmap;
        private readonly Graphics _gr;
        //private readonly Graphics _ghr;
        //private Color _color;

        private readonly List<List<int>> _iterations;
        
        public int Size { get; set; }

        public Mandelbrot(int size)
        {
            Size = size;
            _iterations = new List<List<int>>();

            _bitmap = new Bitmap(Width, Height);
            // _hiResBitmap = new Bitmap(2048, 2048);
            // _color = new Color();
            //
            _gr = default;
            _gr = Graphics.FromImage(_bitmap);
            
            // _ghr = default;
            // _ghr = Graphics.FromImage(_hiResBitmap);
        }
        

        public async Task<Bitmap> Generate()
        {
            //_gr.Clear(Color.White);
            var xSize = Width / Size;
            var ySize = Height / Size;
            
            await Task.Run(() =>
            {
                for (var i = 0; i < Size; i++)
                {
                    var row = new List<int>();
                    
                    for (var j = 0; j < Size; j++)
                    {
                        row.Add(GetIterations(j, i, Size).Result);
                    }
                    _iterations.Add(row);
                }
            });

            var limit = Vector2.Zero;
            var hue = await Task.Run(() => HistogramColoring(Size,Size, out limit));
            
            await Task.Run(() =>
            {
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        var color = ColorConverter.ColorFromHsl(220, 1f, Map(hue[i][j], limit.X, limit.Y, 0f, 1f));
                        
                        var rect = new Rectangle(j * xSize, i * ySize, xSize, ySize);
                        _gr.FillRectangle(new SolidBrush(color), rect);
                    }
                }
            });
        
            return _bitmap;
        }

        
        
        // public async Task<Bitmap> Generate(int hiSize)
        // {
        //     _ghr.Clear(Color.White);
        //     var xSize = 2048 / hiSize;
        //     var ySize = 2048 / hiSize;
        //     
        //     await Task.Run(() =>
        //     {
        //         for (var i = 0; i < hiSize; i++)
        //         {
        //             for (var j = 0; j < hiSize; j++)
        //             {
        //                 
        //                 
        //                 
        //                 // var col = GetIterations(j*xSize, i*ySize, hiSize).Result;
        //                 // var rect = new Rectangle(j * xSize, i * ySize, xSize, ySize);
        //                 // _ghr.FillRectangle(new SolidBrush(col), rect);
        //             }
        //         }
        //     });
        //
        //     return _hiResBitmap;
        // }
        
        private List<List<float>> HistogramColoring(int xSize, int ySize, out Vector2 limit)
        {
            limit = new Vector2();
            
            var hue = new List<List<float>>(Enumerable.Repeat(new List<float>(Enumerable.Repeat(0.0f, Size)),
                Size));
            
            Task.Run(() =>
            {
                var iterationPerPixel = new List<int>(Enumerable.Repeat(0, MaxCycles+1));
                for (var x = 0; x < xSize; x++)
                {
                    for (var y = 0; y < ySize; y++)
                    {
                        var i = _iterations[x][y];
                        iterationPerPixel[i]++;
                    }
                }

                var total = 0;
                for (var i = 0; i < MaxCycles; i++)
                {
                    total += iterationPerPixel[i];
                }
                
                for (var x = 0; x < xSize; x++)
                {
                    for (var y = 0; y < ySize; y++)
                    {
                        var iteration = _iterations[x][y];
                        for (var i = 0; i <= iteration; i++)
                        {
                            hue[x][y] += (float)iterationPerPixel[i] / total;
                        }
                    }
                }
            });

            Task.Delay(100);
            
            var max = float.NegativeInfinity;
            var min = float.PositiveInfinity;

            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    min = Math.Min(min, hue[j][i]);
                    max = Math.Max(max, hue[j][i]);
                }
            }

            limit.X = min;
            limit.Y = max;
            
            return hue;
        }

        private async Task<int> GetIterations(int px, int py, int? size)
        {
            var checker = size != null ? (double) size : 300d;

            var x0 = Map(px, 0, checker,-2.5d, 1d);
            var y0 = Map(py, 0, checker,-1d, 1d);
            
            var x = 0d;
            var y = 0d;
            
            var x2 = 0d;
            var y2 = 0d;

            var it = 0;

            await Task.Run(() =>
            {
                while (x2 + y2 <= 4 && it < MaxCycles)
                {
                    y = 2 * x * y + y0;
                    x = x2 - y2 + x0;
                    x2 = x * x;
                    y2 = y * y;
                    it++;
                }
                
                
            });

            return it;
        }
        
        private static double Map(double value, double s1, double s2, double t1, double t2)
        {
            return t1 + (value - s1) * (t2 - t1) / (s2 - s1);
        }
        
        private static float Map(float value, float s1, float s2, float t1, float t2)
        {
            return t1 + (value - s1) * (t2 - t1) / (s2 - s1);
        }
    }
}