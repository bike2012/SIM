using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SIMproject
{
    class LineShape : BaseShape
    {
        public override void draw(Graphics g)
        {
            float x = P1.X * ImageShow.scale + ImageShow.position.X;
            float y = P1.Y * ImageShow.scale + ImageShow.position.Y;
            float x2 = P2.X * ImageShow.scale + ImageShow.position.X;
            float y2 = P2.Y * ImageShow.scale + ImageShow.position.Y;
            float r = R;
            g.DrawEllipse(Config.anchorPen, x - Config.pointWidth, y - Config.pointWidth, Config.pointWidth * 2, Config.pointWidth * 2);
            g.DrawEllipse(Config.anchorPen, x2 - Config.pointWidth, y2 - Config.pointWidth, Config.pointWidth * 2, Config.pointWidth * 2);
            if (r != 100)
                g.DrawLine(Config.shapePen, new PointF((float)(x - Math.Cos(R) * 5000), (float)(y - Math.Sin(R) * 5000)), new PointF((float)(x + Math.Cos(R) * 5000), (float)(y + Math.Sin(R) * 5000)));

            if (ImageShow.showPara)
            {
                string s = "Pos:(" + P1.X.ToString("0.00") + "," + P1.Y.ToString("0.00") + ")" + Environment.NewLine + "Dir:" + (R*360/Math.PI/2).ToString("0.00");
                Font f = new Font("宋体", 18);
                g.DrawString(s, f, Brushes.GreenYellow, x + 5, y + 5);
            }
        }

        public override void setPara(PointF p1, PointF p2, float r, PointF p3)
        {
            P1 = p1; P2 = p2; R = r;
        }

        public override void setPartPara(int type, PointF p1, PointF p2, float r)
        {
            P1 = p1; P2 = p2; R = r;
        }

        public override void getPara(ref float para1, ref float para2, ref float para3, ref float para4, ref float para5)
        {
            para1 = P1.X; para2 = P1.Y; para5 = R;
        }

        public override int onShape(PointF p1)
        {
            float x = P1.X * ImageShow.scale + ImageShow.position.X;
            float y = P1.Y * ImageShow.scale + ImageShow.position.Y;
            float x2 = P2.X * ImageShow.scale + ImageShow.position.X;
            float y2 = P2.Y * ImageShow.scale + ImageShow.position.Y;
            float r = R * ImageShow.scale;

            if (Math.Sqrt((x - p1.X) * (x - p1.X) + (y - p1.Y) * (y - p1.Y)) <= Config.pointWidth + Config.shapePen.Width)
                return 11;
            if (Math.Sqrt((x2 - p1.X) * (x2 - p1.X) + (y2 - p1.Y) * (y2 - p1.Y)) <= Config.pointWidth + Config.shapePen.Width)
                return 13;
            if (Math.Abs(Math.Cos(R)) > Math.Abs(Math.Sin(R)))
            {
                if (Math.Abs(p1.Y - y - (p1.X - x) / Math.Cos(R) * Math.Sin(R)) <= Config.pointFixRange)
                    return 16;
            }
            else
            {
                if (Math.Abs(p1.X - x - (p1.Y - y) / Math.Sin(R) * Math.Cos(R)) <= Config.pointFixRange)
                    return 16;
            }
            return 0;
        }
    }
}
