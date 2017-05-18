using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SIMproject
{
    public abstract class BaseShape
    {
        public PointF P1, P2, P3;
        public float R;

        public abstract void draw(Graphics g);

        public abstract void setPara(PointF p1, PointF p2, float r, PointF p3);

        public abstract void setPartPara(int type,PointF p1, PointF p2, float r);

        public abstract void getPara(ref float para1, ref float para2, ref float para3, ref float para4, ref float para5);

        public abstract int onShape(PointF p1);
    }
}
