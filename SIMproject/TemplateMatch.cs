using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace SIMproject
{
    class TemplateMatch
    {
        /// <summary>
        /// 模板匹配算法接口
        /// </summary>
        /// <param name="tempfilename">模板图像文件路径</param>
        /// <param name="objfilename">目标图像文件路径</param>
        /// <param name="resultNum">匹配结果数量</param>
        /// <param name="X1">匹配结果矩形区域中心点坐标，同时X1[0]用于输入模板区域中心点坐标</param>
        /// <param name="Y1">匹配结果矩形区域中心点坐标，同时Y1[0]用于输入模板区域中心点坐标</param>
        /// <param name="X2">匹配结果矩形区域一个顶点坐标，同时X2[0]用于输入模板区域一个顶点坐标</param>
        /// <param name="Y2">匹配结果矩形区域一个顶点坐标，同时X2[0]用于输入模板区域一个顶点坐标</param>
        /// <param name="Alpha">匹配结果矩形区域旋转角度</param>
        [DllImport("TemplateMatch.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#4")]
        public static extern void templateMatchDLL(string tempfilename, string objfilename, ref int resultNum, 
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] float[] X1,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] float[] Y1,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] float[] X2,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] float[] Y2,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] float[] Alpha);

        [DllImport("TemplateMatch.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#2")]
        public static extern void templateFinetune(int width, int height, int imgSize,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data,
            int thr, ref int edgeNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)] int[] edgeX,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)] int[] edgeY);

        [DllImport("TemplateMatch.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#5")]
        public static extern void templateSave(int width, int height, int imgSize,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data,
            int thr, int lowAngle, int highAngle, int level, float minRate, int questNum, int angleThr, string filename);

        [DllImport("TemplateMatch.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#1")]
        public static extern void templateDetect(int width, int height, int imgSize,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data,
            string filename, ref int resultNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)] int[] resultX,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)] int[] resultY,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)] int[] dir,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)] float[] rate);

        [DllImport("TemplateMatch.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#3")]
        public static extern void templateLoad(string filename,ref int level, 
            ref float lowAngle, ref float highAngle, ref float minRate, ref int questNum,
            ref int angleThr, ref int width, ref int height, ref int thr);

        public static string tempFilename = null;
        public static string objFilename = null;

        public static int setRectParaState = 0;

        public static RectangleF rect = new RectangleF(0,0,0,0);

        public static Rectangle templateROI = new Rectangle(0, 0, 0, 0);
        public static Rectangle detectROI = new Rectangle(0, 0, 0, 0);
        public static int thr = 100;
        public static int level = 4;
        public static int questNum = 1;
        public static int lowAngle = -10, highAngle = 10;
        public static float minRate = 0.8f;
        public static int angleThr = 30;
        public static int w = 0, h = 0;

        public static string filename = null;


        public static double templateMatch()
        {
            DateTime t1 = DateTime.Now, t2 = t1;

            int resultNum = 1000;
            float[] X1 = new float[1000];
            float[] X2 = new float[1000];
            float[] Y1 = new float[1000];
            float[] Y2 = new float[1000];
            X1[0] = rect.X+rect.Width/2;
            X2[0] = rect.X+rect.Width;
            Y1[0] = rect.Y+rect.Height/2;
            Y2[0] = rect.Y+rect.Height;
            float[] Alpha = new float[1000];

            t1 = DateTime.Now;
            templateMatchDLL(tempFilename, objFilename, ref resultNum, X1, Y1, X2, Y2, Alpha);
            t2 = DateTime.Now;

            for (int i = 0; i < resultNum; i++)
                ImageShow.addRect(new PointF(X1[i], Y1[i]), new PointF(X2[i], Y2[i]), Alpha[i], true);

            return (t2 - t1).TotalMilliseconds;

        }

        public static void callTemplateFinetune()
        {

            if (ImageShow.nowFile != null)
            {
                ImageShow.source = new Bitmap(ImageShow.nowFile);
                Rectangle rect = new Rectangle(0, 0, ImageShow.source.Width, ImageShow.source.Height);
                System.Drawing.Imaging.BitmapData bmpData = ImageShow.source.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, ImageShow.source.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * ImageShow.source.Height;
                byte[] rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                byte[] data = new byte[templateROI.Width * (bmpData.Stride / bmpData.Width) * templateROI.Height];
                for (int i = 0; i < templateROI.Height; i++)
                    for (int j = 0; j < templateROI.Width * (bmpData.Stride / bmpData.Width); j++)
                        data[i* templateROI.Width * (bmpData.Stride / bmpData.Width)+j] = rgbValues[(i + templateROI.Y) * bmpData.Stride + j + templateROI.X * (bmpData.Stride / bmpData.Width)];

                int edgeNum = templateROI.Width*templateROI.Height;
                int[] edgeX = new int[edgeNum];
                int[] edgeY = new int[edgeNum];
                templateFinetune(templateROI.Width, templateROI.Height, templateROI.Height * templateROI.Width * (bmpData.Stride / bmpData.Width), data, thr, ref edgeNum, edgeX, edgeY);

                ImageShow.edgeDots.Clear();
                for (int i = 0; i < edgeNum; i++)
                    ImageShow.addDot(new Point(edgeX[i] + templateROI.X, edgeY[i]+templateROI.Y));

                ImageShow.source.UnlockBits(bmpData);
            }
        }

        public static void callTemplateSave()
        {
            if (ImageShow.nowFile != null)
            {
                ImageShow.source = new Bitmap(ImageShow.nowFile);
                Rectangle rect = new Rectangle(0, 0, ImageShow.source.Width, ImageShow.source.Height);
                System.Drawing.Imaging.BitmapData bmpData = ImageShow.source.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, ImageShow.source.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * ImageShow.source.Height;
                byte[] rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                byte[] data = new byte[templateROI.Width * (bmpData.Stride / bmpData.Width) * templateROI.Height];
                for (int i = 0; i < templateROI.Height; i++)
                    for (int j = 0; j < templateROI.Width * (bmpData.Stride / bmpData.Width); j++)
                        data[i * templateROI.Width * (bmpData.Stride / bmpData.Width) + j] = rgbValues[(i + templateROI.Y) * bmpData.Stride + j + templateROI.X * (bmpData.Stride / bmpData.Width)];

                templateSave(templateROI.Width, templateROI.Height, templateROI.Height * templateROI.Width * (bmpData.Stride / bmpData.Width), data, thr, lowAngle, highAngle, level, minRate, questNum, angleThr, filename);

                ImageShow.source.UnlockBits(bmpData);
            }
        }

        public static void callTemplateLoad()
        {
            float tempLow=0, tempHigh = 0;
            templateLoad(filename, ref level, ref tempLow, ref tempHigh, ref minRate, ref questNum, ref angleThr, ref w, ref h, ref thr);
            lowAngle = (int)Math.Round(tempLow * 180 / Math.PI);
            highAngle = (int)Math.Round(tempHigh * 180 / Math.PI);

        }

        public static double callTemplateDetect()
        {
            DateTime t1 = DateTime.Now, t2 = t1;

            if (ImageShow.nowFile != null && filename!=null && detectROI!=new Rectangle(0,0,0,0))
            {
                ImageShow.source = new Bitmap(ImageShow.nowFile);
                Rectangle rect = new Rectangle(0, 0, ImageShow.source.Width, ImageShow.source.Height);
                System.Drawing.Imaging.BitmapData bmpData = ImageShow.source.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, ImageShow.source.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * ImageShow.source.Height;
                byte[] rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                byte[] data = new byte[detectROI.Width * (bmpData.Stride / bmpData.Width) * detectROI.Height];
                for (int i = 0; i < detectROI.Height; i++)
                    for (int j = 0; j < detectROI.Width * (bmpData.Stride / bmpData.Width); j++)
                        data[i * detectROI.Width * (bmpData.Stride / bmpData.Width) + j] = rgbValues[(i + detectROI.Y) * bmpData.Stride + j + detectROI.X * (bmpData.Stride / bmpData.Width)];


                int resultNum = 1000;
                int[] resultX = new int[resultNum];
                int[] resultY = new int[resultNum];
                int[] dir = new int[resultNum];
                float[] rate = new float[resultNum];

                t1 = DateTime.Now;
                templateDetect(detectROI.Width, detectROI.Height, detectROI.Height * detectROI.Width * (bmpData.Stride / bmpData.Width), data, filename, ref resultNum, resultX, resultY, dir, rate);
                t2 = DateTime.Now;

                for (int i = 0; i < resultNum; i++)
                {
                    ImageShow.addRect(new PointF((float)(resultX[i]+detectROI.X), (float)(resultY[i]+detectROI.Y)), 
                        new PointF((float)(resultX[i] +detectROI.X+ Math.Sqrt(w*w/4+h*h/4)* Math.Cos((double)dir[i] * Math.PI / 180.0 + Math.Atan((double)h/(double)w))),
                            (float)(resultY[i] + detectROI.Y + Math.Sqrt(w * w / 4 + h * h / 4) * Math.Sin((double)dir[i] * Math.PI / 180.0 + Math.Atan((double)h / (double)w)))), (float)(dir[i] * Math.PI / 180.0), false);
                }

                ImageShow.source.UnlockBits(bmpData);
            }
            return (t2 - t1).TotalMilliseconds;
        }
    }
}
