#include <cv.hpp>
#include <highgui.h>

using namespace cv;

//图像模糊
//filename图像文件路径
//ksize卷积核大小
//size返回图像数据量
//data返回图像数据
void __declspec(dllexport) blur(char* filename, int ksize, int size, unsigned char *data)
{
	Mat src = imread(filename);
	cv::blur(src,src,Size(ksize,ksize));
	for (int i=0;i<size;i++)
		data[i] = src.data[i];
}
