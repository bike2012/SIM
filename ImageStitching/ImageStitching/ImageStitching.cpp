
#include <iostream>
#include <fstream>
#include "cv.hpp"
#include "highgui.h"
#include "opencv2/stitching.hpp"
#include <time.h>
#include<vector>

using namespace std;
using namespace cv;

void __declspec(dllexport) imageStitch(char* filenames,char* outname, int w)
{
	int index = 0, pre =0;
	vector<Mat> imgs;

	while (filenames[index])
	{
		if (filenames[index]=='|')
		{
			char *filename = new char[index-pre+1];
			for (int i=pre;i<index;i++)
				filename[i-pre] = filenames[i];
			filename[index-pre] = '\0';
			Mat img = imread(filename);
			imgs.push_back(img);
			pre = index+1;
			
		}
		index++;
	}

    Mat pano;
    Stitcher stitcher = Stitcher::createDefault(false);
	
	//stitcher.setRegistrationResol(0.1);
	stitcher.setWaveCorrection(false);
	//stitcher.setBundleAdjuster(new detail::BundleAdjusterRay());

    Stitcher::Status status = stitcher.stitch(imgs, pano);

    imwrite(outname, pano);
}


Mat mergeLR(Mat left, Mat right, int centerLeft, int centerRight, float minRate)
{
	Mat leftT, rightT;
	resize(left,leftT,Size(),0.25,0.25);
	resize(right,rightT,Size(),0.25,0.25);
	Mat maskT(rightT.rows/2,rightT.cols*minRate,rightT.type());

	for (int i=0;i<maskT.rows;i++)
		for (int j=0;j<maskT.cols;j++)
			for (int k = 0;k<maskT.step.buf[1];k++)
				maskT.data[i*maskT.cols*maskT.step.buf[1]+j*maskT.step.buf[1]+k] = rightT.data[(i+rightT.rows/4)*rightT.cols*rightT.step.buf[1]+(int)(j+rightT.cols*minRate)*rightT.step.buf[1]+k];

	Mat dst;

	matchTemplate(leftT,maskT,dst,cv::TemplateMatchModes::TM_CCOEFF_NORMED);
	double max = 0;
	int tagx = 0,tagy = 0;
	for (int i = 0;i<dst.rows;i++)
		for (int j = 0;j<dst.cols;j++)
			if (dst.at<float>(i,j)>max)
			{
				max = dst.at<float>(i,j);
				tagy = i;
				tagx = j;
			}
	
	tagy*=4;
	tagx*=4;
	
	Mat mask(right.rows/2,right.cols*minRate,right.type());
	
	for (int i=0;i<mask.rows;i++)
		for (int j=0;j<mask.cols;j++)
			for (int k = 0;k<mask.step.buf[1];k++)
				mask.data[i*mask.cols*mask.step.buf[1]+j*mask.step.buf[1]+k] = right.data[(i+right.rows/4)*right.cols*right.step.buf[1]+(int)(j+right.cols*minRate)*right.step.buf[1]+k];

	int width = right.cols*minRate;
	int height = right.rows/2;
	if (tagx<5)
	{
		width+=tagx+5;
		tagx = 0;
	}
	else if (tagx+5+width>=left.cols)
	{
		tagx-=5;
		width = left.cols-tagx-1;
	}
	else
	{
		tagx-=5;
		width +=10;
	}
		if (tagy<5)
	{
		height+=tagy+5;
		tagy = 0;
	}
	else if (tagy+5+height>=left.rows)
	{
		tagy-=5;
		height = left.rows-tagy-1;
	}
	else
	{
		tagy-=5;
		height +=10;
	}

	Mat leftTT(height,width,left.type());
	for (int i=0;i<leftTT.rows;i++)
		for (int j=0;j<leftTT.cols;j++)
			for (int k=0;k<leftTT.step.buf[1];k++)
				leftTT.data[i*leftTT.cols*leftTT.step.buf[1]+j*leftTT.step.buf[1]+k] = left.data[(i+tagy)*left.cols*left.step.buf[1]+(j+tagx)*left.step.buf[1]+k];



	matchTemplate(leftTT,mask,dst,cv::TemplateMatchModes::TM_CCOEFF_NORMED);
	int tempx = tagx,tempy=tagy;
	max = 0;
	tagx = 0,tagy = 0;
	for (int i = 0;i<dst.rows;i++)
		for (int j = 0;j<dst.cols;j++)
			if (dst.at<float>(i,j)>max)
			{
				max = dst.at<float>(i,j);
				tagy = i;
				tagx = j;
			}
	tagx+=tempx;
	tagy+=tempy;
			
	tagy-=right.rows/4;
	tagx-=right.cols*minRate;
	
	int resultWidth = tagx+right.cols;
	int resultLow = (0>tagy)?0:tagy;
	int resultHigh = (left.rows>tagy+right.rows)?tagy+right.rows:left.rows;
	
	Mat result(resultHigh-resultLow,resultWidth,right.type());

	centerRight +=tagx;
	int center = (centerLeft+centerRight)/2;

	int offset = (0<tagy)?0:-tagy;

	for (int i=0;i<result.rows;i++)
		for (int j=0;j<result.cols;j++)
			for (int k = 0;k<result.step.buf[1];k++)
				if (j<= center)
				result.data[i*result.cols*result.step.buf[1]+j*result.step.buf[1]+k] = left.data[(i+resultLow)*left.cols*left.step.buf[1]+j*left.step.buf[1]+k];
				else
					result.data[i*result.cols*result.step.buf[1]+j*result.step.buf[1]+k] =right.data[(i+offset)*right.cols*right.step.buf[1]+(j-tagx)*right.step.buf[1]+k];
	return result;
}

Mat mergeUD(Mat up, Mat down, int centerUp, int centerDown, float minRate)
{
	Mat upT, downT;
	resize(up,upT,Size(),0.25,0.25);
	resize(down,downT,Size(),0.25,0.25);
	Mat maskT(downT.rows*minRate,downT.cols/2,downT.type());

	for (int i=0;i<maskT.rows;i++)
		for (int j=0;j<maskT.cols;j++)
			for (int k = 0;k<maskT.step.buf[1];k++)
				maskT.data[i*maskT.cols*maskT.step.buf[1]+j*maskT.step.buf[1]+k] = downT.data[(int)(i+downT.rows*minRate)*downT.cols*downT.step.buf[1]+(j+downT.cols/4)*downT.step.buf[1]+k];

	Mat dst;

	matchTemplate(upT,maskT,dst,cv::TemplateMatchModes::TM_CCOEFF_NORMED);
	double max = 0;
	int tagx = 0,tagy = 0;
	for (int i = 0;i<dst.rows;i++)
		for (int j = 0;j<dst.cols;j++)
			if (dst.at<float>(i,j)>max)
			{
				max = dst.at<float>(i,j);
				tagy = i;
				tagx = j;
			}
	
	tagy*=4;
	tagx*=4;
	
	Mat mask(down.rows*minRate,down.cols/2,down.type());
	
	for (int i=0;i<mask.rows;i++)
		for (int j=0;j<mask.cols;j++)
			for (int k = 0;k<mask.step.buf[1];k++)
				mask.data[i*mask.cols*mask.step.buf[1]+j*mask.step.buf[1]+k] = down.data[(i+down.rows/4)*down.cols*down.step.buf[1]+(int)(j+down.cols*minRate)*down.step.buf[1]+k];

	int width = down.cols/2;
	int height = down.rows*minRate;
	if (tagx<5)
	{
		width+=tagx+5;
		tagx = 0;
	}
	else if (tagx+5+width>=up.cols)
	{
		tagx-=5;
		width = up.cols-tagx-1;
	}
	else
	{
		tagx-=5;
		width +=10;
	}
		if (tagy<5)
	{
		height+=tagy+5;
		tagy = 0;
	}
	else if (tagy+5+height>=up.rows)
	{
		tagy-=5;
		height = up.rows-tagy-1;
	}
	else
	{
		tagy-=5;
		height +=10;
	}

	Mat upTT(height,width,up.type());
	for (int i=0;i<upTT.rows;i++)
		for (int j=0;j<upTT.cols;j++)
			for (int k=0;k<upTT.step.buf[1];k++)
				upTT.data[i*upTT.cols*upTT.step.buf[1]+j*upTT.step.buf[1]+k] = up.data[(i+tagy)*up.cols*up.step.buf[1]+(j+tagx)*up.step.buf[1]+k];



	matchTemplate(upTT,mask,dst,cv::TemplateMatchModes::TM_CCOEFF_NORMED);
	int tempx = tagx,tempy=tagy;
	max = 0;
	tagx = 0,tagy = 0;
	for (int i = 0;i<dst.rows;i++)
		for (int j = 0;j<dst.cols;j++)
			if (dst.at<float>(i,j)>max)
			{
				max = dst.at<float>(i,j);
				tagy = i;
				tagx = j;
			}
	tagx+=tempx;
	tagy+=tempy;
			
	tagy-=down.rows*minRate;
	tagx-=down.cols/4;
	
	int resultHeight = tagy+down.rows;
	int resultLeft = (0>tagx)?0:tagx;
	int resultRight = (up.cols>tagx+down.cols)?tagx+down.cols:up.cols;
	
	Mat result(resultHeight,resultRight-resultLeft,down.type());

	centerDown +=tagy;
	int center = (centerUp+centerDown)/2;

	int offset = (0<tagx)?0:-tagx;

	for (int i=0;i<result.rows;i++)
		for (int j=0;j<result.cols;j++)
			for (int k = 0;k<result.step.buf[1];k++)
				if (i<= center)
				result.data[i*result.cols*result.step.buf[1]+j*result.step.buf[1]+k] = up.data[i*up.cols*up.step.buf[1]+(j+resultLeft)*up.step.buf[1]+k];
				else
					result.data[i*result.cols*result.step.buf[1]+j*result.step.buf[1]+k] =down.data[(i-tagy)*down.cols*down.step.buf[1]+(j+offset)*down.step.buf[1]+k];
	return result;
}


void __declspec(dllexport) myImageStitch(char* filenames,char* outname, int h, int w, float minRate)
{
	int index = 0, pre =0;
	Mat imgs[16];

	int num = 0;
	while (filenames[index])
	{
		if (filenames[index]=='|')
		{
			char *filename = new char[index-pre+1];
			for (int i=pre;i<index;i++)
				filename[i-pre] = filenames[i];
			filename[index-pre] = '\0';
			imgs[num++] = imread(filename);
			pre = index+1;
			
		}
		index++;
	}

	Mat colImgs[4];
	for (int i=0;i<w;i++)
	{
		colImgs[i] = Mat(imgs[i].rows*h,imgs[i].cols,imgs[i].type());
		for (int j=0;j<h;j++)
		{
			for (int ii = 0;ii<imgs[i].rows;ii++)
				for (int jj = 0;jj<imgs[i].cols;jj++)
					for (int k = 0;k<imgs[i].step.buf[1];k++)
						colImgs[i].data[j*imgs[i].step*imgs[i].rows+ii*imgs[i].step+jj*imgs[i].step.buf[1]+k] = imgs[j*w+i].data[ii*imgs[i].step+jj*imgs[i].step.buf[1]+k];
		}
	}

	Mat result = colImgs[0];
	for (int i=1;i<w;i++)
		result = mergeLR(result, colImgs[i],result.cols-colImgs[i-1].cols/2,colImgs[i].cols/2, minRate);

	Mat rowImgs[4];
	for (int i=0;i<h;i++)
		rowImgs[i] = result(Rect(0,i*imgs[0].rows,result.cols,imgs[0].rows));

	result = rowImgs[0];
	for (int i=1;i<h;i++)
		result = mergeUD(result, rowImgs[i],result.rows-rowImgs[i-1].rows/2,rowImgs[i].rows/2, minRate);

	imwrite(outname,result);
}

void main()
{
	//Mat src = imread("4.bmp");
	//Mat dst = src(Rect(1592,500,1000,1000));
	//imwrite("23.bmp",dst);

	imageStitch("11.bmp|12.bmp|","out.bmp",2);
	//myImageStitch("D:\\test\\SIMproject\\SIMproject\\bin\\Release\\11.bmp|D:\\test\\SIMproject\\SIMproject\\bin\\Release\\12.bmp|D:\\test\\SIMproject\\SIMproject\\bin\\Release\\21.bmp|D:\\test\\SIMproject\\SIMproject\\bin\\Release\\22.bmp|","out.bmp",2,2);//|21.bmp|22.bmp|23.bmp|","out.bmp",2,3);
}