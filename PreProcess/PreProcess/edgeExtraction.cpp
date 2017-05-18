#include <cv.hpp>
using namespace cv;

//Canny��Ե��ȡ�㷨
//width,height������ͼ��Ŀ�͸�
//imgSize������ͼ��������
//data������ͼ������
//lowThr,highThr�㷨��ֵ,�ݶȸ���highThr�����ر�Ϊ��Ե,�ݶ�λ��lowThr-highThr֮��ĸ��ݸ�����������ж�
//edgeNum���ؽ���ı�Ե����
//edgeX,edgeY���ؽ���ı�Ե����
void __declspec(dllexport) canny(int width, int height, int imgSize, unsigned char *data, int lowThr, int highThr, int& edgeNum, int *edgeX, int *edgeY)
{
	Mat src;

	//����ͼ��ͨ����
	int channel = imgSize/width/height + 0.5;

	//����ͼ��ͨ��������Ϊ��Ӧ��Mat����
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
	
	//Canny��Ե��ȡ
	Canny(src, src, lowThr, highThr);

	//��¼��Եλ��
	edgeNum = 0;
	for (int i=0;i<height;i++)
		for (int j=0;j<width;j++)
			if (src.data[i*width+j] == 255)
			{
				edgeX[edgeNum] = j;
				edgeY[edgeNum++] = i;
			}
}

//Sobel��Ե��ȡ�㷨
//width,height������ͼ��Ŀ�͸�
//imgSize������ͼ��������
//data������ͼ������
//ksize�ο�����Χ
//thr�㷨��ֵ,���ڸ��ݶ�ֵ��Ϊ��Ե,��thrΪ-1,����OTSU����Ӧ����ȷ��
//edgeNum���ؽ���ı�Ե����
//edgeX,edgeY���ؽ���ı�Ե����
void __declspec(dllexport) sobel(int width, int height, int imgSize, unsigned char *data, int ksize, int& thr, int& edgeNum, int *edgeX, int *edgeY)
{
	Mat src;

	//����ͼ��ͨ����
	int channel = imgSize/width/height + 0.5;

	//����ͼ��ͨ��������Ϊ��Ӧ��Mat����
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
	//����X��Y�����ݶȣ������кϳ�
	Sobel(src,sobel,src.depth(),1,0,ksize);
	Sobel(src,sobelY,src.depth(),0,1,ksize);
	for (int i=0;i<sobel.rows*sobel.cols;i++)
		sobel.data[i] = sqrt((sobel.data[i]*sobel.data[i]+sobelY.data[i]*sobelY.data[i])/2);

	//���thrΪ-1���������Ӧ�������������thr���ж�ֵ��
	if (thr == -1)
		thr = threshold(sobel,sobel,0,255,THRESH_BINARY | THRESH_OTSU);
	else
		threshold(sobel,sobel,thr,255,THRESH_BINARY);

	//��¼��Եλ��
	edgeNum = 0;
	for (int i=0;i<height;i++)
		for (int j=0;j<width;j++)
			if (sobel.data[i*width+j] == 255)
			{
				edgeX[edgeNum] = j;
				edgeY[edgeNum++] = i;
			}
}
