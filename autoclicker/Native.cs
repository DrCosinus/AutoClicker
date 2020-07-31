﻿using System;
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
            this.X = x;
            this.Y = y;
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

    public enum SCANCODE_FR : ushort
    {
        A = 16,
        Z = 17,
        E = 18,
        R = 19,
        T = 20,
        Y = 21,
        U = 22,
        I = 23,
        O = 24,
        P = 25,
        Q = 30,
        S = 31,
        D = 32,
        F = 33,
        G = 34,
        H = 35,
        J = 36,
        K = 37,
        L = 38,
        M = 39,
        W = 44,
        X = 45,
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
    public static extern int SendInput(int nInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)]INPUT[] pInputs, int cbSize);

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