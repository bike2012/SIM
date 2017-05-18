#include <cv.hpp>
#include <highgui.h>
#include <vector>

using namespace cv;
using namespace std;

//二值化并计算外接矩形
//width,height待处理图像的宽和高
//size待处理图像数据量
//data待处理图像数据
//lowThr,highThr算法阈值,该灰度范围内为目标
//ptNum外接矩形顶点数（4）
//ptX,ptY为外界矩形顶点坐标
void __declspec(dllexport) threshold(int width, int height, int size, unsigned char *data, int lowThr, int highThr, int ptNum, float *ptX, float *ptY)
{
	//根据图像通道数保存为对应的Mat类型
	Mat src;
	if (size == width*height)
	{
		src = Mat(width,height,CV_8UC1);
		for (int i=0;i<size;i++)
			src.data[i] = data[i];
	}
	else if (size == width*height*3)
	{
		src = Mat(width, height, CV_8UC3);
		for (int i=0;i<size;i++)
			src.data[i] = data[i];
		cvtColor(src,src,CV_RGB2GRAY);
	}
	else
	{
		src = Mat(width, height, CV_8UC4);
		for (int i=0;i<size;i++)
			src.data[i] = data[i];
		cvtColor(src,src,CV_RGBA2GRAY);
	}

	//二值化
	vector<Point2f> pts;
	for (int i=0;i<width*height;i++)
		if (src.data[i] >=lowThr && src.data[i]<=highThr)
		{
			pts.push_back(Point2f(i%width,i/width));
			data[i] = 255;
		}
		else
			data[i] = 0;

	//计算最小外接矩形
	RotatedRect rect = minAreaRect(pts);
	Point2f corners[4];
	rect.points(corners);
	for (int i=0;i<4;i++)
	{
		ptX[i] = corners[i].x;
		ptY[i] = corners[i].y;
	}	
}

//计算颜色直方图
//width,height待处理图像的宽和高
//size待处理图像数据量
//data待处理图像数据
//hist为返回的颜色直方图
void __declspec(dllexport) colorHist(int width, int height, int size, unsigned char *data, int *hist)
{
	//根据图像通道数保存为对应的Mat类型
	Mat src;
	if (size == width*height)
	{
		src = Mat(width,height,CV_8UC1);
		for (int i=0;i<size;i++)
			src.data[i] = data[i];
	}
	else if (size == width*height*3)
	{
		src = Mat(width, height, CV_8UC3);
		for (int i=0;i<size;i++)
			src.data[i] = data[i];
		cvtColor(src,src,CV_RGB2GRAY);
	}
	else
	{
		src = Mat(width, height, CV_8UC4);
		for (int i=0;i<size;i++)
			src.data[i] = data[i];
		cvtColor(src,src,CV_RGBA2GRAY);
	}

	//计算直方图
	for (int i=0;i<width*height;i++)
		hist[src.data[i]]++;


}
