using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SIMproject
{
    class DefectDetect
    {
        /// <summary>
        /// 直线检测接口
        /// </summary>
        /// <param name="filename">图像文件路径</param>
        /// <param name="X0">线段一个顶点坐标</param>
        /// <param name="Y0">线段一个顶点坐标</param>
        /// <param name="X1">线段另一个顶点坐标</param>
        /// <param name="Y1">线段另一个顶点坐标</param>
        /// <param name="angle">检测结果直线倾角</param>
        /// <param name="searchWidth">检测范围，在该像素范围内校正直线方程</param>
        /// <returns></returns>
        [DllImport("DefectDetect.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#2")]
        public static extern float getAccuracyLine(string filename, ref float X0, ref float Y0, ref float X1, ref float Y1, ref float angle, int searchWidth, int lineMaxOffset);

        /// <summary>
        /// 缺陷检测接口
        /// </summary>
        /// <param name="tempfilename">模板图像文件路径</param>
        /// <param name="objfilename">目标图像文件路径</param>
        /// <param name="tempx1">模板顶点1坐标</param>
        /// <param name="tempy1">模板顶点1坐标</param>
        /// <param name="tempx2">模板顶点2坐标</param>
        /// <param name="tempy2">模板顶点2坐标</param>
        /// <param name="tempx3">模板顶点3坐标</param>
        /// <param name="tempy3">模板顶点3坐标</param>
        /// <param name="tempx4">模板顶点4坐标</param>
        /// <param name="tempy4">模板顶点4坐标</param>
        /// <param name="objx1">目标顶点1坐标</param>
        /// <param name="objy1">目标顶点1坐标</param>
        /// <param name="objx2">目标顶点2坐标</param>
        /// <param name="objy2">目标顶点2坐标</param>
        /// <param name="objx3">目标顶点3坐标</param>
        /// <param name="objy3">目标顶点3坐标</param>
        /// <param name="objx4">目标顶点4坐标</param>
        /// <param name="objy4">目标顶点4坐标</param>
        /// <param name="colorThr">颜色阈值</param>
        /// <param name="areaThr">尺寸（面积）阈值</param>
        /// <param name="resultNum">缺陷数量</param>
        /// <param name="X">缺陷矩形区域参数</param>
        /// <param name="Y">缺陷矩形区域参数</param>
        /// <param name="W">缺陷矩形区域参数</param>
        /// <param name="H">缺陷矩形区域参数</param>
        [DllImport("DefectDetect.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#1")]
        public static extern void defectDetect(string tempfilename, string objfilename,
                   float tempx1, float tempy1, float tempx2, float tempy2, float tempx3, float tempy3, float tempx4, float tempy4,
                   float objx1, float objy1, float objx2, float objy2, float objx3, float objy3, float objx4, float objy4,
                   int colorThr, int areaThr, ref int resultNum,
             [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 21)] int[] X,
             [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 21)] int[] Y,
             [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 21)] int[] W,
             [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 21)] int[] H);

        public static int lineMaxOffset = 1;
        public static float lineIntegrity = 0;
        public static bool selectLineState = false;
        public static int selectLineIndex = -1;
        public static double runtime = 0;

        public static int tempSelectPointState = 0;
        public static int selectPointState = 0;
        public static List<PointF> tempSelectPointIndex = new List<PointF>();
        public static List<PointF> selectPointIndex = new List<PointF>();
        public static string templateFile = null;
        public static string objectFile = null;

        public static int colorThr = 50;
        public static int areaThr = 100;

        public static void detectLine()
        {
            DateTime t1 = DateTime.Now, t2 = t1;
            if (ImageShow.nowFile!=null)
            {
                float X0 = ImageShow.shapes[selectLineIndex].P1.X;
                float Y0 = ImageShow.shapes[selectLineIndex].P1.Y;
                float X1 = ImageShow.shapes[selectLineIndex].P2.X;
                float Y1 = ImageShow.shapes[selectLineIndex].P2.Y;
                float angle = 0;

                t1 = DateTime.Now;
                lineIntegrity = getAccuracyLine(ImageShow.nowFile, ref X0, ref Y0, ref X1, ref Y1, ref angle, Config.lineFixRange, lineMaxOffset);
                t2 = DateTime.Now;

                ImageShow.shapes[selectLineIndex].setPartPara(1, new PointF(X0, Y0), new PointF(X1, Y1), angle);
            }
            runtime = (t2-t1).TotalMilliseconds;
        }

        public static double detectDefect(ref int isOK)
        {
            DateTime t1 = DateTime.Now, t2 = t1;

            //tempSelectPointIndex.Add(new PointF((float)1152.76, (float)1154.42));
            //tempSelectPointIndex.Add(new PointF((float)1506.53, (float)1118.48));
            //tempSelectPointIndex.Add(new PointF((float)1503.34, (float)1347.21));
            //tempSelectPointIndex.Add(new PointF((float)1166.15, (float)1367.25));

            //selectPointIndex.Add(new PointF((float)1079.04, (float)1087.97));
            //selectPointIndex.Add(new PointF((float)1390.66, (float)1264.99));
            //selectPointIndex.Add(new PointF((float)1254.86, (float)1449.18));
            //selectPointIndex.Add(new PointF((float)966.40, (float)1268.93));

            if (templateFile != null && objectFile != null && tempSelectPointIndex.Count() == 4 && selectPointIndex.Count() == 4)
            {
                int resultNum = 1000;
                int[] X = new int[resultNum];
                int[] Y = new int[resultNum];
                int[] W = new int[resultNum];
                int[] H = new int[resultNum];
                
                t1 = DateTime.Now;
                defectDetect(templateFile, objectFile,
                            tempSelectPointIndex[0].X, tempSelectPointIndex[0].Y,
                            tempSelectPointIndex[1].X, tempSelectPointIndex[1].Y,
                            tempSelectPointIndex[2].X, tempSelectPointIndex[2].Y,
                            tempSelectPointIndex[3].X,tempSelectPointIndex[3].Y,
                            selectPointIndex[0].X, selectPointIndex[0].Y,
                            selectPointIndex[1].X, selectPointIndex[1].Y,
                            selectPointIndex[2].X, selectPointIndex[2].Y,
                            selectPointIndex[3].X, selectPointIndex[3].Y,
                            colorThr, areaThr, ref resultNum, X, Y, W, H);
                t2 = DateTime.Now;

                for (int i = 0; i < resultNum; i++)
                    ImageShow.addRect(new PointF((float)(X[i] + (double)W[i] / 2.0), (float)(Y[i] + (double)H[i] / 2.0)), new PointF((float)X[i], (float)Y[i]), 0, false);

                if (resultNum > 0) isOK = 2;
                else isOK = 1;
            }
            return (t2 - t1).TotalMilliseconds;
        }
    }
}
