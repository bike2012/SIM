using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace SIMproject
{
    class PreProcess
    {
        /// <summary>
        /// 图像降噪接口
        /// </summary>
        /// <param name="filename">图像文件路径</param>
        /// <param name="ksize">卷积核大小</param>
        /// <param name="imgSize">图像尺寸</param>
        /// <param name="data">降噪结果图像</param>
        [DllImport("PreProcess.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#2")]
        public static extern void blur(string filename, int ksize, int imgSize,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data);

        /// <summary>
        /// 二值化接口
        /// </summary>
        /// <param name="width">图像宽</param>
        /// <param name="height">图像高</param>
        /// <param name="imgSize">图像数据量</param>
        /// <param name="data">图像数据</param>
        /// <param name="lowThr">低阈值</param>
        /// <param name="highThr">高阈值</param>
        /// <param name="ptNum">外界矩形顶点数（4）</param>
        /// <param name="ptX">外界矩形顶点坐标</param>
        /// <param name="ptY">外界矩形顶点坐标</param>
        [DllImport("PreProcess.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#6")]
        public static extern void threshold(int width, int height, int imgSize,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data,
            int lowThr, int highThr, int ptNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 7)] float[] ptX,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 7)] float[] ptY);

        /// <summary>
        /// 颜色直方图
        /// </summary>
        /// <param name="width">图像宽</param>
        /// <param name="height">图像高</param>
        /// <param name="imgSize">图像数据量</param>
        /// <param name="data">图像数据</param>
        /// <param name="hist">返回颜色直方图</param>
        [DllImport("PreProcess.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#4")]
        public static extern void colorHist(int width, int height, int imgSize,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 256)] int[] hist);

        /// <summary>
        /// Sobel边缘提取
        /// </summary>
        /// <param name="width">图像宽</param>
        /// <param name="height">图像高</param>
        /// <param name="imgSize">图像数据量</param>
        /// <param name="data">图像数据</param>
        /// <param name="ksize">卷积核大小</param>
        /// <param name="thr">梯度阈值</param>
        /// <param name="edgeNum">返回边缘点数</param>
        /// <param name="edgeX">返回边缘点坐标</param>
        /// <param name="edgeY">返回边缘点坐标</param>
        [DllImport("PreProcess.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#5")]
        public static extern void sobel(int width, int height, int imgSize,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data,
            int ksize, ref int thr, ref int edgeNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] int[] edgeX,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] int[] edgeY);

        /// <summary>
        /// Canny边缘提取
        /// </summary>
        /// <param name="width">图像宽</param>
        /// <param name="height">图像高</param>
        /// <param name="imgSize">图像数据量</param>
        /// <param name="data">图像数据</param>
        /// <param name="lowThr">低阈值</param>
        /// <param name="highThr">高阈值</param>
        /// <param name="edgeNum">返回边缘点数</param>
        /// <param name="edgeX">返回边缘点坐标</param>
        /// <param name="edgeY">返回边缘点坐标</param>
        [DllImport("PreProcess.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#3")]
        public static extern void canny(int width, int height, int imgSize,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data,
            int lowThr, int highThr, ref int edgeNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] int[] edgeX,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] int[] edgeY);

        /// <summary>
        /// LSD边缘提取
        /// </summary>
        /// <param name="width">图像宽</param>
        /// <param name="height">图像高</param>
        /// <param name="imgSize">图像数据量</param>
        /// <param name="data">图像数据</param>
        /// <param name="step">步长</param>
        /// <param name="thr">梯度差阈值</param>
        /// <param name="edgeNum">返回边缘点数</param>
        /// <param name="edgeX">返回边缘点坐标</param>
        /// <param name="edgeY">返回边缘点坐标</param>
        [DllImport("PreProcess.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#1")]
        public static extern void LSD(int width, int height, int imgSize,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data,
            int step, int thr, ref int edgeNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] int[] edgeX,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] int[] edgeY);
        
        public static int mouseX = -1;
        public static int mouseDownX = -1;
        public static int lowThrX = -1;
        public static int highThrX = -1;

        /// <summary>
        /// Smooth paras
        /// </summary>
        public static int ksize = 5;

        /// <summary>
        /// colorHist paras
        /// </summary>
        public static bool colorHistExist = false;
        public static int[] colorHistData = new int[256];

        /// <summary>
        /// threshold paras
        /// </summary>
        public static int lowThr = -1, highThr = -1;

        public static bool showBoundingBox = false;
        public static List<PointF> boundingBox = new List<PointF>();

        /// <summary>
        /// Sobel paras
        /// </summary>
        public static int sobelKsize = 3;
        public static int sobelThr = 100;
        public static bool sobelOTSU = false;

        /// <summary>
        /// Canny paras
        /// </summary>
        public static int cannyLowThr = 50;
        public static int cannyHighThr = 140;

        /// <summary>
        /// LSD paras
        /// </summary>
        public static int LSDStep = 5;
        public static int LSDThr = 10;

        public static double callBlur()
        {
            DateTime t1 = DateTime.Now, t2 = t1;

            if (ImageShow.nowFile != null)
            {
                int imgSize = ImageShow.source.Width * ImageShow.source.Height * 3;
                byte[] data = new byte[imgSize];

                blur(ImageShow.nowFile, ksize, imgSize, data);

                t2 = DateTime.Now;

                ImageShow.modified = new Bitmap(ImageShow.source);
                Rectangle rect = new Rectangle(0, 0, ImageShow.modified.Width, ImageShow.modified.Height);
                System.Drawing.Imaging.BitmapData bmpData = ImageShow.modified.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, ImageShow.modified.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * ImageShow.modified.Height;
                byte[] rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                int channel = Bitmap.GetPixelFormatSize(ImageShow.modified.PixelFormat);
                if (channel == 8)
                {
                    for (int counter = 0; counter < ImageShow.modified.Width * ImageShow.modified.Height; counter++)
                        rgbValues[counter] = data[counter * 3];
                }
                else if (channel == 32)
                {
                    for (int counter = 0; counter < ImageShow.modified.Width * ImageShow.modified.Height; counter++)
                    {
                        rgbValues[counter * 4] = data[counter * 3];
                        rgbValues[counter * 4 + 1] = data[counter * 3 + 1];
                        rgbValues[counter * 4 + 2] = data[counter * 3 + 2];

                    }
                }
                else
                    for (int counter = 0; counter < ImageShow.modified.Width * ImageShow.modified.Height * 3; counter++)
                        rgbValues[counter] = data[counter];

                System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

                ImageShow.modified.UnlockBits(bmpData);

                ImageShow.showModified = 1;
            }
            return (t2 - t1).TotalMilliseconds;
        }

        public static double callColorHist()
        {
            DateTime t1 = DateTime.Now, t2 = t1;
            if (ImageShow.nowFile != null)
            {
                ImageShow.source = new Bitmap(ImageShow.nowFile);
                Rectangle rect = new Rectangle(0, 0, ImageShow.source.Width, ImageShow.source.Height);
                System.Drawing.Imaging.BitmapData bmpData = ImageShow.source.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, ImageShow.source.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * ImageShow.source.Height;
                byte[] rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                t1 = DateTime.Now;
                colorHist(ImageShow.source.Width, ImageShow.source.Height, bytes, rgbValues, colorHistData);
                t2 = DateTime.Now;

                colorHistExist = true;

                ImageShow.source.UnlockBits(bmpData);
            }
            return (t2 - t1).TotalMilliseconds;
        }

        public static void calculateBoundingBox(float[] ptX, float[] ptY)
        {
            boundingBox.Clear();
            boundingBox.Add(new PointF(ptX[0], ptY[0]));
            boundingBox.Add(new PointF(ptX[1], ptY[1]));
            boundingBox.Add(new PointF(ptX[2], ptY[2]));
            boundingBox.Add(new PointF(ptX[3], ptY[3]));
        }

        public static double callThreshold()
        {
            DateTime t1 = DateTime.Now, t2 = t1;
            if (ImageShow.nowFile != null)
            {
                ImageShow.source = new Bitmap(ImageShow.nowFile);
                Rectangle rect = new Rectangle(0, 0, ImageShow.source.Width, ImageShow.source.Height);
                System.Drawing.Imaging.BitmapData bmpData = ImageShow.source.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, ImageShow.source.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * ImageShow.source.Height;
                byte[] rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                float[] ptX = new float[4];
                float[] ptY = new float[4];

                t1 = DateTime.Now;
                threshold(ImageShow.source.Width, ImageShow.source.Height, bytes, rgbValues, lowThr, highThr, 4, ptX, ptY);
                t2 = DateTime.Now;

                calculateBoundingBox(ptX, ptY);

                int channel = bmpData.Stride / bmpData.Width;
                if (channel != 1)
                {
                    for (int i = ImageShow.source.Width * ImageShow.source.Height - 1; i >= 0; i--)
                        for (int j = channel - 1; j >= 0; j--)
                            rgbValues[i * channel + j] = rgbValues[i];
                }

                System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

                ImageShow.source.UnlockBits(bmpData);
            }
            return (t2 - t1).TotalMilliseconds;
        }

        public static double callSobel()
        {
            DateTime t1 = DateTime.Now, t2 = t1;

            if (ImageShow.nowFile != null)
            {
                ImageShow.source = new Bitmap(ImageShow.nowFile);
                Rectangle rect = new Rectangle(0, 0, ImageShow.source.Width, ImageShow.source.Height);
                System.Drawing.Imaging.BitmapData bmpData = ImageShow.source.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, ImageShow.source.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * ImageShow.source.Height;
                byte[] rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                if (sobelOTSU)
                    sobelThr = -1;

                int[] edgeX = new int[bytes];
                int[] edgeY = new int[bytes];
                int edgeNum = 0;

                t1 = DateTime.Now;
                sobel(ImageShow.source.Width, ImageShow.source.Height, bytes, rgbValues, sobelKsize, ref sobelThr, ref edgeNum, edgeX, edgeY);
                t2 = DateTime.Now;

                while (ImageShow.edgeDots != null && ImageShow.edgeDots.Count > 0)
                    ImageShow.edgeDots.RemoveAt(ImageShow.edgeDots.Count - 1);
                for (int i = 0; i < edgeNum; i++)
                    ImageShow.addDot(new Point(edgeX[i], edgeY[i]));
                ImageShow.source.UnlockBits(bmpData);
            }

            return (t2 - t1).TotalMilliseconds;
        }

        public static double callCanny()
        {
            DateTime t1 = DateTime.Now, t2 = t1;

            if (ImageShow.nowFile != null)
            {
                ImageShow.source = new Bitmap(ImageShow.nowFile);
                Rectangle rect = new Rectangle(0, 0, ImageShow.source.Width, ImageShow.source.Height);
                System.Drawing.Imaging.BitmapData bmpData = ImageShow.source.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, ImageShow.source.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * ImageShow.source.Height;
                byte[] rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                int[] edgeX = new int[bytes];
                int[] edgeY = new int[bytes];
                int edgeNum = 0;

                t1 = DateTime.Now;
                canny(ImageShow.source.Width, ImageShow.source.Height, bytes, rgbValues, cannyLowThr, cannyHighThr, ref edgeNum, edgeX, edgeY);
                t2 = DateTime.Now;

                while (ImageShow.edgeDots != null && ImageShow.edgeDots.Count > 0)
                    ImageShow.edgeDots.RemoveAt(ImageShow.edgeDots.Count - 1);
                for (int i = 0; i < edgeNum; i++)
                    ImageShow.addDot(new Point(edgeX[i], edgeY[i]));
                ImageShow.source.UnlockBits(bmpData);
            }

            return (t2 - t1).TotalMilliseconds;
        }

        public static double callLSD()
        {
            DateTime t1 = DateTime.Now, t2 = t1;

            if (ImageShow.nowFile != null)
            {
                ImageShow.source = new Bitmap(ImageShow.nowFile);
                Rectangle rect = new Rectangle(0, 0, ImageShow.source.Width, ImageShow.source.Height);
                System.Drawing.Imaging.BitmapData bmpData = ImageShow.source.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, ImageShow.source.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * ImageShow.source.Height;
                byte[] rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                int[] edgeX = new int[bytes];
                int[] edgeY = new int[bytes];
                int edgeNum = 0;

                t1 = DateTime.Now;
                LSD(ImageShow.source.Width, ImageShow.source.Height, bytes, rgbValues, LSDStep, LSDThr, ref edgeNum, edgeX, edgeY);
                t2 = DateTime.Now;

                while (ImageShow.edgeDots != null && ImageShow.edgeDots.Count > 0)
                    ImageShow.edgeDots.RemoveAt(ImageShow.edgeDots.Count - 1);
                for (int i = 0; i < edgeNum; i++)
                    ImageShow.addDot(new Point(edgeX[i], edgeY[i]));
                ImageShow.source.UnlockBits(bmpData);
            }

            return (t2 - t1).TotalMilliseconds;
        }
    }
}
