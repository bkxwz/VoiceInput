using System.Text.Json;
using System.IO;

namespace VoiceInput.App.Models
{
    public enum RecordingMode
    {
        Hold,
        Toggle
    }

    public class AppSettings
    {
        public RecordingMode RecordingMode { get; set; } = RecordingMode.Hold;
        public int HotkeyModifiers { get; set; } = 0x3; // Ctrl+Alt
        public int HotkeyKey { get; set; } = 0x20; // Space
        public string Language { get; set; } = "zh";
        public bool UseLlmOptimization { get; set; } = false;
        public string ApiBaseUrl { get; set; } = "https://api.openai.com/v1";
        public string ApiKey { get; set; } = "";
        public string LlmModel { get; set; } = "gpt-3.5-turbo";
        public string WhisperModel { get; set; } = "base";

        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "VoiceInput",
            "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch { }
            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch { }
        }
    }
}