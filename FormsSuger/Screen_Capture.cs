using AutoClicker;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace FormsSuger
{
    public class Screen_Capture
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int nHeight);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("user32.dll")]
        static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(int key);

        private Form1 form;
        private Label label;
        private Stopwatch sw;
        private static int SCREEN_WIDTH = Screen.PrimaryScreen.Bounds.Width;
        private static int SCREEN_HEIGHT = Screen.PrimaryScreen.Bounds.Height;

        private static Color TRIGGER_COLOR = Color.FromArgb(255, 255, 15, 44);
        private static Color RDO_WHITE = Color.FromArgb(255, 225, 225, 225);

        Mouse mouse = new Mouse();
        private long[] time_array = new long[10];
        private static Bitmap bm = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        public void Start(Form1 form, Label label)
        {
            this.form = form;
            this.label = label;
            sw = new Stopwatch();
            
            FindReticule_();
        }

        public void FindReticule_()
        {
            new Thread(() =>
            {
                IntPtr desktophWnd;
                IntPtr desktopDc;
                IntPtr memoryDc;
                IntPtr bitmap;
                IntPtr oldBitmap;
                Bitmap fs;

                desktophWnd = GetDesktopWindow();
                desktopDc = GetWindowDC(desktophWnd);
                memoryDc = CreateCompatibleDC(desktopDc);
                bitmap = CreateCompatibleBitmap(desktopDc, SCREEN_WIDTH, SCREEN_HEIGHT);
                oldBitmap = SelectObject(memoryDc, bitmap);

                form.setText("Looking for reticule, hold right click...");
                while (GetKeyState(2) >= 0 && Application.OpenForms.Count > 0)
                {

                }
                form.setText("Keep holding right click...");

                Thread.Sleep(4000);
                BitBlt(memoryDc, 0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, desktopDc, 0, 0, (int)CopyPixelOperation.SourceCopy);
                Thread.Sleep(1000);
                bool breakflag = false;
                BitBlt(memoryDc, 0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, desktopDc, 0, 0, (int)CopyPixelOperation.SourceCopy);
                fs = Image.FromHbitmap(bitmap);
                fs.Save("hello.bmp");
                Smallform smallform = new Smallform();
                smallform.Show();
                smallform.BackColor = Color.Cyan;

                for (int x = ((SCREEN_WIDTH / 2) - (SCREEN_WIDTH / 50)); x < ((SCREEN_WIDTH / 2) + (SCREEN_WIDTH / 50)); x++)
                {
                    if (breakflag)
                    {
                        break;
                    }
                    for (int y = ((SCREEN_HEIGHT / 2) - (SCREEN_HEIGHT / 50)); y < ((SCREEN_HEIGHT / 2) + (SCREEN_HEIGHT / 50)); y++)
                    {
                        while (GetKeyState(2) >= 0 && Application.OpenForms.Count > 0)
                        {
                            form.setText("Looking for reticule, hold right click...");
                        }
                        form.setText("x:" + x + " y:" + y + "\n" + fs.GetPixel(x, y).ToString());
                        smallform.SetPos(x, y);
                        if (fs.GetPixel(x, y) == RDO_WHITE)
                        {

                            Form1.ret_x = x;
                            Form1.ret_y = y;
                            breakflag = true;
                            break;
                        }

                    }
                }
                smallform.Close();
                Trigger();

            }).Start();

        }


        /*
         * I tried many options of optimization when capturing the screen.
         * In the end gdi was the best option with the least amount of latency
         * On my machine I get around 5ms delay between captures.
         */
        public void Trigger()
        {

            new Thread(() =>
            {

                while (Application.OpenForms.Count > 0)
                {
                    
                    if (GetKeyState(2) < 0)
                    {
                        sw.Start();
                        using (Graphics graphics = Graphics.FromImage(bm))
                        {
                            using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                            {
                                IntPtr hSrcDC = gsrc.GetHdc();
                                IntPtr hDC = graphics.GetHdc();
                                BitBlt(hDC, 0, 0, 1, 1, hSrcDC, (Form1.ret_x + Form1.x_offset), (Form1.ret_y + Form1.y_offset), (int)CopyPixelOperation.SourcePaint);
                                Color_Check(bm.GetPixel(0, 0));

                                graphics.ReleaseHdc();
                                gsrc.ReleaseHdc();
                            }

                        }

                        sw.Stop();
                        form.setText((bm.GetPixel(0, 0).ToString()) + "\nx: " + (Form1.ret_x + Form1.x_offset) + " y: " + (Form1.ret_y + Form1.y_offset) + " latency:" + sw.ElapsedMilliseconds.ToString());
                        sw.Reset();
                    }
                    else
                    {
                        Thread.Sleep(1); //to minimize cpu usage
                    }
                }
            }).Start();

        }


        void Color_Check(Color color)
        {
            if (color.R == TRIGGER_COLOR.R)
            {
                Mouse.Click(Mouse.enumtest.leftdown);
                Thread.Sleep(10);
                Mouse.Click(Mouse.enumtest.leftup);
            }

        }
    }
}