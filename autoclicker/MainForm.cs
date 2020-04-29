using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Input;

using INPUT = Native.INPUT;
using POINT = Native.POINT;
using KEYBDINPUT = Native.KEYBDINPUT;
using MOUSEINPUT = Native.MOUSEINPUT;
using INPUT_TYPE = Native.INPUT_TYPE;
using KEYEVENTF = Native.KEYEVENTF;
using MOUSEEVENTF = Native.MOUSEEVENTF;

namespace autoclicker
{
    public partial class MainForm : Form
    {
        private KeyHandler ghk;
        private IntPtr startWindow;
        const string title = "SendInputs";

        public MainForm()
        {
            InitializeComponent();
            this.Text = $"{ title } - { (Environment.Is64BitProcess ? "64bit" : "32bit") }";

            ghk = new KeyHandler(Keys.F11, this);
            ghk.Register();
            System.Diagnostics.Debug.WriteLine($"{Marshal.SizeOf(typeof(INPUT)) } / {Marshal.SizeOf(typeof(KEYBDINPUT))} / {Marshal.SizeOf(typeof(MOUSEINPUT))}");
        }

        private IntPtr GetWindowUnderCursor()
        {
            return Native.WindowFromPoint(MousePosition);
        }
        private string GetWindowName(IntPtr hWnd)
        {
            StringBuilder fileName = new StringBuilder(2000);
            Native.GetWindowText(hWnd, fileName, fileName.Capacity);
            return fileName.ToString();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
            {
                HandleHotKey();
            }
            base.WndProc(ref m);
        }

        private void HandleHotKey()
        {
            ToggleClickTimer();
        }

        private void sendKey(short c)
        {
            INPUT[] inputs = new INPUT[2];

            inputs[0].type = INPUT_TYPE.KEYBOARD;
            inputs[0].ki.wVK = c;
            inputs[0].ki.wScan = 0;
            inputs[0].ki.dwFlags = 0;
            inputs[0].ki.dwExtraInfo = IntPtr.Zero;
            inputs[0].ki.time = 0;

            inputs[1].type = INPUT_TYPE.KEYBOARD;
            inputs[1].ki.wVK = c;
            inputs[1].ki.wScan = 0;
            inputs[1].ki.dwFlags = KEYEVENTF.KEYUP;
            inputs[1].ki.dwExtraInfo = IntPtr.Zero;
            inputs[1].ki.time = 0;

            if (Native.SendInput(2, inputs, Marshal.SizeOf(typeof(INPUT))) == 0)
            {
                System.Diagnostics.Debug.WriteLine($"SendInput failed with code: { Marshal.GetLastWin32Error() }");
            }
        }

        private void sendMouse(MOUSEEVENTF flags)
        {
            INPUT[] input = new INPUT[1];

            input[0].type = INPUT_TYPE.MOUSE;
            input[0].mi.dx = 0;
            input[0].mi.dy = 0;
            input[0].mi.dwFlags = flags;
            input[0].mi.dwExtraInfo = IntPtr.Zero;
            input[0].mi.mouseData = 0;
            input[0].mi.time = 0;

            if (Native.SendInput(1, input, Marshal.SizeOf(typeof(INPUT))) == 0)
            {
                System.Diagnostics.Debug.WriteLine($"SendInput failed with code: { Marshal.GetLastWin32Error() }");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (startWindow != GetWindowUnderCursor())
            {
                ToggleClickTimer();
                return;
            }
            sendKey((short)'J');
            //System.Diagnostics.Debug.WriteLine($"{ MouseButtons }");

            //if (Control.MouseButtons == MouseButtons.XButton1)
            {
                sendMouse(MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.LEFTUP | MOUSEEVENTF.RIGHTDOWN | MOUSEEVENTF.RIGHTUP | MOUSEEVENTF.MIDDLEDOWN | MOUSEEVENTF.MIDDLEUP);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            ToggleClickTimer();
        }

        private void ToggleClickTimer()
        {
            timer1.Interval = (int)numericUpDown1.Value;

            if (!timer1.Enabled)
            {
                timer1.Start();
                startWindow = GetWindowUnderCursor();
                this.Text = $"{ title } - running... \"{ GetWindowName(startWindow) }\"";
                btnStart.Text = "Stop";
                numericUpDown1.Enabled = false;
                
            }
            else
            {
                timer1.Stop();
                this.Text = $"{ title } - stopped";
                btnStart.Text = "Start";
                numericUpDown1.Enabled = true;
            }
        }
    }
}