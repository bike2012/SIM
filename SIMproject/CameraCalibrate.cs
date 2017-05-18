using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace SIMproject
{
    class CameraCalibrate
    {
        static public int extractedNum = 0;
        static public int maxExtractedNum = 20;
        static public float[] cornerX;
        static public float[] cornerY;

        static public int nowCornerNum;
        static public float[] nowCornerX;
        static public float[] nowCornerY;

        static public float[] saveCornerX;
        static public float[] saveCornerY;

        static public bool isUpdated = false;

        static public int manualExtractState = 0;
        static public PointF[] manualCorner = new PointF[4];
        static public int boardWidth = 6;
        static public int boardHeight = 4;
        static public float boardSize = 20;

        static private int imgWidth = 640;
        static private int imgHeight = 480;

        static public float CM11 = 0, CM12 = 0, CM13 = 0, CM21 = 0, CM22 = 0, CM23 = 0, CM31 = 0, CM32 = 0, CM33 = 0;
        static public float D1 = 0, D2 = 0, D3 = 0, D4 = 0, D5 = 0;

        static public bool savePixelSize = false;
        static public float pixelSize = 0;

        static public int paraNum = 30;
        static public float[] para;

        static public int fixSize = 11;
        static public int manualFixSize = 3;


        /// <summary>
        /// 手动辅助选取角点接口
        /// </summary>
        /// <param name="filename">图像文件路径</param>
        /// <param name="cornerNum">角点数量</param>
        /// <param name="cornerX">角点X坐标，同时前4个值用于输入手动辅助角点坐标</param>
        /// <param name="cornerY">角点Y坐标，同时前4个值用于输入手动辅助角点坐标</param>
        /// <param name="boardWidth">标定板宽度</param>
        /// <param name="boardHeight">标定板高度</param>
        /// <returns></returns>
        [DllImport("CameraCalibration.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#2")]
        public static extern bool manualExtract(string filename, ref int cornerNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] float[] cornerX,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] float[] cornerY,
            int boardWidth, int boardHeight, int manualFixSize, int fixSize);

        /// <summary>
        /// 自动选取角点接口
        /// </summary>
        /// <param name="filename">图像文件路径</param>
        /// <param name="cornerNum">角点数量</param>
        /// <param name="cornerX">角点X坐标</param>
        /// <param name="cornerY">角点Y坐标</param>
        /// <param name="boardWidth">标定板宽度</param>
        /// <param name="boardHeight">标定板高度</param>
        /// <returns></returns>
        [DllImport("CameraCalibration.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#1")]
        public static extern bool autoExtract(string filename, ref int cornerNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] float[] cornerX,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] float[] cornerY,
            int boardWidth, int boardHeight, int fixSize);

        /// <summary>
        /// 相机标定接口
        /// </summary>
        /// <param name="imgNum">标定图像数量</param>
        /// <param name="imgWidth">图像宽度</param>
        /// <param name="imgHeight">图像高度</param>
        /// <param name="cornerNum">角点数量</param>
        /// <param name="cornerX">所有图像所有角点的X坐标</param>
        /// <param name="cornerY">所有图像所有角点的Y坐标</param>
        /// <param name="boardWidth">标定板宽度</param>
        /// <param name="boardHeight">标定板高度</param>
        /// <param name="boardSize">标定板格子尺寸</param>
        /// <param name="resultNum">参数维数</param>
        /// <param name="result">标定结果，前9个值为内参，之后为失真</param>
        [DllImport("CameraCalibration.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#3")]
        public static extern void runCalibrate(int imgNum, int imgWidth, int imgHeight, int cornerNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] float[] cornerX,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] float[] cornerY,
            int boardWidth, int boardHeight, float boardSize, ref int resultNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 10)] float[] result);

        /// <summary>
        /// 图像校正接口
        /// </summary>
        /// <param name="filename">图像文件路径</param>
        /// <param name="size">图像像素点数</param>
        /// <param name="data">校正后的图像</param>
        /// <param name="paraNum">参数维数</param>
        /// <param name="para">标定结果参数</param>
        /// <param name="channel">图像深度</param>
        [DllImport("CameraCalibration.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#4")]
        public static extern void runUndistort(string filename,
            int size, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] data,
            int paraNum, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] float[] para, ref int channel);

        [DllImport("CameraCalibration.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#5")]
        public static extern float zAffineMat(int cornerNum,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] float[] cornerX,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] float[] cornerY,
            int boardWidth, int boardHeight, float boardSize, float x1,float y1,float x2,float y2);


        public static double runAutoExtract()
        {
            DateTime t1 = DateTime.Now, t2 = t1;
            ImageShow.cls();

            if (ImageShow.nowFile != null)
            {
                int w = boardWidth;
                int h = boardHeight;
                int cornerNum = w * h;
                float[] cornerX = new float[cornerNum], cornerY = new float[cornerNum];

                t1 = DateTime.Now;

                CameraCalibrate.autoExtract(ImageShow.nowFile, ref cornerNum, cornerX, cornerY, w, h, fixSize);

                t2 = DateTime.Now;

                CameraCalibrate.nowCornerNum = cornerNum;
                if (CameraCalibrate.nowCornerX == null)
                {
                    CameraCalibrate.nowCornerX = new float[cornerNum];
                    CameraCalibrate.nowCornerY = new float[cornerNum];
                }
                for (int i = 0; i < cornerNum; i++)
                {
                    //float x = cornerX[i] * ImageShow.position.Width / ImageShow.initPosition.Width + ImageShow.position.X;
                    //float y = cornerY[i] * ImageShow.position.Height / ImageShow.initPosition.Height + ImageShow.position.Y;
                    //float r = 10 * ImageShow.scale;
                    ImageShow.addPoint(new PointF(cornerX[i], cornerY[i]));
                    CameraCalibrate.nowCornerX[i] = cornerX[i];
                    CameraCalibrate.nowCornerY[i] = cornerY[i];
                }
                CameraCalibrate.isUpdated = true;

            }
            return (t2 - t1).TotalMilliseconds;
        }

        public static double runManualExtract()
        {
            DateTime t1 = DateTime.Now, t2 = t1;
            ImageShow.cls();

            if (ImageShow.nowFile != null)
            {
                int w = boardWidth;
                int h = boardHeight;
                int cornerNum = w * h;
                float[] cornerX = new float[cornerNum], cornerY = new float[cornerNum];
                for (int i = 0; i < 4; i++)
                {
                    cornerX[i] = manualCorner[i].X;
                    cornerY[i] = manualCorner[i].Y;
                }

                t1 = DateTime.Now;

                CameraCalibrate.manualExtract(ImageShow.nowFile, ref cornerNum, cornerX, cornerY, w, h, manualFixSize, fixSize);

                t2 = DateTime.Now;

                CameraCalibrate.nowCornerNum = cornerNum;
                if (CameraCalibrate.nowCornerX == null)
                {
                    CameraCalibrate.nowCornerX = new float[cornerNum];
                    CameraCalibrate.nowCornerY = new float[cornerNum];
                }
                for (int i = 0; i < cornerNum; i++)
                {
                    //float x = cornerX[i] * ImageShow.position.Width / ImageShow.initPosition.Width + ImageShow.position.X;
                    //float y = cornerY[i] * ImageShow.position.Height / ImageShow.initPosition.Height + ImageShow.position.Y;
                    //float r = 10 * ImageShow.scale;
                    ImageShow.addPoint(new PointF(cornerX[i], cornerY[i]));
                    CameraCalibrate.nowCornerX[i] = cornerX[i];
                    CameraCalibrate.nowCornerY[i] = cornerY[i];
                }
                CameraCalibrate.isUpdated = true;
            }

            return (t2 - t1).TotalMilliseconds;
        }

        public static bool extractAccept()
        {
            if (CameraCalibrate.isUpdated)
            {
                int w = boardWidth;
                int h = boardHeight;
                int cornerNum = w * h;
                if (CameraCalibrate.nowCornerNum == cornerNum)
                {
                    if (CameraCalibrate.extractedNum == 0)
                    {
                        imgWidth = ImageShow.source.Width;
                        imgHeight = ImageShow.source.Height;
                        CameraCalibrate.cornerX = new float[cornerNum * CameraCalibrate.maxExtractedNum];
                        CameraCalibrate.cornerY = new float[cornerNum * CameraCalibrate.maxExtractedNum];
                    }
                    for (int i = 0; i < cornerNum; i++)
                    {
                        CameraCalibrate.cornerX[CameraCalibrate.extractedNum * cornerNum + i] = CameraCalibrate.nowCornerX[i];
                        CameraCalibrate.cornerY[CameraCalibrate.extractedNum * cornerNum + i] = CameraCalibrate.nowCornerY[i];
                    }

                    if (!CameraCalibrate.savePixelSize)
                    {
                        saveCornerX = new float[cornerNum];
                        saveCornerY = new float[cornerNum];
                        for (int i = 0; i < cornerNum; i++)
                        {
                            saveCornerX[i] = CameraCalibrate.nowCornerX[i];
                            saveCornerY[i] = CameraCalibrate.nowCornerY[i];
                        }

                        double sum = 0;
                        int num = 0;
                        for (int i = 0; i < boardHeight; i++)
                            for (int j = 0; j < boardWidth - 1; j++)
                            {
                                sum += Math.Sqrt((double)((nowCornerX[i * boardWidth + j] - nowCornerX[i * boardWidth + j + 1]) * (nowCornerX[i * boardWidth + j] - nowCornerX[i * boardWidth + j + 1]) +
                                    (nowCornerY[i * boardWidth + j] - nowCornerY[i * boardWidth + j + 1]) * (nowCornerY[i * boardWidth + j] - nowCornerY[i * boardWidth + j + 1])));
                                num++;
                            }
                        for (int j = 0; j < boardWidth; j++)
                            for (int i = 0; i < boardHeight - 1; i++)
                            {
                                num++;
                                sum += Math.Sqrt((double)((nowCornerX[i * boardWidth + j] - nowCornerX[(i + 1) * boardWidth + j]) * (nowCornerX[i * boardWidth + j] - nowCornerX[(i + 1) * boardWidth + j]) +
                                    (nowCornerY[i * boardWidth + j] - nowCornerY[(i + 1) * boardWidth + j]) * (nowCornerY[i * boardWidth + j] - nowCornerY[(i + 1) * boardWidth + j])));
                            }
                        pixelSize = boardSize / (float)(sum / num);
                    }
                    CameraCalibrate.extractedNum++;

                    CameraCalibrate.isUpdated = false;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public static double calibrate()
        {
            DateTime t1 = DateTime.Now, t2 = t1;

            paraNum = 30;
            para = new float[paraNum];

            t1 = DateTime.Now;

            runCalibrate(extractedNum, imgWidth, imgHeight, extractedNum * boardHeight * boardWidth, cornerX, cornerY, boardWidth, boardHeight, boardSize, ref paraNum, para);

            t2 = DateTime.Now;

            CM11 = para[0]; CM12 = para[1]; CM13 = para[2];
            CM21 = para[3]; CM22 = para[4]; CM23 = para[5];
            CM31 = para[6]; CM32 = para[7]; CM33 = para[8];
            if (paraNum == 14)
            {
                D1 = para[9]; D2 = para[10]; D3 = para[11]; D4 = para[12]; D5 = para[13];
            }

            return (t2 - t1).TotalMilliseconds;
        }

        public static double undistort()
        {
            DateTime t1 = DateTime.Now, t2 = t1;

            ImageShow.cls();

            if (ImageShow.nowFile != null)
            {
                byte[] data = new byte[ImageShow.source.Width * ImageShow.source.Height * 3];
                int channel = 3;

                t1 = DateTime.Now;

                runUndistort(ImageShow.nowFile, ImageShow.source.Width * ImageShow.source.Height * 3, data, paraNum, para, ref channel);

                t2 = DateTime.Now;

                Rectangle rect = new Rectangle(0, 0, ImageShow.source.Width, ImageShow.source.Height);
                System.Drawing.Imaging.BitmapData bmpData = ImageShow.source.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, ImageShow.source.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * ImageShow.source.Height;
                byte[] rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                channel = Bitmap.GetPixelFormatSize(ImageShow.source.PixelFormat);
                if (channel == 8)
                {
                    for (int counter = 0; counter < ImageShow.source.Width * ImageShow.source.Height; counter++)
                        rgbValues[counter] = data[counter*3];
                }
                else
                    for (int counter = 0; counter < ImageShow.source.Width * ImageShow.source.Height * 3; counter++)
                        rgbValues[counter] = data[counter];
        
                System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

                ImageShow.source.UnlockBits(bmpData);
            }
            return (t2 - t1).TotalMilliseconds;

        }

        public static float runMeasure(float x1,float y1,float x2,float y2)
        {
            return zAffineMat(boardWidth * boardHeight, saveCornerX, saveCornerY, boardWidth, boardHeight, boardSize, x1, y1, x2, y2);
        }
    }
}
