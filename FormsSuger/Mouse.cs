using System.Drawing;
using System.Runtime.InteropServices;

namespace AutoClicker
{

    public class Mouse
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        #region dwFlags
        private
        const int LEFTDOWN = 0x02;
        private
        const int LEFTUP = 0x04;
        private
        const int RIGHTDOWN = 0x08;
        private
        const int RIGHTUP = 0x10;
        #endregion

        public enum enumtest
        {
            leftdown = 0x02,
            leftup = 0x04,
            rightdown = 0x08,
            rightup = 0x10

        }

        [DllImport("user32.dll")]
        public static extern void GetCursorPos(out POINT cPoint);
        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int x, int y);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        public static void Click(enumtest t)
        {
            mouse_event((uint)t, (uint)Position.X, (uint)Position.Y, 0, 0);

        }

        public static Point Position {
            get {
                POINT mPoint;
                GetCursorPos(out mPoint);
                return mPoint;
            }
            set {
                SetCursorPos(value.X, value.Y);
            }
        }

    }
}