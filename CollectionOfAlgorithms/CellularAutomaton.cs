using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace WPF_App1
{
    public class CellularAutomaton
    {
        private const int Width = 300;
        private const int Height = 300;

        private readonly Bitmap _bitmap;
        private readonly Graphics _gr;
        private int[] _indices;
        private readonly List<Color> _colors;

        public int Size { get; set; }
        public bool ShowGrid { get; set; }
        public bool AutoRefine { get; set; }

        public CellularAutomaton(int size, bool showGrid)
        {
            Size = size;
            ShowGrid = showGrid;

            _bitmap = new Bitmap(Width, Height);

            _gr = default;
            _gr = Graphics.FromImage(_bitmap);


            _colors = new List<Color>
            {
                Color.White,
                Color.LawnGreen,
                Color.PaleVioletRed,
                Color.DarkCyan
            };
        }

        public async Task<Bitmap> GenerateGrid()
        {
            _gr.Clear(Color.White);
            var xSize = Width / Size;
            var ySize = Height / Size;

            for (var i = 0; i < Size; i++)
            for (var j = 0; j < Size; j++)
            {
                var rect = new Rectangle(j * xSize, i * ySize, xSize, ySize);
                if (_indices != null)
                    _gr.FillRectangle(new SolidBrush(_colors[_indices[i * Size + j]]), rect);
                if (ShowGrid)
                    _gr.DrawRectangle(Pens.Black, rect);
            }

            await Task.Delay(1);
            return _bitmap;
        }

        public async Task<Bitmap> GenerateGrid(int[] probabilities)
        {
            _gr.Clear(Color.White);
            var xSize = Width / Size;
            var ySize = Height / Size;

            _indices = RandomIndices(probabilities);


            await Task.Run(() =>
            {
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        var rect = new Rectangle(j * xSize, i * ySize, xSize, ySize);
                        _gr.FillRectangle(new SolidBrush(_colors[_indices[i * Size + j]]), rect);
                        if (ShowGrid)
                            _gr.DrawRectangle(Pens.Black, rect);
                    }
                }
            });

            return _bitmap;
        }

        public async Task RefineGrid()
        {
            var generation = new int[Size * Size];

            await Task.Run(() =>
            {
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        generation[i * Size + j] = Eval(j, i, new int[3]);
                    }
                }
            });

            Array.Copy(generation, _indices, Size * Size);
        }

        public async Task<int> RefineGrid(int cycles)
        {
            var generation = new int[Size * Size];
            var trees = new int[3];
            do
            {
                cycles++;

                await Task.Run(() =>
                {
                    for (var i = 0; i < Size; i++)
                    {
                        for (var j = 0; j < Size; j++)
                        {
                            generation[i * Size + j] = Eval(j, i, trees);
                        }
                    }
                });

                if (cycles > Size)
                {
                    Array.Copy(generation, _indices, Size * Size);
                    return -1;
                }

                if (generation.SequenceEqual(_indices))
                    break;

                Array.Copy(generation, _indices, Size * Size);
            } while (true);
            
            return cycles;
        }
        

        private int Eval(int x, int y, int[] trees)
        {
            var neighbourCount = 0;

            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    var index = GetIndex(x, y, j, i);

                    if (i == 0 && j == 0) continue;
                    if (_indices[index] > 0)
                    {
                        trees[_indices[index] - 1] += 1;
                        neighbourCount++;
                    }
                }
            }

            var tree = _indices[GetIndex(x, y)];

            if (tree <= 0)
                return neighbourCount >= 5 ? RandomIndex(trees) : tree;

            return neighbourCount < 4 ? 0 : tree;
        }

        private int[] RandomIndices(int[] probs)
        {
            var random = new Random();
            var output = new int[Size * Size];
            var total = probs.Sum();

            for (var i = 0; i < Size * Size; i++)
            {
                var choice = random.Next(0, total);

                if ((choice -= probs[0]) < 0)
                    output[i] = 1;
                else if ((choice -= probs[1]) < 0)
                    output[i] = 2;
                else if ((choice - probs[2]) < 0)
                    output[i] = 3;
                else
                    output[i] = 0;
            }

            return output;
        }

        private static int RandomIndex(int[] prob)
        {
            var random = new Random();
            var total = prob.Sum();

            var choice = random.Next(0, total);

            if ((choice -= prob[0]) < 0)
                return 1;
            if ((choice - prob[1]) < 0)
                return 2;

            return 3;
        }

        private int GetIndex(int x, int y, int dx, int dy)
        {
            return ((y + dy + Size) % Size) * Size + ((x + dx + Size) % Size);
        }

        private int GetIndex(int x, int y)
        {
            return y * Size + x;
        }
    }
}