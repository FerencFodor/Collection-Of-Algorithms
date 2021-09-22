using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Win32;

namespace WPF_App1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private CellularTab _cellularTab;
        private RepetitionTab _repetitionTab;
        private MandelbrotTab _mandelbrotTab;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _cellularTab = new CellularTab(this);
            _repetitionTab = new RepetitionTab(this);
            _mandelbrotTab = new MandelbrotTab(this);

            TbSize.Text = "20";
            TbType1.Text = "9";
            TbType2.Text = "13";
            TbType3.Text = "18";
            TbType4.Text = "5";

            _cellularTab.CellularAutomaton =
                new CellularAutomaton(int.Parse(TbSize.Text), GridCheckBox.IsChecked ?? true);
            _repetitionTab.Graph = new RepetitionGraph(RgShowGrid.IsChecked ?? true);
            _mandelbrotTab.Mandelbrot = new Mandelbrot(int.Parse(ManSize.Text));

            CellularImage.Source = ImageControl.BitmapToImageSource(
                await _cellularTab.CellularAutomaton.GenerateGrid());

            AboutInfo();
        }

        private void AboutInfo()
        {
            AbTbAlgorithm.TextWrapping = TextWrapping.Wrap;
            AbTbAlgorithm.TextAlignment = TextAlignment.Justify;
            AbTbAlgorithm.Margin = new Thickness(2);

            AbTbAlgorithm.Inlines.Add(new Run("Cellular Automaton\n")
                {FontWeight = FontWeights.Bold, TextDecorations = TextDecorations.Underline, FontSize = 15});
            AbTbAlgorithm.Inlines.Add(new Run(
                "The program generates random values, filling the grid, then it's refined by applying a set of rules. " +
                "The Auto Refine check box automates this process by stopping when the grid does not change anymore." +
                " For more info on cellular automata or the Game of Life see "));

            var r = new Run {Text = "Conway's Game of Life.\n\n"};
            var h = new Hyperlink {NavigateUri = new Uri("https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life")};
            h.RequestNavigate += (sender, e) => { Process.Start(e.Uri.ToString()); };
            h.Inlines.Add(r);
            AbTbAlgorithm.Inlines.Add(h);

            AbTbAlgorithm.Inlines.Add(new Run("Pattern Graph\n")
                {FontWeight = FontWeights.Bold, TextDecorations = TextDecorations.Underline, FontSize = 15});
            AbTbAlgorithm.Inlines.Add(new Run("The Program generates a image based on repeated words in a text" +
                                              "or numbers in an array. Other objects have not been tested as the program compares strings."));
        }

        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Cellular Automaton
        private async void CAOnGenerateClick(object sender, RoutedEventArgs e)
        {
            await _cellularTab.OnGenerateClick(sender, e);
        }

        private async void CAOnRefineClick(object sender, RoutedEventArgs e)
        {
            await _cellularTab.OnRefineClick(sender, e);
        }

        private void CAOnLostFocus(object sender, EventArgs e)
        {
            _cellularTab.OnLostFocus(sender, e);
        }

        private void CAOnShowGridClick(object sender, RoutedEventArgs e)
        {
            _cellularTab.OnShowGridClick(sender, e);
        }

        private void CAOnAutoRefineClick(object sender, RoutedEventArgs e)
        {
            _cellularTab.OnAutoRefineClick(sender, e);
        }

        //Repetition Graph
        private void RGOnShowGridClick(object sender, RoutedEventArgs e)
        {
            _repetitionTab.OnShowGridClick(sender, e);
        }

        private async void RGOnBrowseClick(object sender, RoutedEventArgs e)
        {
            await _repetitionTab.OnBrowserClick(sender, e);
        }

        private async void RGOnGenerateClick(object sender, RoutedEventArgs e)
        {
            await _repetitionTab.OnGenerateClick(sender, e);
        }

        private async void RGOnLostFocus(object sender, EventArgs e)
        {
            await _repetitionTab.OnLostFocus(sender, e);
        }

        private void RGOnDarkModeClick(object sender, RoutedEventArgs e)
        {
            _repetitionTab.OnDarkModeClick(sender, e);
        }
        
        //Mandelbrot set
        private void ManOnLostFocus(object sender, EventArgs e)
        {
             _mandelbrotTab.OnLostFocus(sender,e,300);
        }
        
        private void ManOnHiResLostFocus(object sender, EventArgs e)
        {
             _mandelbrotTab.OnLostFocus(sender, e,2048);
        }

        private async void ManOnGenerateClick(object sender, RoutedEventArgs e)
        {
            await _mandelbrotTab.OnGenerateClick(sender, e);
        }
        private async void ManOnSaveClick(object sender, RoutedEventArgs e)
        {
            await _mandelbrotTab.OnSaveClick(sender, e);
        }
    }

    public class CellularTab
    {
        public CellularAutomaton CellularAutomaton { get; set; }
        private readonly MainWindow _window;

        public CellularTab(MainWindow window)
        {
            _window = window;
        }

        public void OnLostFocus(object sender, EventArgs e)
        {
            var scale = int.Parse(_window.TbSize.Text);
            var size = 300;

            if (scale <= 0)
            {
                scale = 1;
            }

            if (scale > size)
            {
                _window.TbSize.Text = $"{size}";
            }
            else if (size % scale != 0)
            {
                _window.TbSize.Text = $"{(int) (300 / Math.Floor(size / (float) scale))}";
            }
        }

        public async Task OnGenerateClick(object sender, RoutedEventArgs e)
        {
            var probabilities = new[]
            {
                int.Parse(_window.TbType1.Text),
                int.Parse(_window.TbType2.Text),
                int.Parse(_window.TbType3.Text),
                int.Parse(_window.TbType4.Text),
            };
            
            var imgSize = int.Parse(_window.TbSize.Text);
            CellularAutomaton.Size = imgSize;


            if (probabilities.All(val => val == 0))
            {
                _window.CellularImage.Source = ImageControl.BitmapToImageSource(
                    await CellularAutomaton.GenerateGrid());
            }
            else
            {
                _window.CellularImage.Source = ImageControl.BitmapToImageSource(
                    await CellularAutomaton.GenerateGrid(probabilities));
            }
        }

        public async Task OnRefineClick(object sender, RoutedEventArgs e)
        {
            _window.RefineButton.IsEnabled = false;
            _window.GenerateButton.IsEnabled = false;
            _window.TbStatusBar.Text = "Processing...";
            if (CellularAutomaton.AutoRefine)
            {
                var task = new Task<Task<int>>(() => CellularAutomaton.RefineGrid(0));
                task.Start();
                var generations = await task.Result;
                
                _window.TbStatusBar.Text = generations < 0 ? 
                    $"Error: Too many cycles! Showing last result." : 
                    $"Done! Refinement Cycles: {generations}";
                
            }
            else
            {
                await Task.Run( () =>  CellularAutomaton.RefineGrid());
                _window.TbStatusBar.Text = "Done!";
            }


            var image = await CellularAutomaton.GenerateGrid();
            _window.CellularImage.Source = ImageControl.BitmapToImageSource(image);
            
            _window.RefineButton.IsEnabled = true;
            _window.GenerateButton.IsEnabled = true;
        }

        public void OnShowGridClick(object sender, RoutedEventArgs e)
        {
            if (_window.GridCheckBox.IsChecked == null)
                _window.TbStatusBar.Text = "Grid Check Box is null!";

            CellularAutomaton.ShowGrid = _window.GridCheckBox.IsChecked ?? true;
        }

        public void OnAutoRefineClick(object sender, RoutedEventArgs e)
        {
            if (_window.RefineCheckBox.IsChecked == null)
                _window.TbStatusBar.Text = "Auto Refine Check Box is null!";

            CellularAutomaton.AutoRefine = _window.RefineCheckBox.IsChecked ?? false;
        }
    }

    public class RepetitionTab
    {
        public RepetitionGraph Graph { get; set; }
        private readonly MainWindow _window;

        public RepetitionTab(MainWindow window)
        {
            _window = window;
        }

        public async Task OnBrowserClick(object sender, RoutedEventArgs e)
        {
            _window.RgTbFile.Text = Graph.OpenDialog();

            if (!await Graph.ProcessFile(_window.RgTbFile.Text))
            {
                _window.TbStatusBar.Text = "File does not exists, it's empty, or it's too large (max 300 words)";
                return;
            }

            if (!string.IsNullOrEmpty(Graph.Text))
            {
                _window.TbStatusBar.Text = "File Loaded!";
                _window.RgFileContent.Text = Graph.Text;
            }
            else
            {
                _window.TbStatusBar.Text = "File does not exists, it's empty, or it's too large (max 300 words)";
                _window.RgFileContent.Text = "";
            }
        }

        public async Task OnGenerateClick(object sender, RoutedEventArgs e)
        {
            _window.RgBrowse.IsEnabled = false;
            _window.RgGenerate.IsEnabled = false;

            if (!await Graph.ProcessFile(_window.RgTbFile.Text))
            {
                _window.TbStatusBar.Text = "File does not exists, it's empty, or it's too large (max 300 words)";
                _window.RgBrowse.IsEnabled = true;
                _window.RgGenerate.IsEnabled = true;
                return;
            }

            _window.TbStatusBar.Text = "Processing...";
            var task = Task.Run(() => Graph.GenerateGraph(false));
            task.Wait();
            var image = task.Result;

            _window.RepetitionImage.Source = ImageControl.BitmapToImageSource(image);

            _window.TbStatusBar.Text = "Done!";
            _window.RgBrowse.IsEnabled = true;
            _window.RgGenerate.IsEnabled = true;
        }

        public void OnShowGridClick(object sender, RoutedEventArgs e)
        {
            if (_window.RgShowGrid.IsChecked == null)
                _window.TbStatusBar.Text = "Grid Check Box is null!";

            Graph.ShowGrid = _window.RgShowGrid.IsChecked ?? true;
        }

        public void OnDarkModeClick(object sender, RoutedEventArgs e)
        {
            if (_window.RgDarkMode.IsChecked == null)
                _window.TbStatusBar.Text = "Dark Mode Check Box is null!";

            Graph.DarkMode = _window.RgDarkMode.IsChecked ?? false;
        }

        public async Task OnLostFocus(object sender, EventArgs e)
        {
            if (!await Graph.ProcessFile(_window.RgTbFile.Text))
            {
                _window.TbStatusBar.Text = "File does not exists, it's empty, or it's too large (max 300 words)";
                _window.RgFileContent.Text = "";
            }
            else
                _window.RgFileContent.Text = Graph.Text;
        }
    }

    public class MandelbrotTab
    {
        public Mandelbrot Mandelbrot { get; set; }
        private readonly MainWindow _window;

        public MandelbrotTab(MainWindow window)
        {
            _window = window;
        }
        
        public void OnLostFocus(object sender, EventArgs e, int size)
        {
            var scale = int.Parse(_window.TbSize.Text);

            if (scale <= 0)
            {
                scale = 1;
            }

            if (scale > size)
            {
                scale = size;
            }
            else if (size % scale != 0)
            {
                scale = (int) (size / Math.Floor(size / (float) scale));
            }

            _window.TbSize.Text = $"{scale}";
        }

        public async Task OnGenerateClick(object sender, RoutedEventArgs e)
        {
            var imgSize = int.Parse(_window.ManSize.Text);

            Mandelbrot.Size = imgSize;
            
            _window.MandelbrotImage.Source =  ImageControl.BitmapToImageSource(
                await Mandelbrot.Generate());
        }

        public async Task OnSaveClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                FileName = "Mandelbrot", 
                DefaultExt = "bmp", 
                Filter = "Bitmap images (*bmp)|*.bmp"
            };

            
            if (dialog.ShowDialog() == false)
            {
                MessageBox.Show("Something went wrong. Try again!", 
                    "Alert", 
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            _window.TbStatusBar.Text = "Processing";
            
            var hiResSize = int.Parse(_window.ManHighRes.Text);
            var image = await Mandelbrot.Generate();
            
            image.Save(dialog.FileName, ImageFormat.Bmp);
            
            _window.TbStatusBar.Text = $"File saved as {dialog.FileName}!";

        }
        
    }
}