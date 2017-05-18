using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace SIMproject
{
    public partial class ImageDragTool : BaseTool
    {
        public override void mouseDown(PointF p1)
        {
        }

        public override void mouseUp(PointF p3)
        {
        }

        public override void mouseDrag(PointF p1, PointF p2, PointF p3)
        {
            ImageShow.position.X += p3.X - p2.X;
            ImageShow.position.Y += p3.Y - p2.Y;
        }

        public override double mouseClick(PointF p1)
        {
            return 0;
        }
    }
}
