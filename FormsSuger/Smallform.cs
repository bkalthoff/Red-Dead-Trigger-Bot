using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FormsSuger
{

    public partial class Smallform : Form
    {
        private delegate void SafeCallDelegate(int x, int y);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private
        const UInt32 TOPMOST_FLAGS = 0x0001 | 0x0002;
        [DllImport("user32.dll")]
        [
        return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public Smallform()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);

        }

        private void Smallform_Load(object sender, EventArgs e)
        {
            this.Width = 0;
            this.Height = 0;
        }

        public void SetPos(int x, int y)
        {
            this.Invoke(new MethodInvoker(() => this.Top = y));
            this.Invoke(new MethodInvoker(() => this.Left = x));
        }

    }
}