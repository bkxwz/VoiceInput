using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace VoiceInput.Speech
{
    public class WhisperService
    {
        private readonly string _whisperExecutable = "whisper.exe";
        private readonly string _modelPath;

        public WhisperService(string modelPath)
        {
            _modelPath = modelPath;
        }

        public async Task<string> TranscribeAsync(byte[] audioData, string language = "zh")
        {
            var tempAudioFile = Path.GetTempFileName() + ".wav";
            await File.WriteAllBytesAsync(tempAudioFile, audioData);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = _whisperExecutable,
                Arguments = $"--model {_modelPath} --language {language} " +
                            $"--file {tempAudioFile}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = processStartInfo };
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();

            process.WaitForExit();

            File.Delete(tempAudioFile);

            return output;
        }
    }
}