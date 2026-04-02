using System;
using System.Windows;

namespace VoiceInput.UI
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void OnSaveClicked(object sender, RoutedEventArgs e)
        {
            // Example save logic
            var hotkey = HotkeyBox.Text;
            var modelPath = ModelPathBox.Text;
            var apiBaseUrl = ApiUrlBox.Text;
            var apiKey = ApiKeyBox.Password;

            // Save settings to config or model

            MessageBox.Show("Settings saved successfully.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}