#include <cv.hpp>
using namespace cv;

//Canny边缘提取算法
//width,height待处理图像的宽和高
//imgSize待处理图像数据量
//data待处理图像数据
//lowThr,highThr算法阈值,梯度高于highThr的像素必为边缘,梯度位于lowThr-highThr之间的根据附近像素情况判定
//edgeNum返回结果的边缘数量
//edgeX,edgeY返回结果的边缘坐标
void __declspec(dllexport) canny(int width, int height, int imgSize, unsigned char *data, int lowThr, int highThr, int& edgeNum, int *edgeX, int *edgeY)
{
	Mat src;

	//计算图像通道数
	int channel = imgSize/width/height + 0.5;

	//根据图像通道数保存为对应的Mat类型
	switch (channel)
	{
	case 1:
		src = Mat(height,width,CV_8UC1);
		for (int i=0;i<imgSize;i++)
			src.data[i] = data[i];
		break;
	case 3:
		src = Mat(height,width,CV_8UC3);
		for (int i=0;i<imgSize;i++)
			src.data[i] = data[i];
		cvtColor(src,src,CV_RGB2GRAY);
		break;
	case 4:
		src = Mat(height,width,CV_8UC4);
		for (int i=0;i<imgSize;i++)
			src.data[i] = data[i];
		cvtColor(src,src,CV_RGBA2GRAY);
		break;
	default:
		return;
	};
	
	//Canny边缘提取
	Canny(src, src, lowThr, highThr);

	//记录边缘位置
	edgeNum = 0;
	for (int i=0;i<height;i++)
		for (int j=0;j<width;j++)
			if (src.data[i*width+j] == 255)
			{
				edgeX[edgeNum] = j;
				edgeY[edgeNum++] = i;
			}
}

//Sobel边缘提取算法
//width,height待处理图像的宽和高
//imgSize待处理图像数据量
//data待处理图像数据
//ksize参考领域范围
//thr算法阈值,大于该梯度值作为边缘,如thr为-1,采用OTSU自适应方法确定
//edgeNum返回结果的边缘数量
//edgeX,edgeY返回结果的边缘坐标
void __declspec(dllexport) sobel(int width, int height, int imgSize, unsigned char *data, int ksize, int& thr, int& edgeNum, int *edgeX, int *edgeY)
{
	Mat src;

	//计算图像通道数
	int channel = imgSize/width/height + 0.5;

	//根据图像通道数保存为对应的Mat类型
	switch (channel)
	{
	case 1:
		src = Mat(height,width,CV_8UC1);
		for (int i=0;i<imgSize;i++)
			src.data[i] = data[i];
		break;
	case 3:
		src = Mat(height,width,CV_8UC3);
		for (int i=0;i<imgSize;i++)
			src.data[i] = data[i];
		cvtColor(src,src,CV_RGB2GRAY);
		break;
	case 4:
		src = Mat(height,width,CV_8UC4);
		for (int i=0;i<imgSize;i++)
			src.data[i] = data[i];
		cvtColor(src,src,CV_RGBA2GRAY);
		break;
	default:
		return;
	};

	Mat sobel, sobelY;
	//计算X及Y方向梯度，并进行合成
	Sobel(src,sobel,src.depth(),1,0,ksize);
	Sobel(src,sobelY,src.depth(),0,1,ksize);
	for (int i=0;i<sobel.rows*sobel.cols;i++)
		sobel.data[i] = sqrt((sobel.data[i]*sobel.data[i]+sobelY.data[i]*sobelY.data[i])/2);

	//如果thr为-1则采用自适应方法，否则根据thr进行二值化
	if (thr == -1)
		thr = threshold(sobel,sobel,0,255,THRESH_BINARY | THRESH_OTSU);
	else
		threshold(sobel,sobel,thr,255,THRESH_BINARY);

	//记录边缘位置
	edgeNum = 0;
	for (int i=0;i<height;i++)
		for (int j=0;j<width;j++)
			if (sobel.data[i*width+j] == 255)
			{
				edgeX[edgeNum] = j;
				edgeY[edgeNum++] = i;
			}
}
