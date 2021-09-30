using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace WPF_App1.Model
{
    public class CellularModel : INotifyPropertyChanged
    {

        private int _width;
        private int _height;
        private int _windowWidth;
        private int _windowHeight;
        private List<int> _probabilities;
        private ImageSource _output;

        private List<Color> _colors;

        public List<Color> Colors
        {
            get => _colors ??= new List<Color>
                {
                    Color.FromRgb(255,255,255),
                    Color.FromRgb(124,252,0),
                    Color.FromRgb(219,112,147),
                    Color.FromRgb(0,139,139)
                };
            
            set => _colors = value;
        }
        
        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged("Width");
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged("Height");
            }
        }
        
        public int WindowWidth
        {
            get => _windowWidth;
            set
            {
                _windowWidth = value;
                OnPropertyChanged("WindowWidth");
            }
        }
        
        public int WindowHeight
        {
            get => _windowHeight;
            set
            {
                _windowHeight = value;
                OnPropertyChanged("WindowHeight");
            }
        }
        

        public List<int> Probabilities
        {
            get => _probabilities;
            set
            {
                _probabilities = value;
                OnPropertyChanged("Probabilities");
            }
        }

        public ImageSource Output
        {
            get => _output;
            set
            {
                _output = value;
                OnPropertyChanged("Output");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}