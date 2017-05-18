using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SIMproject
{
    class ImageShow
    {
        static public Bitmap source;
        static public Bitmap modified;
        static public RectangleF position = new RectangleF(0,0,640,480);
        static public RectangleF initPosition = new RectangleF(0, 0, 640, 480);
        static public float scale = 1;
        static public string nowFile = null;
        static public int showEdge = 0;
        static public int showModified = 0;

        static public bool showPara = true;

        static public List<BaseShape> shapes = new List<BaseShape>();
        static public List<BaseShape> edgeDots = new List<BaseShape>();

        static public void cls()
        {
            while (shapes.Count != 0)
                shapes.RemoveAt(shapes.Count-1);
            while (edgeDots.Count != 0)
                edgeDots.RemoveAt(edgeDots.Count - 1);
        }

        static public void addCircle(PointF P, float r)
        {
            BaseShape newShape = (BaseShape) new CircleShape();
            newShape.setPara(P, P, r, new PointF(0, 0));
            shapes.Add(newShape);
        }

        static public bool editLast(PointF P1, PointF P2, float r)
        {
            if (shapes.Count == 0) return false;
            shapes.ElementAt(shapes.Count - 1).setPara(P1, P2, r, new PointF(0, 0));
            return true;
        }

        static public void addPoint(PointF P)
        {
            BaseShape newShape = (BaseShape)new PointShape();
            newShape.setPara(P, P, Config.pointWidth, new PointF(0, 0));
            shapes.Add(newShape);
        }

        static public void addRect(PointF P1, PointF P2, float r, bool showPts)
        {
            BaseShape newShape = (BaseShape)new RectShape();
            newShape.setPara(P1, P2, r, new PointF(0, 0));
            if (!showPts) newShape.setPartPara(3, P1, P2, r);
            shapes.Add(newShape);
        }

        static public void addDot(Point P1)
        {
            BaseShape newShape = (BaseShape)new DotShape();
            newShape.setPara(new PointF((float)P1.X, (float)P1.Y), new PointF((float)P1.X, (float)P1.Y), 0, new PointF(0, 0));
            edgeDots.Add(newShape);
        }

        static public void addLine(PointF P1, float r)
        {
            BaseShape newShape = (BaseShape)new LineShape();
            newShape.setPara(P1, P1, 0, new PointF(0, 0));
            shapes.Add(newShape);
        }

        static public void addTriangle(PointF P1, PointF P2, PointF P3)
        {
            BaseShape newShape = (BaseShape)new TriangleShape();
            newShape.setPara(P1, P2, 0, P3);
            shapes.Add(newShape);
        }

    }
}
