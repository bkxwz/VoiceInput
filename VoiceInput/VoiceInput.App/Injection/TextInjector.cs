using VoiceInput.App.Utils;
using System.Runtime.InteropServices;

namespace VoiceInput.App.Injection
{
    public class TextInjector
    {
        public void InjectText(string text)
        {
            var hwnd = NativeMethods.GetForegroundWindow();
            if (hwnd == IntPtr.Zero) return;

            // Backup clipboard
            var originalClipboard = GetClipboardText();

            // Handle IME
            HandleImeForInjection(hwnd);

            // Set clipboard
            SetClipboardText(text);

            // Simulate Ctrl+V
            SendCtrlV();

            // Restore clipboard
            if (originalClipboard != null)
                SetClipboardText(originalClipboard);
        }

        private string? GetClipboardText()
        {
            if (!NativeMethods.OpenClipboard(IntPtr.Zero)) return null;

            try
            {
                var hData = NativeMethods.GetClipboardData(13); // CF_UNICODETEXT
                if (hData == IntPtr.Zero) return null;

                var text = Marshal.PtrToStringUni(hData);
                return text;
            }
            finally
            {
                NativeMethods.CloseClipboard();
            }
        }

        private void SetClipboardText(string text)
        {
            if (!NativeMethods.OpenClipboard(IntPtr.Zero)) return;

            try
            {
                NativeMethods.EmptyClipboard();
                var hGlobal = Marshal.StringToHGlobalUni(text);
                NativeMethods.SetClipboardData(13, hGlobal);
            }
            finally
            {
                NativeMethods.CloseClipboard();
            }
        }

        private void HandleImeForInjection(IntPtr hwnd)
        {
            var hImc = NativeMethods.ImmGetContext(hwnd);
            if (hImc != IntPtr.Zero)
            {
                uint conversion, sentence;
                if (NativeMethods.ImmGetConversionStatus(hImc, out conversion, out sentence))
                {
                    // Check if it's CJK IME
                    if ((conversion & 0x0001) != 0) // IME_CMODE_NATIVE
                    {
                        // Switch to alphanumeric mode
                        NativeMethods.ImmSetConversionStatus(hImc, NativeMethods.IME_CMODE_ALPHANUMERIC, sentence);
                        // Could restore later, but for simplicity, leave it
                    }
                }
                NativeMethods.ImmReleaseContext(hwnd, hImc);
            }
        }

        private void SendCtrlV()
        {
            var inputs = new NativeMethods.INPUT[4];

            // Ctrl down
            inputs[0] = new NativeMethods.INPUT
            {
                type = NativeMethods.INPUT_KEYBOARD,
                u = new NativeMethods.InputUnion
                {
                    ki = new NativeMethods.KEYBDINPUT
                    {
                        wVk = NativeMethods.VK_CONTROL,
                        dwFlags = NativeMethods.KEYEVENTF_KEYDOWN
                    }
                }
            };

            // V down
            inputs[1] = new NativeMethods.INPUT
            {
                type = NativeMethods.INPUT_KEYBOARD,
                u = new NativeMethods.InputUnion
                {
                    ki = new NativeMethods.KEYBDINPUT
                    {
                        wVk = NativeMethods.VK_V,
                        dwFlags = NativeMethods.KEYEVENTF_KEYDOWN
                    }
                }
            };

            // V up
            inputs[2] = new NativeMethods.INPUT
            {
                type = NativeMethods.INPUT_KEYBOARD,
                u = new NativeMethods.InputUnion
                {
                    ki = new NativeMethods.KEYBDINPUT
                    {
                        wVk = NativeMethods.VK_V,
                        dwFlags = NativeMethods.KEYEVENTF_KEYUP
                    }
                }
            };

            // Ctrl up
            inputs[3] = new NativeMethods.INPUT
            {
                type = NativeMethods.INPUT_KEYBOARD,
                u = new NativeMethods.InputUnion
                {
                    ki = new NativeMethods.KEYBDINPUT
                    {
                        wVk = NativeMethods.VK_CONTROL,
                        dwFlags = NativeMethods.KEYEVENTF_KEYUP
                    }
                }
            };

            NativeMethods.SendInput(4, inputs, Marshal.SizeOf<NativeMethods.INPUT>());
        }
    }
}