using Whisper.net;
using VoiceInput.App.Models;
using System.Net.Http;
using System.IO;

namespace VoiceInput.App.Speech
{
    public class SpeechRecognizer : IDisposable
    {
        private WhisperProcessor? _processor;
        private string _modelsDir;
        private string _currentLanguage;
        private Action<string>? _onPartialResult;

        public SpeechRecognizer(AppSettings settings)
        {
            _currentLanguage = settings.Language;
            _modelsDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "VoiceInput", "models");

            Directory.CreateDirectory(_modelsDir);
            EnsureModelDownloaded(settings.WhisperModel);
            InitializeProcessor(settings.WhisperModel);
        }

        private void EnsureModelDownloaded(string modelName)
        {
            var modelPath = Path.Combine(_modelsDir, $"{modelName}.bin");
            if (File.Exists(modelPath)) return;

            // Download model (simplified)
            var url = $"https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-{modelName}.bin";
            using var client = new HttpClient();
            var data = client.GetByteArrayAsync(url).Result;
            File.WriteAllBytes(modelPath, data);
        }

        private void InitializeProcessor(string modelName)
        {
            var modelPath = Path.Combine(_modelsDir, $"{modelName}.bin");
            _processor = WhisperFactory.FromPath(modelPath).CreateBuilder()
                .WithLanguage(_currentLanguage)
                .Build();
        }

        public void SetLanguage(string language)
        {
            _currentLanguage = language;
            // Reinitialize if needed
        }

        public void StartRecognition(Action<string> onPartialResult = null)
        {
            _onPartialResult = onPartialResult;
        }

        public async Task<string?> RecognizeAsync(byte[] audioData)
        {
            if (_processor == null) return null;

            using var stream = new MemoryStream(audioData);
            var result = await _processor.ProcessAsync(stream);
            return result.GetText();
        }

        public void Dispose()
        {
            _processor?.Dispose();
        }
    }
}