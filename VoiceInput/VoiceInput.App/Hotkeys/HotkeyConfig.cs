namespace VoiceInput.Hotkeys
{
    public class HotkeyConfig
    {
        public Key ModifierKey1 { get; set; } = Key.LeftCtrl;
        public Key ModifierKey2 { get; set; } = Key.LeftAlt;
        public Key TriggerKey { get; set; } = Key.Space;
        public bool IsToggleMode { get; set; } = false;
    }
}