using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static Native;

namespace autoclicker
{
    public partial class MainForm : Form
    {
        private readonly KeyHandler ghk;
        private IntPtr startWindow;
        private const string title = "Clix";

        private IntPtr targetWindow = IntPtr.Zero;

        public MainForm()
        {
            InitializeComponent();
            UpdateFormTitleAndEnableComponents();

            ghk = new KeyHandler(Keys.F6, this);
            ghk.Register();
            System.Diagnostics.Debug.WriteLine($"{Marshal.SizeOf(typeof(INPUT)) } / {Marshal.SizeOf(typeof(KEYBDINPUT))} / {Marshal.SizeOf(typeof(MOUSEINPUT))}");

            //EnumWindows((hwnd, lParam) => { return false; }, 0);

            targetWindow = IntPtr.Zero;
            foreach (Process process in Process.GetProcesses())
            {
                if (process.MainWindowHandle != IntPtr.Zero && process.ProcessName == "Cookie Clicker")
                {
                    targetWindow = process.MainWindowHandle;
                    Trace.WriteLine($"{process.ProcessName} - FOUND - { (int)process.MainWindowHandle:X8}");
                }
            }
        }

        private void UpdateFormTitleAndEnableComponents()
        {
            Text = $"{ title } - { (Environment.Is64BitProcess ? "64bit" : "32bit") } - { (EffectTimer.Enabled ? $"running on \"{ GetWindowName(startWindow) }\"" : "stopped") } ";
            ButtonStart.Text = EffectTimer.Enabled ? "Stop" : "Start";
            numericUpDown1.Enabled = !EffectTimer.Enabled;
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

        private bool clickOn = true;

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
            if (targetWindow != IntPtr.Zero)
            {
                IntPtr hdc = GetWindowDC(targetWindow);
                Graphics g = Graphics.FromHwnd(targetWindow);
                Pen p = new Pen(Color.Red, 10);
                g.DrawRectangle(p, 10, 10, 50, 50);
                ReleaseDC(targetWindow, hdc);
            }
            if (startWindow != GetWindowUnderCursor())
            {
                ToggleEffectTimer();
                return;
            }

            InputBuffer inputs = new InputBuffer();

            //inputs.SendKey((ushort)SCANCODE_FR.L);
            //inputs.SendKey((ushort)SCANCODE_FR.K);
            //inputs.SendKey((ushort)SCANCODE_FR.M);

            if (CheckMouseButtonJustPressed(MouseButtons.XButton2))
            {
                clickOn = !clickOn;
            }

            if (clickOn | CheckMouseButtonDown(MouseButtons.XButton1))
            {
                inputs.SendMouse(MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.LEFTUP);
            }

            inputs.Commit();

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
            }
            else
            {
                EffectTimer.Stop();
                startWindow = IntPtr.Zero;
            }
            UpdateFormTitleAndEnableComponents();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            keyPressed.Text = $"{e.KeyCode}";
        }
    }
}