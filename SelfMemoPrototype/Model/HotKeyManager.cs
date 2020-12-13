using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;

namespace SelfMemoPrototype.Model
{
    public class HotKeyManager
    {
        private static HotKeyHelper _hotkey = null;

        private static EventHandler _handler { get; set; }

        public static void Initialize(Window window, EventHandler handler)
        {
            _hotkey = new HotKeyHelper(window);
            _handler = handler;

            // 初期ショートカットを登録
            ModifierKeys modifier = (ModifierKeys)Properties.Settings.Default.EnumModifierKeys;
            Key key = (Key)Properties.Settings.Default.EnumKey;
            RegisterHotKey(modifier, key);
        }

        public static void Dispose()
        {
            _hotkey.Dispose();
        }

        /// <summary>
        /// Initializeで指定したハンドラを起動するグローバルホットキーを上書き登録する
        /// </summary>
        /// <param name="modifier"></param>
        /// <param name="key"></param>
        /// <returns>正しく登録できればTrue</returns>
        public static bool RegisterHotKey(ModifierKeys modifier, Key key)
        {
            if (modifier == ModifierKeys.None) return false;
            if (_handler == null) return false;

            _hotkey.UnregisterAll();
            _hotkey.Register(modifier, key, _handler);

            // 設定したModifierとKeyを記憶
            Properties.Settings.Default.EnumModifierKeys = (int)modifier;
            Properties.Settings.Default.EnumKey = (int)key;
            Properties.Settings.Default.Save();

            return true;
        }

        /// <summary>
        /// 登録中のGlobal Hot Keyを表す文字列を返す
        /// </summary>
        /// <returns>ホットキー文字列</returns>
        public static string GetGHKeyString()
        {
            Key key = (Key)Properties.Settings.Default.EnumKey;
            ModifierKeys modifier = (ModifierKeys)Properties.Settings.Default.EnumModifierKeys;

            string ret = string.Empty;
         
            foreach(ModifierKeys mod in Enum.GetValues(typeof(ModifierKeys)))
            {
                if (mod == ModifierKeys.None) continue;

                if (modifier.HasFlag(mod))
                {
                    if (!string.IsNullOrEmpty(ret)) ret += " + ";
                    ret += mod.ToString();
                }
            }
            ret += " + " + key.ToString();

            return ret;
        }
    }
}
