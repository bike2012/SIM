#include "cv.hpp"
#include "highgui.h"
#include <vector>
#include <iostream>

 using namespace cv;
 using namespace std;

 #define CALIB //打开此宏则为标定，否则为矫正

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

 void main(int argc,char *argv[])
 {

	 
	 int board_w = 11,board_h=10;

     for (int j=0; j<board_w*board_h;j++)
     {
         obj.push_back(Point3f(j/board_w*27, (j%board_w)*27, 0.0f));
     }
	 int X[4],Y[4];

	 	 	 X[3] = 130;Y[3]=197;
	 X[0] = 235,Y[0]=70;
	 X[1] = 430;Y[1]= 231;
	 X[2] = 407;Y[2]= 404;
	 cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image5.tif",X,Y,11,10);

	 X[0] = 190;Y[0]=193;
	 X[1] = 416,Y[1]=207;
	 X[2] = 466;Y[2]= 363;
	 X[3] = 167;Y[3]= 354;
	 cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image1.tif",X,Y,11,10);
	 	 X[0] = 194;Y[0]=129;
	 X[1] = 445,Y[1]=146;
	 X[2] = 478;Y[2]= 360;
	 X[3] = 173;Y[3]= 350;
	 cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image2.tif",X,Y,11,10);
	 	 X[0] = 217;Y[0]=120;
	 X[1] = 471,Y[1]=163;
	 X[2] = 461;Y[2]= 383;
	 X[3] = 155;Y[3]= 332;
	 cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image3.tif",X,Y,11,10);
	 	 X[0] = 264;Y[0]=128;
	 X[1] = 533,Y[1]=108;
	 X[2] = 513;Y[2]= 357;
	 X[3] = 180;Y[3]= 333;
	 cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image4.tif",X,Y,11,10);

	 	 X[0] = 134;Y[0]=166;
	 X[1] = 522,Y[1]=201;
	 X[2] = 429;Y[2]= 399;
	 X[3] = 143;Y[3]= 384;
	 cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image6.tif",X,Y,11,10);

	 	 X[0] = 215;Y[0]=161;
	 X[1] = 473,Y[1]=123;
	 X[2] = 411;Y[2]= 332;
	 X[3] = 193;Y[3]= 421;
	 cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image7.tif",X,Y,11,10);

	 	 X[0] = 107;Y[0]=134;
	 X[1] = 366,Y[1]=92;
	 X[2] = 327;Y[2]= 303;
	 X[3] = 112;Y[3]= 394;
	 cornerExtract("D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image8.tif",X,Y,11,10);



	 Mat CM=Mat::eye(3,3,CV_32FC1);
     Mat D;
	 vector<Mat> rvecs, tvecs;

     
     calibrateCamera(objectPoints, imagePoints, Size(640,480), CM, D, rvecs, tvecs);
	 
	 //catch(Exception){}
     //保存结果
     FileStorage fs1("mycalib.yml", FileStorage::WRITE);
     fs1 << "CM" << CM;
     fs1 << "D" << D;

     fs1.release();

 #ifdef CALIB
  //   int numBoards = 10; // 图像个数
  //   int board_w = 6;   // 棋盘格X方向格子个数-1
  //   int board_h = 4;   // 棋盘格Y方向格子个数-1

  //   Size board_sz = Size(board_w, board_h);
  //   int board_n = board_w*board_h;

  //   //vector <vector<Point3f>> objectPoints;
  //   //vector <vector<Point2f>> imagePoints;
  //   vector <Point2f> corners;
  //   
  //   Mat img, gray;
  //   int success = 0;
  //   int k = 0;
  //   bool found = false;

  //   Size sz;

  //   
  //   vector<Point3f> obj;
  //   for (int j=0; j<board_w*board_h;j++)
  //   {
  //       obj.push_back(Point3f(j/board_w*20, (j%board_w)*20, 0.0f));
  //   }

  //   //读入一个文件夹下图像
  //   char *path = "calib_example\\";
  //   int count = 0;
  //   char numidx[10] = {0};
  //   char filename[20] = {0};


	 //for (count=0; count<numBoards;count++)
  //   {

  //       memset(numidx,0,10);
  //       itoa(count+1,numidx,10);

  //       memset(filename,0,20);
  //       strcat(filename,path);
  //       strcat(filename,numidx);
  //       strcat(filename,".tif");
		// char filen[83] = "D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\image\\gray\\chess1.bmp";
		// filen[77] = '0'+count;
  //       img  = imread(filen);//"D:\\test\\SIM_Profect_DLLs\\CameraCalibration\\CameraCalibration\\calib_example\\Image15.tif");
		// cout<<filen<<endl;
		// imshow("img",img);
		// //waitKey();
  //       sz = Size(img.cols,img.rows);

  //       if (img.empty())
  //       {
  //           continue;
  //       }

  //       cvtColor(img, gray, CV_BGR2GRAY);
		// 		 imshow("img",gray);
		// //waitKey();
  //       
  //       found = findChessboardCorners(gray, board_sz, corners);
  //          //CV_CALIB_CB_ADAPTIVE_THRESH | CV_CALIB_CB_FILTER_QUADS);
  //       // returns bool if found or not

  //       if (found)
  //       {
  //           imagePoints.push_back(corners);
  //           objectPoints.push_back(obj);
  //           printf ("Corners stored\n");
  //           success++;
  //       }

		// cout<<found<<endl;
		// cout<<corners.size()<<endl;
		// for (int i=0;i<corners.size();i++)
		//	 cout<<corners[i].x<<"\t"<<corners[i].y<<endl;
  //       
  //       cornerSubPix(gray, corners, Size(11, 11), Size(-1, -1),
  //           TermCriteria(CV_TERMCRIT_EPS | CV_TERMCRIT_ITER, 30, 0.1));
  //       drawChessboardCorners(img, board_sz, corners, found);
  //       imshow("corners", img);
  //       waitKey(1);

  //       img.release();
  //   }
  //   

	 //Mat CM = Mat(3, 3, CV_32FC1,Scalar::all(0));
  //   Mat D;// = Mat(1,5, CV_32FC1, Scalar::all(0));
  //   vector<Mat> rvecs;
	 //vector<Mat> tvecs;


  //   
	 //calibrateCamera(objectPoints, imagePoints, sz, CM, D, rvecs, tvecs);

  //   //保存结果
  //   FileStorage fs1("mycalib.yml", FileStorage::WRITE);
  //   fs1 << "CM" << CM;
  //   fs1 << "D" << D;

  //   fs1.release();
 #else
     //读入图像
     char *filename = "Imgs\\1.jpg";
     Mat Img = imread(filename);
     Mat ImgUndistort = Img.clone();
     Mat CM = Mat(3, 3, CV_32FC1);
     Mat D;


     //导入相机内参和畸变系数矩阵
     FileStorage fs2("mycalib.yml",FileStorage::READ);
     fs2["CM"]>>CM;
     fs2["D"]>>D;
     fs2.release();

     //矫正
     undistort(Img,ImgUndistort,CM,D);

     imshow("img",Img);
     imshow("undistort",ImgUndistort);

     waitKey(0);

     Img.release();
     ImgUndistort.release();
 #endif
 } 