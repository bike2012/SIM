
#include <cv.hpp>
#include <highgui.h>
#include <iostream>
#include <fstream>

using namespace std;
using namespace cv;

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

void AnchorPointFormer(const Mat& src, Mat& anchor, int step, int thr)
{
	for (int i=0;i<=(src.rows-1)/step;i++)
		for (int j=0;j<=(src.cols-1)/step;j++)
		{
			int max = 0, tagi = 0, tagj = 0;
			int row = (src.rows<i*step+step)?src.rows:i*step+step;
			int col = (src.cols<j*step+step)?src.cols:j*step+step;

			for (int ii=i*step;ii<row;ii++)
				for (int jj=j*step;jj<col;jj++)
				{
					if (src.data[ii*src.cols+jj]>max)
					{
						max=src.data[ii*src.cols+jj];
						tagi=ii;
						tagj=jj;
					}
				}
				if (src.data[tagi*src.cols+tagj]>thr)
					anchor.data[tagi*anchor.cols+tagj]=255;
		}
}

void AnchorPointMy(const Mat& src, Mat& anchor, int step, int thr)
{
	for (int i=0;i<src.rows;i++)
		for (int j=0;j<src.cols;j++)
		{
			int temp = src.data[i*src.cols+j];
			if (temp<thr) continue;
			bool pp = true;
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

void AnchorPoint(const Mat& src, const Mat& direction, Mat& anchor, int step, int thr)
{
	for (int i=0;i<src.rows;i+=step)
		for (int j=0;j<src.cols;j++)
		{
			if (direction.data[i*src.cols+j]==0)
			{
				if (i-1>=0 && src.data[i*src.cols+j]-src.data[(i-1)*src.cols+j]>thr)
					if (i+1<src.rows && src.data[i*src.cols+j]-src.data[(i+1)*src.cols+j]>thr)
						anchor.data[i*anchor.cols+j]=255;
			}
			else
			{
				if (j-1>=0 && src.data[i*src.cols+j]-src.data[i*src.cols+j-1]>thr)
					if (j+1<src.cols && src.data[i*src.cols+j]-src.data[i*src.cols+j+1]>thr)
						anchor.data[i*anchor.cols+j]=255;
			}
		}

		for (int i=0;i<src.rows;i++)
			for (int j=0;j<src.cols;j+=step)
			{
				if (direction.data[i*src.cols+j]==0)
				{
					if (i-1>=0 && src.data[i*src.cols+j]-src.data[(i-1)*src.cols+j]>thr)
						if (i+1<src.rows && src.data[i*src.cols+j]-src.data[(i+1)*src.cols+j]>thr)
							anchor.data[i*anchor.cols+j]=255;
				}
				else
				{
					if (j-1>=0 && src.data[i*src.cols+j]-src.data[i*src.cols+j-1]>thr)
						if (j+1<src.cols && src.data[i*src.cols+j]-src.data[i*src.cols+j+1]>thr)
							anchor.data[i*anchor.cols+j]=255;
				}
			}
}

void Link(const Mat& src, const Mat& direction, Mat& anchor, int i, int j,int preI,int preJ)
{
	if (anchor.data[i*anchor.cols+j] == 254) return;
	anchor.data[i*anchor.cols+j] = 254;
	if (direction.data[i*anchor.cols+j]==0)
	{
		int ii = i, jj = j, max = -1;//, count = 0;
		if (i-1>=0 && j-1>=0 && j-1!=preJ)//(i-1!=preI || j-1!=preJ))
		{
			if (src.data[(i-1)*src.cols+(j-1)]>max)
			{
				ii = i - 1;
				jj = j - 1;
				max = src.data[(i-1)*src.cols+(j-1)];
			}
		}
		if (j-1>=0 && j-1!=preJ)//(i!=preI || j-1!=preJ))
		{
			if (src.data[(i)*src.cols+(j-1)]>max)
			{
				ii = i;
				jj = j - 1;
				max = src.data[(i)*src.cols+(j-1)];
			}
		}
		if (i+1<src.rows && j-1>=0  && j-1!=preJ)//(i+1!=preI || j-1!=preJ))
		{
			if (src.data[(i+1)*src.cols+(j-1)]>max)
			{
				ii = i + 1;
				jj = j - 1;
				max = src.data[(i+1)*src.cols+(j-1)];
			}
		}
		if (i-1>=0 && j+1<src.cols && j+1!=preJ)// (i-1!=preI || j+1!=preJ))
		{
			if (src.data[(i-1)*src.cols+(j+1)]>max)
			{
				ii = i - 1;
				jj = j + 1;
				max = src.data[(i-1)*src.cols+(j+1)];
			}
		}
		if (j+1<src.cols && j+1!=preJ)//(i!=preI || j+1!=preJ))
		{
			if (src.data[(i)*src.cols+(j+1)]>max)
			{
				ii = i;
				jj = j + 1;
				max = src.data[(i)*src.cols+(j+1)];
			}
		}
		if (i+1<src.rows && j+1<src.cols && j+1!=preJ)//(i+1!=preI || j+1!=preJ))
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
	else
	{
		int ii = i, jj = j, max = -1;//, count = 0;
		if (i-1>=0 && j-1>=0 && i-1!=preI)//(i-1!=preI || j-1!=preJ))
		{
			if (src.data[(i-1)*src.cols+(j-1)]>max)
			{
				ii = i - 1;
				jj = j - 1;
				max = src.data[(i-1)*src.cols+(j-1)];
			}
		}
		if (i-1>=0 && i-1!=preI)//(i-1!=preI || j!=preJ))
		{
			if (src.data[(i-1)*src.cols+(j)]>max)
			{
				ii = i-1;
				jj = j;
				max = src.data[(i-1)*src.cols+(j)];
			}
		}
		if (i-1>=0 && j+1<src.cols && i-1!=preI)//(i-1!=preI || j+1!=preJ))
		{
			if (src.data[(i-1)*src.cols+(j+1)]>max)
			{
				ii = i - 1;
				jj = j + 1;
				max = src.data[(i-1)*src.cols+(j+1)];
			}
		}
		if (i+1<src.rows && j-1>=0 && i+1!=preI)//(i+1!=preI || j-1!=preJ))
		{
			if (src.data[(i+1)*src.cols+(j-1)]>max)
			{
				ii = i + 1;
				jj = j - 1;
				max = src.data[(i+1)*src.cols+(j-1)];
			}
		}		
		if (i+1<src.rows && i+1!=preI)//(i+1!=preI || j!=preJ))
		{
			if (src.data[(i+1)*src.cols+(j)]>max)
			{
				ii = i+1;
				jj = j;
				max = src.data[(i+1)*src.cols+(j)];
			}
		}
		if (i+1<src.rows && j+1<src.cols && i+1!=preI)//(i+1!=preI || j+1!=preJ))
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

int Average(const Mat& src)
{
	int total = 0;
	for (int i=0;i<src.rows;i++)
		for (int j=0;j<src.cols;j++)
			total+=src.data[i*src.cols+j];
	return total/(src.rows*src.cols);
}

void __declspec(dllexport) newEdgeDetect(char* filename, int step, int thr, int imgSize, unsigned char* data)
{
	Mat origin = imread(filename);

	Mat pic;
	cvtColor(origin,pic, CV_BGR2GRAY);

	Mat gradient(pic.rows,pic.cols,CV_8UC1,cv::Scalar(0)),direction(pic.rows,pic.cols,CV_8UC1,cv::Scalar(0));
	Mat anchor(pic.rows,pic.cols,CV_8UC1,cv::Scalar(0));
	Mat anchorMy(pic.rows,pic.cols,CV_8UC1,cv::Scalar(0));
	
	ComputeGradient(pic, gradient, direction);

	AnchorPointMy(gradient, anchorMy, step, thr);

	AnchorLink(gradient, direction, anchorMy);



	for (int i=0;i<imgSize;i++)
		data[i] = anchorMy.data[i];

	
}

