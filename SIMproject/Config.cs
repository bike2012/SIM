using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SIMproject
{
    class Config
    {
        public static int pointWidth = 3;

        public static string[] tabControlNames = new string[9] { "相机标定", "图像预处理", "边缘检测", "特征点提取", "纠偏控制", "机器人通讯检测", "模板匹配", "图像拼接", "颜色识别"};
        
        static public Pen shapePen = new Pen(Color.Blue, 2);
        static public Pen anchorPen = new Pen(Color.Red, 2);
        static public Pen dotPen = new Pen(Color.Red, 1);

        static public int lineFixRange = 20;
        static public int pointFixRange = 5;
    }
}
