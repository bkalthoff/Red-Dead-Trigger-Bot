using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FormsSuger
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer time = new System.Windows.Forms.Timer();

        Screen_Capture t;
        public static int ret_x;
        public static int ret_y;
        delegate void SetTextCallback(string text);
        delegate void setColorCallback(Color c);
        private bool onlyEnemies = true;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private
        const UInt32 SWP_NOSIZE = 0x0001;
        private
        const UInt32 SWP_NOMOVE = 0x0002;
        private
        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        public bool running = true;
        public static int x_offset = 0;
        public static int y_offset = 0;

        [DllImport("user32.dll")]
        [
        return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public Form1()
        {
            InitializeComponent();
            t = new Screen_Capture();

            t.Start(this, label1);
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);

        }

        public void setText(String s)
        {
            if (this.label1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setText);
                try
                {
                    this.Invoke(d, new object[] {
                        s
                    });
                }
                catch (Exception e)
                {
                    //great practice
                }
            }
            else
            {
                this.label1.Text = s;

            }
        }

        public void SetColor(Color c)
        {
            this.Invoke(new MethodInvoker(() => this.BackColor = c));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (onlyEnemies) onlyEnemies = false;
            else if (!onlyEnemies) onlyEnemies = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            if (!Int32.TryParse(textBox1.Text, out x_offset))
            {
                label1.Text = "0";
                return;
            }
            x_offset = Int32.Parse(textBox1.Text);

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

            if (!Int32.TryParse(textBox2.Text, out y_offset))
            {
                label1.Text = "0";
                return;
            }
            y_offset = Int32.Parse(textBox2.Text);

        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            running = false;
        }

    }

}