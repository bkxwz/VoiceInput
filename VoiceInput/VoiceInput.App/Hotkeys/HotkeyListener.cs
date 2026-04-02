using System;
using System.Windows.Input;

namespace VoiceInput.Hotkeys
{
    public class HotkeyListener
    {
        private readonly HotkeyManager _hotkeyManager;
        private readonly HotkeyConfig _config;
        private bool _isKeyDown;
        private bool _isRecording;

        public event EventHandler RecordingStarted;
        public event EventHandler RecordingStopped;

        public HotkeyListener(HotkeyConfig config)
        {
            _config = config;
            _hotkeyManager = new HotkeyManager();
            _hotkeyManager.KeyDown += OnKeyDown;
            _hotkeyManager.KeyUp += OnKeyUp;
        }

        public void StartListening()
        {
            _hotkeyManager.Start();
        }

        public void StopListening()
        {
            _hotkeyManager.Stop();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!_isKeyDown && e.Key == _config.TriggerKey && Keyboard.IsKeyDown(_config.ModifierKey1) && Keyboard.IsKeyDown(_config.ModifierKey2))
            {
                _isKeyDown = true;

                if (_config.IsToggleMode)
                {
                    if (_isRecording)
                    {
                        _isRecording = false;
                        RecordingStopped?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        _isRecording = true;
                        RecordingStarted?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    _isRecording = true;
                    RecordingStarted?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == _config.TriggerKey)
            {
                _isKeyDown = false;

                if (!_config.IsToggleMode && _isRecording)
                {
                    _isRecording = false;
                    RecordingStopped?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}