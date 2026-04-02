using System.Net;
using System.Threading.Tasks;

namespace VoiceInput.Models
{
    public static class WhisperModelDownloader
    {
        private const string BaseUrl = "https://example-whisper-model-repo.com";

        public static async Task DownloadModelAsync(string modelPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(modelPath));

            using var client = new WebClient();
            await client.DownloadFileTaskAsync(new Uri($"{BaseUrl}/base.bin"), modelPath);
        }
    }
}