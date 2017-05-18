#include <cv.hpp>
#include <vector>
#include <iostream>
#include <time.h>

using namespace cv;
using namespace std;

double getAngle(Vec4i line1, Vec4i line2, float maxGap, Point2f &pt)
{
	if ((abs(line1.val[0] - line2.val[0]) > maxGap || abs(line1.val[1] - line2.val[1]) > maxGap)
		&&(abs(line1.val[0] - line2.val[2]) > maxGap || abs(line1.val[1] - line2.val[3]) > maxGap)
		&&(abs(line1.val[2] - line2.val[0]) > maxGap || abs(line1.val[3] - line2.val[1]) > maxGap)
		&&(abs(line1.val[2] - line2.val[2]) > maxGap || abs(line1.val[3] - line2.val[3]) > maxGap))
		return -1;

	float x,y;
	if (line1.val[2] == line1.val[0])
	{
		if (line2.val[2] == line2.val[0])
			return -1;
		float k2 = float(line2.val[3]-line2.val[1])/float(line2.val[2]-line2.val[0]);
		float b2 = line2.val[1]-k2*line2.val[0];
		x = line1.val[0];
		y = k2*x+b2;
	}
	else if (line2.val[2] == line2.val[0])
	{
		float k1 = float(line1.val[3]-line1.val[1])/float(line1.val[2]-line1.val[0]);
		float b1 = line1.val[1]-k1*line1.val[0];
		x = line2.val[0];
		y = k1*x+b1;
	}
	else
	{
		float k1 = float(line1.val[3]-line1.val[1])/float(line1.val[2]-line1.val[0]);
		float b1 = line1.val[1]-k1*line1.val[0];
		float k2 = float(line2.val[3]-line2.val[1])/float(line2.val[2]-line2.val[0]);
		float b2 = line2.val[1]-k2*line2.val[0];

		if (fabs(k1-k2)<0.01) return -1;
		x = (b1-b2)/(k2-k1);
		y = k1*x+b1;
	}

	float x1,y1,x2,y2;
	if (fabs(line1.val[0]-x)<=maxGap && fabs(line1.val[1]-y)<=maxGap)
	{
		x1 = line1.val[2];
		y1 = line1.val[3];
	}
	else if (fabs(line1.val[2]-x)<=maxGap && fabs(line1.val[3]-y)<=maxGap)
	{
		x1 = line1.val[0];
		y1 = line1.val[1];
	}
	else
		return -1;

	if (fabs(line2.val[0]-x)<=maxGap && fabs(line2.val[1]-y)<=maxGap)
	{
		x2 = line2.val[2];
		y2 = line2.val[3];
	}
	else if (fabs(line2.val[2]-x)<=maxGap && fabs(line2.val[3]-y)<=maxGap)
	{
		x2 = line2.val[0];
		y2 = line2.val[1];
	}
	else
		return -1;

	pt.x = x;pt.y = y;
	float a = sqrt((x1-x2)*(x1-x2)+(y1-y2)*(y1-y2));
	float b = sqrt((x1-x)*(x1-x)+(y1-y)*(y1-y));
	float c = sqrt((x2-x)*(x2-x)+(y2-y)*(y2-y));

	return acos((b*b+c*c-a*a)/(2*b*c))*180.0/CV_PI;
}

bool testTriangle(Vec4i a,Vec4i b,Vec4i c,double angle2,double angle3,double range, Point2f pt, Point2f *triangle,float maxGap)
{
	if ((fabs(c.val[0]-pt.x)<=maxGap  && fabs(c.val[1]-pt.y)<=maxGap)
		||(fabs(c.val[2]-pt.x)<=maxGap && fabs(c.val[3]-pt.y)<=maxGap))
		return false;

	Point2f pt1,pt2;
	float a1 = getAngle(a,c,maxGap,pt1);
	if (a1>=0)
	{
		if (a1>=angle2-range && a1<=angle2+range)
		{
			float a2 = getAngle(b,c,maxGap,pt2);
			if (a2>=0 && a2>=angle3-range && a2<=angle3+range)
			{
				triangle[0] = pt;
				triangle[1] = pt1;
				triangle[2] = pt2;
				return true;
			}
			else
				return false;
		}
		else if (a1>=angle3-range && a1<=angle3+range)
		{
			float a2 = getAngle(b,c,maxGap,pt2);
			if (a2>=0 && a2>=angle2-range && a2<=angle2+range)
			{
				triangle[0] = pt;
				triangle[1] = pt1;
				triangle[2] = pt2;
				return true;
			}
			else
				return false;
		}
		else return false;
	}

	return false;
}

void __declspec(dllexport) triangleDetect(int width, int height, int imgSize, unsigned char *data,float angle1, float angle2, float angle3, float range,int thr, float minLength, float maxGap, int& resultNum, float *ptX, float *ptY)
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

	vector<Vec4i> lines;
	HoughLinesP(img,lines,1,CV_PI/180,thr,minLength,maxGap);

	int maxNum = resultNum;
	resultNum =0;

	for (int i = 0;i<lines.size();i++)
		for (int j = 0;j<lines.size();j++)
			if (i!=j)
			{
				Point2f pt;
				if (i == 0 && j == 7)
					i = 0;
				float angle = getAngle(lines[i],lines[j],maxGap,pt);
				if (angle>=0 && angle>=angle1-range && angle<=angle1+range)
				{
					for (int k = 0;k<lines.size();k++)
						if (k!=i && k!=j)
						{
							Point2f *triangle = new Point2f[3];
							if (testTriangle(lines[i],lines[j],lines[k],angle2,angle3,range,pt,triangle,maxGap))
							{
								if (resultNum<maxNum)
								{
									ptX[resultNum*3] = triangle[0].x;
									ptY[resultNum*3] = triangle[0].y;
									ptX[resultNum*3+1] = triangle[1].x;
									ptY[resultNum*3+1] = triangle[1].y;
									ptX[resultNum*3+2] = triangle[2].x;
									ptY[resultNum*3+2] = triangle[2].y;
									resultNum++;
								}
							}
						}
				}
			}
}

//void main()
//{
//	Mat img = imread("shape.bmp");
//	//resize(img,img,Size(),0.5,0.5);
//	Mat edge;
//	Canny(img,edge,100,200);	
//
//	triangle_detect(edge,60,60,60,50,20,30,20);
//}
