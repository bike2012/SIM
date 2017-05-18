using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SIMproject
{
    class TriangleShape : BaseShape
    {
        public override void draw(Graphics g)
        {
            float x1 = P1.X * ImageShow.scale + ImageShow.position.X;
            float y1 = P1.Y * ImageShow.scale + ImageShow.position.Y;

            float x2 = P2.X * ImageShow.scale + ImageShow.position.X;
            float y2 = P2.Y * ImageShow.scale + ImageShow.position.Y;

            float x3 = P3.X * ImageShow.scale + ImageShow.position.X;
            float y3 = P3.Y * ImageShow.scale + ImageShow.position.Y;

            g.DrawLine(Config.shapePen, new PointF(x1, y1), new PointF(x2, y2));
            g.DrawLine(Config.shapePen, new PointF(x1, y1), new PointF(x3, y3));
            g.DrawLine(Config.shapePen, new PointF(x3, y3), new PointF(x2, y2));

            if (ImageShow.showPara)
            {
                //string s = "Pos:(" + P1.X.ToString("0.00") + "," + P1.Y.ToString("0.00") + ")" + Environment.NewLine + "Rad:" + R.ToString("0.00");
                //Font f = new Font("宋体", 18);
                //g.DrawString(s, f, Brushes.GreenYellow, x + 5, y + 5);
            }
        }

        public override void setPara(PointF p1, PointF p2, float r, PointF p3)
        {
            P1 = p1; P2 = p2; R = r; P3 = p3;
        }

        public override void setPartPara(int type, PointF p1, PointF p2, float r)
        {
        }

        public override void getPara(ref float para1, ref float para2, ref float para3, ref float para4, ref float para5)
        {
            para1 = P1.X; para2 = P1.Y; para5 = R;
        }

        public override int onShape(PointF p1)
        {
            return 0;
        }
    }
}
