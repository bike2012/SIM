#include "cv.hpp"
#include "highgui.h"
#include <vector>
#include <iostream>
#include <fstream>

 using namespace cv;
 using namespace std;

 //#define CALIB //打开此宏则为标定，否则为矫正

 vector <vector<Point3f>> objectPoints;
 vector <vector<Point2f>> imagePoints;
 	 vector<Point3f> obj;


 bool cornerExtract(string filename, int *cornerX, int *cornerY, int boardWidth, int boardHeight)
{
	vector<Point2f> pos;
	for (int i=0;i<4;i++) pos.push_back(Point2f(cornerX[i],cornerY[i]));
	Mat gray = imread(filename,0);
	cornerSubPix(gray, pos, Size(3, 3), Size(-1, -1),
             TermCriteria(CV_TERMCRIT_EPS | CV_TERMCRIT_ITER, 30, 0.1));

	vector<Point2f> corners;

	vector<Point2f> edges[4];
	Size step((max(pos[0].x,max(pos[1].x,max(pos[2].x,pos[3].x)))-min(pos[0].x,max(pos[1].x,max(pos[2].x,pos[3].x))))/(boardWidth*2.0),(max(pos[0].y,max(pos[1].y,max(pos[2].y,pos[3].y)))-min(pos[0].y,max(pos[1].y,max(pos[2].y,pos[3].y))))/(boardHeight*2.0));
	step = Size(min(step.width,11),min(step.height,11));
	cout<<step.width<<'\t'<<step.height<<endl;
	for (int y=0;y<boardHeight;y++)
	{
		if (y==0)
		{
			edges[0].push_back(pos[0]);
			edges[1].push_back(pos[1]);
		}
		else if (y==boardHeight-1)
		{
			edges[0].push_back(pos[3]);
			edges[1].push_back(pos[2]);
		}
		else
		{
			vector<Point2f> temp;
			temp.push_back(Point2f(edges[0][y-1].x+(pos[3].x-edges[0][y-1].x)/(boardHeight-y),edges[0][y-1].y+(pos[3].y-edges[0][y-1].y)/(boardHeight-y)));
			temp.push_back(Point2f(edges[1][y-1].x+(pos[2].x-edges[1][y-1].x)/(boardHeight-y),edges[1][y-1].y+(pos[2].y-edges[1][y-1].y)/(boardHeight-y)));
			cornerSubPix(gray, temp, step, Size(-1, -1), TermCriteria(CV_TERMCRIT_EPS | CV_TERMCRIT_ITER, 30, 0.1));
			edges[0].push_back(temp[0]);
			edges[1].push_back(temp[1]);
		}
	}
	for (int x=0;x<boardWidth;x++)
	{
		if (x==0)
		{
			edges[2].push_back(pos[0]);
			edges[3].push_back(pos[3]);
		}
		else if (x==boardWidth-1)
		{
			edges[2].push_back(pos[1]);
			edges[3].push_back(pos[2]);
		}
		else
		{
			vector<Point2f> temp;
			temp.push_back(Point2f(edges[2][x-1].x+(pos[1].x-edges[2][x-1].x)/(boardWidth-x),edges[2][x-1].y+(pos[1].y-edges[2][x-1].y)/(boardWidth-x)));
			temp.push_back(Point2f(edges[3][x-1].x+(pos[2].x-edges[3][x-1].x)/(boardWidth-x),edges[3][x-1].y+(pos[2].y-edges[3][x-1].y)/(boardWidth-x)));
			cornerSubPix(gray, temp, step, Size(-1, -1), TermCriteria(CV_TERMCRIT_EPS | CV_TERMCRIT_ITER, 30, 0.1));
			edges[2].push_back(temp[0]);
			edges[3].push_back(temp[1]);
		}
	}


		for (int y=0;y<boardHeight;y++)
				for (int x=0;x<boardWidth;x++)
		{
			Point2f left(edges[0][y]);
			Point2f right(edges[1][y]);
			Point2f up(edges[2][x]);
			Point2f down(edges[3][x]);

			float a1,b1,a2,b2;
			bool vertical1=false,vertical2=false;
			if (fabs(right.x-left.x)>0.001)
			{
				a1 = (right.y-left.y)/(right.x-left.x);
				b1 = left.y-a1*left.x;
			}
			else
			{
				vertical1 = true;
				a1 = left.x;
			}

			if (fabs(down.x-up.x)>0.001)
			{
				a2 = (down.y-up.y)/(down.x-up.x);
				b2 = up.y-a2*up.x;
			}
			else
			{
				vertical2 = true;
				a2 = up.x;
			}

			if (vertical1 && vertical2) return false;
			if (vertical1)
				corners.push_back(Point2f(a1,a1*a2+b2));
			else if (vertical2)
				corners.push_back(Point2f(a2,a2*a1+b1));
			else if (fabs(a1-a2)<0.001) return false;
			else
				corners.push_back(Point2f((b1-b2)/(a2-a1),(b1-b2)/(a2-a1)*a1+b1));
		}
	cornerSubPix(gray, corners, step, Size(-1, -1), TermCriteria(CV_TERMCRIT_EPS | CV_TERMCRIT_ITER, 30, 0.1));
	//for (int i=0;i<corners.size();i++)
	//	cv::circle(gray,corners[i],3,Scalar(0));
	Mat img = imread(filename);
    drawChessboardCorners(img, Size(boardWidth,boardHeight), corners, true);
    imshow("corners", img);
    waitKey(1);
	imagePoints.push_back(corners);
	objectPoints.push_back(obj);
}

// void main(int argc,char *argv[])
// {
//
//	 #ifdef CALIB
// 
//	 //int board_w = 11,board_h=10;
//
//  //   for (int j=0; j<board_w*board_h;j++)
//  //   {
//  //       obj.push_back(Point3f(j/board_w*27, (j%board_w)*27, 0.0f));
//  //   }
//	 //int X[4],Y[4];
//
//	 //	 	 X[3] = 130;Y[3]=197;
//	 //X[0] = 235,Y[0]=70;
//	 //X[1] = 430;Y[1]= 231;
//	 //X[2] = 407;Y[2]= 404;
//	 //cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image5.tif",X,Y,11,10);
//
//	 //X[0] = 190;Y[0]=193;
//	 //X[1] = 416,Y[1]=207;
//	 //X[2] = 466;Y[2]= 363;
//	 //X[3] = 167;Y[3]= 354;
//	 //cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image1.tif",X,Y,11,10);
//	 //	 X[0] = 194;Y[0]=129;
//	 //X[1] = 445,Y[1]=146;
//	 //X[2] = 478;Y[2]= 360;
//	 //X[3] = 173;Y[3]= 350;
//	 //cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image2.tif",X,Y,11,10);
//	 //	 X[0] = 217;Y[0]=120;
//	 //X[1] = 471,Y[1]=163;
//	 //X[2] = 461;Y[2]= 383;
//	 //X[3] = 155;Y[3]= 332;
//	 //cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image3.tif",X,Y,11,10);
//	 //	 X[0] = 264;Y[0]=128;
//	 //X[1] = 533,Y[1]=108;
//	 //X[2] = 513;Y[2]= 357;
//	 //X[3] = 180;Y[3]= 333;
//	 //cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image4.tif",X,Y,11,10);
//
//	 //	 X[0] = 134;Y[0]=166;
//	 //X[1] = 522,Y[1]=201;
//	 //X[2] = 429;Y[2]= 399;
//	 //X[3] = 143;Y[3]= 384;
//	 //cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image6.tif",X,Y,11,10);
//
//	 //	 X[0] = 215;Y[0]=161;
//	 //X[1] = 473,Y[1]=123;
//	 //X[2] = 411;Y[2]= 332;
//	 //X[3] = 193;Y[3]= 421;
//	 //cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image7.tif",X,Y,11,10);
//
//	 //	 X[0] = 107;Y[0]=134;
//	 //X[1] = 366,Y[1]=92;
//	 //X[2] = 327;Y[2]= 303;
//	 //X[3] = 112;Y[3]= 394;
//	 //cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image8.tif",X,Y,11,10);
//
//
//
//	 //Mat CM=Mat::eye(3,3,CV_32FC1);
//  //   Mat D;
//	 //vector<Mat> rvecs, tvecs;
//
//  //   
//  //   calibrateCamera(objectPoints, imagePoints, Size(640,480), CM, D, rvecs, tvecs);
//	 //
//	 ////catch(Exception){}
//  //   //保存结果
//  //   FileStorage fs1("mycalib.yml", FileStorage::WRITE);
//  //   fs1 << "CM" << CM;
//  //   fs1 << "D" << D;
//
//  //   fs1.release();
//
//     int numBoards = 10; // 图像个数
//     int board_w = 6;   // 棋盘格X方向格子个数-1
//     int board_h = 4;   // 棋盘格Y方向格子个数-1
//
//     Size board_sz = Size(board_w, board_h);
//     int board_n = board_w*board_h;
//
//     //vector <vector<Point3f>> objectPoints;
//     //vector <vector<Point2f>> imagePoints;
//     vector <Point2f> corners;
//     
//     Mat img, gray;
//     int success = 0;
//     int k = 0;
//     bool found = false;
//
//     Size sz;
//
//     
//     vector<Point3f> obj;
//     for (int j=0; j<board_w*board_h;j++)
//     {
//         obj.push_back(Point3f(j/board_w*20, (j%board_w)*20, 0.0f));
//     }
//
//     //读入一个文件夹下图像
//     char *path = "calib_example\\";
//     int count = 0;
//     char numidx[10] = {0};
//     char filename[20] = {0};
//
//
//	 for (count=0; count<numBoards;count++)
//     {
//
//         memset(numidx,0,10);
//         itoa(count+1,numidx,10);
//
//         memset(filename,0,20);
//         strcat(filename,path);
//         strcat(filename,numidx);
//         strcat(filename,".tif");
//		 char filen[83] = "D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\image\\gray\\chess1.bmp";
//		 filen[77] = '0'+count;
//         img  = imread(filen);//"D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image15.tif");
//		 cout<<filen<<endl;
//		 imshow("img",img);
//		 //waitKey();
//         sz = Size(img.cols,img.rows);
//
//         if (img.empty())
//         {
//             continue;
//         }
//
//         cvtColor(img, gray, CV_BGR2GRAY);
//		 		 imshow("img",gray);
//		 //waitKey();
//         
//         found = findChessboardCorners(gray, board_sz, corners);
//            //CV_CALIB_CB_ADAPTIVE_THRESH | CV_CALIB_CB_FILTER_QUADS);
//         // returns bool if found or not
//
//         if (found)
//         {
//             imagePoints.push_back(corners);
//             objectPoints.push_back(obj);
//             printf ("Corners stored\n");
//             success++;
//         }
//
//		 cout<<found<<endl;
//		 cout<<corners.size()<<endl;
//		 for (int i=0;i<corners.size();i++)
//			 cout<<corners[i].x<<"\t"<<corners[i].y<<endl;
//         
//         cornerSubPix(gray, corners, Size(11, 11), Size(-1, -1),
//             TermCriteria(CV_TERMCRIT_EPS | CV_TERMCRIT_ITER, 30, 0.1));
//         drawChessboardCorners(img, board_sz, corners, found);
//         imshow("corners", img);
//         waitKey(1);
//
//         img.release();
//     }
//     
//
//	 Mat CM = Mat(3, 3, CV_32FC1,Scalar::all(0));
//     Mat D;// = Mat(1,5, CV_32FC1, Scalar::all(0));
//     vector<Mat> rvecs;
//	 vector<Mat> tvecs;
//
//
//     
//	 calibrateCamera(objectPoints, imagePoints, sz, CM, D, rvecs, tvecs);
//
//     //保存结果
//     FileStorage fs1("mycalib.yml", FileStorage::WRITE);
//     fs1 << "CM" << CM;
//     fs1 << "D" << D;
//
//     fs1.release();
// #else
//     //读入图像
//     //char *filename = "D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image5.tif";
//char *filename = "D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\image\\gray\\chess1.bmp";
//     Mat Img = imread(filename);
//     Mat ImgUndistort = Img.clone();
//     Mat CM = Mat(3, 3, CV_32FC1);
//     Mat D;
//
//
//     //导入相机内参和畸变系数矩阵
//     FileStorage fs2("mycalib.yml",FileStorage::READ);
//     fs2["CM"]>>CM;
//     fs2["D"]>>D;
//     fs2.release();
//
//     //矫正
//     undistort(Img,ImgUndistort,CM,D);
//
//     imshow("img",Img);
//     imshow("undistort",ImgUndistort);
//
//     waitKey();
//
//     Img.release();
//     ImgUndistort.release();
// #endif
// } 


bool __declspec(dllexport) manualExtract(char* filename, int &cornerNum, float *cornerX, float *cornerY, int boardWidth, int boardHeight, int manualFixSize, int fixSize)
{
	vector<Point2f> pos;
	for (int i=0;i<4;i++) pos.push_back(Point2f(cornerX[i],cornerY[i]));
	Mat gray = imread(filename,0);
	cornerSubPix(gray, pos, Size(manualFixSize, manualFixSize), Size(-1, -1), TermCriteria(CV_TERMCRIT_EPS | CV_TERMCRIT_ITER, 30, 0.1));

	vector<Point2f> corners;
	vector<Point2f> edges[4];
	Size step((max(pos[0].x,max(pos[1].x,max(pos[2].x,pos[3].x)))-min(pos[0].x,max(pos[1].x,max(pos[2].x,pos[3].x))))/(boardWidth*2.0),(max(pos[0].y,max(pos[1].y,max(pos[2].y,pos[3].y)))-min(pos[0].y,max(pos[1].y,max(pos[2].y,pos[3].y))))/(boardHeight*2.0));
	step = Size(min(step.width,fixSize),min(step.height,fixSize));
	//Size step(fixSize,fixSize);
	
	for (int y=0;y<boardHeight;y++)
	{
		if (y==0)
		{
			edges[0].push_back(pos[0]);
			edges[1].push_back(pos[1]);
		}
		else if (y==boardHeight-1)
		{
			edges[0].push_back(pos[3]);
			edges[1].push_back(pos[2]);
		}
		else
		{
			vector<Point2f> temp;
			temp.push_back(Point2f(edges[0][y-1].x+(pos[3].x-edges[0][y-1].x)/(boardHeight-y),edges[0][y-1].y+(pos[3].y-edges[0][y-1].y)/(boardHeight-y)));
			temp.push_back(Point2f(edges[1][y-1].x+(pos[2].x-edges[1][y-1].x)/(boardHeight-y),edges[1][y-1].y+(pos[2].y-edges[1][y-1].y)/(boardHeight-y)));
			cornerSubPix(gray, temp, step, Size(-1, -1), TermCriteria(CV_TERMCRIT_EPS | CV_TERMCRIT_ITER, 30, 0.1));
			edges[0].push_back(temp[0]);
			edges[1].push_back(temp[1]);
		}
	}
	for (int x=0;x<boardWidth;x++)
	{
		if (x==0)
		{
			edges[2].push_back(pos[0]);
			edges[3].push_back(pos[3]);
		}
		else if (x==boardWidth-1)
		{
			edges[2].push_back(pos[1]);
			edges[3].push_back(pos[2]);
		}
		else
		{
			vector<Point2f> temp;
			temp.push_back(Point2f(edges[2][x-1].x+(pos[1].x-edges[2][x-1].x)/(boardWidth-x),edges[2][x-1].y+(pos[1].y-edges[2][x-1].y)/(boardWidth-x)));
			temp.push_back(Point2f(edges[3][x-1].x+(pos[2].x-edges[3][x-1].x)/(boardWidth-x),edges[3][x-1].y+(pos[2].y-edges[3][x-1].y)/(boardWidth-x)));
			cornerSubPix(gray, temp, step, Size(-1, -1), TermCriteria(CV_TERMCRIT_EPS | CV_TERMCRIT_ITER, 30, 0.1));
			edges[2].push_back(temp[0]);
			edges[3].push_back(temp[1]);
		}
	}


	for (int y=0;y<boardHeight;y++)
		for (int x=0;x<boardWidth;x++)
		{
			Point2f left(edges[0][y]);
			Point2f right(edges[1][y]);
			Point2f up(edges[2][x]);
			Point2f down(edges[3][x]);

			float a1,b1,a2,b2;
			bool vertical1=false,vertical2=false;
			if (fabs(right.x-left.x)>0.001)
			{
				a1 = (right.y-left.y)/(right.x-left.x);
				b1 = left.y-a1*left.x;
			}
			else
			{
				vertical1 = true;
				a1 = left.x;
			}

			if (fabs(down.x-up.x)>0.001)
			{
				a2 = (down.y-up.y)/(down.x-up.x);
				b2 = up.y-a2*up.x;
			}
			else
			{
				vertical2 = true;
				a2 = up.x;
			}

			if (vertical1 && vertical2) return false;
			if (vertical1)
				corners.push_back(Point2f(a1,a1*a2+b2));
			else if (vertical2)
				corners.push_back(Point2f(a2,a2*a1+b1));
			else if (fabs(a1-a2)<0.001) return false;
			else
				corners.push_back(Point2f((b1-b2)/(a2-a1),(b1-b2)/(a2-a1)*a1+b1));
		}
	cornerSubPix(gray, corners, step, Size(-1, -1), TermCriteria(CV_TERMCRIT_EPS | CV_TERMCRIT_ITER, 30, 0.1));
	cornerNum = corners.size();
	for (int i=0;i<cornerNum;i++)
	{
		cornerX[i] = corners[i].x;
		cornerY[i] = corners[i].y;
	}
	return true;
}

bool __declspec(dllexport) autoExtract(char* filename, int &cornerNum, float *cornerX, float *cornerY, int boardWidth, int boardHeight, int fixSize)
{
	Mat gray  = imread(filename,0);
	Size size = Size(gray.cols,gray.rows);
	vector <Point2f> corners;
	bool found = findChessboardCorners(gray, Size(boardWidth, boardHeight), corners);

	if (found && corners.size() == boardWidth*boardHeight)
	{
		cornerSubPix(gray, corners, Size(fixSize, fixSize), Size(-1, -1),	TermCriteria(CV_TERMCRIT_EPS | CV_TERMCRIT_ITER, 30, 0.1));
		cornerNum = corners.size();
		for (int i=0;i<cornerNum;i++)
		{
			cornerX[i] = corners[i].x;
			cornerY[i] = corners[i].y;
		}
		return true;
	}
	else 
		return false;
}

void __declspec(dllexport) runCalibrate(int imgNum, int imgWidth, int imgHeight, int cornerNum, float *cornerX, float *cornerY, int boardWidth, int boardHeight, float boardSize, int &resultNum, float *result)
{
	vector<vector<Point3f>> objectPoints;
	vector<vector<Point2f>> imagePoints;
	vector<Point3f> obj;

	for (int i=0;i<boardWidth*boardHeight;i++)
		obj.push_back(Point3f(i/boardWidth*boardSize, (i%boardWidth)*boardSize, 0.0f));
	
	for (int i=0;i<imgNum;i++)
	{
		vector<Point2f> corners;
		for (int j=0;j<boardWidth*boardHeight;j++)
			corners.push_back(Point2f(cornerX[i*boardWidth*boardHeight+j],cornerY[i*boardWidth*boardHeight+j]));
		objectPoints.push_back(obj);
		imagePoints.push_back(corners);
	}

	Mat CM = Mat(3, 3, CV_32FC1,Scalar::all(0));
	Mat D;
	vector<Mat> rvecs, tvecs;

	cv::calibrateCamera(objectPoints, imagePoints, Size(imgWidth,imgHeight), CM, D, rvecs, tvecs);

	resultNum = CM.size().height*CM.size().width + D.size().height*D.size().width;
	for (int i=0;i<9;i++) result[i] = CM.at<double>(i/3,i%3);
	for (int i=9;i<resultNum;i++) result[i] = D.at<double>(0,i-9);
}

void __declspec(dllexport) runUndistort(char* filename, int size, unsigned char *data, int paraNum, float *para, int &channel)
{
	Mat CM = Mat(3, 3, CV_32FC1);
	for (int i=0;i<9;i++)
		CM.at<float>(i/3,i%3) = para[i];
	Mat D = Mat(1,paraNum-9,CV_32FC1);
	for (int i=0;i<paraNum-9;i++)
		D.at<float>(0,i) = para[i+9];

	Mat img = imread(filename);
	Mat imgUndistort = img.clone();

	undistort(img, imgUndistort, CM, D);

	for (int i=0;i<imgUndistort.cols*imgUndistort.rows*imgUndistort.channels();i++)
		data[i] = imgUndistort.data[i];	

	channel = imgUndistort.channels();
}

double zDistance(Point2f pt1, Point2f pt2)
{
	return sqrt((pt1.x-pt2.x)*(pt1.x-pt2.x)+(pt1.y-pt2.y)*(pt1.y-pt2.y));
}

Point2f zTransform(vector<Point2f> image, vector<Point2f> object, int boardWidth, int boardHeight, Point2f pt)
{
	Point2f img[4];Point2f obj[4];

	double min = zDistance(image[0],pt);
	int minTag = 0;
	for (int i=1;i<image.size();i++)
		if (zDistance(image[i],pt)<min)
		{
			min = zDistance(image[i],pt);
			minTag = i;
		}
	img[0] = image[minTag];obj[0] = object[minTag];

	int x = minTag&boardWidth;
	int y = minTag/boardWidth;

	min = 1000000;
	int minTagX,minTagY;
	for (int i=-1;i<=1;i++)
		for (int j=-1;j<=1;j++)
			if (i*j==0 && i+j!=0)
				if (y+j>=0 && y+j<boardHeight && x+i>=0 && x+i<boardWidth)
					if (zDistance(image[(y+j)*boardWidth+(x+i)],pt)<min)
					{
						min = zDistance(image[(y+j)*boardWidth+(x+i)],pt);
						minTagX = x+i;
						minTagY = y+j;
					}
	img[1] = image[minTagY*boardWidth+minTagX];obj[1] = object[minTagY*boardWidth+minTagX];

	min = 1000000;
	int minTagX2,minTagY2;
	for (int i=-1;i<=1;i++)
		for (int j=-1;j<=1;j++)
			if (i*j==0 && i+j!=0)
				if (y+j>=0 && y+j<boardHeight && x+i>=0 && x+i<boardWidth && y+j!=minTagY && x+i!=minTagX)
					if (zDistance(image[(y+j)*boardWidth+(x+i)],pt)<min)
					{
						min = zDistance(image[(y+j)*boardWidth+(x+i)],pt);
						minTagX2 = x+i;
						minTagY2 = y+j;
					}
	img[2] = image[minTagY2*boardWidth+minTagX2];obj[2] = object[minTagY2*boardWidth+minTagX2];

	int minTagX3,minTagY3;
	if (minTagX!=x)
	{
		if (minTagX2!=x)
			minTagX3 = x;
		else
			minTagX3 = minTagX;
	}
	else minTagX3 = minTagX2;
	if (minTagY!=y)
	{
		if (minTagY2!=y)
			minTagY3 = y;
		else
			minTagY3 = minTagY;
	}
	else minTagY3 = minTagY2;

	img[3] = image[minTagY3*boardWidth+minTagX3];obj[3] = object[minTagY3*boardWidth+minTagX3];

	Mat transform = getPerspectiveTransform(img,obj);
	Mat ptM(3,1,CV_64FC1);
	ptM.at<double>(0,0) = pt.x;
	ptM.at<double>(1,0) = pt.y;
	ptM.at<double>(2,0) = 1;
	Mat ptDst;
	gemm(transform,ptM,1,NULL,0,ptDst);
	return Point2f(ptDst.at<double>(0,0)/ptDst.at<double>(2,0),ptDst.at<double>(1,0)/ptDst.at<double>(2,0));

}

float __declspec(dllexport) zAffineTrans(int cornerNum, float *cornerX, float *cornerY, int boardWidth, int boardHeight, float boardSize, float ptX1, float ptY1, float ptX2, float ptY2)
{
	vector<Point2f> image;
	vector<Point2f> object;
	
	for (int i=0;i<cornerNum;i++)
	{
		image.push_back(Point2f(cornerX[i],cornerY[i]));
		object.push_back(Point2f((i/boardWidth)*boardSize,(i%boardWidth)*boardSize));
	}

	Point2f pt1(ptX1,ptY1);
	Point2f pt2(ptX2,ptY2);

	pt1 = zTransform(image,object,boardWidth,boardHeight,pt1);
	pt2 = zTransform(image,object,boardWidth,boardHeight,pt2);
	
	return sqrt((pt1.x-pt2.x)*(pt1.x-pt2.x)+(pt1.y-pt2.y)*(pt1.y-pt2.y));
}

void main()
{
	int cornerNum = 24;
	float cornerX[24],cornerY[24];
	cornerX[0] = 0;cornerY[0] = 0;
	cornerX[1] = 0;cornerY[1] = 100;
	cornerX[2] = 100;cornerY[2] = 0;
	cornerX[3] = 100;cornerY[3] = 100;

			cornerX[0]=	204.49408;
		cornerX[1]=	254.9529;
		cornerX[2]=	305.604218;
		cornerX[3]=	356.629822	;
		cornerX[4]=	407.908478	;
		cornerX[5]=	459.2493	;
		cornerX[6]=	209.50206	;
		cornerX[7]=	259.387756	;
		cornerX[8]=	309.575134	;
		cornerX[9]=	360.1398	;
		cornerX[10]=	410.679443	;
		cornerX[11]=	461.582642	;
		cornerX[12]=	214.553574	;
		cornerX[13]=	263.8717	;
		cornerX[14]=	313.60556	;
		cornerX[15]=	363.44223	;
		cornerX[16]=	413.650269	;
		cornerX[17]=	463.867645	;
		cornerX[18]=	219.4158	;
		cornerX[19]=	268.329132	;
		cornerX[20]=	317.448883	;
		cornerX[21]=	366.8848	;
		cornerX[22]=	416.521576	;
		cornerX[23]=	466.196136	;

				cornerY[0]=	166.690109	;
		cornerY[1]=	163.982773	;
		cornerY[2]=	160.572372	;
		cornerY[3]=	157.853745	;
		cornerY[4]=	154.460938	;
		cornerY[5]=	151.580276	;
		cornerY[6]=	215.321014	;
		cornerY[7]=	212.467239	;
		cornerY[8]=	209.5421	;
		cornerY[9]=	206.448914	;
		cornerY[10]=	204.069229	;
		cornerY[11]=	200.6209	;
		cornerY[12]=	263.290222	;
		cornerY[13]=	260.038055	;
		cornerY[14]=	257.485565	;
		cornerY[15]=	254.677368	;
		cornerY[16]=	251.781265	;
		cornerY[17]=	249.074677	;
		cornerY[18]=	309.8268	;
		cornerY[19]=	307.1212	;
		cornerY[20]=	304.219238	;
		cornerY[21]=	301.572968	;
		cornerY[22]=	298.703	;
		cornerY[23]=	296.2295	;


	cout<<zAffineTrans(cornerNum,cornerX,cornerY,6,4,20,149.14,120.71,176,357);
	cin>>cornerNum;
}