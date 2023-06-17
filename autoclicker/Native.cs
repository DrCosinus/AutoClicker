using System;
using System.Runtime.InteropServices;
using System.Text;

static class Native
{
    [DllImport("user32.dll")]
    public static extern IntPtr WindowFromPoint(System.Drawing.Point Point);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern uint GetWindowModuleFileName(IntPtr hwnd,
        StringBuilder lpszFileName, uint cchFileNameMax);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out POINT lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator System.Drawing.Point(POINT p)
        {
            return new System.Drawing.Point(p.X, p.Y);
        }

        public static implicit operator POINT(System.Drawing.Point p)
        {
            return new POINT(p.X, p.Y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public enum INPUT_TYPE : int
    {
        MOUSE = 0,
        KEYBOARD = 1,
        HARDWARE = 2,
    }

    [Flags]
    public enum MOUSEEVENTF : int
    {
        MOVE = 0x0001,
        LEFTDOWN = 0x0002,
        LEFTUP = 0x0004,
        RIGHTDOWN = 0x0008,
        RIGHTUP = 0x0010,
        MIDDLEDOWN = 0x0020,
        MIDDLEUP = 0x0040,
        XDOWN = 0x0080,
        XUP = 0x0100,
        WHEEL = 0x0800,
        HWHEEL = 0x1000,
        MOVE_NOCOALESCE = 0x2000,
        VIRTUALDESK = 0x4000,
        ABSOLUTE = 0x8000
    }

    [Flags]
    public enum KEYEVENTF : int
    {
        EXTENTEDKEY = 1,
        KEYUP = 2,
        UNICODE = 4,
        SCANCODE = 8,
    }

    // https://kbdlayout.info/KBDFR/scancodes
    public enum SCANCODE_FR : ushort
    {
        ESCAPE = 1, N1, N2, N3, N4, N5, N6, N7, N8, N9, N0, CLOSED_BRACKET, EQUAL, BACK,
        TAB, A, Z, E, R, T, Y, U, I, O, P, CIRCON, DOLLAR, RETURN, LCONTROL,
        Q, S, D, F, G, H, J, K, L, M, PERCENT, POSTSCRIPT, LSHIFT, ASTERISK,
        W, X, C, V, B, N, COMMA, SEMICOLON, COLON, EXCLAMATION, RSHIFT, PN_MULTIPLY, LMENU, SPACE, CAPITAL,
        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10,
        NUMLOK, SCROLL, PN_HOME, PN_UP, PN_PRIOR, PN_SUBSTRACT, PN_LEFT, PN_CLEAR, PN_RIGHT, PN_ADD, PN_END,
        PN_DOWN, PN_NEXT, PN_INSERT, PN_DELETE, SNAPSHOT, KEY55, LESSERTHAN, F11, F12 /*= 88*/
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int dx;              // 0
        public int dy;              // 4
        public int mouseData;       // 8
        public MOUSEEVENTF dwFlags; // 12
        public int time;            // 16
        public IntPtr dwExtraInfo;  // 20
    }                               //    24

    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public short wVK;           // 0
        public SCANCODE_FR wScan;   // 2
        public KEYEVENTF dwFlags;   // 4
        public uint time;           // 8
        public IntPtr dwExtraInfo;  // 12
    }                               //    16

    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT
    {
        [FieldOffset(0)]
        public INPUT_TYPE type;
        [FieldOffset(4)]
        public MOUSEINPUT mi;
        [FieldOffset(4)]
        public KEYBDINPUT ki;
    };

    [DllImport("User32.dll", SetLastError = true)]
    public static extern int SendInput(int nInputs, ref INPUT pInputs, int cbSize);

    [DllImport("User32.dll", SetLastError = true)]
    public static extern int SendInput(int nInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] INPUT[] pInputs, int cbSize);

    [DllImport("user32.dll")]
    public static extern IntPtr GetMessageExtraInfo();


    [Flags]
    public enum HOTKEY_MOD : int
    {
        ALT = 1,
        CONTROL = 2,
        SHIFT = 4,
        WIN = 8,
        NOREPEAT = 0x4000,
    }

    [DllImport("user32.dll")]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, HOTKEY_MOD fsModifiers, int vk);

    [DllImport("user32.dll")]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    //SetWindowsHookEx
}