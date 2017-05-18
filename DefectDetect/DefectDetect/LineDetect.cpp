#include <cv.hpp>
#include <highgui.h>
#include <iostream>

using namespace std;
using namespace cv;


void getSingleLine( Mat src, float rho, float theta, double &rho_out, double& angle_out)
{
	CvMat *img = new CvMat(src);

    cv::AutoBuffer<int> _accum, _sort_buf;
    cv::AutoBuffer<float> _tabSin, _tabCos;

    const uchar* image;
    int step, width, height;
    int numangle, numrho;
    int total = 0;
    int i, j;
    float irho = 1 / rho;
    double scale;
    //再次确保输入图像的正确性
    CV_Assert( CV_IS_MAT(img) && CV_MAT_TYPE(img->type) == CV_8UC1 );

    image = img->data.ptr;    //得到图像的指针
    step = img->step;    //得到图像的步长
    width = img->cols;    //得到图像的宽
    height = img->rows;    //得到图像的高
    //由角度和距离的分辨率得到角度和距离的数量，即霍夫变换后角度和距离的个数
    numangle = cvRound(CV_PI / theta);
    numrho = cvRound(((width + height) * 2 + 1) / rho);

    //为累加器数组分配内存空间
    //该累加器数组其实就是霍夫空间，它是用一维数组表示二维空间
    _accum.allocate((numangle+2) * (numrho+2));
    //为排序数组分配内存空间
    _sort_buf.allocate(numangle * numrho);
    //为正弦和余弦列表分配内存空间
    _tabSin.allocate(numangle);
    _tabCos.allocate(numangle);
    //分别定义上述内存空间的地址指针
    int *accum = _accum, *sort_buf = _sort_buf;
    float *tabSin = _tabSin, *tabCos = _tabCos;
    //累加器数组清零
    memset( accum, 0, sizeof(accum[0]) * (numangle+2) * (numrho+2) );

    float ang = 0;
    //为避免重复运算，事先计算好sinθi/ρ和cosθi/ρ
    for(int n = 0; n < numangle; ang += theta, n++ )
    {
        tabSin[n] = (float)(sin((double)ang) * irho);
        tabCos[n] = (float)(cos((double)ang) * irho);
    }

    // stage 1. fill accumulator
    //执行步骤1，逐点进行霍夫空间变换，并把结果放入累加器数组内
    for( i = 0; i < height; i++ )
        for( j = 0; j < width; j++ )
        {
            //只对图像的非零值处理，即只对图像的边缘像素进行霍夫变换
            if( image[i * step + j] != 0 )
                for(int n = 0; n < numangle; n++ )
                {
                    int r = cvRound( j * tabCos[n] + i * tabSin[n] );
                    r += (numrho - 1) / 2;
                    //r表示的是距离，n表示的是角点，在累加器内找到它们所对应的位置（即霍夫空间内的位置），其值加1
                    accum[(n+1) * (numrho+2) + r+1]+=image[i * step + j];
                }
        }

	int rr = 0,nn=0,max=0;
	for(int r = 0; r < numrho; r++ )  
        for(int n = 0; n < numangle; n++ )  
			if (accum[(n+1) * (numrho+2) + r+1]>max)
			{
				max = accum[(n+1) * (numrho+2) + r+1];
				rr = r;
				nn = n;
			}
	rho_out = (rr - (numrho - 1)*0.5f) * rho;  
	angle_out = nn * theta;  

}


float __declspec(dllexport) getAccuracyLine(char* filename, float &X0, float &Y0, float &X1, float &Y1, float &angle,int searchWidth, int lineMaxOffset)
{
	Mat src = imread(filename, -1);
	//cv::cvtColor(src,src,cv::COLOR_RGB2GRAY);

	Mat gradient = Mat::zeros(src.rows,src.cols,CV_8UC1);

	int left = min(X0,X1)-searchWidth*1.5<1?1:min(X0,X1)-searchWidth*1.5;
	int right = max(X0,X1)+searchWidth*1.5>src.cols-1?src.cols-1:max(X0,X1)+searchWidth*1.5;
	int top = min(Y0,Y1)-searchWidth*1.5<1?1:min(Y0,Y1)-searchWidth*1.5;
	int bottom = max(Y0,Y1)+searchWidth*1.5>src.rows-1?src.rows-1:max(Y0,Y1)+searchWidth*1.5;

	int type;
	double k,b;

	if (X0 == X1 && Y0 == Y1)return 0;

	if (abs(X1-X0)>abs(Y1-Y0))
	{
		type = 0;
		k = double(Y1-Y0)/double(X1-X0);
		b = Y0-k*X0;
	}
	else 
	{
		type = 1;
		k = double(X1-X0)/double(Y1-Y0);
		b = X0-k*Y0;
	}

	for (int y=top;y<bottom;y++)
		for (int x = left;x<right;x++)
		{
			bool near = false;
			if (type == 0)
			{
				if (y>=k*x+b-searchWidth && y<=k*x+b+searchWidth)
					near = true;
			}
			else
			{
				if (x>=k*y+b-searchWidth && x<=k*y+b+searchWidth)
					near = true;
			}

			if (near)
			{
				int gx = abs(src.data[(y-1)*src.step+(x-1)*src.step.p[1]]+2*src.data[(y)*src.step+(x-1)*src.step.p[1]]+src.data[(y+1)*src.step+(x-1)*src.step.p[1]]-src.data[(y-1)*src.step+(x+1)*src.step.p[1]]-2*src.data[(y)*src.step+(x+1)*src.step.p[1]]-src.data[(y+1)*src.step+(x+1)*src.step.p[1]]);
				//int gx = abs(src.at<uchar>(y-1,x-1)+2*src.at<uchar>(y,x-1)+src.at<uchar>(y+1,x-1)-src.at<uchar>(y-1,x+1)-2*src.at<uchar>(y,x+1)-src.at<uchar>(y+1,x+1));
				int gy = abs(src.data[(y-1)*src.step+(x-1)*src.step.p[1]]+2*src.data[(y+1)*src.step+(x)*src.step.p[1]]+src.data[(y-1)*src.step+(x+1)*src.step.p[1]]-src.data[(y+1)*src.step+(x-1)*src.step.p[1]]-2*src.data[(y+1)*src.step+(x)*src.step.p[1]]-src.data[(y+1)*src.step+(x+1)*src.step.p[1]]);
				//int gy = abs(src.at<uchar>(y-1,x-1)+2*src.at<uchar>(y-1,x)+src.at<uchar>(y-1,x+1)-src.at<uchar>(y+1,x-1)-2*src.at<uchar>(y+1,x)-src.at<uchar>(y+1,x+1));
				gradient.data[y*gradient.cols+x] = (gx+gy)/8;
			}
		}

	//imshow("gradient",gradient);
	//waitKey();

	double rho = 0,theta = 0;
	getSingleLine(gradient, 1,CV_PI/720,rho,theta);

	

	//Mat show = imread("12.bmp");
		float integrity = 0;

	{
    Point pt1, pt2;
    double a = cos(theta), b = -sin(theta);
    double x = a*rho, y = -b*rho;
	if (fabs(a)<fabs(b))
	{
		Y0 = y+(X0-x)/b*a;
		Y1 = y+(X1-x)/b*a;

		double x0,y0,x1,y1;
		if (X0<X1)
		{
			x0=X0;y0=Y0;x1=X1;y1=Y1;
		}
		else
		{
			x0=X1;y0=Y1;x1=X0;y1=Y0;
		}
		int avg = 0, num = 0;
		for (int x = x0;x<=x1+1;x++)
		{
			int y = (x-x0)*(y1-y0)/(x1-x0)+y0;
			avg+=gradient.data[y*gradient.cols+x];
			num++;
		}
		avg/=num;
		avg*=0.8;
		int okNum = 0;
		for (int x = x0;x<=x1+1;x++)
		{
			int y = (x-x0)*(y1-y0)/(x1-x0)+y0;
			for (int i=0;i<=lineMaxOffset;i++)
			{
				if (y-i>=0)
					if (gradient.data[(y-i)*gradient.cols+x]>=avg)
					{
						okNum++;
						break;
					}
				if (y+i<gradient.rows)
					if (gradient.data[(y+i)*gradient.cols+x]>=avg)
					{
						okNum++;
						break;
					}
			}
		}
		integrity = (double)okNum/(double)num;
	}
	else
	{
		X0 = x+(Y0-y)/a*b;
		X1 = x+(Y1-y)/a*b;

		double x0,y0,x1,y1;
		if (Y0<Y1)
		{
			x0=X0;y0=Y0;x1=X1;y1=Y1;
		}
		else
		{
			x0=X1;y0=Y1;x1=X0;y1=Y0;
		}
		int avg = 0, num = 0;
		for (int y = y0;y<=y1+1;y++)
		{
			int x = (y-y0)*(x1-x0)/(y1-y0)+x0;
			avg+=gradient.data[y*gradient.cols+x];
			num++;
		}
		avg/=num;
		avg*=0.8;
		int okNum = 0;
		for (int y = y0;y<=y1+1;y++)
		{
			int x = (y-y0)*(x1-x0)/(y1-y0)+x0;
			for (int i=0;i<=lineMaxOffset;i++)
			{
				if (x-i>=0)
					if (gradient.data[y*gradient.cols+x-i]>=avg)
					{
						okNum++;
						break;
					}
				if (x+i<gradient.cols)
					if (gradient.data[y*gradient.cols+x+i]>=avg)
					{
						okNum++;
						break;
					}
			}
		}
		integrity = (double)okNum/(double)num;
	}
	}

	

	theta-=CV_PI/2;
	if (theta<0) theta+=CV_PI*2;
	angle = theta;
	return integrity;
	//cout<<angle<<endl;
}

//void main()
//{
//
//float X0 = 1068;//963;//914;
//float Y0 = 300;//749;//799;
//float X1 = 1491;//1460;//1414;
//float Y1 = 504;//640;//683;
//float theta = 0;
//	getAccuracyLine("12.bmp",X0,Y0,X1,Y1,theta,20);
//
//}


