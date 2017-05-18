using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace SIMproject
{
    class ImageStitching
    {
        /// <summary>
        /// 图像拼接接口
        /// </summary>
        /// <param name="filenames">图像文件路径，以|分割多个文件路径</param>
        /// <param name="outname">拼接结果文件路径</param>
        [DllImport("ImageStitching.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#1")]
        public static extern void imageStitch(string filenames, string outname, int w);

        /// <summary>
        /// 快速图像拼接接口
        /// </summary>
        /// <param name="filenames">图像文件路径，以|分割多个文件路径</param>
        /// <param name="outname">拼接结果文件路径</param>
        [DllImport("ImageStitching.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#2")]
        public static extern void myImageStitch(string filenames, string outname, int w, int h, float minRate);

        public static string[] originFilenameList = new string[4];
        public static Bitmap[] originImageList = new Bitmap[4];
        public static string[] fastFilenameList = new string[16];
        public static Bitmap[] fastImageList = new Bitmap[16];
        public static int fastFilenameNum = 0;

        private static string folder = Environment.CurrentDirectory;

        public static float minRate = 0.1f;

        static public double callImageStitch(int type,int w,int h)
        {
            string filenames = "";

            if (type == 1)
            {
                for (int i=0;i<w;i++)
                    filenames += originFilenameList[i] + '|';
            }
            else
            {
                for (int i = 0; i < h; i++)
                    for (int j = 0; j < w; j++)
                        filenames += fastFilenameList[i * 4 + j] + '|';
            }

            string outname = folder+'\\' + DateTime.Now.ToLongDateString() +DateTime.Now.ToLongTimeString().Replace(":","_")+ ".bmp";

            DateTime t1 = DateTime.Now, t2 = t1;

            if (type == 1)
                imageStitch(filenames, outname, w);
            else
                myImageStitch(filenames, outname, h, w, minRate);

            t2 = DateTime.Now;

            ImageShow.nowFile = outname;
            ImageShow.source = new Bitmap(outname);

            return (t2 - t1).TotalMilliseconds;
        }
    }
}
