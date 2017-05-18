using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace SIMproject
{
    public abstract class BaseTool
    {
        public abstract void mouseDown(PointF p1);

        public abstract void mouseUp(PointF p3);

        public abstract void mouseDrag(PointF p1, PointF p2, PointF p3);

        public abstract double mouseClick(PointF p1);
    }
}
