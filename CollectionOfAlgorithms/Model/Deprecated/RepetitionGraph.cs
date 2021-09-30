using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace WPF_App1
{
    public class RepetitionGraph
    {
        private const int MaxSize = 300;

        public string Text { get; private set; }
        public bool ShowGrid { get; set; }
        public bool DarkMode { get; set; }
        private bool[] Data { get; set; }

        private int _size;
        private OpenFileDialog _dialog;
        private Bitmap _bitmap;
        private Graphics _gr;
        private Dictionary<bool, Color[]> _modes;

        public RepetitionGraph(bool showGrid)
        {
            ShowGrid = showGrid;
            _modes = new Dictionary<bool, Color[]>
            {
                {false, new[] {Color.White, Color.Red}},
                {true, new[] {Color.Black, Color.Aqua}}
            };

            _bitmap = new Bitmap(300, 300);

            _gr = default;
            _gr = Graphics.FromImage(_bitmap);

            _dialog = new OpenFileDialog
            {
                Multiselect = false,
                DefaultExt = "*.txt",
                Filter = "Text documents (.txt)|*.txt"
            };
        }

        public string OpenDialog()
        {
            var result = _dialog.ShowDialog();

            return result == true ? _dialog.FileName : "";
        }

        public async Task<bool> ProcessFile(string filepath)
        {
            if (!File.Exists(filepath) || new FileInfo(filepath).Length == 0)
                return false;

            string[] splitData;

            using (var fileStream = File.OpenRead(filepath))
            {
                var data = new byte[fileStream.Length];
                await fileStream.ReadAsync(data, 0, (int) fileStream.Length);

                var con = Encoding.Default.GetString(data);

                if (con.Split(' ').Length > 300)
                    return false;

                Text = con;

                splitData = con.Split(' ')
                    .Select(s => s.Trim('.', ',').ToLowerInvariant())
                    .ToArray();
            }

            _size = splitData.Length;
            var temp = new bool[_size * _size];

            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    temp[i * _size + j] = splitData[i] == splitData[j];
                }
            }

            Data = temp;

            return true;
        }

        public async Task<Bitmap> GenerateGraph(bool isEmpty)
        {
            _gr.Clear(_modes[DarkMode][0]);
            var scale = (MaxSize - MaxSize % _size) / _size;
            var offset = MaxSize % _size;
            offset = offset % 2 == 1 ? offset - 1 : offset;
            offset /= 2;

            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    var rect = new Rectangle(j * scale + offset, i * scale + offset, scale, scale);
                    if (!isEmpty && Data[i * _size + j])
                        _gr.FillRectangle(new SolidBrush(_modes[DarkMode][1]), rect);
                    if (ShowGrid)
                    {
                        _gr.DrawRectangle(Pens.Black, rect);
                    }
                }
            }

            await Task.Delay(1);
            return _bitmap;
        }
    }
}