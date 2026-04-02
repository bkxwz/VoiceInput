using System;

namespace VoiceInput.Utils
{
    public static class RmsCalculator
    {
        public static double CalculateRms(byte[] audioBuffer, int bytesRecorded)
        {
            double sum = 0;
            for (int i = 0; i < bytesRecorded; i += 2)
            {
                short sample = BitConverter.ToInt16(audioBuffer, i);
                double sample32 = sample / 32768.0;
                sum += sample32 * sample32;
            }

            return Math.Sqrt(sum / (bytesRecorded / 2));
        }
    }
}