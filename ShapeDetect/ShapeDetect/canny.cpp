#include <opencv2/opencv.hpp>
#include<opencv2/highgui/highgui.hpp>
#include<opencv2/imgproc/imgproc.hpp>
#include <string.h>


using namespace cv;

void __declspec(dllexport) edgeDetect(char* filename, double cannyThr, int imgSize, unsigned char* data)
//void __declspec(dllexport) edgeDetect(char* filename, double cannyThr, char* outname)
{
	//载入原始图  
	Mat src = imread(filename);  
	
	//转成灰度图，降噪，用canny，最后将得到的边缘作为掩码，拷贝原图到效果图上，得到彩色的边缘图
	
	Mat edge, gray;

	// 【2】将原图像转换为灰度图像
	cvtColor(src, gray, CV_BGR2GRAY);

	// 【3】先用使用 3x3内核来降噪
	//blur(gray, gray, Size(3, 3));

	// 【4】运行Canny算子
	Canny(gray, edge, cannyThr/2, cannyThr);

	for (int i=0;i<imgSize;i++)
		data[i] = edge.data[i];

	//imwrite(outname,dst);
}
