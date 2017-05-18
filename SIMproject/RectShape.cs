using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SIMproject
{
    class RectShape : BaseShape
    {
        private PointF[] pts = new PointF[4];
        float dirx, diry;
        double dist;
        float dirr;
        bool showPts = true;

        public override void draw(Graphics g)
        {            
            float r = R;

            float x1 = P1.X * ImageShow.scale + ImageShow.position.X;
            float y1 = P1.Y * ImageShow.scale + ImageShow.position.Y;
            float x2 = P2.X * ImageShow.scale + ImageShow.position.X;
            float y2 = P2.Y * ImageShow.scale + ImageShow.position.Y;

            pts[0] = new PointF(x2, y2);
            double a = Math.Sin((double)r), b = -Math.Cos((double)r), c = 0 - x1 * a - y1 * b;
            pts[1] = new PointF((float)(x2 - 2 * a * ((a * x2 + b * y2 + c) / (a * a + b * b))), (float)(y2 - 2 * b * ((a * x2 + b * y2 + c) / (a * a + b * b))));
            pts[2] = new PointF((x1 * 2 - x2), (y1 * 2 - y2));
            a = Math.Sin((double)(r - Math.PI / 2.0)); b = -Math.Cos((double)(r - Math.PI / 2.0)); c = 0 - x1 * a - y1 * b;
            pts[3] = new PointF((float)(x2 - 2 * a * ((a * x2 + b * y2 + c) / (a * a + b * b))), (float)(y2 - 2 * b * ((a * x2 + b * y2 + c) / (a * a + b * b))));
            g.DrawPolygon(Config.shapePen, pts);

            double dist = Math.Sqrt((pts[2].X - pts[1].X) * (pts[2].X - pts[1].X) + (pts[2].Y - pts[1].Y) * (pts[2].Y - pts[1].Y)) / 4;
            
            dirx = x1 + (float)(Math.Cos((double)r) * dist);
            diry = y1 + (float)(Math.Sin((double)r) * dist);
            if (showPts)
            {
                g.DrawLine(Config.shapePen, new PointF(x1, y1), new PointF(dirx, diry));
                g.DrawEllipse(Config.anchorPen, x1 - Config.pointWidth, y1 - Config.pointWidth, Config.pointWidth * 2, Config.pointWidth * 2);
                g.DrawEllipse(Config.anchorPen, dirx - Config.pointWidth, diry - Config.pointWidth, Config.pointWidth * 2, Config.pointWidth * 2);


                for (int i = 0; i < 4; i++)
                    g.DrawEllipse(Config.anchorPen, pts[i].X - Config.pointWidth, pts[i].Y - Config.pointWidth, Config.pointWidth * 2, Config.pointWidth * 2);
            }

            if (ImageShow.showPara && showPts)
            {
                string s = "Pos:(" + P1.X.ToString("0.00") + "," + P1.Y.ToString("0.00") + ")"
                    + Environment.NewLine + "Dir:" + (R*360/Math.PI/2).ToString("0.00");
                Font f = new Font("宋体", 18);
                g.DrawString(s, f, Brushes.GreenYellow, x1 + 5, y1 + 5);
            }
        }

        public override void setPara(PointF p1, PointF p2, float r, PointF p3)
        {
            P1 = p1; P2 = p2; R = r;

            dist = Math.Sqrt((P2.X - P1.X) * (P2.X - P1.X) + (P2.Y - P1.Y) * (P2.Y - P1.Y));

            float xr = P2.X - P1.X;
            float yr = P2.Y - P1.Y;

            dirr = 0;
            if (xr == 0)
            {
                if (yr >= 0) dirr = (float)Math.PI / 2;
                else dirr = (float)Math.PI / 2 * 3;
            }
            else
            {
                dirr = (float)Math.Atan(yr / xr);
                if (xr < 0)
                {
                    dirr += (float)Math.PI;
                }
                else
                {
                    if (yr < 0)
                        dirr += (float)Math.PI * 2;
                }
            }

        }

        public override void setPartPara(int type, PointF p1, PointF p2, float r)
        {
            if (type == 1)
            {
                dirr += r - R;

                P2.X = P1.X + (float)(Math.Cos((double)dirr) * dist);
                P2.Y = P1.Y + (float)(Math.Sin((double)dirr) * dist);

                R = r;

            }
            else if (type == 2)
            {
                setPara(p1, p2, r, new PointF(0, 0));
            }
            else if (type == 3)
                showPts = false;
        }

        public override void getPara(ref float para1, ref float para2, ref float para3, ref float para4, ref float para5)
        {
            float r = R;
            float x1 = P1.X;
            float y1 = P1.Y;
            float x2 = P2.X;
            float y2 = P2.Y;

            pts[0] = new PointF(x2, y2);
            double a = Math.Sin((double)r), b = -Math.Cos((double)r), c = 0 - x1 * a - y1 * b;
            pts[1] = new PointF((float)(x2 - 2 * a * ((a * x2 + b * y2 + c) / (a * a + b * b))), (float)(y2 - 2 * b * ((a * x2 + b * y2 + c) / (a * a + b * b))));
            pts[2] = new PointF((x1 * 2 - x2), (y1 * 2 - y2));
            a = Math.Sin((double)(r - Math.PI / 2.0)); b = -Math.Cos((double)(r - Math.PI / 2.0)); c = 0 - x1 * a - y1 * b;
            pts[3] = new PointF((float)(x2 - 2 * a * ((a * x2 + b * y2 + c) / (a * a + b * b))), (float)(y2 - 2 * b * ((a * x2 + b * y2 + c) / (a * a + b * b))));

            para1 = x1;
            para2 = y1;
            para3 = (float)Math.Sqrt((pts[2].X - pts[1].X) * (pts[2].X - pts[1].X) + (pts[2].Y - pts[1].Y) * (pts[2].Y - pts[1].Y));
            para4 = (float)Math.Sqrt((pts[3].X - pts[2].X) * (pts[3].X - pts[2].X) + (pts[3].Y - pts[2].Y) * (pts[3].Y - pts[2].Y));
            para5 = R;
        }

        public override int onShape(PointF p1)
        {
            float x = P1.X * ImageShow.scale + ImageShow.position.X;
            float y = P1.Y * ImageShow.scale + ImageShow.position.Y;
            if (Math.Sqrt((x - p1.X) * (x - p1.X) + (y - p1.Y) * (y - p1.Y)) <= Config.pointWidth + Config.shapePen.Width)
                return 1;
            if (Math.Sqrt((dirx - p1.X) * (dirx - p1.X) + (diry - p1.Y) * (diry - p1.Y)) <= Config.pointWidth + Config.shapePen.Width)
                return 3;
            for (int i = 0; i < 4; i++)
                if (Math.Sqrt((pts[i].X - p1.X) * (pts[i].X - p1.X) + (pts[i].Y - p1.Y) * (pts[i].Y - p1.Y)) <= Config.pointWidth + Config.shapePen.Width)
                    return 4;
            return 0;
        }
    }
}
