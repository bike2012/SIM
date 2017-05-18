using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SIMproject
{
    public partial class form : Form
    {
        private BaseTool nowTool = null;
        private Point p1, p2,p3;
        private bool mouseDown = false;

        ToolTip tooltip1, tooltip2, tooltip3, tooltip4, tooltip5, tooltip6, tooltip7, tooltip8, tooltip9, tooltip10, tooltip11, tooltip12, tooltip13, tooltip14;

        //private Hashtable toolTable = null;
        //隐藏选项卡
        Hashtable htPages = new Hashtable();
        public void DeletePage(string strName)

        {

            foreach (TabPage tabPage in tabControl1.TabPages)

            {

                if (tabPage.Name == strName)

                {

                    tabControl1.TabPages.Remove(tabPage);

                    htPages.Add(strName, tabPage);

                    break;

                }

            }

        }

        public form()
        {
            InitializeComponent();
            EdgeExtractionSelect_ComboBox.SelectedIndex = 0;
            ColorMode_ComboBox.SelectedIndex = 0;
            ZXingCodeType_ComboBox.SelectedIndex = 0;
            ColorRecog.initColorTab();
            //DeletePage("tabPage9");
            DeletePage("tabPage8");
            DeletePage("tabPage7");
            DeletePage("tabPage6");
            //DeletePage("tabPage5");
            //DeletePage("tabPage3");
        }

        private void cameraInit_Button_Click(object sender, EventArgs e)
        {
        //    Camera.CameraInitial();
        }

        private void saveImage_Button_Click(object sender, EventArgs e)
        {
            if (ShowEdge_Button.Text == "显示边缘")
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "图片文件|*.bmp|所有文件|*.*";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Bitmap pic = new Bitmap(ImageShow.source);
                        string tempfile = saveFileDialog.FileName;
                        pic.Save(tempfile, pic.RawFormat);
                    }
            }
            else
            {
                if (ImageShow.edgeDots.Count != 0)
                {

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "图片文件|*.bmp|所有文件|*.*";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Bitmap pic = new Bitmap(ImageShow.source.Width, ImageShow.source.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);


                        Rectangle rect = new Rectangle(0, 0, pic.Width, pic.Height);
                        System.Drawing.Imaging.BitmapData bmpData = pic.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, pic.PixelFormat);

                        // Get the address of the first line.获取首行地址  
                        IntPtr ptr = bmpData.Scan0;

                        // Declare an array to hold the bytes of the bitmap.定义数组保存位图  
                        int bytes = Math.Abs(bmpData.Stride) * pic.Height;
                        byte[] rgbValues = new byte[bytes];

                        // Copy the RGB values into the array.复制RGB值到数组  
                        System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                        for (int i = 0; i < bytes; i++)
                            rgbValues[i] = 0;

                        foreach (var shape in ImageShow.edgeDots)
                        {
                            rgbValues[(int)(shape.P1.Y * Math.Abs(bmpData.Stride) + shape.P1.X)] = 255;
                            ///rgbValues[(int)(shape.P1.Y * Math.Abs(bmpData.Stride) + shape.P1.X * 4) + 1] = 255;
                            //rgbValues[(int)(shape.P1.Y * Math.Abs(bmpData.Stride) + shape.P1.X * 4) + 2] = 255;
                            //rgbValues[(int)(shape.P1.Y * Math.Abs(bmpData.Stride) + shape.P1.X * 4) + 3] = 255;
                        }

                        // Copy the RGB values back to the bitmap 把RGB值拷回位图  
                        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

                        // Unlock the bits.解锁  
                        pic.UnlockBits(bmpData);

                        string tempfile = saveFileDialog.FileName;
                        pic.Save(tempfile, pic.RawFormat);
                    }
                }
            }
        }

        //private void singleFrame_Button_Click(object sender, EventArgs e)
        //{
        //    ImageShow.showEdge = 0;
        //    ImageShow.showModified = 0;
        ////    Camera.ImageGrab(0);

        //    Bitmap pic = new Bitmap(Camera.imgPara.SizeX, Camera.imgPara.SizeY, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        //    Rectangle rect = new Rectangle(0, 0, pic.Width, pic.Height);
        //    System.Drawing.Imaging.BitmapData bmpData = pic.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, pic.PixelFormat);
            
        //    // Get the address of the first line.获取首行地址  
        //    IntPtr ptr = bmpData.Scan0;

        //    // Declare an array to hold the bytes of the bitmap.定义数组保存位图  
        //    int bytes = Math.Abs(bmpData.Stride) * pic.Height;
        //    byte[] rgbValues = new byte[bytes];

        //    // Copy the RGB values into the array.复制RGB值到数组  
        //    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

        //    Console.WriteLine(DateTime.Now);

        //    int resultNum=100;
        //    int[] resultX=new int[100];int[] resultY=new int[100];int[] resultRX=new int[100];int[] resultRY=new int[100];
        //    Program.test(pic.Width, pic.Height, pic.Width * pic.Height, Camera.imgBuf.Array, ref resultNum, resultY, resultX, resultRY, resultRX, 40, 15, 5, 10);
            
        //    Console.WriteLine(DateTime.Now);

        //    // Set every third value to 255. A 24bpp bitmap will look red.  把每像素第3个值设为255.24bpp的位图将变红  
        //    for (int counter = 0; counter < pic.Width * pic.Height; counter++)
        //    {
        //        var val = Camera.imgBuf.Array[counter];
        //        rgbValues[counter * 3] = val;
        //        rgbValues[counter * 3 + 1] = val;
        //        rgbValues[counter * 3 + 2] = val;
        //    }

        //    Console.WriteLine(DateTime.Now);

        //    // Copy the RGB values back to the bitmap 把RGB值拷回位图  
        //    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

        //    // Unlock the bits.解锁  
        //    pic.UnlockBits(bmpData);

        //    for (int i = 0; i < resultNum; i++)
        //    {
        //        //Console.WriteLine(resultX[i] + "   " + resultY[i]);
        //        Unility.drawRect(pic, new Rectangle(resultX[i], resultY[i], resultRX[i], resultRY[i]), Color.Red);
        //    }
                

        //    presentBox.Image = pic;
        //    presentBox.SizeMode = PictureBoxSizeMode.StretchImage;
        //    presentBox.Refresh();

        //}

        private void loadImage_Button_Click(object sender, EventArgs e)
        {
            ImageShow.showEdge = 0;
            ImageShow.showModified = 0;
            ImageShow.cls();
            ShowBoundingBox_Button.Text = "显示外接矩形";
            PreProcess.showBoundingBox = false;
            imageDragTool.Select();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileName != "")
                {
                    try
                    {
                        ImageShow.source = new Bitmap(openFileDialog.FileName);
                        ImageShow.position = new Rectangle(0, 0, ImageShow.source.Width, ImageShow.source.Height);
                        ImageShow.initPosition = ImageShow.position;
                        ImageShow.scale = 1;
                        ImageShow.nowFile = openFileDialog.FileName;
                        presentBox.Refresh();
                    }
                    catch
                    {
                        MessageBox.Show("请输入正确的图像文件！");
                    }
                }
            }
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Rectangle tabArea = tabControl1.GetTabRect(e.Index);//主要是做个转换来获得TAB项的RECTANGELF 
            RectangleF tabTextArea = (RectangleF)(tabControl1.GetTabRect(e.Index));
            Graphics g = e.Graphics;
            StringFormat sf = new StringFormat();//封装文本布局信息 
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Near;
            Font font = this.cameraInit_Button.Font;
            SolidBrush brush = new SolidBrush(Color.Black);//绘制边框的画笔 
            g.DrawString(Config.tabControlNames[e.Index], font, brush, tabTextArea, sf);
        }

        private void presentBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (ImageShow.showModified>0)
            {
                if (ImageShow.modified != null) g.DrawImage(ImageShow.modified, ImageShow.position);
            }
            else
            {
                if (ImageShow.source != null) g.DrawImage(ImageShow.source, ImageShow.position);
            }

            if (ImageShow.shapes != null)
                foreach (var shape in ImageShow.shapes)
                    shape.draw(g);
            
            if (ImageShow.edgeDots != null)
                foreach (var shape in ImageShow.edgeDots)
                    shape.draw(g);

            if (PreProcess.showBoundingBox && PreProcess.boundingBox.Count!=0)
            {
                PointF[] pts = new PointF[4];
                for (int i = 0; i < 4; i++)
                {
                    pts[i].X = PreProcess.boundingBox[i].X * ImageShow.scale + ImageShow.position.X;
                    pts[i].Y = PreProcess.boundingBox[i].Y * ImageShow.scale + ImageShow.position.Y;
                }

                Pen edgePen = new Pen(Color.Blue, 1);
                edgePen.DashPattern = new float[] { 3, 3 };
                g.DrawPolygon(edgePen, pts);
            }
            if (figureDetect.showBoundingBox && figureDetect.boundingBox.Count != 0)
            {
                for (int j = 0; j < figureDetect.boundingBox.Count / 4; j++)
                {
                    PointF[] pts = new PointF[4];
                    for (int i = 0; i < 4; i++)
                    {
                        pts[i].X = figureDetect.boundingBox[j * 4 + i].X * ImageShow.scale + ImageShow.position.X;
                        pts[i].Y = figureDetect.boundingBox[j * 4 + i].Y * ImageShow.scale + ImageShow.position.Y;
                    }

                    PointF center = new PointF((pts[0].X+pts[1].X+pts[2].X+pts[3].X)/4,(pts[0].Y+pts[1].Y+pts[2].Y+pts[3].Y)/4);
                    Pen centerPen = new Pen(Color.Red, 1);
                    g.DrawEllipse(centerPen, center.X - 2, center.Y - 2, 5, 5);
                    string s = "Pos:(" + center.X.ToString("0.00") + "," + center.Y.ToString("0.00") + ")" + Environment.NewLine + "Area:" + figureDetect.boundingBoxArea[j].ToString();
                    Font f = new Font("宋体", 18);
                    g.DrawString(s, f, Brushes.GreenYellow, center.X + 5, center.Y + 5);
                    
                    Pen edgePen = new Pen(Color.Blue, 1);
                    edgePen.DashPattern = new float[] { 3, 3 };
                    g.DrawPolygon(edgePen, pts);
                }
            }

        }

        private void zoomIn_Button_Click(object sender, EventArgs e)
        {
            if (ImageShow.scale < 2)
            {
                var centerX = (presentBox.Width / 2.0 - ImageShow.position.X) / ImageShow.position.Width * ImageShow.initPosition.Width;
                var centerY = (presentBox.Height / 2.0 - ImageShow.position.Y) / ImageShow.position.Height * ImageShow.initPosition.Height;
                ImageShow.scale += 0.1F;
                ImageShow.position.X = (float)(presentBox.Width / 2.0 - centerX * ImageShow.scale);
                ImageShow.position.Y = (float)(presentBox.Height / 2.0 - centerY * ImageShow.scale);
                ImageShow.position.Width = ImageShow.initPosition.Width * ImageShow.scale;
                ImageShow.position.Height = ImageShow.initPosition.Height * ImageShow.scale;

                presentBox.Refresh();
            }
        }

        private void zoomOut_Button_Click(object sender, EventArgs e)
        {
            if (ImageShow.scale >0.2)
            {
                var centerX = (presentBox.Width / 2.0 - ImageShow.position.X) / ImageShow.position.Width * ImageShow.initPosition.Width;
                var centerY = (presentBox.Height / 2.0 - ImageShow.position.Y) / ImageShow.position.Height * ImageShow.initPosition.Height;
                ImageShow.scale -= 0.1F;
                ImageShow.position.X = (float)(presentBox.Width / 2.0 - centerX * ImageShow.scale);
                ImageShow.position.Y = (float)(presentBox.Height / 2.0 - centerY * ImageShow.scale);
                ImageShow.position.Width = ImageShow.initPosition.Width * ImageShow.scale;
                ImageShow.position.Height = ImageShow.initPosition.Height * ImageShow.scale;

                presentBox.Refresh();
            }
        }

        private void initScale_Button_Click(object sender, EventArgs e)
        {
            ImageShow.position.X = 0;
            ImageShow.position.Y = 0;
            ImageShow.position.Width = presentBox.Width;
            ImageShow.position.Height = ImageShow.position.Width / ImageShow.initPosition.Width * ImageShow.initPosition.Height;

            if (ImageShow.position.Height > presentBox.Height)
            {
                ImageShow.position.Height = presentBox.Height;
                ImageShow.position.Width = ImageShow.position.Height / ImageShow.initPosition.Height * ImageShow.initPosition.Width;
            }

            ImageShow.scale = ImageShow.position.Width / ImageShow.initPosition.Width;

            presentBox.Refresh();
        }

        private void presentBox_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            p1 = new Point(e.X, e.Y);
            p2 = p1;
            if (nowTool != null)
                nowTool.mouseDown(p1);
        }

        private void presentBox_MouseMove(object sender, MouseEventArgs e)
        {
            int x = (int)((e.X - ImageShow.position.X) / ImageShow.scale);
            int y = (int)((e.Y - ImageShow.position.Y) / ImageShow.scale);
            mousePosition_StatusLabel.Text = "坐标(X:" + x.ToString() + ",Y:" + y.ToString() + ")";
            int R = 0, G = 0, B = 0;
            if (ImageShow.source != null)
                if (x >= 0 && x < ImageShow.source.Width && y >= 0 && y < ImageShow.source.Height)
                {
                    //if (Bitmap.GetPixelFormatSize(ImageShow.source.PixelFormat) == 8)
                    R = ImageShow.source.GetPixel(x, y).R;
                    G = ImageShow.source.GetPixel(x, y).G;
                    B = ImageShow.source.GetPixel(x, y).B;
                }
            mouseColor_StatusLabel.Text = "颜色(R:" + R.ToString() + ",G:" + G.ToString() + ",B:" + B.ToString() + ")";
            if (nowTool != null)
            {
                if (mouseDown)
                {
                    p3 = new Point(e.X, e.Y);
                    nowTool.mouseDrag(p1, p2, p3);
                    p2 = p3;
                    presentBox.Refresh();
                    if (figureDetect.setCircleParaState)
                    {
                        float para1=0,para2=0,para3=0,para4=0,para5=0;
                        ImageShow.shapes[figureDetect.shapeIndex].getPara(ref para1, ref para2, ref para3, ref para4, ref para5);
                        circleRadiusSet_Text.Text = para5.ToString();
                    }
                    if (figureDetect.setRectParaState)
                    {
                        float para1 = 0, para2 = 0, para3 = 0, para4 = 0, para5 = 0;
                        ImageShow.shapes[figureDetect.shapeIndex].getPara(ref para1, ref para2, ref para3, ref para4, ref para5);
                        rectWidthSet_Text.Text = para3.ToString("0.00");
                        rectHeightSet_Text.Text = para4.ToString("0.00");
                    }
                    if (TemplateMatch.setRectParaState == 1)
                    {
                        float para1 = 0, para2 = 0, para3 = 0, para4 = 0, para5 = 0;
                        ImageShow.shapes[figureDetect.shapeIndex].getPara(ref para1, ref para2, ref para3, ref para4, ref para5);
                        TemplateMatch.templateROI = new Rectangle((int)(para1 - para3 / 2), (int)(para2 - para4 / 2), (int)para3, (int)para4);
                    }
                    if (TemplateMatch.setRectParaState == 2)
                    {
                        float para1 = 0, para2 = 0, para3 = 0, para4 = 0, para5 = 0;
                        ImageShow.shapes[figureDetect.shapeIndex].getPara(ref para1, ref para2, ref para3, ref para4, ref para5);
                        TemplateMatch.detectROI = new Rectangle((int)(para1 - para3 / 2), (int)(para2 - para4 / 2), (int)para3, (int)para4);
                    }
                }
            }
        }

        private void presentBox_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            if (nowTool != null)
                nowTool.mouseUp(p3);
            if (rectTool_Button.Checked || circleTool_Button.Checked) editTool_Button.Select();
            if (DefectDetect.selectLineState)
            {
                presentBox.Refresh();
                runtime_StatusLabel.Text = "直线检测运行时间：" + DefectDetect.runtime.ToString("0.0") + "ms";
                lineIntegrity_Text.Text = (DefectDetect.lineIntegrity*100).ToString("0.00") + "%";
            }
            if (DistanceMeasure.lineSelectState == 1)
            {
                float para1 = 0, para2 = 0, para3 = 0, para4 = 0, para5 = 0;
                ImageShow.shapes[DistanceMeasure.lineShapeIndex].getPara(ref para1, ref para2, ref para3, ref para4, ref para5);
                DistanceMeasure.lineParaA = Math.Sin((double)para5);
                DistanceMeasure.lineParaB = -Math.Cos((double)para5);
                DistanceMeasure.lineParaC = 0 - DistanceMeasure.lineParaA * para1 - DistanceMeasure.lineParaB * para2;

                LineParaA_Text.Text = DistanceMeasure.lineParaA.ToString("0.00");
                LineParaB_Text.Text = DistanceMeasure.lineParaB.ToString("0.00");
                LineParaC_Text.Text = DistanceMeasure.lineParaC.ToString("0.00");

                DistanceMeasure.lineSelectState = 0;
            }
        }

        private void imageDragTool_CheckedChanged(object sender, EventArgs e)
        {
            if (imageDragTool.Checked)
            {
                nowTool = (BaseTool)(new ImageDragTool());
                presentBox.Cursor = Cursors.SizeAll;
            }
            else
            {
                presentBox.Cursor = Cursors.Arrow;
            }
        }

        private void autoExtract_Button_Click(object sender, EventArgs e)
        {
            double time = CameraCalibrate.runAutoExtract();
            runtime_StatusLabel.Text = "自动取点运行时间：" + time.ToString("0.0") + "ms";
            presentBox.Refresh();

        }

        private void extractAccept_Button_Click(object sender, EventArgs e)
        {
            if (CameraCalibrate.extractAccept())
            {
                extractedNum_Text.Text = CameraCalibrate.extractedNum.ToString();
                extractedNum_Text.Refresh();
                if (!CameraCalibrate.savePixelSize)
                    PixelSize_Text.Text = CameraCalibrate.pixelSize.ToString("0.00") + "(毫米/像素)";
            }
            else
            {
                MessageBox.Show("取点确认失败，请重新取点！");
            }

        }

        private void extarctReset_Button_Click(object sender, EventArgs e)
        {
            CameraCalibrate.extractedNum = 0;
        }

        private void boardWidth_Text_TextChanged(object sender, EventArgs e)
        {
            CameraCalibrate.extractedNum = 0;
            try
            {
                CameraCalibrate.boardWidth = int.Parse(boardWidth_Text.Text);
            }
            catch (Exception)
            {
                //CameraCalibrate.boardWidth = 0;
                MessageBox.Show("请输入一个整数");
            }
            //MessageBox.Show("已修改标定板参数，请重新取点！");
        }

        private void boardHeight_Text_TextChanged(object sender, EventArgs e)
        {
            CameraCalibrate.extractedNum = 0;
            try
            {
                CameraCalibrate.boardHeight = int.Parse(boardHeight_Text.Text);
            }
            catch (Exception)
            {
                //CameraCalibrate.boardHeight = 0;
                MessageBox.Show("请输入一个整数");
            }
            //MessageBox.Show("已修改标定板参数，请重新取点！");
        }

        private void manualExtract_Button_Click(object sender, EventArgs e)
        {
            ImageShow.cls();
            MessageBox.Show("请按顺时针顺序选取标定板顶点！");
            imageDragTool.Select();
            CameraCalibrate.manualExtractState = 4;
            pointTool_Button.Select();
        }

        private void pointTool_Button_CheckedChanged(object sender, EventArgs e)
        {
            if (pointTool_Button.Checked)
            {
                nowTool = (BaseTool)(new PointTool());
                presentBox.Cursor = Cursors.Cross;
            }
            else
                presentBox.Cursor = Cursors.Arrow;
        }

        private void presentBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (nowTool != null)
            {
                bool showRuntime = false;
                if (CameraCalibrate.manualExtractState == 1) showRuntime = true;
                p1 = new Point(e.X, e.Y);
                double time = nowTool.mouseClick(p1);
                presentBox.Refresh();
                if (DistanceMeasure.pointSelectState == 1)
                {
                    float para1 = 0, para2 = 0, para3 = 0, para4 = 0, para5 = 0;
                    ImageShow.shapes[DistanceMeasure.shapeIndex].getPara(ref para1, ref para2, ref para3, ref para4, ref para5);
                    pointX_Text1.Text = para1.ToString("0.00");
                    pointY_Text1.Text = para2.ToString("0.00");
                    DistanceMeasure.pointSelectState = 0;
                }
                if (DistanceMeasure.pointSelectState == 2)
                {
                    float para1 = 0, para2 = 0, para3 = 0, para4 = 0, para5 = 0;
                    ImageShow.shapes[DistanceMeasure.shapeIndex].getPara(ref para1, ref para2, ref para3, ref para4, ref para5);
                    pointX_Text2.Text = para1.ToString("0.00");
                    pointY_Text2.Text = para2.ToString("0.00");
                    DistanceMeasure.pointSelectState = 0;
                }
                if (DistanceMeasure.lineSelectState == 2)
                {
                    float para1 = 0, para2 = 0, para3 = 0, para4 = 0, para5 = 0;
                    ImageShow.shapes[DistanceMeasure.lineShapeIndex].getPara(ref para1, ref para2, ref para3, ref para4, ref para5);
                    DistanceMeasure.lineParaA = Math.Sin((double)para5);
                    DistanceMeasure.lineParaB = -Math.Cos((double)para5);
                    DistanceMeasure.lineParaC = 0 - DistanceMeasure.lineParaA * para1 - DistanceMeasure.lineParaB * para2;

                    LineParaA_Text.Text = DistanceMeasure.lineParaA.ToString("0.00");
                    LineParaB_Text.Text = DistanceMeasure.lineParaB.ToString("0.00");
                    LineParaC_Text.Text = DistanceMeasure.lineParaC.ToString("0.00");

                    DistanceMeasure.lineSelectState = 0;
                }
                if (DefectDetect.selectPointState > 0)
                {
                    if (DefectDetect.selectPointState == 4)
                    {
                        objPt1_Text.Text = "(" + DefectDetect.selectPointIndex[0].X.ToString("0.00") + "," + DefectDetect.selectPointIndex[0].Y.ToString("0.00") + ")";
                    }
                    if (DefectDetect.selectPointState == 3)
                    {
                        objPt2_Text.Text = "(" + DefectDetect.selectPointIndex[1].X.ToString("0.00") + "," + DefectDetect.selectPointIndex[1].Y.ToString("0.00") + ")";
                    }
                    if (DefectDetect.selectPointState == 2)
                    {
                        objPt3_Text.Text = "(" + DefectDetect.selectPointIndex[2].X.ToString("0.00") + "," + DefectDetect.selectPointIndex[2].Y.ToString("0.00") + ")";
                    }
                    if (DefectDetect.selectPointState == 1)
                    {
                        objPt4_Text.Text = "(" + DefectDetect.selectPointIndex[3].X.ToString("0.00") + "," + DefectDetect.selectPointIndex[3].Y.ToString("0.00") + ")";
                    }
                    DefectDetect.selectPointState--;
                }
                if (DefectDetect.tempSelectPointState > 0)
                {
                    if (DefectDetect.tempSelectPointState == 4)
                    {
                        tempPt1_Text.Text = "(" + DefectDetect.tempSelectPointIndex[0].X.ToString("0.00") + "," + DefectDetect.tempSelectPointIndex[0].Y.ToString("0.00") + ")";
                    }
                    if (DefectDetect.tempSelectPointState == 3)
                    {
                        tempPt2_Text.Text = "(" + DefectDetect.tempSelectPointIndex[1].X.ToString("0.00") + "," + DefectDetect.tempSelectPointIndex[1].Y.ToString("0.00") + ")";
                    }
                    if (DefectDetect.tempSelectPointState == 2)
                    {
                        tempPt3_Text.Text = "(" + DefectDetect.tempSelectPointIndex[2].X.ToString("0.00") + "," + DefectDetect.tempSelectPointIndex[2].Y.ToString("0.00") + ")";
                    }
                    if (DefectDetect.tempSelectPointState == 1)
                    {
                        tempPt4_Text.Text = "(" + DefectDetect.tempSelectPointIndex[3].X.ToString("0.00") + "," + DefectDetect.tempSelectPointIndex[3].Y.ToString("0.00") + ")";
                    }
                    DefectDetect.tempSelectPointState--;
                }

                colorBox.Refresh();

                if (showRuntime)
                    runtime_StatusLabel.Text = "手动辅助取点运行时间：" + time.ToString("0.0") + "ms";
            }
        }

        private void runCalibrate_Button_Click(object sender, EventArgs e)
        {
            if (CameraCalibrate.extractedNum > 0)
            {
                double time = CameraCalibrate.calibrate();

                runtime_StatusLabel.Text = "相机标定运行时间：" + time.ToString("0.0") + "ms";

                CM11.Text = CameraCalibrate.CM11.ToString("0.0");
                CM12.Text = CameraCalibrate.CM12.ToString("0.0");
                CM13.Text = CameraCalibrate.CM13.ToString("0.0");
                CM21.Text = CameraCalibrate.CM21.ToString("0.0");
                CM22.Text = CameraCalibrate.CM22.ToString("0.0");
                CM23.Text = CameraCalibrate.CM23.ToString("0.0");
                CM31.Text = CameraCalibrate.CM31.ToString("0.0");
                CM32.Text = CameraCalibrate.CM32.ToString("0.0");
                CM33.Text = CameraCalibrate.CM33.ToString("0.0");

                D1.Text = CameraCalibrate.D1.ToString("0.0");
                D2.Text = CameraCalibrate.D2.ToString("0.0");
                D3.Text = CameraCalibrate.D3.ToString("0.0");
                D4.Text = CameraCalibrate.D4.ToString("0.0");
                D5.Text = CameraCalibrate.D5.ToString("0.0");

                PixelSize_Text.Text = CameraCalibrate.pixelSize.ToString("0.00") + "(毫米/像素)";
            }
            else
                MessageBox.Show("请先取点！");
        }

        private void boardSize_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                CameraCalibrate.boardSize = float.Parse(boardSize_Text.Text);
            }
            catch(Exception)
            {
                MessageBox.Show("请输入一个浮点数");
            }
        }


        private void ImageProcess_Load(object sender, EventArgs e)
        {
            EdgeExtraction_TabControl.Height = EdgeExtraction_Panel.Height + 20;

            tooltip1 = new ToolTip();
            tooltip1.AutoPopDelay = 5000;
            tooltip1.IsBalloon = true;
            tooltip2 = new ToolTip();
            tooltip2.AutoPopDelay = 5000;
            tooltip2.IsBalloon = true;
            tooltip3 = new ToolTip();
            tooltip3.AutoPopDelay = 5000;
            tooltip3.IsBalloon = true;
            tooltip4 = new ToolTip();
            tooltip4.AutoPopDelay = 5000;
            tooltip4.IsBalloon = true;
            tooltip5 = new ToolTip();
            tooltip5.AutoPopDelay = 5000;
            tooltip5.IsBalloon = true;
            tooltip6 = new ToolTip();
            tooltip6.AutoPopDelay = 5000;
            tooltip6.IsBalloon = true;
            tooltip7 = new ToolTip();
            tooltip7.AutoPopDelay = 5000;
            tooltip7.IsBalloon = true;
            tooltip8 = new ToolTip();
            tooltip8.AutoPopDelay = 5000;
            tooltip8.IsBalloon = true;
            tooltip9 = new ToolTip();
            tooltip9.AutoPopDelay = 5000;
            tooltip9.IsBalloon = true;
            tooltip10 = new ToolTip();
            tooltip10.AutoPopDelay = 5000;
            tooltip10.IsBalloon = true;
            tooltip11 = new ToolTip();
            tooltip11.AutoPopDelay = 5000;
            tooltip11.IsBalloon = true;
            tooltip12 = new ToolTip();
            tooltip12.AutoPopDelay = 5000;
            tooltip12.IsBalloon = true;
            tooltip13 = new ToolTip();
            tooltip13.AutoPopDelay = 5000;
            tooltip13.IsBalloon = true;
            tooltip14 = new ToolTip();
            tooltip14.AutoPopDelay = 5000;
            tooltip14.IsBalloon = true;
        }

        private void CM11_MouseHover(object sender, EventArgs e)
        {
            tooltip1.SetToolTip(CM11, CameraCalibrate.CM11.ToString());

        }

        private void CM12_MouseHover(object sender, EventArgs e)
        {
            tooltip2.SetToolTip(CM12, CameraCalibrate.CM12.ToString());

        }

        private void CM13_MouseHover(object sender, EventArgs e)
        {
            tooltip3.SetToolTip(CM13, CameraCalibrate.CM13.ToString());

        }

        private void CM21_MouseHover(object sender, EventArgs e)
        {
            tooltip4.SetToolTip(CM21, CameraCalibrate.CM21.ToString());

        }

        private void CM22_MouseHover(object sender, EventArgs e)
        {
            tooltip5.SetToolTip(CM22, CameraCalibrate.CM22.ToString());

        }

        private void CM23_MouseHover(object sender, EventArgs e)
        {
            tooltip6.SetToolTip(CM23, CameraCalibrate.CM23.ToString());

        }

        private void CM31_MouseHover(object sender, EventArgs e)
        {
            tooltip7.SetToolTip(CM31, CameraCalibrate.CM31.ToString());

        }

        private void CM32_MouseHover(object sender, EventArgs e)
        {
            tooltip8.SetToolTip(CM32, CameraCalibrate.CM32.ToString());

        }

        private void CM33_MouseHover(object sender, EventArgs e)
        {
            tooltip9.SetToolTip(CM33, CameraCalibrate.CM33.ToString());

        }

        private void D1_MouseHover(object sender, EventArgs e)
        {
            tooltip10.SetToolTip(D1, CameraCalibrate.D1.ToString());

        }

        private void D2_MouseHover(object sender, EventArgs e)
        {
            tooltip11.SetToolTip(D2, CameraCalibrate.D2.ToString());

        }

        private void D3_MouseHover(object sender, EventArgs e)
        {
            tooltip12.SetToolTip(D3, CameraCalibrate.D3.ToString());

        }

        private void D4_MouseHover(object sender, EventArgs e)
        {
            tooltip13.SetToolTip(D4, CameraCalibrate.D4.ToString());

        }

        private void D5_MouseHover(object sender, EventArgs e)
        {
            tooltip14.SetToolTip(D5, CameraCalibrate.D5.ToString());

        }

        private void runUndistort_Button_Click(object sender, EventArgs e)
        {
            double time = CameraCalibrate.undistort();
            runtime_StatusLabel.Text = "图像校正运行时间：" + time.ToString("0.0") + "ms";
            presentBox.Image = ImageShow.source;
            presentBox.Refresh();
        }

        private void circleTool_Button_CheckedChanged(object sender, EventArgs e)
        {
            if (circleTool_Button.Checked)
            {
                nowTool = (BaseTool)(new CircleTool());
                presentBox.Cursor = Cursors.Cross;
            }
            else
            {
                presentBox.Cursor = Cursors.Arrow;
            }
        }

        private void editTool_Button_CheckedChanged(object sender, EventArgs e)
        {
            if (editTool_Button.Checked)
            {
                nowTool = (BaseTool)(new EditTool());
            }
            else
            {
            }
        }

        private void rectTool_Button_CheckedChanged(object sender, EventArgs e)
        {
            if (rectTool_Button.Checked)
            {
                nowTool = (BaseTool)(new RectTool());
                //presentBox.Cursor = Cursors.Cross;
            }
            else
            {
                figureDetect.setRectParaState = false;
                TemplateMatch.setRectParaState = 0;
                //presentBox.Cursor = Cursors.Arrow;
            }
        }

        private void setCirclePara_Button_Click(object sender, EventArgs e)
        {
            ImageShow.cls();
            imageDragTool.Select();
            figureDetect.setCircleParaState = true;
            circleTool_Button.Select();
        }

        private void circleDetect_Button_Click(object sender, EventArgs e)
        {
            ImageShow.cls();
            figureDetect.setCircleParaState = false;
            int resultNum = 1000;
            float[] resultX = new float[resultNum];
            float[] resultY = new float[resultNum];
            float[] resultR = new float[resultNum];

            try
            {
                double low = double.Parse(circleRadiusSet_Text.Text) * (1-figureDetect.circleRadiusRange/100.0);
                double high = double.Parse(circleRadiusSet_Text.Text) * (1 + figureDetect.circleRadiusRange / 100.0);
                if (ImageShow.nowFile != null && circleRadiusSet_Text.Text != "0")
                {
                    DateTime t1 = DateTime.Now;
                    figureDetect.circleDetect(ImageShow.nowFile, low, high, double.Parse(CannyHighThr_Text.Text), 20, ref resultNum, resultX, resultY, resultR, figureDetect.colorLowThr, figureDetect.colorHighThr, figureDetect.colorRate);
                    DateTime t2 = DateTime.Now;
                    runtime_StatusLabel.Text = "圆形检测运行时间：" + (t2 - t1).TotalMilliseconds.ToString("0.0") + "ms";
                    for (int i = 0; i < resultNum; i++)
                        ImageShow.addCircle(new PointF(resultX[i], resultY[i]), resultR[i]);
                }
            }
            catch
            {
                MessageBox.Show("请输入正确的参数");
            }
            presentBox.Refresh();

        }

        private void setRectPara_Button_Click(object sender, EventArgs e)
        {
            ImageShow.cls();
            imageDragTool.Select();
            figureDetect.setRectParaState = true;
            rectTool_Button.Select();
        }

        private void rectDetect_Button_Click(object sender, EventArgs e)
        {
            ImageShow.cls();
            figureDetect.setRectParaState = false;
            int resultNum = 8000;
            float[] resultData= new float[resultNum];

            try
            {
                double width = double.Parse(rectWidthSet_Text.Text);
                double height = double.Parse(rectHeightSet_Text.Text);
                if (ImageShow.nowFile != null && rectWidthSet_Text.Text != "0" && rectHeightSet_Text.Text != "0")
                {
                    int edgeType = EdgeExtractionSelect_ComboBox.SelectedIndex;
                    int para1, para2;
                    switch (edgeType)
                    {
                        case 0:
                            para1 = PreProcess.sobelKsize;
                            para2 = PreProcess.sobelThr;
                            break;
                        case 1:
                            para1 = PreProcess.cannyLowThr;
                            para2 = PreProcess.cannyHighThr;
                            break;
                        default:
                            para1 = PreProcess.cannyLowThr;
                            para2 = PreProcess.cannyHighThr;
                            break;
                    }
                    DateTime t1 = DateTime.Now;
                    figureDetect.rectDetect(ImageShow.nowFile, width, height, figureDetect.rectWHRange, ref resultNum, resultData, figureDetect.colorLowThr, figureDetect.colorHighThr,figureDetect.colorRate, edgeType,para1,para2);
                    DateTime t2 = DateTime.Now;
                    runtime_StatusLabel.Text = "矩形检测运行时间：" + (t2 - t1).TotalMilliseconds.ToString("0.0") + "ms";
                    //figureDetect.rectDetectWithRate(ImageShow.nowFile, 0.1, 10.0, double.Parse(edgeDetectPara_Text.Text), ref resultNum, resultData);
                    for (int i = 0; i < resultNum; i++)
                        ImageShow.addRect(new PointF(resultData[i * 5], resultData[i * 5 + 1]), new PointF(resultData[i * 5 + 2], resultData[i * 5 + 3]), resultData[i * 5 + 4], true);
                }
            }
            catch
            {
                MessageBox.Show("请输入正确的参数");
            }
            presentBox.Refresh();
        }

        private void distanceMeasure_Button_Click(object sender, EventArgs e)
        {
            DistanceMeasure.pointSelectState = 0;
            double x1 = double.Parse(pointX_Text1.Text);
            double y1 = double.Parse(pointY_Text1.Text);
            double x2 = double.Parse(pointX_Text2.Text);
            double y2 = double.Parse(pointY_Text2.Text);

            double dist = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
            measureResult_Text.Text = dist.ToString("0.00") + "像素";
            if (CameraCalibrate.pixelSize != 0)
            {
                measureResultMM_Text.Text = (CameraCalibrate.runMeasure((float)x1, (float)y1, (float)x2, (float)y2)).ToString("0.00") + "毫米";
            }
        }

        private void manualPointSelect_Button1_Click(object sender, EventArgs e)
        {
            imageDragTool.Select();
            DistanceMeasure.pointSelectState = 1;
            pointTool_Button.Select();
        }

        private void manualPointSelect_Button2_Click(object sender, EventArgs e)
        {
            imageDragTool.Select();
            DistanceMeasure.pointSelectState = 2;
            pointTool_Button.Select();
        }

        private void autoPointSelect_Button1_Click(object sender, EventArgs e)
        {
            DistanceMeasure.pointSelectState = 1;
            presentBox.Cursor = Cursors.Arrow;
            nowTool = (BaseTool)(new SelectTool());
        }

        private void autoPointSelect_Button2_Click(object sender, EventArgs e)
        {
            DistanceMeasure.pointSelectState = 2;
            presentBox.Cursor = Cursors.Arrow;
            nowTool = (BaseTool)(new SelectTool());
        }
        
        private void edgeDetectPara_Tracker_Scroll(object sender, EventArgs e)
        {
            CannyLowThr_Text.Text = (CannyLowThr_Tracker.Value*5).ToString();


        }



        private void showOrigin_Button_Click(object sender, EventArgs e)
        {
           
            if (ImageShow.source!=null)
                ImageShow.position = new Rectangle(0, 0, ImageShow.source.Width, ImageShow.source.Height);
            ImageShow.scale = 1;
            ImageShow.showEdge = 0;
            ImageShow.showModified = 0;
            ImageShow.cls();
            ShowBoundingBox_Button.Text = "显示外接矩形";
            PreProcess.showBoundingBox = false;
            figureDetect.showBoundingBox = false;
            imageDragTool.Select();
            presentBox.Refresh();
        }

        private void edgeStepPara_Tracker_Scroll(object sender, EventArgs e)
        {
            edgeStepPara_Text.Text = edgeStepPara_Tracker.Value.ToString();
        }

        private void edgeThrPara_Tracker_Scroll(object sender, EventArgs e)
        {
            edgeThrPara_Text.Text = edgeThrPara_Tracker.Value.ToString();
        }

        private void edgeStepPara_Text_TextChanged(object sender, EventArgs e)
        {
            //figureDetect.edgeDetectStep = int.Parse(edgeStepPara_Text.Text);
            //figureDetect.edgeDetectThr = int.Parse(edgeThrPara_Text.Text);
            PreProcess.LSDStep = int.Parse(edgeStepPara_Text.Text);
            PreProcess.LSDThr = int.Parse(edgeThrPara_Text.Text);
            if (ShowEdge_Button.Text == "隐藏边缘")
            {
                ImageShow.cls();
                //double time = figureDetect.newEdgeDetectFun();
                double time = PreProcess.callLSD();
                runtime_StatusLabel.Text = "边缘提取运行时间：" + time.ToString("0.0") + "ms";
                presentBox.Refresh();
            }
        }

        private void edgeThrPara_Text_TextChanged(object sender, EventArgs e)
        {
            //figureDetect.edgeDetectStep = int.Parse(edgeStepPara_Text.Text);
            //figureDetect.edgeDetectThr = int.Parse(edgeThrPara_Text.Text);
            
            PreProcess.LSDStep = int.Parse(edgeStepPara_Text.Text);
            PreProcess.LSDThr = int.Parse(edgeThrPara_Text.Text);
            if (ShowEdge_Button.Text == "隐藏边缘")
            {
                ImageShow.cls();
                //double time = figureDetect.newEdgeDetectFun();
                double time = PreProcess.callLSD();
                runtime_StatusLabel.Text = "边缘提取运行时间：" + time.ToString("0.0") + "ms";
                presentBox.Refresh();
            }
        }

        private void codeSelect_Button_Click(object sender, EventArgs e)
        {
            if (ImageShow.nowFile != null)
            {
                imageDragTool.Select();
                CodeRecog.setRectParaState = 1;
                CodeRecog.shapeIndex = -1;
                rectTool_Button.Select();
            }
        }

        private void codeProcess_Button_Click(object sender, EventArgs e)
        {
            if (ImageShow.nowFile!=null)
                if (CodeRecog.setRectParaState == 1 && CodeRecog.shapeIndex != -1)
                {
                    double time = CodeRecog.codeRecog(1);
                    runtime_StatusLabel.Text = "维码识别运行时间：" + time.ToString("0.0") + "ms";
                    Codes_List.Items.Clear();
                    if (CodeRecog.codeNum == 0)
                        Codes_List.Items.Add("未成功识别！");
                    for (int i = 0; i < CodeRecog.codeNum; i++)
                        Codes_List.Items.Add("("+CodeRecog.codeType[i] + ")" + CodeRecog.codeContent[i]);
                    Codes_List.Refresh();
                }
        }

        private void preprocessKsize_Tracker_Scroll(object sender, EventArgs e)
        {
            preprocessKsize_Text.Text = preprocessKsize_Tracker.Value.ToString();
        }

        private void preprocessKsize_Text_TextChanged(object sender, EventArgs e)
        {
            PreProcess.ksize = int.Parse(preprocessKsize_Text.Text);
        }

        private void showPreprocessResult_Button_Click(object sender, EventArgs e)
        {
            if (ImageShow.nowFile != null)
            {
                double time = PreProcess.callBlur();
                runtime_StatusLabel.Text = "图像降噪运行时间：" + time.ToString("0.0") + "ms";
                presentBox.Refresh();
            }
        }

        private void imageStitching_Button_Click(object sender, EventArgs e)
        {
            int n = 0;
            for (int i = 0; i < 4; i++)
                if (ImageStitching.originFilenameList[i] != null)
                    n++;
                else
                    break;
            if (n < 2)
                MessageBox.Show("请输入完整的拼接图像");
            else
            {
                double time = ImageStitching.callImageStitch(1, n, 1);
                runtime_StatusLabel.Text = "图像拼接运行时间：" + time.ToString("0.0") + "ms";
                presentBox.Refresh();
            }
        }

        private void myImageStitching_Button_Click(object sender, EventArgs e)
        {
            int w = -1,h = -1;
            for (int i=0;i<4;i++)
                for (int j=0;j<4;j++)
                    if ((i + 1) * (j + 1) == ImageStitching.fastFilenameNum)
                    {
                        bool isOk = true;
                        for (int ii = 0; ii <= i; ii++)
                            for (int jj = 0; jj <= j; jj++)
                                if (ImageStitching.fastFilenameList[ii * 4 + jj] == null)
                                    isOk = false;
                        if (isOk)
                        {
                            w = j + 1; h = i + 1;
                        }
                    }
            if (w == -1)
                MessageBox.Show("请输入完整的拼接图像");
            else
            {
                double time = ImageStitching.callImageStitch(2, w, h);
                runtime_StatusLabel.Text = "图像拼接运行时间：" + time.ToString("0.0") + "ms";
                presentBox.Refresh();
            }
        }

        private void ShowPara_Check_CheckedChanged(object sender, EventArgs e)
        {
            ImageShow.showPara = ShowPara_Check.Checked;
            presentBox.Refresh();
        }

        private void lineTool_Button_CheckedChanged(object sender, EventArgs e)
        {
            if (lineTool_Button.Checked)
            {
                nowTool = (BaseTool)(new LineTool());
                presentBox.Cursor = Cursors.Cross;
            }
            else
            {
                DefectDetect.selectLineState = false;
                presentBox.Cursor = Cursors.Arrow;
            }
        }

        private void selectLine_Button_Click(object sender, EventArgs e)
        {
            imageDragTool.Select();
            DefectDetect.selectLineState = true;
            lineTool_Button.Select();
        }

        private void SetTemplateFile_Button_Click(object sender, EventArgs e)
        {
            DefectDetect.templateFile = ImageShow.nowFile;
        }

        private void SetObjectFIle_Button_Click(object sender, EventArgs e)
        {
            DefectDetect.objectFile = ImageShow.nowFile;
        }

        private void SetTemplatePts_Button_Click(object sender, EventArgs e)
        {
            imageDragTool.Select();
            DefectDetect.tempSelectPointState = 4;
            DefectDetect.tempSelectPointIndex = new List<PointF>();

            pointTool_Button.Select();
        }

        private void SetObjectPts_Button_Click(object sender, EventArgs e)
        {
            imageDragTool.Select();
            DefectDetect.selectPointState = 4;
            DefectDetect.selectPointIndex = new List<PointF>();
            pointTool_Button.Select();
        }

        private void DefectDetect_Button_Click(object sender, EventArgs e)
        {
            int isOK = 0;
            double time = DefectDetect.detectDefect(ref isOK);
            runtime_StatusLabel.Text = "缺陷检测运行时间：" + time.ToString("0.0") + "ms";
            if (isOK == 1)
            {
                DefectDetectResult_Text.Text = "合格";
                DefectDetectResult_Text.ForeColor = Color.Green;
            }
            else if (isOK == 2)
            {
                DefectDetectResult_Text.Text = "不合格";
                DefectDetectResult_Text.ForeColor = Color.Red;
            }
            presentBox.Refresh();
        }

        private void ColorThr_Text_TextChanged(object sender, EventArgs e)
        {
            DefectDetect.colorThr = (int)double.Parse(ColorThr_Text.Text);
        }

        private void SizeThr_Text_TextChanged(object sender, EventArgs e)
        {
            DefectDetect.areaThr = (int)double.Parse(SizeThr_Text.Text);
        }

        private void SetTMRect_Button_Click(object sender, EventArgs e)
        {
            ImageShow.cls();
            imageDragTool.Select();
            TemplateMatch.setRectParaState = 1;
            rectTool_Button.Select();
        }

        private void form_Resize(object sender, EventArgs e)
        {
            EdgeExtraction_TabControl.Height = EdgeExtraction_Panel.Height + 20;
        }

        private void EdgeExtractionSelect_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            EdgeExtraction_TabControl.SelectedIndex = EdgeExtractionSelect_ComboBox.SelectedIndex;            
        }

        private void ShowColorHist_Button_Click(object sender, EventArgs e)
        {
            double time = PreProcess.callColorHist();
            runtime_StatusLabel.Text = "颜色直方图计算运行时间：" + time.ToString("0.0") + "ms";
            ColorChart.Refresh();
        }

        private void ColorChart_Paint(object sender, PaintEventArgs e)
        {
            if (PreProcess.colorHistExist)
            {
                int max = 0;
                for (int i = 0; i < 256; i++)
                    if (PreProcess.colorHistData[i] > max) max = PreProcess.colorHistData[i];

                Graphics g = e.Graphics;
                int preX = (ColorChart.Width - 256) / 2;
                int preY = ColorChart.Height - (PreProcess.colorHistData[0] * 100 / max + (ColorChart.Height - 115) / 2);
                for (int i = 1; i < 256; i++)
                {
                    int x = i + (ColorChart.Width - 256) / 2;
                    int y = ColorChart.Height - (PreProcess.colorHistData[i] * 100 / max + (ColorChart.Height - 115) / 2);
                    g.DrawLine(new Pen(Color.Blue, 1), new Point(preX, preY), new Point(x, y));
                    preX = x;
                    preY = y;
                }

                Font f = new Font("宋体", 14);

                if (PreProcess.mouseX > -1)
                {
                    g.DrawLine(new Pen(Color.FromArgb(64, 255, 0, 0), 1), new Point(PreProcess.mouseX, 16), new Point(PreProcess.mouseX, ColorChart.Height - 1));
                    g.DrawString((PreProcess.mouseX - (ColorChart.Width - 256) / 2).ToString(), f, Brushes.Blue, PreProcess.mouseX, 0);
                }
            
                if (PreProcess.mouseDownX > -1)
                {
                    for (int i=Math.Min(PreProcess.mouseX, PreProcess.mouseDownX)+1;i<=Math.Max(PreProcess.mouseX, PreProcess.mouseDownX)-1;i++)
                        g.DrawLine(new Pen(Color.FromArgb(16, 255, 0, 0), 1), new Point(i, 16), new Point(i, ColorChart.Height - 1));
                    g.DrawLine(new Pen(Color.Red, 1), new Point(PreProcess.mouseDownX, 16), new Point(PreProcess.mouseDownX, ColorChart.Height - 1));
                    g.DrawString((PreProcess.mouseDownX - (ColorChart.Width - 256) / 2).ToString(), f, Brushes.Blue, PreProcess.mouseDownX, 0);
                }
                if (PreProcess.lowThrX > -1)
                {
                    if (PreProcess.lowThrX == PreProcess.highThrX)
                    {
                        g.DrawLine(new Pen(Color.Red, 1), new Point(PreProcess.lowThrX, 16), new Point(PreProcess.lowThrX, ColorChart.Height - 1));
                        g.DrawString((PreProcess.lowThrX - (ColorChart.Width - 256) / 2).ToString(), f, Brushes.Blue, PreProcess.lowThrX, 0);
                    }
                    else
                    {
                        g.DrawLine(new Pen(Color.Red, 1), new Point(PreProcess.lowThrX, 16), new Point(PreProcess.lowThrX, ColorChart.Height - 1));
                        g.DrawString((PreProcess.lowThrX - (ColorChart.Width - 256) / 2).ToString(), f, Brushes.Blue, PreProcess.lowThrX, 0);
                        g.DrawLine(new Pen(Color.Red, 1), new Point(PreProcess.highThrX, 16), new Point(PreProcess.highThrX, ColorChart.Height - 1));
                        g.DrawString((PreProcess.highThrX - (ColorChart.Width - 256) / 2).ToString(), f, Brushes.Blue, PreProcess.highThrX, 0);
                        for (int i = PreProcess.lowThrX + 1; i <= PreProcess.highThrX - 1; i++)
                            g.DrawLine(new Pen(Color.FromArgb(16, 255, 0, 0), 1), new Point(i, 16), new Point(i, ColorChart.Height - 1));
                    }
                }
            }
        }

        private void ColorChart_MouseMove(object sender, MouseEventArgs e)
        {
            if (PreProcess.colorHistExist)
            {
                int LeftX = (ColorChart.Width - 256) / 2;
                int RightX = 255 + (ColorChart.Width - 256) / 2;
                int x = e.X;
                if (x < LeftX) x = LeftX;
                if (x > RightX) x = RightX;

                PreProcess.mouseX = x;

                ColorChart.Refresh();
            }
        }

        private void ColorChart_MouseDown(object sender, MouseEventArgs e)
        {
            int LeftX = (ColorChart.Width - 256) / 2;
            int RightX = 255 + (ColorChart.Width - 256) / 2;
            int x = e.X;
            if (x >= LeftX && x < RightX)
            {
                PreProcess.mouseDownX = x;
                PreProcess.lowThrX = PreProcess.highThrX = -1;
            }
        }

        private void ColorChart_MouseUp(object sender, MouseEventArgs e)
        {
            PreProcess.lowThrX = Math.Min(PreProcess.mouseX, PreProcess.mouseDownX);
            PreProcess.highThrX = Math.Max(PreProcess.mouseX, PreProcess.mouseDownX);

            if (PreProcess.mouseX == PreProcess.mouseDownX)
                PreProcess.highThrX = 255 + (ColorChart.Width - 256) / 2;

            PreProcess.lowThr = PreProcess.lowThrX - (ColorChart.Width - 256) / 2;
            PreProcess.highThr = PreProcess.highThrX - (ColorChart.Width - 256) / 2;

            PreProcess.mouseDownX = -1;

            double time = PreProcess.callThreshold();
            runtime_StatusLabel.Text = "图像二值化运行时间：" + time.ToString("0.0") + "ms";
            ColorChart.Refresh();
            presentBox.Refresh();
        }

        private void ShowBoundingBox_Button_Click(object sender, EventArgs e)
        {
            if (ShowBoundingBox_Button.Text == "显示外接矩形")
            {
                ShowBoundingBox_Button.Text = "隐藏外接矩形";
                PreProcess.showBoundingBox = true;
            }
            else
            {
                ShowBoundingBox_Button.Text = "显示外接矩形";
                PreProcess.showBoundingBox = false;
            }
            presentBox.Refresh();
        }

        private void ShowEdge_Button_Click(object sender, EventArgs e)
        {
            if (ShowEdge_Button.Text == "显示边缘")
            {
                ShowEdge_Button.Text = "隐藏边缘";
                double time;
                switch (EdgeExtraction_TabControl.SelectedIndex)
                {
                    case 0:
                        time = PreProcess.callSobel();
                        if (PreProcess.sobelOTSU)
                            SobelThr_Tracker.Value = PreProcess.sobelThr;
                        runtime_StatusLabel.Text = "Sobel边缘提取运行时间：" + time.ToString("0.0") + "ms";
                        break;
                    case 1:
                        time = PreProcess.callCanny();
                        runtime_StatusLabel.Text = "Sobel边缘提取运行时间：" + time.ToString("0.0") + "ms";
                        break;
                    case 2:
                        //figureDetect.edgeDetectStep = int.Parse(edgeStepPara_Text.Text);
                        //figureDetect.edgeDetectThr = int.Parse(edgeThrPara_Text.Text);
                        PreProcess.LSDStep = int.Parse(edgeStepPara_Text.Text);
                        PreProcess.LSDThr = int.Parse(edgeThrPara_Text.Text);
                        //time = figureDetect.newEdgeDetectFun();
                        time = PreProcess.callLSD();

                        runtime_StatusLabel.Text = "LSD边缘提取运行时间：" + time.ToString("0.0") + "ms";
                        break;
                }
                presentBox.Refresh();
            }
            else
            {
                ShowEdge_Button.Text = "显示边缘";
                while (ImageShow.edgeDots != null && ImageShow.edgeDots.Count > 0)
                    ImageShow.edgeDots.RemoveAt(ImageShow.edgeDots.Count - 1);
                presentBox.Refresh();
            }
        }

        private void Redo_Button_Click(object sender, EventArgs e)
        {
            if (ImageShow.edgeDots!=null && ImageShow.shapes.Count>0)
                ImageShow.shapes.RemoveAt(ImageShow.shapes.Count - 1);
            presentBox.Refresh();
        }

        private void SobelKsize_Tracker_ValueChanged(object sender, EventArgs e)
        {
            bool update = false;
            if (PreProcess.sobelKsize != SobelKsize_Tracker.Value * 2 + 1 && ShowEdge_Button.Text == "隐藏边缘")
                update = true;
            PreProcess.sobelKsize = SobelKsize_Tracker.Value * 2 + 1;
            SobelKsize_Text.Text = PreProcess.sobelKsize.ToString();

            if (update)
            {
                double time = PreProcess.callSobel();
                if (PreProcess.sobelOTSU)
                    SobelThr_Tracker.Value = PreProcess.sobelThr;
                runtime_StatusLabel.Text = "Sobel边缘提取运行时间：" + time.ToString("0.0") + "ms";
                presentBox.Refresh();
            }
        }

        private void SobelThr_Tracker_ValueChanged(object sender, EventArgs e)
        {
            bool update = false;
            if (PreProcess.sobelThr != SobelThr_Tracker.Value * 2 + 1 && ShowEdge_Button.Text == "隐藏边缘")
                update = true;
            PreProcess.sobelThr = SobelThr_Tracker.Value;
            SobelThr_Text.Text = PreProcess.sobelThr.ToString();

            if (update)
            {
                double time = PreProcess.callSobel();
                if (PreProcess.sobelOTSU)
                    SobelThr_Tracker.Value = PreProcess.sobelThr;
                runtime_StatusLabel.Text = "Sobel边缘提取运行时间：" + time.ToString("0.0") + "ms";
                presentBox.Refresh();
            }
        }

        private void SobelOTSU_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            PreProcess.sobelOTSU = SobelOTSU_CheckBox.Checked;
            SobelThr_Tracker.Enabled = !SobelOTSU_CheckBox.Checked;
        }

        private void ManualFixSize_Tracker_ValueChanged(object sender, EventArgs e)
        {
            CameraCalibrate.manualFixSize = ManualFixSize_Tracker.Value * 2 + 1;
            ManualFixSize_Text.Text = CameraCalibrate.manualFixSize.ToString();
        }

        private void FixSize_Tracker_ValueChanged(object sender, EventArgs e)
        {
            CameraCalibrate.fixSize = FixSize_Tracker.Value * 2 + 1;
            FixSize_Text.Text = CameraCalibrate.fixSize.ToString();
        }

        private void SavePixelSize_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CameraCalibrate.savePixelSize = SavePixelSize_CheckBox.Checked;
        }

        private void FigureColorLowThr_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                figureDetect.colorLowThr = int.Parse(FigureColorLowThr_Text.Text);
            }
            catch
            {
                MessageBox.Show("请输入一个整数");
            }

        }

        private void FigureColorHighThr_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                figureDetect.colorHighThr = int.Parse(FigureColorHighThr_Text.Text);
            }
            catch
            {
                MessageBox.Show("请输入一个整数");
            }
        }

        private void FigureColorRate_Tracker_ValueChanged(object sender, EventArgs e)
        {
            figureDetect.colorRate = FigureColorRate_Tracker.Value;
            FigureColorRate_Text.Text = figureDetect.colorRate.ToString() + "%";            
        }

        private void CircleRadiusRange_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                figureDetect.circleRadiusRange = double.Parse(CircleRadiusRange_Text.Text);
                if (figureDetect.circleRadiusRange < 0 || figureDetect.circleRadiusRange > 100)
                {
                    figureDetect.circleRadiusRange = 10;
                    MessageBox.Show("请输入一个0-100之间的浮点数");
                }
            }
            catch
            {
                MessageBox.Show("请输入一个0-100之间的浮点数");
            }
        }

        private void RectWHRange_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                figureDetect.rectWHRange = double.Parse(RectWHRange_Text.Text);
                if (figureDetect.rectWHRange < 0 || figureDetect.rectWHRange > 100)
                {
                    figureDetect.rectWHRange = 10;
                    MessageBox.Show("请输入一个0-100之间的浮点数");
                }
            }
            catch
            {
                MessageBox.Show("请输入一个0-100之间的浮点数");
            }
        }

        private void CannyLowThr_Tracker_ValueChanged(object sender, EventArgs e)
        {
            bool update = false;
            if (PreProcess.cannyLowThr != CannyLowThr_Tracker.Value * 5 && ShowEdge_Button.Text == "隐藏边缘")
                update = true;
            PreProcess.cannyLowThr = CannyLowThr_Tracker.Value * 5;
            CannyLowThr_Text.Text = PreProcess.cannyLowThr.ToString();

            if (update)
            {
                double time = PreProcess.callCanny();
                runtime_StatusLabel.Text = "Canny边缘提取运行时间：" + time.ToString("0.0") + "ms";
                presentBox.Refresh();
            }
        }

        private void CannyHighThr_Tracker_ValueChanged(object sender, EventArgs e)
        {
            bool update = false;
            if (PreProcess.cannyHighThr != CannyHighThr_Tracker.Value * 5 && ShowEdge_Button.Text == "隐藏边缘")
                update = true;
            PreProcess.cannyHighThr = CannyHighThr_Tracker.Value * 5;
            CannyHighThr_Text.Text = PreProcess.cannyHighThr.ToString();

            if (update)
            {
                double time = PreProcess.callCanny();
                runtime_StatusLabel.Text = "Canny边缘提取运行时间：" + time.ToString("0.0") + "ms";
                presentBox.Refresh();
            }
        }

        private void LineFixRange_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Config.lineFixRange = int.Parse(LineFixRange_Text.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("请输入一个正整数");
            }
        }

        private void PointFixRange_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Config.pointFixRange = int.Parse(PointFixRange_Text.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("请输入一个正整数");
            }
        }

        private void manualLineSelect_Button_Click(object sender, EventArgs e)
        {
            imageDragTool.Select();
            DistanceMeasure.lineSelectState = 1;
            lineTool_Button.Select();
        }

        private void LineParaA_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DistanceMeasure.lineParaA = double.Parse(LineParaA_Text.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("请输入一个浮点数");
            }
        }

        private void LineParaB_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DistanceMeasure.lineParaB = double.Parse(LineParaB_Text.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("请输入一个浮点数");
            }
        }

        private void LineParaC_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DistanceMeasure.lineParaC = double.Parse(LineParaC_Text.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("请输入一个浮点数");
            }

        }

        private void autoLineSelect_Button_Click(object sender, EventArgs e)
        {
            DistanceMeasure.lineSelectState = 2;
            presentBox.Cursor = Cursors.Arrow;
            nowTool = (BaseTool)(new SelectTool());
        }

        private void PLDistanceMeasure_Button_Click(object sender, EventArgs e)
        {
            DistanceMeasure.pointSelectState = 0;
            DistanceMeasure.lineSelectState = 0;
            double x1 = double.Parse(pointX_Text1.Text);
            double y1 = double.Parse(pointY_Text1.Text);

            double a = DistanceMeasure.lineParaB;
            double b = -DistanceMeasure.lineParaA;
            double c = 0 - x1 * a - y1 * b;

            double x2,y2;
            if (b==0)
            {
                y2 = (c/a*DistanceMeasure.lineParaA-DistanceMeasure.lineParaC)/(DistanceMeasure.lineParaB-b/a*DistanceMeasure.lineParaA);
                x2 = (0 - b * y2 - c) / a;
            }
            else
            { 
                x2 = (c / b * DistanceMeasure.lineParaB - DistanceMeasure.lineParaC) / (DistanceMeasure.lineParaA - a / b * DistanceMeasure.lineParaB);
                y2 = (0 - a * x2 - c) / b;
            }

            ImageShow.addPoint(new PointF((float)x2, (float)y2));
            presentBox.Refresh();

            double dist = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
            PLMeasureResult_Text.Text = dist.ToString("0.00") + "像素";
            if (CameraCalibrate.pixelSize != 0)
                PLMeasureResultMM_Text.Text = (CameraCalibrate.runMeasure((float)x1, (float)y1, (float)x2, (float)y2)).ToString("0.00") + "毫米";
        }

        private void lineMaxOffset_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DefectDetect.lineMaxOffset = int.Parse(lineMaxOffset_Text.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("请输入一个整数");
            }
        }

        private void imageStitchSource11_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ImageStitching.originFilenameList[0] = null;
                ImageStitching.originImageList[0] = null;
                imageStitchSource11.Image = null;
                imageStitchSource11.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.originFilenameList[0] = openFileDialog.FileName;
                            ImageStitching.originImageList[0] = new Bitmap(openFileDialog.FileName);
                            imageStitchSource11.Image = ImageStitching.originImageList[0];
                            imageStitchSource11.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void imageStitchSource12_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ImageStitching.originFilenameList[1] = null;
                ImageStitching.originImageList[1] = null;
                imageStitchSource12.Image = null;
                imageStitchSource12.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.originFilenameList[1] = openFileDialog.FileName;
                            ImageStitching.originImageList[1] = new Bitmap(openFileDialog.FileName);
                            imageStitchSource12.Image = ImageStitching.originImageList[1];
                            imageStitchSource12.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void imageStitchSource13_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ImageStitching.originFilenameList[2] = null;
                ImageStitching.originImageList[2] = null;
                imageStitchSource13.Image = null;
                imageStitchSource13.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.originFilenameList[2] = openFileDialog.FileName;
                            ImageStitching.originImageList[2] = new Bitmap(openFileDialog.FileName);
                            imageStitchSource13.Image = ImageStitching.originImageList[2];
                            imageStitchSource13.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void imageStitchSource14_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ImageStitching.originFilenameList[3] = null;
                ImageStitching.originImageList[3] = null;
                imageStitchSource14.Image = null;
                imageStitchSource14.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.originFilenameList[3] = openFileDialog.FileName;
                            ImageStitching.originImageList[3] = new Bitmap(openFileDialog.FileName);
                            imageStitchSource14.Image = ImageStitching.originImageList[3];
                            imageStitchSource14.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource11_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[0]!=null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[0] = null;
                ImageStitching.fastImageList[0] = null;
                fastImageStitchSource11.Image = null;
                fastImageStitchSource11.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[0] = openFileDialog.FileName;
                            ImageStitching.fastImageList[0] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource11.Image = ImageStitching.fastImageList[0];
                            fastImageStitchSource11.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource12_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[1] != null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[1] = null;
                ImageStitching.fastImageList[1] = null;
                fastImageStitchSource12.Image = null;
                fastImageStitchSource12.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[1] = openFileDialog.FileName;
                            ImageStitching.fastImageList[1] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource12.Image = ImageStitching.fastImageList[1];
                            fastImageStitchSource12.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource13_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[2] != null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[2] = null;
                ImageStitching.fastImageList[2] = null;
                fastImageStitchSource13.Image = null;
                fastImageStitchSource13.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[2] = openFileDialog.FileName;
                            ImageStitching.fastImageList[2] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource13.Image = ImageStitching.fastImageList[2];
                            fastImageStitchSource13.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource14_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[3] != null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[3] = null;
                ImageStitching.fastImageList[3] = null;
                fastImageStitchSource14.Image = null;
                fastImageStitchSource14.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[3] = openFileDialog.FileName;
                            ImageStitching.fastImageList[3] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource14.Image = ImageStitching.fastImageList[3];
                            fastImageStitchSource14.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource21_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[4] != null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[4] = null;
                ImageStitching.fastImageList[4] = null;
                fastImageStitchSource21.Image = null;
                fastImageStitchSource21.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[4] = openFileDialog.FileName;
                            ImageStitching.fastImageList[4] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource21.Image = ImageStitching.fastImageList[4];
                            fastImageStitchSource21.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource22_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[5] != null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[5] = null;
                ImageStitching.fastImageList[5] = null;
                fastImageStitchSource22.Image = null;
                fastImageStitchSource22.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[5] = openFileDialog.FileName;
                            ImageStitching.fastImageList[5] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource22.Image = ImageStitching.fastImageList[5];
                            fastImageStitchSource22.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource23_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[6] != null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[6] = null;
                ImageStitching.fastImageList[6] = null;
                fastImageStitchSource23.Image = null;
                fastImageStitchSource23.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[6] = openFileDialog.FileName;
                            ImageStitching.fastImageList[6] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource23.Image = ImageStitching.fastImageList[6];
                            fastImageStitchSource23.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource24_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[7] != null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[7] = null;
                ImageStitching.fastImageList[7] = null;
                fastImageStitchSource24.Image = null;
                fastImageStitchSource24.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[7] = openFileDialog.FileName;
                            ImageStitching.fastImageList[7] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource24.Image = ImageStitching.fastImageList[7];
                            fastImageStitchSource24.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource31_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[8] != null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[8] = null;
                ImageStitching.fastImageList[8] = null;
                fastImageStitchSource31.Image = null;
                fastImageStitchSource31.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[8] = openFileDialog.FileName;
                            ImageStitching.fastImageList[8] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource31.Image = ImageStitching.fastImageList[8];
                            fastImageStitchSource31.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource32_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[9] != null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[9] = null;
                ImageStitching.fastImageList[9] = null;
                fastImageStitchSource32.Image = null;
                fastImageStitchSource32.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[9] = openFileDialog.FileName;
                            ImageStitching.fastImageList[9] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource32.Image = ImageStitching.fastImageList[9];
                            fastImageStitchSource32.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource33_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[10] != null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[10] = null;
                ImageStitching.fastImageList[10] = null;
                fastImageStitchSource33.Image = null;
                fastImageStitchSource33.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[10] = openFileDialog.FileName;
                            ImageStitching.fastImageList[10] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource33.Image = ImageStitching.fastImageList[10];
                            fastImageStitchSource33.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource34_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[11] != null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[11] = null;
                ImageStitching.fastImageList[11] = null;
                fastImageStitchSource34.Image = null;
                fastImageStitchSource34.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[11] = openFileDialog.FileName;
                            ImageStitching.fastImageList[11] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource34.Image = ImageStitching.fastImageList[11];
                            fastImageStitchSource34.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource41_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[12] != null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[12] = null;
                ImageStitching.fastImageList[12] = null;
                fastImageStitchSource41.Image = null;
                fastImageStitchSource41.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[12] = openFileDialog.FileName;
                            ImageStitching.fastImageList[12] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource41.Image = ImageStitching.fastImageList[12];
                            fastImageStitchSource41.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource42_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[13] != null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[13] = null;
                ImageStitching.fastImageList[13] = null;
                fastImageStitchSource42.Image = null;
                fastImageStitchSource42.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[13] = openFileDialog.FileName;
                            ImageStitching.fastImageList[13] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource42.Image = ImageStitching.fastImageList[13];
                            fastImageStitchSource42.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource43_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[14] != null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[14] = null;
                ImageStitching.fastImageList[14] = null;
                fastImageStitchSource43.Image = null;
                fastImageStitchSource43.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[14] = openFileDialog.FileName;
                            ImageStitching.fastImageList[14] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource43.Image = ImageStitching.fastImageList[14];
                            fastImageStitchSource43.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void fastImageStitchSource44_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ImageStitching.fastFilenameList[15] != null)
                    ImageStitching.fastFilenameNum--;
                ImageStitching.fastFilenameList[15] = null;
                ImageStitching.fastImageList[15] = null;
                fastImageStitchSource44.Image = null;
                fastImageStitchSource44.Refresh();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图片文件|*.bmp;*.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.tif;*.tiff;*.png;*.ico|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName != "")
                    {
                        try
                        {
                            ImageStitching.fastFilenameList[15] = openFileDialog.FileName;
                            ImageStitching.fastImageList[15] = new Bitmap(openFileDialog.FileName);
                            ImageStitching.fastFilenameNum++;
                            fastImageStitchSource44.Image = ImageStitching.fastImageList[15];
                            fastImageStitchSource44.Refresh();
                        }
                        catch
                        {
                            MessageBox.Show("请输入正确的图像文件！");
                        }
                    }
                }
            }
        }

        private void form_SizeChanged(object sender, EventArgs e)
        {
            presentBox.Size = new Size(tableLayoutPanel52.Size.Width - 466, tableLayoutPanel52.Size.Height - 6);
          
        }

        private void LowArea_Text_TextChanged(object sender, EventArgs e)
        {
            figureDetect.lowArea = int.Parse(LowArea_Text.Text);
        }

        private void HighArea_Text_TextChanged(object sender, EventArgs e)
        {
            figureDetect.highArea = int.Parse(HighArea_Text.Text);
        }

        private void AreaDetect_Button_Click(object sender, EventArgs e)
        {
            double time = figureDetect.callAreaDetect();
            runtime_StatusLabel.Text = "面积检测计算运行时间：" + time.ToString("0.0") + "ms";
            presentBox.Refresh();
        }

        private void Angle1_Text_TextChanged(object sender, EventArgs e)
        {
            figureDetect.angle1 = float.Parse(Angle1_Text.Text);
        }

        private void Angle2_Text_TextChanged(object sender, EventArgs e)
        {
            figureDetect.angle2 = float.Parse(Angle2_Text.Text);
        }

        private void Angle3_Text_TextChanged(object sender, EventArgs e)
        {
            figureDetect.angle3 = float.Parse(Angle3_Text.Text);
        }

        private void AngleRange_Text_TextChanged(object sender, EventArgs e)
        {
            figureDetect.range = float.Parse(AngleRange_Text.Text);
        }

        private void MinLength_Text_TextChanged(object sender, EventArgs e)
        {
            figureDetect.minLength = float.Parse(MinLength_Text.Text);
        }

        private void MaxGap_Text_TextChanged(object sender, EventArgs e)
        {
            figureDetect.maxGap = float.Parse(MaxGap_Text.Text);
        }

        private void TriangleDetect_Button_Click(object sender, EventArgs e)
        {
            double time = figureDetect.callTriangleDetect();
            runtime_StatusLabel.Text = "三角形检测计算运行时间：" + time.ToString("0.0") + "ms";
            presentBox.Refresh();
        }

        private void colorBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int i = 0; i <= 9; i++)
                g.DrawLine(new Pen(Color.Gray, 3), new Point(i*39+1,0), new Point(i*39+1,197));
            for (int i = 0; i <= 5; i++)
                g.DrawLine(new Pen(Color.Gray, 3), new Point(0, i * 39+1), new Point(353, i * 39+1));

            for (int i = 0; i < ColorRecog.colorTab.Count; i++)
                g.DrawRectangle(new Pen(ColorRecog.colorTab[i], 19), new Rectangle((i % 9) * 39 + 12, (i / 9) * 39 + 12, 17, 17));
            for (int i = ColorRecog.colorTab.Count; i < 45; i++)
            {
                g.DrawRectangle(new Pen(Color.Red, 1), new Rectangle((i % 9) * 39 + 20, (i / 9) * 39 + 12, 1, 17));
                g.DrawRectangle(new Pen(Color.Red, 1), new Rectangle((i % 9) * 39 + 12, (i / 9) * 39 + 20, 17, 1));
            }
            Pen selectPen = new Pen(Color.Red, 3);
            selectPen.DashPattern = new float[] { 3, 3 };
            if (ColorRecog.result != -1)
                g.DrawRectangle(selectPen, new Rectangle((ColorRecog.result % 9) * 39 + 1, (ColorRecog.result / 9) * 39 + 1, 38, 38));
        }

        private void colorBox_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i=0;i<45;i++)
                if (e.X >= (i % 9) * 39 + 3 && e.X <= (i % 9) * 39 + 38 && e.Y >= (i / 9) * 39 + 3 && e.Y <= (i / 9) * 39 + 38)
                {
                    if (i < ColorRecog.colorTab.Count())
                    {
                        ColorRecog.colorTab.RemoveAt(i);
                        colorBox.Refresh();
                    }
                    else
                    {
                        ColorDialog colorForm = new ColorDialog();

                        if (colorForm.ShowDialog() == DialogResult.OK)
                        {

                            ColorRecog.colorTab.Add(colorForm.Color);
                            colorBox.Refresh();
                        }
                    }
                    break;
                }
        }

        private void ColorRecog_Button_Click(object sender, EventArgs e)
        {
            nowTool = (BaseTool)(new ColorTool());
            presentBox.Cursor = Cursors.Cross;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //tabControl1.TabPage3.Parent = null;
            System.Threading.Thread.Sleep(1000);
            MessageBox.Show("未连接硬件，请重试！");

        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
            MessageBox.Show("请先调试硬件连接");
        }

        private void ColorMode_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ColorRecog.mode = ColorMode_ComboBox.SelectedIndex;
        }

        private void StitchMinRate_Text_TextChanged(object sender, EventArgs e)
        {
            ImageStitching.minRate = float.Parse(StitchMinRate_Text.Text);
        }

        private void TMthr_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TemplateMatch.thr = int.Parse(TMthr_Text.Text);
            }
            catch (Exception ex)
            {
                Object a = new object();
                ex.Equals(a);
            }
        }

        private void TMlowAngle_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TemplateMatch.lowAngle = int.Parse(TMlowAngle_Text.Text);
            }
            catch (Exception ex)
            {
                Object a = new object();
                ex.Equals(a);
            }
        }

        private void TMhighAngle_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TemplateMatch.highAngle = int.Parse(TMhighAngle_Text.Text);
            }
            catch (Exception ex)
            {
                Object a = new object();
                ex.Equals(a);
            }
        }

        private void TMlevel_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TemplateMatch.level = int.Parse(TMlevel_Text.Text);
            }
            catch (Exception ex)
            {
                Object a = new object();
                ex.Equals(a);
            }
        }

        private void TMquestNum_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TemplateMatch.questNum = int.Parse(TMquestNum_Text.Text);
            }
            catch (Exception ex)
            {
                Object a = new object();
                ex.Equals(a);
            }
        }

        private void TMminRate_Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TemplateMatch.minRate = float.Parse(TMminRate_Text.Text);
            }
            catch (Exception ex)
            {
                Object a = new object();
                ex.Equals(a);
            }
        }

        private void ShowTemplateFinetune_Button_Click(object sender, EventArgs e)
        {
            if (TemplateMatch.templateROI!=new Rectangle(0,0,0,0))
                TemplateMatch.callTemplateFinetune();
            presentBox.Refresh();
        }

        private void SaveTemplate_Button_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "模板文件|*.tr|所有文件|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                TemplateMatch.filename = saveFileDialog.FileName;
                TemplateMatch.callTemplateSave();
            }
        }

        private void LoadTemplate_Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "模板文件|*.tr|所有文件|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TemplateMatch.filename = openFileDialog.FileName;
                TemplateMatch.callTemplateLoad();
                TMlevel_Text.Text = TemplateMatch.level.ToString();
                TMlowAngle_Text.Text = TemplateMatch.lowAngle.ToString();
                TMhighAngle_Text.Text = TemplateMatch.highAngle.ToString();
                TMminRate_Text.Text = TemplateMatch.minRate.ToString("0.0");
                TMquestNum_Text.Text = TemplateMatch.questNum.ToString();
                TMthr_Text.Text = TemplateMatch.thr.ToString();
            }
        }

        private void SetTMObjectRect_Button_Click(object sender, EventArgs e)
        {
            ImageShow.cls();
            imageDragTool.Select();
            TemplateMatch.setRectParaState = 2;
            rectTool_Button.Select();
        }

        private void TemplateMatch_Button_Click(object sender, EventArgs e)
        {
            double time = TemplateMatch.callTemplateDetect();
            runtime_StatusLabel.Text = "模板匹配运行时间：" + time.ToString("0.0") + "ms";
            presentBox.Refresh();
        }

        private void RotateMode_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (RotateMode_CheckBox.Checked)
                CodeRecog.rotateMode = 1;
            else
                CodeRecog.rotateMode = 0;
        }

        private void ZXingCodeProcess_Button_Click(object sender, EventArgs e)
        {
            if (ImageShow.nowFile != null)
                if (CodeRecog.setRectParaState == 1 && CodeRecog.shapeIndex != -1)
                {
                    Console.WriteLine(ZXingCodeType_ComboBox.SelectedItem.ToString());
                    CodeRecog.initZXingReader(ZXingCodeType_ComboBox.SelectedItem.ToString());
                    double time = CodeRecog.ZXingCodeRecog();
                    runtime_StatusLabel.Text = "维码识别运行时间：" + time.ToString("0.0") + "ms";
                    Codes_List.Items.Clear();
                    if (CodeRecog.codeNum == 0)
                        Codes_List.Items.Add("未成功识别！");
                    for (int i = 0; i < CodeRecog.codeNum; i++)
                        Codes_List.Items.Add("(" + CodeRecog.codeType[i] + ")" + CodeRecog.codeContent[i]);
                    Codes_List.Refresh();
                }
        }




    }
}
