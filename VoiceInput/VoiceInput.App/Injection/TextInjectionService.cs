using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace VoiceInput.Injection
{
    public class TextInjectionService
    {
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        public void InjectText(string text)
        {
            Thread thread = new Thread(() => ClipboardTextInjectionThread(text));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        private void ClipboardTextInjectionThread(string text)
        {
            string originalClipboard = System.Windows.Clipboard.ContainsText() ? System.Windows.Clipboard.GetText() : "";

            try
            {
                // Set clipboard to new text
                System.Windows.Clipboard.SetText(text);

                // Simulate Ctrl+V
                keybd_event(0x11, 0, 0, UIntPtr.Zero); // Ctrl
                keybd_event(0x56, 0, 0, UIntPtr.Zero); // V
                keybd_event(0x56, 0, 2, UIntPtr.Zero); // V Up
                keybd_event(0x11, 0, 2, UIntPtr.Zero); // Ctrl Up
            }
            finally
            {
                // Restore original clipboard
                System.Windows.Clipboard.SetText(originalClipboard);
            }
        }
    }
}