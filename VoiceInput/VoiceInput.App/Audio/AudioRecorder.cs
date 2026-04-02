using System;
using NAudio.Wave;

namespace VoiceInput.Audio
{
    public class AudioRecorder
    {
        private WaveInEvent _waveIn;
        private MemoryStream _memoryStream;
        private WaveFileWriter _waveWriter;

        public event EventHandler<byte[]> DataAvailable;

        public AudioRecorder()
        {
            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(16000, 1) // 16kHz, Mono
            };
            _waveIn.DataAvailable += OnDataAvailable;
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            _memoryStream.Write(e.Buffer, 0, e.BytesRecorded);
            DataAvailable?.Invoke(this, e.Buffer);
        }

        public void StartRecording()
        {
            _memoryStream = new MemoryStream();
            _waveWriter = new WaveFileWriter(_memoryStream, _waveIn.WaveFormat);
            _waveIn.StartRecording();
        }

        public void StopRecording()
        {
            _waveIn.StopRecording();
            _waveWriter?.Dispose();
            _waveWriter = null;
        }

        public byte[] GetRecordedData()
        {
            return _memoryStream?.ToArray();
        }
    }
}