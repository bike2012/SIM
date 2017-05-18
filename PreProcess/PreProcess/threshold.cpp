#include <cv.hpp>
#include <highgui.h>
#include <vector>

using namespace cv;
using namespace std;

//��ֵ����������Ӿ���
//width,height������ͼ��Ŀ�͸�
//size������ͼ��������
//data������ͼ������
//lowThr,highThr�㷨��ֵ,�ûҶȷ�Χ��ΪĿ��
//ptNum��Ӿ��ζ�������4��
//ptX,ptYΪ�����ζ�������
void __declspec(dllexport) threshold(int width, int height, int size, unsigned char *data, int lowThr, int highThr, int ptNum, float *ptX, float *ptY)
{
	//����ͼ��ͨ��������Ϊ��Ӧ��Mat����
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

	//��ֵ��
	vector<Point2f> pts;
	for (int i=0;i<width*height;i++)
		if (src.data[i] >=lowThr && src.data[i]<=highThr)
		{
			pts.push_back(Point2f(i%width,i/width));
			data[i] = 255;
		}
		else
			data[i] = 0;

	//������С��Ӿ���
	RotatedRect rect = minAreaRect(pts);
	Point2f corners[4];
	rect.points(corners);
	for (int i=0;i<4;i++)
	{
		ptX[i] = corners[i].x;
		ptY[i] = corners[i].y;
	}	
}

//������ɫֱ��ͼ
//width,height������ͼ��Ŀ�͸�
//size������ͼ��������
//data������ͼ������
//histΪ���ص���ɫֱ��ͼ
void __declspec(dllexport) colorHist(int width, int height, int size, unsigned char *data, int *hist)
{
	//����ͼ��ͨ��������Ϊ��Ӧ��Mat����
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

	//����ֱ��ͼ
	for (int i=0;i<width*height;i++)
		hist[src.data[i]]++;


}
