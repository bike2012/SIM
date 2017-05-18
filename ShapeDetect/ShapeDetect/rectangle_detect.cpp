#include "cv.h"
#include "highgui.h"
#include <math.h>
#include <string.h>
#include <iostream>
using namespace std;

double dist(CvPoint* pt1, CvPoint* pt2)
{
	return sqrt((pt1->x-pt2->x)*(pt1->x-pt2->x)+(pt1->y-pt2->y)*(pt1->y-pt2->y));
}

//longtowitdh用来检测长宽比
double longtowitdh(CvPoint* pt1, CvPoint* pt2, CvPoint* pt0){
	double dx1 = pt1->x - pt0->x;
	double dy1 = pt1->y - pt0->y;
	double dx2 = pt2->x - pt0->x;
	double dy2 = pt2->y - pt0->y;
	double rate = sqrt((dx1*dx1 + dy1*dy1) / (dx2*dx2 + dy2*dy2));
	return rate>1 ? rate : (1 / rate);
}

//angle函数用来返回（两个向量之间找到角度的余弦值）
double angle(CvPoint* pt1, CvPoint* pt2, CvPoint* pt0)
{
	double dx1 = pt1->x - pt0->x;
	double dy1 = pt1->y - pt0->y;
	double dx2 = pt2->x - pt0->x;
	double dy2 = pt2->y - pt0->y;
	return (dx1*dx2 + dy1*dy2) / sqrt((dx1*dx1 + dy1*dy1)*(dx2*dx2 + dy2*dy2) + 1e-10);
}

// 返回图像中找到的所有轮廓序列，并且序列存储在内存存储器中

bool PtInAnyRect(CvPoint pCur, CvPoint pLT, CvPoint pLB, CvPoint pRB, CvPoint pRT )   
{   
    //任意四边形有4个顶点  
    int nCount = 4;  
    CvPoint RectPoints[4] = { pLT, pLB, pRB, pRT };  
    int nCross = 0;  
    for (int i = 0; i < nCount; i++)   
    {   
        //依次取相邻的两个点  
        CvPoint pStart = RectPoints[i];   
        CvPoint pEnd = RectPoints[(i + 1) % nCount];  
  
        //相邻的两个点是平行于x轴的，肯定不相交，忽略  
        if ( pStart.y == pEnd.y )   
            continue;  
  
        //交点在pStart,pEnd的延长线上，pCur肯定不会与pStart.pEnd相交，忽略  
        if ( pCur.y < min(pStart.y, pEnd.y) || pCur.y > max( pStart.y, pEnd.y ) )   
            continue;  

        //求当前点和x轴的平行线与pStart,pEnd直线的交点的x坐标  
        double x = (double)(pCur.y - pStart.y) * (double)(pEnd.x - pStart.x) / (double)(pEnd.y - pStart.y) + pStart.x;  
  
        //若x坐标大于当前点的坐标，则有交点  
        if ( x > pCur.x )   
            nCross++;   
    }  
  
    // 单边交点为偶数，点在多边形之外   
    return (nCross % 2 == 1);   
}  


void __declspec(dllexport) rectDetect(char* filename,  double width,double height, double range, int& resultNum, float *resultData,int colorLowThr, int colorHighThr, int rate, int edgeType, int para1, int para2)
{
	IplImage* img = cvLoadImage(filename, 1);
		cvSmooth(img,img,CV_GAUSSIAN,7,7);
	CvMemStorage* storage = cvCreateMemStorage(0);
		IplImage* pImg8u = cvCreateImage(cvGetSize(img), 8, 1);
	cvCvtColor(img, pImg8u, CV_BGR2GRAY);
	CvSeq* contours;
	int i, c, l, N = 11;
	CvSize sz = cvSize(img->width & -2, img->height & -2);

	IplImage* timg = cvCloneImage(img);
	IplImage* gray = cvCreateImage(sz, 8, 1);
	IplImage* grayTmp = cvCreateImage(sz, 8, 1);
	IplImage* pyr = cvCreateImage(cvSize(sz.width / 2, sz.height / 2), 8, 3);
	IplImage* tgray;
	CvSeq* result;
	double s, t;
	// 创建一个空序列用于存储轮廓角点
	CvSeq* squares = cvCreateSeq(0, sizeof(CvSeq), sizeof(CvPoint), storage);

	cvSetImageROI(timg, cvRect(0, 0, sz.width, sz.height));
	// 过滤噪音
	cvPyrDown(timg, pyr, 7);
	cvPyrUp(pyr, timg, 7);
	tgray = cvCreateImage(sz, 8, 1);

	// 红绿蓝3色分别尝试提取
	for (c = 0; c < 3; c++)
	{
		// 提取 the c-th color plane
		cvSetImageCOI(timg, c + 1);
		cvCopy(timg, tgray, 0);

		// 尝试各种阈值提取得到的（N=11）
		for (l = 0; l < N; l++)
		{
			// apply Canny. Take the upper threshold from slider
			// Canny helps to catch squares with gradient shading  
			if (l == 0)
			{
				switch (edgeType)
				{
				case 0:
					cvSobel(tgray,gray,1,0,para1);
					cvSobel(tgray,grayTmp,0,1,para1);
					for (int i=0;i<gray->height*gray->width;i++)
						((uchar *)(gray->imageData))[i] = sqrt((((uchar *)(gray->imageData))[i]*((uchar *)(gray->imageData))[i]+((uchar *)(grayTmp->imageData))[i]*((uchar *)(grayTmp->imageData))[i])/2);
					cvThreshold(gray,gray,para2,255,CV_THRESH_BINARY);
					break;
				case 1:
					cvCanny(tgray, gray, para1, para2, 5);
					break;
				default:
					cvCanny(tgray, gray, para1, para2, 5);
					break;
				}

				//使用任意结构元素膨胀图像
				cvDilate(gray, gray, 0, 1);
			}
			else
			{
				// apply threshold if l!=0:
				cvThreshold(tgray, gray, (l + 1) * 255 / N, 255, CV_THRESH_BINARY);
			}

			// 找到所有轮廓并且存储在序列中
			cvFindContours(gray, storage, &contours, sizeof(CvContour),
				CV_RETR_LIST, CV_CHAIN_APPROX_SIMPLE, cvPoint(0, 0));

			// 遍历找到的每个轮廓contours
			while (contours)
			{
				//用指定精度逼近多边形曲线
				result = cvApproxPoly(contours, sizeof(CvContour), storage,
					CV_POLY_APPROX_DP, cvContourPerimeter(contours)*0.02, 0);


				if (result->total == 4 &&
					fabs(cvContourArea(result, CV_WHOLE_SEQ)) > 500 &&
					fabs(cvContourArea(result, CV_WHOLE_SEQ)) < 100000 &&
					cvCheckContourConvexity(result))
				{
					s = 0;

					for (i = 0; i < 5; i++)
					{
						// find minimum angle between joint edges (maximum of cosine)
						if (i >= 2)
						{
							t = fabs(angle(
								(CvPoint*)cvGetSeqElem(result, i),
								(CvPoint*)cvGetSeqElem(result, i - 2),
								(CvPoint*)cvGetSeqElem(result, i - 1)));
							s = s > t ? s : t;

						}
					}

					// if 余弦值 足够小，可以认定角度为90度直角
					//cos0.1=83度，能较好的趋近直角
					//K为长宽比
					if (s < 0.1){
						double dist1 = dist((CvPoint*)cvGetSeqElem(result, i),(CvPoint*)cvGetSeqElem(result, i - 1));
						double dist2 = dist((CvPoint*)cvGetSeqElem(result, i-1),(CvPoint*)cvGetSeqElem(result, i - 2));
						if ((dist1/width>1-range/100.0 && dist1/width<1+range/100.0 && dist2/height>1-range/100.0 &&dist2/height < 1+range/100.0) || (dist1/height>1-range/100.0 && dist1/height<1+range/100.0 && dist2/width>1-range/100.0 && dist2/width<1+range/100.0))
							for (i = 0; i < 4; i++)
								cvSeqPush(squares,
								(CvPoint*)cvGetSeqElem(result, i));
					}
				}

				// 继续查找下一个轮廓
				contours = contours->h_next;
			}
		}
	}
	cvReleaseImage(&gray);
	cvReleaseImage(&pyr);
	cvReleaseImage(&tgray);
	cvReleaseImage(&timg);

	CvSeqReader reader;
	cvStartReadSeq(squares, &reader, 0);

	resultNum = 0;
	// read 4 sequence elements at a time (all vertices of a square)
	for (int i = 0; i < squares->total; i += 4)
	{
		CvPoint pt[4], *rect = pt;
		int count = 4;

		// read 4 vertices
		CV_READ_SEQ_ELEM(pt[0], reader);
		CV_READ_SEQ_ELEM(pt[1], reader);
		CV_READ_SEQ_ELEM(pt[2], reader);
		CV_READ_SEQ_ELEM(pt[3], reader);

		bool repeat = false;
		float centerX = (pt[0].x+pt[1].x+pt[2].x+pt[3].x)/4;
		float centerY = (pt[0].y+pt[1].y+pt[2].y+pt[3].y)/4;
		for (int j=0;j<resultNum;j++)
			if (fabs(resultData[j*5]-centerX)<width/2 && fabs(resultData[j*5+1]-centerY)<height/2)
				repeat = true;

		if (!repeat)
		{
			int num = 0, total = 0;
			for (int i=min(pt[0].y,min(pt[1].y,min(pt[2].y,pt[3].y)));i<=max(pt[0].y,max(pt[1].y,max(pt[2].y,pt[3].y)));i++)
				for (int j=min(pt[0].x,min(pt[1].x,min(pt[2].x,pt[3].x)));j<=max(pt[0].x,max(pt[1].x,max(pt[2].x,pt[3].x)));j++)
				{
					CvPoint nowPt(j,i);
					if (PtInAnyRect(nowPt,pt[0],pt[1],pt[2],pt[3]))
					{
						total++;
						if ((int)((uchar *)(pImg8u->imageData + i*pImg8u->widthStep))[j]>=colorLowThr && (int)((uchar *)(pImg8u->imageData + i*pImg8u->widthStep))[j]<=colorHighThr)
							num++;
					}
				}
			if (num*100/total<rate)
				repeat = true;
		}

		if (!repeat)
		{
			resultData[resultNum*5] = centerX;
			resultData[resultNum*5+1] = centerY;
			resultData[resultNum*5+2] = pt[1].x;
			resultData[resultNum*5+3] = pt[1].y;

			float xr = pt[1].x - pt[0].x;
			float yr = pt[1].y - pt[0].y;

			float dirr = 0;
			if (xr == 0)
			{
				if (yr >= 0) dirr = CV_PI/2.0;
				else dirr = CV_PI/ 2.0 * 3;
			}
			else
			{
				dirr = atan(yr / xr);
				if (xr < 0)
				{
					dirr += CV_PI;
				}
				else
				{
					if (yr < 0)
						dirr += CV_PI * 2;
				}
			}

			resultData[resultNum*5+4] = dirr;
			resultNum++;
		}

		// draw the square as a closed polyline
	}

	cvClearMemStorage(storage);
}



//void __declspec(dllexport)
	void rectDetectWithRate(char* filename,  double lowRate,double highRate, double cannyThr, int& resultNum, float *resultData)
{
	IplImage* img = cvLoadImage(filename, 1);
	cvSmooth(img,img,CV_GAUSSIAN,7,7);
	CvMemStorage* storage = cvCreateMemStorage(0);
	CvSeq* contours;
	int i, c, l, N = 11;
	CvSize sz = cvSize(img->width & -2, img->height & -2);

	IplImage* timg = cvCloneImage(img);
	IplImage* gray = cvCreateImage(sz, 8, 1);
	IplImage* pyr = cvCreateImage(cvSize(sz.width / 2, sz.height / 2), 8, 3);
	IplImage* tgray;
	CvSeq* result;
	double s, t;
	// 创建一个空序列用于存储轮廓角点
	CvSeq* squares = cvCreateSeq(0, sizeof(CvSeq), sizeof(CvPoint), storage);

	cvSetImageROI(timg, cvRect(0, 0, sz.width, sz.height));
	// 过滤噪音
	cvPyrDown(timg, pyr, 7);
	cvPyrUp(pyr, timg, 7);
	tgray = cvCreateImage(sz, 8, 1);

	// 红绿蓝3色分别尝试提取
	for (c = 0; c < 3; c++)
	{
		// 提取 the c-th color plane
		cvSetImageCOI(timg, c + 1);
		cvCopy(timg, tgray, 0);

		// 尝试各种阈值提取得到的（N=11）
		for (l = 0; l < N; l++)
		{
			// apply Canny. Take the upper threshold from slider
			// Canny helps to catch squares with gradient shading  
			if (l == 0)
			{
				cvCanny(tgray, gray, 0, cannyThr, 5);

				//使用任意结构元素膨胀图像
				cvDilate(gray, gray, 0, 1);
			}
			else
			{
				// apply threshold if l!=0:
				cvThreshold(tgray, gray, (l + 1) * 255 / N, 255, CV_THRESH_BINARY);
			}

			// 找到所有轮廓并且存储在序列中
			cvFindContours(gray, storage, &contours, sizeof(CvContour),
				CV_RETR_LIST, CV_CHAIN_APPROX_SIMPLE, cvPoint(0, 0));

			// 遍历找到的每个轮廓contours
			while (contours)
			{
				//用指定精度逼近多边形曲线
				result = cvApproxPoly(contours, sizeof(CvContour), storage,
					CV_POLY_APPROX_DP, cvContourPerimeter(contours)*0.02, 0);


				if (result->total == 4 &&
					fabs(cvContourArea(result, CV_WHOLE_SEQ)) > 500 &&
					fabs(cvContourArea(result, CV_WHOLE_SEQ)) < 100000 &&
					cvCheckContourConvexity(result))
				{
					s = 0;

					for (i = 0; i < 5; i++)
					{
						// find minimum angle between joint edges (maximum of cosine)
						if (i >= 2)
						{
							t = fabs(angle(
								(CvPoint*)cvGetSeqElem(result, i),
								(CvPoint*)cvGetSeqElem(result, i - 2),
								(CvPoint*)cvGetSeqElem(result, i - 1)));
							s = s > t ? s : t;

						}
					}

					// if 余弦值 足够小，可以认定角度为90度直角
					//cos0.1=83度，能较好的趋近直角
					//K为长宽比
					if (s < 0.1){
						double k = fabs(longtowitdh(
							(CvPoint*)cvGetSeqElem(result, i),
							(CvPoint*)cvGetSeqElem(result, i - 2),
							(CvPoint*)cvGetSeqElem(result, i - 1)));
						if (k >= lowRate&&k<=highRate){
							for (i = 0; i < 4; i++)
								cvSeqPush(squares,
								(CvPoint*)cvGetSeqElem(result, i));
						}
					}
				}

				// 继续查找下一个轮廓
				contours = contours->h_next;
			}
		}
	}
	cvReleaseImage(&gray);
	cvReleaseImage(&pyr);
	cvReleaseImage(&tgray);
	cvReleaseImage(&timg);

	CvSeqReader reader;
	cvStartReadSeq(squares, &reader, 0);
	IplImage* cpy = cvCloneImage(img);
	resultNum = squares->total;
	// read 4 sequence elements at a time (all vertices of a square)
	for (int i = 0; i < squares->total; i += 4)
	{
		CvPoint pt[4], *rect = pt;
		int count = 4;

		// read 4 vertices
		CV_READ_SEQ_ELEM(pt[0], reader);
		CV_READ_SEQ_ELEM(pt[1], reader);
		CV_READ_SEQ_ELEM(pt[2], reader);
		CV_READ_SEQ_ELEM(pt[3], reader);

		resultData[i*5] = (pt[0].x+pt[1].x+pt[2].x+pt[3].x)/4;
		resultData[i*5+1] = (pt[0].y+pt[1].y+pt[2].y+pt[3].y)/4;
		resultData[i*5+2] = pt[1].x;
		resultData[i*5+3] = pt[1].y;

		float xr = pt[1].x - pt[0].x;
		float yr = pt[1].y - pt[0].y;

		float dirr = 0;
		if (xr == 0)
		{
			if (yr >= 0) dirr = CV_PI/2.0;
			else dirr = CV_PI/ 2.0 * 3;
		}
		else
		{
			dirr = atan(yr / xr);
			if (xr < 0)
			{
				dirr += CV_PI;
			}
			else
			{
				if (yr < 0)
					dirr += CV_PI * 2;
			}
		}

		resultData[i*5+4] = dirr;


		// draw the square as a closed polyline
		cvPolyLine(cpy, &rect, &count, 1, 1, CV_RGB(0, 255, 0), 2, CV_AA, 0);
	}

	cvNamedWindow("123");
	cvShowImage("123", cpy);
	cvWaitKey();
	cvClearMemStorage(storage);
}

void main2()
{
	int resultNum = 5000;
	float *result = new float[5000];
	rectDetectWithRate("123.bmp",0.1,10.0,50,resultNum, result);
}