
#include <cv.hpp>
#include <highgui.h>
#include <iostream>
#include <fstream>

using namespace std;
using namespace cv;

//计算梯度
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

//计算显著边缘点（局部最大值点）
void AnchorPointMy(const Mat& src, Mat& anchor, int step, int thr)
{
	for (int i=0;i<src.rows;i++)
		for (int j=0;j<src.cols;j++)
		{
			int temp = src.data[i*src.cols+j];
			if (temp<thr) continue;
			bool pp = true;
			//判断是否为局部最大值点
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

//DFS方法连接边缘显著点
void Link(const Mat& src, const Mat& direction, Mat& anchor, int i, int j,int preI,int preJ)
{
	if (anchor.data[i*anchor.cols+j] == 254) return;
	anchor.data[i*anchor.cols+j] = 254;
	//沿边缘方向进行连接，根据梯度方向确定连接搜索方向
	if (direction.data[i*anchor.cols+j]==0)
	{
		//对边缘方向的6个相邻点进行搜索，寻找最大后继
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
		//基于最大后继进行递归连接
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

//通过调用link方法连接边缘显著点
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

//LSD边缘提取
//width,height待处理图像的宽和高
//imgSize待处理图像数据量
//data待处理图像数据
//step,thr算法阈值,step影响显著点数量,thr影响连接规则
//edgeNum返回结果的边缘数量
//edgeX,edgeY返回结果的边缘坐标
void __declspec(dllexport) LSD(int width, int height, int imgSize, unsigned char *data, int step, int thr, int& edgeNum, int *edgeX, int *edgeY)
{
	Mat pic;

	//计算图像通道数
	int channel = imgSize/width/height + 0.5;

	//根据图像通道数保存为对应的Mat类型
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
	
	//计算梯度
	ComputeGradient(pic, gradient, direction);
	
	//提取显著点
	AnchorPointMy(gradient, anchorMy, step, thr);

	//连接显著点
	AnchorLink(gradient, direction, anchorMy);

	//记录边缘位置
	edgeNum = 0;
	for (int i=0;i<height;i++)
		for (int j=0;j<width;j++)
			if (anchorMy.data[i*width+j] == 254)
			{
				edgeX[edgeNum] = j;
				edgeY[edgeNum++] = i;
			}	
}

