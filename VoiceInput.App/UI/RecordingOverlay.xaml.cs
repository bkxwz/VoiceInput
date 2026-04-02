using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VoiceInput.UI
{
    public partial class RecordingOverlay : Window
    {
        private Rectangle[] _waveBars;
        private Random _random = new Random();

        public RecordingOverlay()
        {
            InitializeComponent();
            SetupWaveform();
        }

        private void SetupWaveform()
        {
            _waveBars = new Rectangle[5];
            double[] weights = { 0.5, 0.8, 1.0, 0.75, 0.55 };

            for (int i = 0; i < 5; i++)
            {
                var bar = new Rectangle
                {
                    Width = 8,
                    Height = 20 * weights[i],
                    Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    Margin = new Thickness(i * 10, 0, 0, 0)
                };

                WaveformGrid.Children.Add(bar);
                _waveBars[i] = bar;
            }
        }

        public void UpdateWaveform(double rms)
        {
            for (int i = 0; i < _waveBars.Length; i++)
            {
                double randomFactor = _random.NextDouble() * 0.04; // ±4%
                _waveBars[i].Height = rms * randomFactor;
            }
        }
    }
}