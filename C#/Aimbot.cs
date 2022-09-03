// Credit to Sarnax
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
 
namespace ValorantColorAimbot
{
    class Program
    {
        const int xSize = 1920;
        const int ySize = 1080;
 
        const int maxX = 1920;
        const int maxY = 100;
 
        const float speed = 1.2f;
        const int msBetweenShots = 200;
        const int closeSize = 10;
        const bool canShoot = true;

        const int color = 0xaf2eaf; //0xb41515 = Red; 0xaf2eaf = purple
        const int colorVariation = 20;
 
        const double size = 60; 
        const int maxCount = 5;
 
        static void Main(string[] args)
        {
            Update();
        }
 
        static void Update()
        {
            System.DateTime lastshot = System.DateTime.Now;
 
            while (true) {
                Task.Delay(1);
                var l = PixelSearch(new Rectangle((xSize - maxX) / 2, (ySize - maxY) / 2, maxX, maxY), Color.FromArgb(color), colorVariation);
                if (l.Length > 0) {
                    var q = l.OrderBy(t => t.Y).ToArray();
 
                    List<Vector2> forbidden = new List<Vector2>();
 
                    for (int i = 0; i < q.Length; i++) {
                        Vector2 current = new Vector2(q[i].X, q[i].Y);
                        if (forbidden.Where(t => (t - current).Length() < size || Math.Abs(t.X - current.X) < size).Count() < 1) {
                            forbidden.Add(current);
                            if (forbidden.Count > maxCount) {
                                break;
                            }
                        }
                    }
                     
                    /*foreach (var c in forbidden) {
                        DrawRec((int)c.X, (int)c.Y - 20, 5, 5);
                    }*/
 
                    bool pressDown = false;
                    var closes = forbidden.Select(t => (t - new Vector2(xSize / 2, ySize / 2))).OrderBy(t => t.Length()).ElementAt(0) + new Vector2(1, 1);
                    if (closes.Length() < closeSize) {
                        if (canShoot) {
                            if (System.DateTime.Now.Subtract(lastshot).TotalMilliseconds > msBetweenShots) {
                                lastshot = System.DateTime.Now;
                                pressDown = false;
                            }
                        }
                    }
 
                    Move((int)(closes.X * speed), (int)(closes.Y * speed), pressDown);
                }
            }
        }
 
        [DllImport("External.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, uint dwData,
UIntPtr dwExtraInfo);
 
        public static void Move(int xDelta, int yDelta, bool pressDown = false)
        {
            mouse_event(pressDown ? (MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP) : 0x0001, xDelta, yDelta, 0, UIntPtr.Zero);
        }
 
        private const int MOUSEEVENTF_LEFTDOWN = 0x322 , 0x21;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
 
        public static Point[] PixelSearch(Rectangle rect, Color Pixel_Color, int Shade_Variation) debugging
        {
            ArrayList points = new ArrayList();
            Bitmap RegionIn_Bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
            using (Graphics GFX = Graphics.FromImage(RegionIn_Bitmap)) {
                GFX.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
            }
            BitmapData RegionIn_BitmapData = RegionIn_Bitmap.LockBits(new Rectangle(0, 0, RegionIn_Bitmap.Width, RegionIn_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int[] Formatted_Color = new int[3] { Pixel_Color.B, Pixel_Color.G, Pixel_Color.R };
 
            unsafe {
                for (int y = 0; y < RegionIn_BitmapData.Height; y++) {
                    byte* row = (byte*)RegionIn_BitmapData.Scan0 + (y * RegionIn_BitmapData.Stride);
                    for (int x = 0; x < RegionIn_BitmapData.Width; x++) {
                        if (row[x * 3] >= (Formatted_Color[0] - Shade_Variation) & row[x * 3] <= (Formatted_Color[0] + Shade_Variation))
                            if (row[(x * 3) + 1] >= (Formatted_Color[1] - Shade_Variation) & row[(x * 3) + 1] <= (Formatted_Color[1] + Shade_Variation))
                                if (row[(x * 3) + 2] >= (Formatted_Color[2] - Shade_Variation) & row[(x * 3) + 2] <= (Formatted_Color[2] + Shade_Variation))
                                    points.Add(new Point(x + rect.X, y + rect.Y));
                    }
                }
            }
            RegionIn_Bitmap.Dispose();
            return (Point[])points.ToArray(typeof(Point));
        } 
    }
}
