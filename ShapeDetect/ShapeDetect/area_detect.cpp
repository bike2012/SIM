#include <cv.hpp>
#include <vector>
#include <queue>
#include <iostream>
#include <time.h>

using namespace cv;
using namespace std;

#define bfs_4_neighbour 0
#define bfs_8_neighbour 1

vector<Point> bfs(Mat &img, Point pt, int targetColor, int highArea, int mode)
{
	queue<Point> queue;
	queue.push(pt);
	img.data[pt.y*img.cols+pt.x] = 255-targetColor;

	vector<Point> pts;
	pts.push_back(pt);
	int num = 1;

	while (!queue.empty())
	{
		if (mode == bfs_4_neighbour)
		{
			for (int p = -1;p<=1;p++)
				for (int q = -1;q<=1;q++)
					if (p*q==0 && p+q!=0)
					{
						Point temp = queue.front();
						if (temp.x+p>=0 && temp.x+p<img.cols && temp.y+q>=0 && temp.y+q<img.rows)
							if (img.data[(temp.y+q)*img.cols+temp.x+p] == targetColor)
							{
								img.data[(temp.y+q)*img.cols+temp.x+p] = 255-targetColor;
								queue.push(Point(temp.x+p,temp.y+q));
								pts.push_back(Point(temp.x+p,temp.y+q));
							}
					}
		}
		if (mode == bfs_8_neighbour)
		{
			for (int p = -1;p<=1;p++)
				for (int q = -1;q<=1;q++)
					if (p!=0 || q!=0)
					{
						Point temp = queue.front();
						if (temp.x+p>=0 && temp.x+p<img.cols && temp.y+q>=0 && temp.y+q<img.rows)
							if (img.data[(temp.y+q)*img.cols+temp.x+p] == targetColor)
							{
								img.data[(temp.y+q)*img.cols+temp.x+p] = 255-targetColor;
								queue.push(Point(temp.x+p,temp.y+q));
								pts.push_back(Point(temp.x+p,temp.y+q));
							}
					}
		}
		queue.pop();
	}
	return pts;
}

void __declspec(dllexport) areaDetect(int width, int height, int imgSize, unsigned char *data, int lowArea, int highArea, int targetColor, int mode, int &resultNum, float *resultX, float *resultY)
{
	Mat img;

	//计算图像通道数
	int channel = imgSize/width/height;

	//根据图像通道数保存为对应的Mat类型
	switch (channel)
	{
	case 1:
		img = Mat(height,width,CV_8UC1);
		for (int i=0;i<imgSize;i++)
			img.data[i] = data[i];
		break;
	case 3:
		img = Mat(height,width,CV_8UC3);
		for (int i=0;i<imgSize;i++)
			img.data[i] = data[i];
		cvtColor(img,img,CV_RGB2GRAY);
		break;
	case 4:
		img = Mat(height,width,CV_8UC4);
		for (int i=0;i<imgSize;i++)
			img.data[i] = data[i];
		cvtColor(img,img,CV_RGBA2GRAY);
		break;
	default:
		return;
	};

	resize(img, img, Size(),0.5,0.5);
	lowArea/=4;
	highArea/=4;

	
	int maxNum = resultNum;
	resultNum = 0;

	for (int i = 0;i<img.rows;i++)
		for (int j= 0;j<img.cols;j++)
			if (img.data[i*img.cols+j] == targetColor)
			{
				vector<Point> pts = bfs(img,Point(j,i),targetColor,highArea,mode);
				if (pts.size()>=lowArea && pts.size()<=highArea)
				{
					RotatedRect rect = minAreaRect(pts);
					Point2f pts[4];
					rect.points(pts);
					if (resultNum<maxNum)
					{
						for (int k = 0;k<4;k++)
						{
							resultX[resultNum*4+k] = pts[k].x*2;
							resultY[resultNum*4+k] = pts[k].y*2;
						}
						resultNum++;
					}
				}
			}
}

void main4()
{
	//Mat img = imread("123.bmp");

	//cvtColor(img,img,CV_RGB2GRAY);
	//threshold(img,img,0,255,THRESH_OTSU);

	//clock_t t1,t2;
	//t1 = clock();
	//areaDetect(img,10000,20000,255,bfs_4_neighbour);
	//t2 = clock();
	//cout<<t2-t1<<endl;
	//cin>>t1;
}