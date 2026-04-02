using System.Windows;
using VoiceInput.UI;

namespace VoiceInput.ViewModels
{
    public class TrayViewModel
    {
        public void OpenSettings()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var settingsWindow = new SettingsWindow();
                settingsWindow.ShowDialog();
            });
        }

        public void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}