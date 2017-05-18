#include <cv.hpp>
#include <highgui.h>

using namespace cv;

//ͼ��ģ��
//filenameͼ���ļ�·��
//ksize����˴�С
//size����ͼ��������
//data����ͼ������
void __declspec(dllexport) blur(char* filename, int ksize, int size, unsigned char *data)
{
	Mat src = imread(filename);
	cv::blur(src,src,Size(ksize,ksize));
	for (int i=0;i<size;i++)
		data[i] = src.data[i];
}
