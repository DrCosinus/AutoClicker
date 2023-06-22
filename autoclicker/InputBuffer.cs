using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static Native;

namespace autoclicker
{
    internal class InputBuffer
    {
        private List<INPUT> inputs = new List<INPUT>();

        public void SendKey(ushort scanCode)
        {
            inputs.Add(CreateKeyScanCode(scanCode, false));
            inputs.Add(CreateKeyScanCode(scanCode, true));
        }
        public void SendMouse(MOUSEEVENTF flags)
        {
            inputs.Add(CreateMouse(flags));
        }
        public void Commit()
        {
            if (inputs.Count > 0)
                if (Native.SendInput(inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT))) == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"SendInput failed with code: { Marshal.GetLastWin32Error() }");
                    inputs.Clear();
                }
        }
    private static INPUT CreateKeyScanCode(ushort scanCode, bool keyUp = false)
        {
            var input = new INPUT
            {
                type = INPUT_TYPE.KEYBOARD
            };
            input.ki.wVK = 0;
            input.ki.wScan = scanCode;
            input.ki.dwFlags = KEYEVENTF.SCANCODE | (keyUp ? KEYEVENTF.KEYUP : 0);
            input.ki.dwExtraInfo = IntPtr.Zero;
            input.ki.time = 0;

            return input;
        }

        private static INPUT CreateMouse(MOUSEEVENTF flags)
        {
            var input = new INPUT
            {
                type = INPUT_TYPE.MOUSE
            };
            input.mi.dx = 0;
            input.mi.dy = 0;
            input.mi.dwFlags = flags;
            input.mi.dwExtraInfo = IntPtr.Zero;
            input.mi.mouseData = 0;
            input.mi.time = 0;

            return input;
        }
    }
}
