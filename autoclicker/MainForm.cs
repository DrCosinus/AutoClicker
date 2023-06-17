using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using INPUT = Native.INPUT;
using INPUT_TYPE = Native.INPUT_TYPE;
using KEYBDINPUT = Native.KEYBDINPUT;
using KEYEVENTF = Native.KEYEVENTF;
using MOUSEEVENTF = Native.MOUSEEVENTF;
using MOUSEINPUT = Native.MOUSEINPUT;
using SCANCODE = Native.SCANCODE_FR;

namespace autoclicker
{
    public partial class MainForm : Form
    {
        private readonly KeyHandler ghk;
        private IntPtr startWindow;
        const string title = "SendInputs";

        public MainForm()
        {
            InitializeComponent();
            Text = $"{ title } - { (Environment.Is64BitProcess ? "64bit" : "32bit") }";

            ghk = new KeyHandler(Keys.F6, this);
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
            ToggleEffectTimer();
        }

        private void SendKey(IList<INPUT> inputs, SCANCODE scanCode)
        {
            {
                var input = new INPUT
                {
                    type = INPUT_TYPE.KEYBOARD
                };
                input.ki.wVK = 0;
                input.ki.wScan = scanCode;
                input.ki.dwFlags = KEYEVENTF.SCANCODE;
                input.ki.dwExtraInfo = IntPtr.Zero;
                input.ki.time = 0;

                inputs.Add(input);
            }

            {
                var input = new INPUT
                {
                    type = INPUT_TYPE.KEYBOARD
                };
                input.ki.wVK = 0;
                input.ki.wScan = scanCode;
                input.ki.dwFlags = KEYEVENTF.KEYUP | KEYEVENTF.SCANCODE;
                input.ki.dwExtraInfo = IntPtr.Zero;
                input.ki.time = 0;

                inputs.Add(input);
            }
        }

        private void SendMouse(IList<INPUT> inputs, MOUSEEVENTF flags)
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

            inputs.Add(input);
        }

        private bool clickOn = false;

        MouseButtons previousMouseButtons = MouseButtons.None;

        bool CheckMouseButtonJustPressed(MouseButtons button)
        {
            return !previousMouseButtons.HasFlag(button) & MouseButtons.HasFlag(button);
        }

        bool CheckMouseButtonDown(MouseButtons button)
        {
            return MouseButtons.HasFlag(button);
        }

        private void EffectTimer_Tick(object sender, EventArgs e)
        {
            if (startWindow != GetWindowUnderCursor())
            {
                ToggleEffectTimer();
                return;
            }

            List<INPUT> inputs = new List<INPUT>();

            SendKey(inputs, SCANCODE.L);
            SendKey(inputs, SCANCODE.K);
            SendKey(inputs, SCANCODE.M);

            if (CheckMouseButtonJustPressed(MouseButtons.XButton2))
            {
                clickOn = !clickOn;
            }

            if (clickOn | CheckMouseButtonDown(MouseButtons.XButton1))
            {
                SendMouse(inputs, MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.LEFTUP | MOUSEEVENTF.RIGHTDOWN | MOUSEEVENTF.RIGHTUP | MOUSEEVENTF.MIDDLEDOWN | MOUSEEVENTF.MIDDLEUP);
            }

            if (Native.SendInput(inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT))) == 0)
            {
                System.Diagnostics.Debug.WriteLine($"SendInput failed with code: { Marshal.GetLastWin32Error() }");
            }

            previousMouseButtons = MouseButtons;
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            ToggleEffectTimer();
        }

        private void ToggleEffectTimer()
        {
            EffectTimer.Interval = (int)numericUpDown1.Value;

            if (!EffectTimer.Enabled)
            {
                EffectTimer.Start();
                startWindow = GetWindowUnderCursor();
                Text = $"{ title } - running... \"{ GetWindowName(startWindow) }\"";
                ButtonStart.Text = "Stop";
                numericUpDown1.Enabled = false;

            }
            else
            {
                EffectTimer.Stop();
                Text = $"{ title } - stopped";
                ButtonStart.Text = "Start";
                numericUpDown1.Enabled = true;
            }
        }
    }
}