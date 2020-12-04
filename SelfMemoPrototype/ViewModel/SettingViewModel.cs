using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using SelfMemoPrototype.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SelfMemoPrototype.ViewModel
{
    class SettingViewModel : BindableBase
    {
        public ReactivePropertySlim<bool> ModifierShift { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> ModifierAlt { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> ModifierCtrl { get; set; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> ModifierWindows { get; set; } = new ReactivePropertySlim<bool>();

        public ReactivePropertySlim<string> ShortcutKey { get; set; } = new ReactivePropertySlim<string>();

        public ReactivePropertySlim<string> Message { get; set; } = new ReactivePropertySlim<string>();

        public SettingViewModel()
        {
            Key key = (Key)Properties.Settings.Default.EnumKey;
            ModifierKeys modifier = (ModifierKeys)Properties.Settings.Default.EnumModifierKeys;

            ShortcutKey.Value = key.ToString();

            ModifierShift.Value = modifier.HasFlag(ModifierKeys.Shift);
            ModifierAlt.Value = modifier.HasFlag(ModifierKeys.Alt);
            ModifierCtrl.Value = modifier.HasFlag(ModifierKeys.Control);
            ModifierWindows.Value = modifier.HasFlag(ModifierKeys.Windows);
        }

        private DelegateCommand _setGlobalHotkeyCmd;

        public DelegateCommand SetGlobalHotkeyCmd
        {
            get
            {
                return _setGlobalHotkeyCmd = _setGlobalHotkeyCmd ?? new DelegateCommand(SetGlobalHotkey);
            }
        }
        public void SetGlobalHotkey()
        {
            ModifierKeys modifier = ModifierShift.Value ? ModifierKeys.Shift : ModifierKeys.None;
            modifier |= ModifierAlt.Value ? ModifierKeys.Alt : ModifierKeys.None;
            modifier |= ModifierCtrl.Value ? ModifierKeys.Control : ModifierKeys.None;
            modifier |= ModifierWindows.Value ? ModifierKeys.Windows : ModifierKeys.None;

            if (Enum.TryParse<Key>(ShortcutKey.Value, out Key key))
            {
                HotKeyManager.RegisterHotKey(modifier, key);
                Message.Value = "ショートカットキーを更新しました: ";
                Message.Value += (modifier.HasFlag(ModifierKeys.Control) ? "Ctrl+" : "");
                Message.Value += (modifier.HasFlag(ModifierKeys.Alt) ? "Alt+" : "");
                Message.Value += (modifier.HasFlag(ModifierKeys.Shift) ? "Shift+" : "");
                Message.Value += (modifier.HasFlag(ModifierKeys.Windows) ? "Win+" : "");
                Message.Value += key.ToString();
            }
            else
            {
                Message.Value = "キーの記述が不正です。";
            }
        }
    }
}
