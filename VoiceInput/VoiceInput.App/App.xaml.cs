using System.IO;
using System.Threading.Tasks;
using System.Windows;
using VoiceInput.Audio;
using VoiceInput.Hotkeys;
using VoiceInput.UI;
using VoiceInput.Speech;
using VoiceInput.LLM;
using VoiceInput.Injection;
using VoiceInput.Models;

namespace VoiceInput
{
    public partial class App : Application
    {
        private readonly AudioRecorder _audioRecorder;
        private readonly WhisperService _whisperService;
        private readonly TextInjectionService _textInjectionService;
        private readonly RecordingOverlay _recordingOverlay;
        private readonly LLMRefiner _llmRefiner;

        public App()
        {
            _audioRecorder = new AudioRecorder();
            _whisperService = new WhisperService("%APPDATA%/VoiceInput/models/base.bin");
            _textInjectionService = new TextInjectionService();
            _recordingOverlay = new RecordingOverlay();
            _llmRefiner = new LLMRefiner("https://api.openai.com/v1/chat/completions", "your-api-key");

            _audioRecorder.DataAvailable += OnAudioDataAvailable;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string modelPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                "VoiceInput/models/base.bin");
            if (!File.Exists(modelPath))
            {
                await WhisperModelDownloader.DownloadModelAsync(modelPath);
            }
        }

        private async void OnAudioDataAvailable(object sender, byte[] data)
        {
            _recordingOverlay.Show();
            string transcription = await _whisperService.TranscribeAsync(data);
            transcription = await _llmRefiner.RefineAsync(transcription);
            _textInjectionService.InjectText(transcription);
            _recordingOverlay.Hide();
        }
    }
}