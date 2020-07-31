using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using HOTKEY_MOD = Native.HOTKEY_MOD;

namespace autoclicker
{
    public class KeyHandler
    {
        private int key;
        private IntPtr hWnd;
        private int id;

        public KeyHandler(Keys key, Form form)
        {
            this.key = (int)key;
            this.hWnd = form.Handle;
            id = this.GetHashCode();
        }

        public override int GetHashCode()
        {
            return key ^ hWnd.ToInt32();
        }

        public bool Register()
        {
            return Native.RegisterHotKey(hWnd, id, 0, key);
        }

        public bool Unregister()
        {
            return Native.UnregisterHotKey(hWnd, id);
        }
    }
}
