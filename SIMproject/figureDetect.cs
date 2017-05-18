using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace SIMproject
{
    class figureDetect
    {
        [DllImport("ShapeDetect.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#1")]
        public static extern void areaDetect(int width, int height, int imgSize,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data,
            int lowArea, int highArea, int targetColor, int mode, ref int resultNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 9)] float[] resultX,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 9)] float[] resultY,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 9)] int[] area);

        /// <summary>
        /// 圆形检测接口
        /// </summary>
        /// <param name="filename">图像文件路径</param>
        /// <param name="low">半径下限</param>
        /// <param name="high">半径上限</param>
        /// <param name="cannyThr">Canny边缘检测阈值</param>
        /// <param name="weightThr">权重阈值</param>
        /// <param name="resultNum">检测结果数量</param>
        /// <param name="resultX">检测结果坐标</param>
        /// <param name="resultY">检测结果坐标</param>
        /// <param name="resultR">检测结果半径</param>
        [DllImport("ShapeDetect.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#2")]
        public static extern void circleDetect(string filename, double low, double high,
            double cannyThr, double weightThr, ref int resultNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)] float[] resultX,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)] float[] resultY,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)] float[] resultR,
            int colorLowThr, int colorHighThr, int rate);

        /// <summary>
        /// Canny边缘检测接口
        /// </summary>
        /// <param name="filename">图像文件路径</param>
        /// <param name="cannyThr">Canny边缘检测阈值</param>
        /// <param name="imgSize">图像尺寸</param>
        /// <param name="data">边缘检测结果图像</param>
        [DllImport("ShapeDetect.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#3")]
        public static extern void edgeDetect(string filename, double cannyThr, int imgSize,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data);

        /// <summary>
        /// 矩形检测接口
        /// </summary>
        /// <param name="filename">图像文件路径</param>
        /// <param name="width">矩形宽度</param>
        /// <param name="height">矩形高度</param>
        /// <param name="cannyThr">Canny边缘检测阈值</param>
        /// <param name="resultNum">检测结果数量</param>
        /// <param name="resultData">检测结果数据，每5个值表示一个矩形，分别为矩形位置尺寸和角度</param>
        [DllImport("ShapeDetect.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#5")]
        public static extern void rectDetect(string filename, double width, double height, double range, ref int resultNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] float[] resultData,
            int colorLowThr, int colorHighThr, int rate, int edgeType, int para1, int para2);

        /// <summary>
        /// 连续边缘检测接口
        /// </summary>
        /// <param name="filename">图像文件路径</param>
        /// <param name="step">步长</param>
        /// <param name="thr">阈值</param>
        /// <param name="imgSize">图像尺寸</param>
        /// <param name="data">边缘检测结果图像</param>
        [DllImport("ShapeDetect.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#4")]
        public static extern void newEdgeDetect(string filename, int step, int thr, int imgSize,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] data);

        [DllImport("ShapeDetect.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#6")]
        public static extern void triangleDetect(int width, int height, int imgSize,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data,
            float angle1, float angle2, float angle3, float range,int thr, float minLength, float maxGap, ref int resultNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 12)] float[] resultX,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 12)] float[] resultY);

        public static bool setCircleParaState = false;
        public static bool setRectParaState = false;
        public static int shapeIndex = 0;
        public static double edgeDetectPara = 140;
        public static int edgeDetectStep = 5;
        public static int edgeDetectThr = 10;

        public static int colorLowThr = 0;
        public static int colorHighThr = 255;
        public static int colorRate = 0;

        public static double rectWHRange = 10;
        public static double circleRadiusRange = 10;

        public static int lowArea = 100;
        public static int highArea = 1000;
        public static int targetColor = 255;
        public static int mode = 0;
        public static bool showBoundingBox = false;
        public static List<PointF> boundingBox = new List<PointF>();
        public static List<int> boundingBoxArea = new List<int>();

        public static float angle1 = 60, angle2 = 60, angle3 = 60, range = 50;
        public static int triangleLineThr = 20;
        public static float minLength = 30, maxGap = 20;

        public static void calculateBoundingBox(float[] ptX, float[] ptY, int[] area, int index)
        {
            boundingBox.Add(new PointF(ptX[index + 0], ptY[index + 0]));
            boundingBox.Add(new PointF(ptX[index + 1], ptY[index + 1]));
            boundingBox.Add(new PointF(ptX[index + 2], ptY[index + 2]));
            boundingBox.Add(new PointF(ptX[index + 3], ptY[index + 3]));
            boundingBoxArea.Add(area[index / 4]);
        }

        public static double edgeDetectFun()
        {
            DateTime t1 = DateTime.Now, t2 = t1;
            int imgSize = ImageShow.source.Width * ImageShow.source.Height;
            byte[] data = new byte[imgSize];

            t1 = DateTime.Now;

            edgeDetect(ImageShow.nowFile, edgeDetectPara, imgSize, data);

            t2 = DateTime.Now;

            for (int i=0;i<imgSize;i++)
                if (data[i] == 255)
                    ImageShow.addDot(new Point(i % ImageShow.source.Width, i / ImageShow.source.Width));

            return (t2 - t1).TotalMilliseconds;
        }

        public static double newEdgeDetectFun()
        {
            DateTime t1 = DateTime.Now, t2 = t1;
            int imgSize = ImageShow.source.Width * ImageShow.source.Height;
            byte[] data = new byte[imgSize];

            t1 = DateTime.Now;

            newEdgeDetect(ImageShow.nowFile, edgeDetectStep, edgeDetectThr, imgSize, data);

            t2 = DateTime.Now;

            for (int i = 0; i < imgSize; i++)
                if (data[i] == 254)
                    ImageShow.addDot(new Point(i % ImageShow.source.Width, i / ImageShow.source.Width));

            return (t2 - t1).TotalMilliseconds;
        }

        public static double callAreaDetect()
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

                int resultNum = 1000;
                float[] resultX = new float[resultNum];
                float[] resultY = new float[resultNum];
                int[] area = new int[resultNum];

                t1 = DateTime.Now;
                areaDetect(ImageShow.source.Width, ImageShow.source.Height, bytes, rgbValues, lowArea, highArea, targetColor, mode, ref resultNum, resultX, resultY, area);
                t2 = DateTime.Now;

                boundingBox.Clear();
                boundingBoxArea.Clear();
                for (int i = 0; i < resultNum; i++)
                    calculateBoundingBox(resultX, resultY, area, i * 4);
                showBoundingBox = true;

                ImageShow.source.UnlockBits(bmpData);
            }

            return (t2 - t1).TotalMilliseconds;
        }

        public static double callTriangleDetect()
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

                int resultNum = 1000;
                float[] resultX = new float[resultNum*3];
                float[] resultY = new float[resultNum*3];

                t1 = DateTime.Now;
                triangleDetect(ImageShow.source.Width, ImageShow.source.Height, bytes, rgbValues, angle1, angle2, angle3, range, triangleLineThr, minLength, maxGap, ref resultNum, resultX, resultY);
                t2 = DateTime.Now;

                for (int i = 0; i < resultNum; i++)
                {
                    //Console.WriteLine(resultX[i * 3].ToString()+","+resultY[i * 3].ToString()+","+resultX[i * 3 + 1].ToString()+","+ resultY[i * 3 + 1].ToString()+","+resultX[i * 3 + 2].ToString()+","+ resultY[i * 3 + 2].ToString());
                    ImageShow.addTriangle(new PointF(resultX[i * 3], resultY[i * 3]), new PointF(resultX[i * 3 + 1], resultY[i * 3 + 1]), new PointF(resultX[i * 3 + 2], resultY[i * 3 + 2]));
                }

                ImageShow.source.UnlockBits(bmpData);
            }

            return (t2 - t1).TotalMilliseconds;
        }
    }

}
