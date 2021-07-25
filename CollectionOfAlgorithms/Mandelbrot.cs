using System;
using System.Drawing;
using System.Threading.Tasks;

namespace WPF_App1
{
    public class Mandelbrot
    {
        private const int Width = 300;
        private const int Height = 300;
        private const int MaxCycles = 1000;

        private readonly Bitmap _bitmap;
        private  Bitmap _hiResBitmap;
        private readonly Graphics _gr;
        private readonly Graphics _ghr;
        private Color _color;

        public int Size { get; set; }

        public Mandelbrot(int size)
        {
            Size = size;

            _bitmap = new Bitmap(Width, Height);
            _hiResBitmap = new Bitmap(2048, 2048);
            _color = new Color();

            _gr = default;
            _gr = Graphics.FromImage(_bitmap);

            _ghr = default;
            _ghr = Graphics.FromImage(_hiResBitmap);
        }

        public async Task<Bitmap> Generate()
        {
            _gr.Clear(Color.White);
            var xSize = Width / Size;
            var ySize = Height / Size;
            
            await Task.Run(() =>
            {
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        var col =  GetPixel(j, i, null).Result;
                        var rect = new Rectangle(j * xSize, i * ySize, xSize, ySize);
                        _gr.FillRectangle(new SolidBrush(col), rect);
                    }
                }
            });

            return _bitmap;
        }
        
        public async Task<Bitmap> Generate(int hiSize)
        {
            _ghr.Clear(Color.White);
            var xSize = 2048 / hiSize;
            var ySize = 2048 / hiSize;
            
            await Task.Run(() =>
            {
                for (var i = 0; i < hiSize; i++)
                {
                    for (var j = 0; j < hiSize; j++)
                    {
                        var col = GetPixel(j, i, hiSize).Result;
                        var rect = new Rectangle(j * xSize, i * ySize, xSize, ySize);
                        _ghr.FillRectangle(new SolidBrush(col), rect);
                    }
                }
            });

            return _hiResBitmap;
        }

        private async Task<Color> GetPixel(int px, int py, int? size)
        {
            var checker = size != null ? (double) size : 300;

            var x0 = Map(px, 0, checker,-2.5f, 1f);
            var y0 = Map(py, 0, checker,-1f, 1f);

            var x = 0d;
            var y = 0d;
            
            int it;

            await Task.Run(() =>
            {
                for (it = 0; x * x + y * y <= 2 * 2 && it < MaxCycles; it++)
                {
                    var temp = x * x - y * y + x0;
                    y = 2 * x * y + y0;
                    x = temp;
                }

                var r = it % 10;
                var b = it % 100 - r;
                var g = it % 1000 - b;

                b = b / 10;
                g = g / 100;

                _color = Color.FromArgb(
                    3 + 25 * r,
                    3 + 25 * g,
                    3 + 25 * b
                    );
                
                
                //"Dirty Gold"
                /*var r = (it % 400) + 1;
                var b = (it % 500) + 200;
                var g = (it % 700) + 300;
                
                 r = (int)Map(r, 1,400, 1,256);
                 b = (int)Map(b, 200,700, 1,256);
                 g = (int)Map(g, 300,1000, 1,256);
                 
                _color = Color.FromArgb(
                    Math.Min(r-1,255),
                    Math.Min(g-1,255),
                    Math.Min(b-1,255)
                );*/

            });
            
            return _color;
        }
        
        private static double Map(double value, double s1, double s2, double t1, double t2)
        {
            return t1 + (value - s1) * (t2 - t1) / (s2 - s1);
        }
    }
}