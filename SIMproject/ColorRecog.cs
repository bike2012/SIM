using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SIMproject
{
    class ColorRecog
    {
        [DllImport("ColorRecog.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#1")]
        public static extern int colorRecog(int r, int g, int b, int colorNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] int[] R,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] int[] G,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] int[] B, int mode);

        static public List<Color> colorTab = new List<Color>();
        static public int r, g, b;
        static public int result = -1;
        static public int mode = 0;

        public static double callColorRecog()
        {
            DateTime t1 = DateTime.Now, t2 = t1;

            int colorNum = colorTab.Count();
            int[] R = new int[colorNum];
            int[] G = new int[colorNum];
            int[] B = new int[colorNum];

            for (int i = 0;i<colorNum;i++)
            {
                R[i] = colorTab[i].R;
                G[i] = colorTab[i].G;
                B[i] = colorTab[i].B;
            }

            t1 = DateTime.Now;
            result = colorRecog(r, g, b, colorNum, R, G, B, mode);
            t2 = DateTime.Now;
            
            return (t2 - t1).TotalMilliseconds;
        }

        public static void initColorTab()
        {
            colorTab.Clear();
            colorTab.Add(Color.Red);
            colorTab.Add(Color.Green);
            colorTab.Add(Color.Blue);
            colorTab.Add(Color.Orange);
            colorTab.Add(Color.Yellow);
            //colorTab.Add(Color.Purple);
            //colorTab.Add(Color.Pink);
            //colorTab.Add(Color.Brown);
            //colorTab.Add(Color.Lime);
            //colorTab.Add(Color.White);
            //colorTab.Add(Color.Black);
        }
    }
}
