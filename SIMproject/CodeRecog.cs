using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ZXing;

namespace SIMproject
{
    class CodeRecog
    {
        /// <summary>
        /// 维码识别接口
        /// </summary>
        /// <param name="filename">图像文件路径</param>
        /// <param name="type">1为一维码，2为二维码</param>
        /// <param name="typeLength">维码类型字符串长度</param>
        /// <param name="contentLength">维码内容字符串长度</param>
        /// <param name="codeType">维码类型字符串</param>
        /// <param name="codeContent">维码内容字符串</param>
        /// <returns></returns>
        [DllImport("Decoder.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "#1")]
        public static extern int decode(string filename, int type,
            ref int codeNum, int maxLength,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] int[] typeLength,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] int[] contentLength,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] codeType,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] codeContent, int rotateMode);

        RectangleF ROI = new RectangleF(0, 0, 1, 1);

        public static int setRectParaState = 0;
        public static int shapeIndex = -1;

        public static int codeNum = 0;
        public static int maxCodeNum = 100;
        public static int maxLength = 1000;
        public static List<string> codeType = new List<string>();
        public static List<string> codeContent = new List<string>();
        public static int rotateMode = 1;

        public static BarcodeReader reader = null;

        private static Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(source, section.Width, section.Height);

            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);
            System.Drawing.Imaging.BitmapData bmpData = source.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, source.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            int bytes = Math.Abs(bmpData.Stride) * source.Height;
            byte[] rgbValues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            Rectangle rect2 = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData2 = bmp.LockBits(rect2, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);

            IntPtr ptr2 = bmpData2.Scan0;

            int bytes2 = Math.Abs(bmpData2.Stride) * bmp.Height;
            byte[] rgbValues2 = new byte[bytes2];

            System.Runtime.InteropServices.Marshal.Copy(ptr2, rgbValues2, 0, bytes2);

            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmpData2.Stride; j++)
                {
                    if (j % 4 == 3) rgbValues2[i * bmpData2.Stride + j] = 255;
                    else
                        rgbValues2[i * bmpData2.Stride + j] = rgbValues[(i + section.Y) * bmpData.Stride + j * (bmpData.Stride / bmpData.Width) / (bmpData2.Stride / bmpData2.Width) + section.X * bmpData.Stride / bmpData.Width];
                }

            System.Runtime.InteropServices.Marshal.Copy(rgbValues2, 0, ptr2, bytes2);
            source.UnlockBits(bmpData);
            bmp.UnlockBits(bmpData2);

            return bmp;
        }

        public static double codeRecog(int type)
        {
            DateTime t1 = DateTime.Now, t2 = t1;

            float r = ImageShow.shapes[shapeIndex].R;
            float x1 = ImageShow.shapes[shapeIndex].P1.X;
            float y1 = ImageShow.shapes[shapeIndex].P1.Y;
            float x2 = ImageShow.shapes[shapeIndex].P2.X;
            float y2 = ImageShow.shapes[shapeIndex].P2.Y;

            PointF[] pts = new PointF[4];
            pts[0] = new PointF(x2, y2);
            double a = Math.Sin((double)r), b = -Math.Cos((double)r), c = 0 - x1 * a - y1 * b;
            pts[1] = new PointF((float)(x2 - 2 * a * ((a * x2 + b * y2 + c) / (a * a + b * b))), (float)(y2 - 2 * b * ((a * x2 + b * y2 + c) / (a * a + b * b))));
            pts[2] = new PointF((x1 * 2 - x2), (y1 * 2 - y2));
            a = Math.Sin((double)(r - Math.PI / 2.0)); b = -Math.Cos((double)(r - Math.PI / 2.0)); c = 0 - x1 * a - y1 * b;
            pts[3] = new PointF((float)(x2 - 2 * a * ((a * x2 + b * y2 + c) / (a * a + b * b))), (float)(y2 - 2 * b * ((a * x2 + b * y2 + c) / (a * a + b * b))));


            float x, y, width, height;
            x = Math.Min(pts[0].X, Math.Min(pts[1].X, Math.Min(pts[2].X, pts[3].X)));
            y = Math.Min(pts[0].Y, Math.Min(pts[1].Y, Math.Min(pts[2].Y, pts[3].Y)));
            width = Math.Max(pts[0].X, Math.Max(pts[1].X, Math.Max(pts[2].X, pts[3].X))) - x + 1;
            height = Math.Max(pts[0].Y, Math.Max(pts[1].Y, Math.Max(pts[2].Y, pts[3].Y))) - y + 1;

            if (x < 0)
            {
                width += x;
                x = 0;
            }
            if (y < 0)
            {
                height += y;
                y = 0;
            }
            

            Bitmap source = new Bitmap(ImageShow.nowFile);

            if (x + width >= source.Width)
                width = source.Width - x - 1;
            if (y + height >= source.Height)
                height = source.Height - y - 1;

            Rectangle section = new Rectangle(new Point((int)x, (int)y), new Size((int)width, (int)height));

            Bitmap CroppedImage = CropImage(source, section);

            string tempfile = Environment.CurrentDirectory + '\\' + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString().Replace(":", "_") + ".bmp";
            CroppedImage.Save(tempfile);
            
            codeNum = maxCodeNum;
            codeType.Clear();
            codeContent.Clear();
            int[] typeLength = new int[codeNum];
            int[] contentLength = new int[codeNum];
            byte[] codeTypeCh = new byte[maxLength * codeNum], codeContentCh = new byte[maxLength * codeNum];

            t1 = DateTime.Now;

            if (decode(tempfile, type, ref codeNum, maxLength*codeNum,typeLength, contentLength, codeTypeCh, codeContentCh,rotateMode) >0)
            {                
                for (int j = 0; j < codeNum; j++)
                {
                    string tempType = "";
                    for (int i = 0; i < typeLength[j]; i++)
                        tempType = tempType + (char)codeTypeCh[i+ j * maxLength];
                    string tempContent = "";
                    for (int i = 0; i < contentLength[j]; i++)
                        tempContent = tempContent + (char)codeContentCh[i + j * maxLength];
                    codeType.Add(tempType);
                    codeContent.Add(tempContent);
                }
            }

            t2 = DateTime.Now;

            return (t2 - t1).TotalMilliseconds;
        }

        public static double ZXingCodeRecog()
        {
            DateTime t1 = DateTime.Now, t2 = t1;

            float r = ImageShow.shapes[shapeIndex].R;
            float x1 = ImageShow.shapes[shapeIndex].P1.X;
            float y1 = ImageShow.shapes[shapeIndex].P1.Y;
            float x2 = ImageShow.shapes[shapeIndex].P2.X;
            float y2 = ImageShow.shapes[shapeIndex].P2.Y;

            PointF[] pts = new PointF[4];
            pts[0] = new PointF(x2, y2);
            double a = Math.Sin((double)r), b = -Math.Cos((double)r), c = 0 - x1 * a - y1 * b;
            pts[1] = new PointF((float)(x2 - 2 * a * ((a * x2 + b * y2 + c) / (a * a + b * b))), (float)(y2 - 2 * b * ((a * x2 + b * y2 + c) / (a * a + b * b))));
            pts[2] = new PointF((x1 * 2 - x2), (y1 * 2 - y2));
            a = Math.Sin((double)(r - Math.PI / 2.0)); b = -Math.Cos((double)(r - Math.PI / 2.0)); c = 0 - x1 * a - y1 * b;
            pts[3] = new PointF((float)(x2 - 2 * a * ((a * x2 + b * y2 + c) / (a * a + b * b))), (float)(y2 - 2 * b * ((a * x2 + b * y2 + c) / (a * a + b * b))));


            float x, y, width, height;
            x = Math.Min(pts[0].X, Math.Min(pts[1].X, Math.Min(pts[2].X, pts[3].X)));
            y = Math.Min(pts[0].Y, Math.Min(pts[1].Y, Math.Min(pts[2].Y, pts[3].Y)));
            width = Math.Max(pts[0].X, Math.Max(pts[1].X, Math.Max(pts[2].X, pts[3].X))) - x + 1;
            height = Math.Max(pts[0].Y, Math.Max(pts[1].Y, Math.Max(pts[2].Y, pts[3].Y))) - y + 1;

            if (x < 0)
            {
                width += x;
                x = 0;
            }
            if (y < 0)
            {
                height += y;
                y = 0;
            }

            Bitmap source = new Bitmap(ImageShow.nowFile);

            if (x + width >= source.Width)
                width = source.Width - x - 1;
            if (y + height >= source.Height)
                height = source.Height - y - 1;

            Rectangle section = new Rectangle(new Point((int)x, (int)y), new Size((int)width, (int)height));

            Bitmap CroppedImage = CropImage(source, section);
            string tempfile = Environment.CurrentDirectory + '\\' + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString().Replace(":", "_") + ".bmp";
            CroppedImage.Save(tempfile);

            Bitmap img = new Bitmap(tempfile);

            //Bitmap img = new Bitmap(ImageShow.nowFile);

            t1 = DateTime.Now;


            Result[] results  = new Result[1000];
            results = reader.DecodeMultiple(img);

            t2 = DateTime.Now;

            codeType.Clear();
            codeContent.Clear();
            int num = 0;
            try
            {
                while (results[num] != null)
                {
                    codeType.Add(results[num].BarcodeFormat.ToString());
                    codeContent.Add(results[num].Text);
                    num++;
                }
            }
            catch (Exception ex)
            {
                Object aa= new object();
                ex.Equals(aa);

            }
            codeNum = num;

            return (t2 - t1).TotalMilliseconds;
        }

        public static void initZXingReader(string type)
        {
            reader = new BarcodeReader();
            //reader.Options.TryHarder = true;

            reader.Options.Hints.Add(new KeyValuePair<DecodeHintType, object>(DecodeHintType.TRY_HARDER, true));
            reader.Options.PossibleFormats = new List<BarcodeFormat>();

            switch (type)
            {
                case "全部类型":
                    break;
                case "全部一维码类型":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.All_1D);
                    break;
                case "AZTEC":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.AZTEC);
                    break;
                case "CODABAR":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.CODABAR);
                    break;
                case "CODE_128":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.CODE_128);
                    break;
                case "CODE_39":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.CODE_39);
                    break;
                case "CODE_93":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.CODE_93);
                    break;
                case "DATA_MATRIX":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.DATA_MATRIX);
                    break;
                case "EAN_13":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.EAN_13);
                    break;
                case "EAN_8":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.EAN_8);
                    break;
                case "ITF":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.ITF);
                    break;
                case "MAXICODE":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.MAXICODE);
                    break;
                case "MSI":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.MSI);
                    break;
                case "PDF_417":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.PDF_417);
                    break;
                case "PLESSEY":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.PLESSEY);
                    break;
                case "QR_CODE":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.QR_CODE);
                    break;
                case "RSS_14":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.RSS_14);
                    break;
                case "RSS_EXPANDED":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.RSS_EXPANDED);
                    break;
                case "UPC_A":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.UPC_A);
                    break;
                case "UPC_E":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.UPC_E);
                    break;
                case "UPC_EAN_EXTENSION":
                    reader.Options.PossibleFormats.Add(BarcodeFormat.UPC_EAN_EXTENSION);
                    break;
                default:
                    break;
            }
        }
    }
}
