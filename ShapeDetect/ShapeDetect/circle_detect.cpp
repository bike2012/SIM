#include "cv.h"
#include "highgui.h"
#include <math.h>
#include <string.h>
#include <iostream>

using namespace std;

void __declspec(dllexport) circleDetect(char* filename, double low, double high, double cannyThr, double weightThr, int &resultNum, float *resultX, float *resultY, float *resultR, int colorLowThr, int colorHighThr, int rate)
{
	IplImage* img = cvLoadImage(filename, 1);
	CvSeq * circles = NULL;
	IplImage* pImg8u = cvCreateImage(cvGetSize(img), 8, 1);
	CvMemStorage* storage = cvCreateMemStorage(0);
	cvCvtColor(img, pImg8u, CV_BGR2GRAY);
	//�����cvSmoothһ�£��ٵ���cvHoughCircles
	cvSmooth(pImg8u, pImg8u, CV_GAUSSIAN, 7, 7);
	circles = cvHoughCircles(pImg8u, storage, CV_HOUGH_GRADIENT,
		1, //��С�ֱ��ʣ�Ӧ��>=1
		low, //�ò��������㷨���������ֵ�������ͬԲ֮�����С����
		cannyThr, //����Canny�ı�Ե��ֵ���ޣ����ޱ���Ϊ���޵�һ��
		weightThr, //�ۼ����ķ�ֵ
		low, //��СԲ�뾶 
		high //���Բ�뾶
		);
	int k;
	resultNum = 0;
	for (k = 0; k<circles->total; k++)
	{
		float *p = (float*)cvGetSeqElem(circles, k);
		int num = 0, total = 0;
		for (int i=p[1]-p[2];i<=p[1]+p[2];i++)
			for (int j=p[0]-p[2];j<=p[0]+p[2];j++)
				if (i>=0 && i<pImg8u->height && j>=0 && j<pImg8u->width)
					if ((i-p[1])*(i-p[1])+(j-p[0])*(j-p[0])<=p[2]*p[2])
					{
						total++;
						//cout<<(int)((uchar *)(pImg8u->imageData + i*pImg8u->widthStep))[j]<<'\t';
						if ((int)((uchar *)(pImg8u->imageData + i*pImg8u->widthStep))[j]>=colorLowThr && (int)((uchar *)(pImg8u->imageData + i*pImg8u->widthStep))[j]<=colorHighThr)
							num++;
					}
		//cout<<endl<<num<<'\t'<<total<<endl;
		if (num*100/total>=rate)
		{
			resultX[resultNum] = p[0];
			resultY[resultNum] = p[1];
			resultR[resultNum++] = p[2];
		}
	}
	cvClearMemStorage(storage);
}

	void main3()
	{
		int resultNum = 100;
		float resultX[100],resultY[100],resultR[100];
		circleDetect("D:\\test\\SIMproject\\SIMproject\\bin\\Release\\1.bmp",15.3,18.7,140,17*1.5,resultNum,resultX,resultY,resultR,200,255,0);
	}