#include <opencv2/opencv.hpp>
#include <iostream>
#include <fstream>
#include <zbar.h>

using namespace cv;
using namespace std;
using namespace zbar;

int __declspec(dllexport) decode(char* filename, int type, int& codeNum, int maxSize, int *firlength, int *seclength, char* fir, char* sec, int rotate){
		int maxLength = maxSize/codeNum;
	int maxCodeNum = codeNum;
	codeNum = 0;
	
	Mat srcImage;
	srcImage = imread(filename);

	Mat grayPart;
	if (srcImage.channels() == 3)
	{
		cvtColor(srcImage, grayPart, CV_RGB2GRAY);
	}
	else
	{
		grayPart = srcImage.clone();
	}

	/*Mat show;
	resize(grayPart,show,Size(),0.5,0.5);
	imshow("grayPart2",show);

	float k[9]={-1.0, -1.0, -1.0,   
		-1.0, 9 , -1.0,  
		-1.0, -1.0, -1.0}; 
	Mat kernel(3,3,CV_32FC1,k);*/
	//filter2D(grayPart,grayPart,grayPart.depth(),kernel);
	//grayPart.convertTo(grayPart,-1,2);
	
	//filter2D(grayPart,grayPart,grayPart.depth(),kernel);
	//threshold(grayPart,grayPart,130,255,CV_THRESH_OTSU);

	//Mat show;
	//resize(grayPart,show,Size(),0.5,0.5);
	//imshow("grayPart",show);
	//waitKey();

	int minAngle,maxAngle;
	if (rotate>0)
	{
		minAngle = -30;maxAngle = 30;
	}
	else
	{
		minAngle = 0;maxAngle = 0;
	}


	for (int angle = minAngle;angle<=maxAngle;angle+=30)
	{
		Mat rotatedGrayPart;
		Mat rotateMat = getRotationMatrix2D(Point2f(grayPart.cols/2,grayPart.rows/2),angle,1);

		if (angle!=0)
		warpAffine(grayPart,rotatedGrayPart,rotateMat ,grayPart.size() );  

		int col = rotatedGrayPart.cols;   // extract dimensions
		int row = rotatedGrayPart.rows;
		
		Image image(col,row, "Y800", rotatedGrayPart.data, col*row);

		ImageScanner scanner;

		scanner.set_config(ZBAR_NONE, ZBAR_CFG_ENABLE, 1);

		int n = scanner.scan(image);
		cout<<n<<'\t';

		for (Image::SymbolIterator symbol = image.symbol_begin(); symbol != image.symbol_end(); ++symbol)
		{
			if (codeNum>=maxCodeNum) return n;
			bool exist = false;
			for (int i=0;i<codeNum;i++)
			{
				bool same = true;
				if (seclength[i] != symbol->get_data().length())
					continue;
				for (int j = 0;j<seclength[i];j++)
					if (symbol->get_data()[j] != sec[maxLength*i+j])
					{
						same = false;
						break;
					}
				if (same)
					exist = true;
			}

			if (!exist)
			{
			firlength[codeNum] = symbol->get_type_name().length();
			seclength[codeNum] = symbol->get_data().length();
			if (firlength[codeNum]>maxLength)
				firlength[codeNum] = maxLength;
			if (seclength[codeNum]>maxLength)
				seclength[codeNum] = maxLength;
			for (int i=0;i<firlength[codeNum];i++)
				fir[maxLength*codeNum+i] = symbol->get_type_name()[i];
			for (int i=0;i<seclength[codeNum];i++)
				sec[maxLength*codeNum+i] = symbol->get_data()[i];
			codeNum++;
			}
			image.set_data(NULL, 0);
		}
	}
	cout<<codeNum<<endl;
	return codeNum;
}

bool __declspec(dllexport) decodeROI(char* filename, int type, int x, int y, int width, int height, int &firlength, int &seclength, char* fir, char* sec){
	Mat srcImage, grayImage;
	srcImage = imread(filename);
	
	if (srcImage.channels() == 3)
	{
		cvtColor(srcImage, grayImage, CV_RGB2GRAY);
	}
	else
	{
		grayImage = srcImage.clone();
	}
	
	Rect target;
	target.x = x;
	target.y = y;
	target.width = width;
	target.height = height;
	Mat grayPart = grayImage(target);

	//Mat grayPart;
	//resize(grayImage,grayPart,Size(width,height));
	//for (int i=0;i<height;i++)
	//	for (int j=0;j<width;j++)
	//		grayPart.data[i*width+j] = grayImage.data[(i+y)*grayImage.cols+(j+x)];
	//imshow("123",grayPart);
	//waitKey();

	int col = grayPart.cols;   // extract dimensions
	int row = grayPart.rows;
	Image image(col,row, "Y800", grayPart.data, col*row);
	ImageScanner scanner;
	scanner.set_config(ZBAR_NONE, ZBAR_CFG_ENABLE, 1);

	int n = scanner.scan(image);

	for (Image::SymbolIterator symbol = image.symbol_begin(); symbol != image.symbol_end(); ++symbol)
	{
		firlength = symbol->get_type_name().length();
		seclength = symbol->get_data().length();
		for (int i=0;i<firlength;i++)
			fir[i] = symbol->get_type_name()[i];
		for (int i=0;i<seclength;i++)
			sec[i] = symbol->get_data()[i];
		image.set_data(NULL, 0);
		if (n==0) return false;
		else return true;
	}
	return false;
}

void main()
{
	int a[100],b[100];
	char* aa = new char[100000];
	char* bb = new char[100000];
	
	int n = 100;
	decode("part.bmp",2,n, n*1000, a,b,aa,bb,1);
	cout<<a<<'\t'<<b<<endl;
	cout<<aa<<endl<<bb<<endl;
	cin>>n;
}