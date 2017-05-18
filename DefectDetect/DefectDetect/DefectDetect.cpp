// findflaw1.cpp : 定义控制台应用程序的入口点。
//

#include "time.h"
#include "opencv2/highgui/highgui.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include <iostream>
#include <stdio.h>
#include <algorithm>
#include <queue>
#include <fstream>

using namespace std;
using namespace cv;



Rect bfs(Mat src, int x, int y, int srcColor, int dstColor, int& num)
{
	queue<Point> queue;
	queue.push(Point(x,y));
	src.data[x*src.cols+y] = dstColor;
	num=1;
	int maxx=-1,maxy=-1,minx=-1,miny=-1;
	while (!queue.empty())
	{
		for (int i=-1;i<=1;i++)
			for (int j=-1;j<=1;j++)
				if (i+j!=0 && i+j!=-2 && i+j!=2)
					if (queue.front().x+i>=0 && queue.front().x+i<src.rows && queue.front().y+j>=0 && queue.front().y+j<src.cols)
						if (src.data[(queue.front().x+i)*src.cols+queue.front().y+j]==srcColor)
						{
							queue.push(Point(queue.front().x+i,queue.front().y+j));
							if (minx==-1 || queue.front().x+i<minx) minx=queue.front().x+i;
							if (miny==-1 || queue.front().y+j<miny) miny=queue.front().y+j;
							if (maxx==-1 || queue.front().x+i>maxx) maxx=queue.front().x+i;
							if (maxy==-1 || queue.front().y+j>maxy) maxy=queue.front().y+j;
							src.data[(queue.front().x+i)*src.cols+queue.front().y+j]=dstColor;
							num++;
						}
						queue.pop();
	}
	
	return Rect(minx,miny,maxx-minx+1,maxy-miny+1);
}


void match(Mat temp, Mat obj, vector<Point2f> tempPts, vector<Point2f> objPts, vector<Rect> &results, int colorThr, int areaThr)
{
	if (tempPts.size()!=4 || objPts.size()!=4) return;
	Rect tempROI(tempPts[0].x,tempPts[0].y,tempPts[0].x+1,tempPts[0].y+1);
	for (int i=1;i<tempPts.size();i++)
	{
		if (tempPts[i].x<tempROI.x) tempROI.x = tempPts[i].x;
		if (tempPts[i].x+1>tempROI.width) tempROI.width=tempPts[i].x+1;
		if (tempPts[i].y<tempROI.y) tempROI.y = tempPts[i].y;
		if (tempPts[i].y+1>tempROI.height) tempROI.height=tempPts[i].y+1;
	}
	Rect objROI(objPts[0].x,objPts[0].y,objPts[0].x+1,objPts[0].y+1);
	for (int i=1;i<objPts.size();i++)
	{
		if (objPts[i].x<objROI.x) objROI.x = objPts[i].x;
		if (objPts[i].x+1>objROI.width) objROI.width=objPts[i].x+1;
		if (objPts[i].y<objROI.y) objROI.y = objPts[i].y;
		if (objPts[i].y+1>objROI.height) objROI.height=objPts[i].y+1;
	}
	tempROI.width -=tempROI.x;tempROI.height-=tempROI.y;objROI.width-=objROI.x;objROI.height-=objROI.y;

	temp = Mat(temp,tempROI);
	obj = Mat(obj,objROI);

	for (int i=0;i<tempPts.size();i++)
	{
		tempPts[i].x-=tempROI.x;
		tempPts[i].y-=tempROI.y;
		objPts[i].x-=objROI.x;
		objPts[i].y-=objROI.y;
	}

	Mat cvt = getPerspectiveTransform(tempPts,objPts);
	warpPerspective(temp,temp,cvt,obj.size());

	Mat mask = Mat::zeros(obj.size(),CV_8UC1);
	for (int i=0;i<4;i++)
		line(mask,objPts[i],objPts[(i+1)%4],Scalar(255));
	int tempNum =0;
	bfs(mask,mask.rows/2,mask.cols/2,0,255,tempNum);
	Mat offsetCount = Mat::zeros(3,3,CV_32SC1);
	for (int i = 0; i < obj.rows; i++)
		for (int j = 0; j < obj.cols; j++)
			if (mask.data[i*obj.cols+j] == 255)
			{
				int startX = (j - 1);
				int endX = (j + 1);
				int startY = (i - 1);
				int endY = (i + 1);
				if (startX < 0){ startX = 0; }
				if (startY < 0){ startY = 0; }
				if (endX > obj.cols-1){ endX = obj.cols-1; }
				if (endY > obj.rows-1){ endY = obj.rows-1; }

				int min = 255;
				int dx = 0,dy = 0;
				for (int y = startY; y <= endY; y++)
				{
					for (int x = startX; x <= endX; x++)
					{
						int diff = abs(obj.data[i*obj.step + j*obj.step.p[1]] - temp.data[y*temp.step + x*temp.step.p[1]]);
						if (diff < min)
						{
							min = diff;
							dx = x-startX;
							dy = y-startY;
						}
					}
				}
				offsetCount.at<int>(dx,dy)++;					
			}

			int max = 0, tagi = 0;
	for (int i=0;i<9;i++)
		if (offsetCount.at<int>(i)>max)
		{
			max = offsetCount.at<int>(i);
			tagi =i;
		}
	int offsetX = 0,offsetY = 0;
	if (tagi!=5 && max>tempNum/3)
	{
		offsetX = tagi%3-1;
		offsetY = tagi/3-1;
	}

	//for (int i=0;i<9;i++)
		//cout<<offsetCount.at<int>(i)<<'\t';
	//cout<<tempNum<<endl;

	Mat result = Mat::zeros(obj.size(),CV_8UC1);
	for (int i = 0; i <= obj.rows - 1; i++)
		for (int j = 0; j <= obj.cols - 1; j++)
			if (mask.data[i*mask.cols+j] == 255)
			{
				int startX = j - 2+offsetX;
				int endX = j + 2+offsetX;
				int startY = i - 2+offsetY;
				int endY = i + 2+offsetY;
				if (startX >= obj.cols || startY >= obj.rows || endX < 0 || endY < 0)
					continue;

				if (startX < 0){ startX = 0; }
				if (startY < 0){ startY = 0; }
				if (endX >= obj.cols){ endX = obj.cols - 1; }
				if (endY >= obj.rows){ endY = obj.rows - 1; }

				result.data[i*result.cols + j] = 255;
				for (int y = startY; y <= endY; y++)
					for (int x = startX; x <= endX; x++)
						if (abs(obj.data[i*obj.step + j*obj.step.p[1]] - temp.data[y*temp.step + x*temp.step.p[1]])<colorThr)
						{
							result.data[i*result.cols + j] = 0;
							break;
						}
			}
	for (int i=0;i<result.rows;i++)
		for (int j=0;j<result.cols;j++)
			if (result.data[i*result.cols+j] == 255)
			{
				int tempNum = 0;
				Rect tempRect = bfs(result,i,j,255,0,tempNum);
				if (tempNum >= areaThr)
					results.push_back(Rect(tempRect.y+objROI.x,tempRect.x+objROI.y,tempRect.height,tempRect.width));
			}
}

void __declspec(dllexport) defectDetect(char* tempfilename, char* objfilename, 
				   float tempx1, float tempy1,float tempx2, float tempy2,float tempx3, float tempy3,float tempx4, float tempy4,
				   float objx1, float objy1,float objx2, float objy2,float objx3, float objy3,float objx4, float objy4,
				   int colorThr, int areaThr, int &resultNum, int *X, int *Y, int *W, int *H)
{
	Mat temp = imread(tempfilename);
	Mat obj = imread(objfilename);

	vector<Point2f> tempPts,objPts;
	tempPts.push_back(Point2f(tempx1,tempy1));tempPts.push_back(Point2f(tempx2,tempy2));tempPts.push_back(Point2f(tempx3,tempy3));tempPts.push_back(Point2f(tempx4,tempy4));
	objPts.push_back(Point2f(objx1,objy1));objPts.push_back(Point2f(objx2,objy2));objPts.push_back(Point2f(objx3,objy3));objPts.push_back(Point2f(objx4,objy4));

	vector<Rect> results;
	match(temp,obj,tempPts,objPts,results,colorThr,areaThr);
	resultNum = results.size();
	for (int i=0;i<resultNum;i++)
	{
		X[i] = results[i].x;
		Y[i] = results[i].y;
		W[i] = results[i].width;
		H[i] = results[i].height;
	}
}

int main()
{

	int resultNum = 0;
	int *X = new int[1000];int *Y = new int[1000];int *W = new int[1000];int*H = new int[1000];
	defectDetect("1.bmp","4.bmp",489,767,1778,758,1696,1456,482,1408,765,386,1818,1129,1345,1649,384,902,50,100,resultNum,X,Y,W,H);
}
