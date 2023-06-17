using System;
using System.Windows.Forms;

namespace autoclicker
{
    public class KeyHandler
    {
        private readonly int key;
        private readonly IntPtr hWnd;
        private readonly int id;

        public KeyHandler(Keys key, Form form)
        {
            this.key = (int)key;
            hWnd = form.Handle;
            id = GetHashCode();
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
