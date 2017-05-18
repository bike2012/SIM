
#include <cv.hpp>
#include <highgui.h>
#include <iostream>
#include <fstream>

using namespace std;
using namespace cv;

//�����ݶ�
void ComputeGradient(const Mat& src, Mat& gradient, Mat& direction)
{
	int w = src.cols, h = src.rows;
	for (int i=1;i<h-1;i++)
		for (int j=1;j<w-1;j++)
		{
			int horizonal = abs(src.data[(i-1)*w+j-1]*1+src.data[i*w+j-1]*2+src.data[(i+1)*w+j-1]*1-
				src.data[(i-1)*w+j+1]*1-src.data[i*w+j+1]*2-src.data[(i+1)*w+j+1]*1)/8;
			int vertical = abs(src.data[(i-1)*w+j-1]*1+src.data[(i-1)*w+j]*2+src.data[(i-1)*w+j+1]*1-
				src.data[(i+1)*w+j-1]*1-src.data[(i+1)*w+j]*2-src.data[(i+1)*w+j+1]*1)/8;
			gradient.data[i*w+j]=horizonal+vertical;
			if (horizonal>vertical) direction.data[i*w+j]=255;else direction.data[i*w+j]=0;
		}
}

//����������Ե�㣨�ֲ����ֵ�㣩
void AnchorPointMy(const Mat& src, Mat& anchor, int step, int thr)
{
	for (int i=0;i<src.rows;i++)
		for (int j=0;j<src.cols;j++)
		{
			int temp = src.data[i*src.cols+j];
			if (temp<thr) continue;
			bool pp = true;
			//�ж��Ƿ�Ϊ�ֲ����ֵ��
			for (int ii=i-step;ii<=i+step;ii++)
			{
				for (int jj=j-step;jj<=j+step;jj++)
				{
					if (ii>=0 && ii<src.rows && jj>=0 && jj<src.cols)
						if (src.data[ii*src.cols+jj]>temp)
						{
							pp = false;
							break;
						}
				}
				if (!pp) break;
			}
			if (pp)
				anchor.data[i*anchor.cols+j]=255;
		}
}

//DFS�������ӱ�Ե������
void Link(const Mat& src, const Mat& direction, Mat& anchor, int i, int j,int preI,int preJ)
{
	if (anchor.data[i*anchor.cols+j] == 254) return;
	anchor.data[i*anchor.cols+j] = 254;
	//�ر�Ե����������ӣ������ݶȷ���ȷ��������������
	if (direction.data[i*anchor.cols+j]==0)
	{
		//�Ա�Ե�����6�����ڵ����������Ѱ�������
		int ii = i, jj = j, max = -1;
		if (i-1>=0 && j-1>=0 && j-1!=preJ)
		{
			if (src.data[(i-1)*src.cols+(j-1)]>max)
			{
				ii = i - 1;
				jj = j - 1;
				max = src.data[(i-1)*src.cols+(j-1)];
			}
		}
		if (j-1>=0 && j-1!=preJ)
		{
			if (src.data[(i)*src.cols+(j-1)]>max)
			{
				ii = i;
				jj = j - 1;
				max = src.data[(i)*src.cols+(j-1)];
			}
		}
		if (i+1<src.rows && j-1>=0  && j-1!=preJ)
		{
			if (src.data[(i+1)*src.cols+(j-1)]>max)
			{
				ii = i + 1;
				jj = j - 1;
				max = src.data[(i+1)*src.cols+(j-1)];
			}
		}
		if (i-1>=0 && j+1<src.cols && j+1!=preJ)
		{
			if (src.data[(i-1)*src.cols+(j+1)]>max)
			{
				ii = i - 1;
				jj = j + 1;
				max = src.data[(i-1)*src.cols+(j+1)];
			}
		}
		if (j+1<src.cols && j+1!=preJ)
		{
			if (src.data[(i)*src.cols+(j+1)]>max)
			{
				ii = i;
				jj = j + 1;
				max = src.data[(i)*src.cols+(j+1)];
			}
		}
		if (i+1<src.rows && j+1<src.cols && j+1!=preJ)
		{
			if (src.data[(i+1)*src.cols+(j+1)]>max)
			{
				ii = i + 1;
				jj = j + 1;
				max = src.data[(i+1)*src.cols+(j+1)];
			}
		}
		//��������̽��еݹ�����
		if (max>src.data[i*src.cols+j]*2/4) Link(src, direction, anchor, ii, jj,i,j);
	}
	else
	{
		int ii = i, jj = j, max = -1;
		if (i-1>=0 && j-1>=0 && i-1!=preI)
		{
			if (src.data[(i-1)*src.cols+(j-1)]>max)
			{
				ii = i - 1;
				jj = j - 1;
				max = src.data[(i-1)*src.cols+(j-1)];
			}
		}
		if (i-1>=0 && i-1!=preI)
		{
			if (src.data[(i-1)*src.cols+(j)]>max)
			{
				ii = i-1;
				jj = j;
				max = src.data[(i-1)*src.cols+(j)];
			}
		}
		if (i-1>=0 && j+1<src.cols && i-1!=preI)
		{
			if (src.data[(i-1)*src.cols+(j+1)]>max)
			{
				ii = i - 1;
				jj = j + 1;
				max = src.data[(i-1)*src.cols+(j+1)];
			}
		}
		if (i+1<src.rows && j-1>=0 && i+1!=preI)
		{
			if (src.data[(i+1)*src.cols+(j-1)]>max)
			{
				ii = i + 1;
				jj = j - 1;
				max = src.data[(i+1)*src.cols+(j-1)];
			}
		}		
		if (i+1<src.rows && i+1!=preI)
		{
			if (src.data[(i+1)*src.cols+(j)]>max)
			{
				ii = i+1;
				jj = j;
				max = src.data[(i+1)*src.cols+(j)];
			}
		}
		if (i+1<src.rows && j+1<src.cols && i+1!=preI)
		{
			if (src.data[(i+1)*src.cols+(j+1)]>max)
			{
				ii = i + 1;
				jj = j + 1;
				max = src.data[(i+1)*src.cols+(j+1)];
			}
		}
		if (max>src.data[i*src.cols+j]*2/4) Link(src, direction, anchor, ii, jj,i,j);
	}

}

//ͨ������link�������ӱ�Ե������
void AnchorLink(const Mat& src, const Mat& direction, Mat& anchor)
{
	for (int i=0;i<src.rows;i++)
		for (int j=0;j<src.cols;j++)
		{
			if (anchor.data[i*anchor.cols+j]==255)
			{
				Link(src,direction, anchor,i,j,i-1,j-1);
				anchor.data[i*anchor.cols+j]=255;
				Link(src,direction, anchor,i,j,i+1,j+1);
			}
		}
}

//LSD��Ե��ȡ
//width,height������ͼ��Ŀ�͸�
//imgSize������ͼ��������
//data������ͼ������
//step,thr�㷨��ֵ,stepӰ������������,thrӰ�����ӹ���
//edgeNum���ؽ���ı�Ե����
//edgeX,edgeY���ؽ���ı�Ե����
void __declspec(dllexport) LSD(int width, int height, int imgSize, unsigned char *data, int step, int thr, int& edgeNum, int *edgeX, int *edgeY)
{
	Mat pic;

	//����ͼ��ͨ����
	int channel = imgSize/width/height + 0.5;

	//����ͼ��ͨ��������Ϊ��Ӧ��Mat����
	switch (channel)
	{
	case 1:
		pic = Mat(height,width,CV_8UC1);
		for (int i=0;i<imgSize;i++)
			pic.data[i] = data[i];
		break;
	case 3:
		pic = Mat(height,width,CV_8UC3);
		for (int i=0;i<imgSize;i++)
			pic.data[i] = data[i];
		cvtColor(pic,pic,CV_RGB2GRAY);
		break;
	case 4:
		pic = Mat(height,width,CV_8UC4);
		for (int i=0;i<imgSize;i++)
			pic.data[i] = data[i];
		cvtColor(pic,pic,CV_RGBA2GRAY);
		break;
	default:
		return;
	};

	Mat gradient(pic.rows,pic.cols,CV_8UC1,cv::Scalar(0)),direction(pic.rows,pic.cols,CV_8UC1,cv::Scalar(0));
	Mat anchor(pic.rows,pic.cols,CV_8UC1,cv::Scalar(0));
	Mat anchorMy(pic.rows,pic.cols,CV_8UC1,cv::Scalar(0));
	
	//�����ݶ�
	ComputeGradient(pic, gradient, direction);
	
	//��ȡ������
	AnchorPointMy(gradient, anchorMy, step, thr);

	//����������
	AnchorLink(gradient, direction, anchorMy);

	//��¼��Եλ��
	edgeNum = 0;
	for (int i=0;i<height;i++)
		for (int j=0;j<width;j++)
			if (anchorMy.data[i*width+j] == 254)
			{
				edgeX[edgeNum] = j;
				edgeY[edgeNum++] = i;
			}	
}

