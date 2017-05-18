#include <cv.hpp>
#include <iostream>

using namespace std;
using namespace cv;

#define RGB_Mode 0
#define YUV_Mode 1
#define HSL_Mode 2

int __declspec(dllexport) colorRecog(int r,int g,int b, int colorNum, int *R, int *G, int *B, int mode)
{
	if (mode == 0)
	{
		int min = -1;
		int tag = -1;
		for (int i=0;i<colorNum;i++)
		{
			int dist = (abs(r-R[i])+abs(g-G[i])+abs(b-B[i]))/3;
			if (dist<min || min == -1)
			{
				min = dist;
				tag = i;
			}
		}

		return tag;
	}
	else if (mode == 1)
	{
		int min = -1;
		int tag = -1;
		double y= 0.299*r + 0.587*g + 0.114*b;
		double u= -0.147*r - 0.289*g + 0.436*b;
		double v= 0.615*r - 0.515*g - 0.100*b;
		for (int i=0;i<colorNum;i++)
		{
			double Y= 0.299*R[i] + 0.587*G[i] + 0.114*B[i];
			double U= -0.147*R[i] - 0.289*G[i] + 0.436*B[i];
			double V= 0.615*R[i] - 0.515*G[i] - 0.100*B[i];
			int dist = sqrt((u-U)*(u-U)+(v-V)*(v-V));
			if (dist<min || min == -1)
			{
				min = dist;
				tag = i;
			}
		}
		return tag;
	}
	else if (mode == 2)
	{
		Mat src(1,colorNum+1,CV_8UC3);
		src.data[0] = b;src.data[1] = g;src.data[2] = r;
		for (int i=1;i<=colorNum;i++)
		{
			src.data[i*3] = B[i-1];
			src.data[i*3+1] = G[i-1];
			src.data[i*3+2] = R[i-1];
		}

		cvtColor(src,src,CV_BGR2HLS);
		int h = src.data[0], l = src.data[1], s = src.data[2];

		int min = -1;
		int tag = -1;
		for (int i=1;i<colorNum;i++)
		{
			int H = src.data[i*3];
			int S = src.data[i*3+2];
			int dist = sqrt((h-H)*(h-H)+(s-S)*(s-S));
			if (dist<min || min == -1)
			{
				min = dist;
				tag = i-1;
			}
		}
	}
}