#include <cv.hpp>

#include <iostream>
#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/legacy/legacy.hpp>
#include <opencv2/nonfree/features2d.hpp>

#include <vector>
#include <algorithm>




using namespace std;
using namespace cv;

bool deleteErrors(vector<DMatch> &matches, vector<KeyPoint> points)
{
	float centerX = 0, centerY = 0;
	int size = matches.size();
	for (int i=0;i<matches.size();i++)
	{
		float x = points[matches[i].trainIdx].pt.x;
		float y = points[matches[i].trainIdx].pt.y;

		centerX+=x;centerY+=y;
	}
	centerX/=matches.size();centerY/=matches.size();

	float avgDistX = 0, avgDistY = 0;
	for (int i=0;i<size;i++)
	{
		float x = points[matches[i].trainIdx].pt.x;
		float y = points[matches[i].trainIdx].pt.y;

		avgDistX+=fabs(x-centerX);
		avgDistY+=fabs(y-centerY);
	}
	avgDistX/=size;
	avgDistY/=size;

	for (int i=size-1;i>=0;i--)
	{
		float x = points[matches[i].trainIdx].pt.x;
		float y = points[matches[i].trainIdx].pt.y;

		if (fabs(x-centerX)>avgDistX*2 || fabs(y-centerY)>avgDistY*2)
			matches.erase(matches.begin()+i);
	}

	return (size==matches.size())?true:false;
}


bool deleteErrors2(vector<Mat> &mats)
{
	float centerP1 = 0, centerP2 = 0, centerP3 = 0, centerP4 = 0, centerP5 = 0, centerP6 = 0;
	int size = mats.size();
	for (int i=0;i<mats.size();i++)
	{
		float p1 = mats[i].at<double>(0,0);
		float p2 = mats[i].at<double>(0,1);
		float p3 = mats[i].at<double>(0,2);
		float p4 = mats[i].at<double>(1,0);
		float p5 = mats[i].at<double>(1,1);
		float p6 = mats[i].at<double>(1,2);

		centerP1+=p1;centerP2+=p2;centerP3+=p3;centerP4+=p4;centerP5+=p5;centerP6+=p6;
	}
	centerP1/=mats.size();centerP2/=mats.size();centerP3/=mats.size();centerP4/=mats.size();centerP5/=mats.size();centerP6/=mats.size();

	float avgDistP1 = 0, avgDistP2 = 0, avgDistP3 = 0, avgDistP4 = 0, avgDistP5 = 0, avgDistP6 = 0;
	for (int i=0;i<size;i++)
	{
		float p1 = mats[i].at<double>(0,0);
		float p2 = mats[i].at<double>(0,1);
		float p3 = mats[i].at<double>(0,2);
		float p4 = mats[i].at<double>(1,0);
		float p5 = mats[i].at<double>(1,1);
		float p6 = mats[i].at<double>(1,2);

		avgDistP1+=fabs(p1-centerP1);avgDistP2+=fabs(p2-centerP2);avgDistP3+=fabs(p3-centerP3);avgDistP4+=fabs(p4-centerP4);avgDistP5+=fabs(p5-centerP5);avgDistP6+=fabs(p6-centerP6);
	}
	avgDistP1/=size;avgDistP2/=size;avgDistP3/=size;avgDistP4/=size;avgDistP5/=size;avgDistP6/=size;

	for (int i=size-1;i>=0;i--)
	{
		float p1 = mats[i].at<double>(0,0);
		float p2 = mats[i].at<double>(0,1);
		float p3 = mats[i].at<double>(0,2);
		float p4 = mats[i].at<double>(1,0);
		float p5 = mats[i].at<double>(1,1);
		float p6 = mats[i].at<double>(1,2);

		if (fabs(p1-centerP1)>avgDistP1*2 || fabs(p2-centerP2)>avgDistP2*2 ||fabs(p3-centerP3)>avgDistP3*2 || fabs(p4-centerP4)>avgDistP4*2 || fabs(p5-centerP5)>avgDistP5*2 || fabs(p6-centerP6)>avgDistP6*2)
			mats.erase(mats.begin()+i);
	}

	return (size==mats.size())?true:false;
}

class ptStruct
{
public:
	Point pt;
	float data;
	ptStruct(){}
	ptStruct(int x, int y, float indata)
	{
		pt.x = x;
		pt.y = y;
		data = indata;
	}

	 bool operator < (const ptStruct &a) const
	 {
		 if (data>a.data) return true;
		 else return false;
	 }

};


void simpleMethod(Mat temp, Mat obj, int &resultNum, float *X1, float *Y1, float *X2, float *Y2, float *Alpha)
{
	Mat result;
	matchTemplate(obj,temp,result,CV_TM_CCOEFF_NORMED);

	float max = 0;
	for (int i=0;i<result.rows;i++)
		for (int j=0;j<result.cols;j++)
		{
			float data = *(((float*)result.data)+i*result.cols+j);
			if (data>max) max = data;
		}

	Vector<ptStruct> pts;
	for (int i=0;i<result.rows;i++)
		for (int j=0;j<result.cols;j++)
		{
			float data = *(((float*)result.data)+i*result.cols+j);
			if (data>max*0.8)
				pts.push_back(ptStruct(j,i,data));
		}

	sort(pts.begin(),pts.end());

	Vector<ptStruct> results;
	for (int i=0;i<pts.size();i++)
	{
		//cout<<pts[i].data<<'\t';
		bool isOK = true;
		for (int j=0;j<results.size();j++)
			if (abs(pts[i].pt.x-results[j].pt.x)<=temp.cols && abs(pts[i].pt.y-results[j].pt.y)<=temp.rows)
			{
				isOK = false;
				break;
			}
		if (isOK)
			results.push_back(pts[i]);
	}

	resultNum = results.size();
	for (int i=0;i<results.size();i++)
	{
		X1[i] = results[i].pt.x+temp.cols/2;
		Y1[i] = results[i].pt.y+temp.rows/2;
		X2[i] = results[i].pt.x+temp.cols;
		Y2[i] = results[i].pt.y+temp.rows;
		Alpha[i] = 0;
	}
}


void __declspec(dllexport) templateMatchDLL(char* tempfilename, char* objfilename, int &resultNum, float *X1, float *Y1, float *X2, float *Y2, float *Alpha)
{
    Mat temp=imread(tempfilename);
	Rect ROI;

	ROI.x = min(X1[0],X2[0])-abs(X1[0]-X2[0]);ROI.y=min(Y1[0],Y2[0])-abs(Y1[0]-Y2[0]);
	ROI.width=2*abs(X1[0]-X2[0]);ROI.height=2*abs(Y1[0]-Y2[0]);
	temp = Mat(temp,ROI);
    Mat obj=imread(objfilename);
    
	// 检测surf特征点
    vector<KeyPoint> keypoints1,keypoints2;     
	
    SurfFeatureDetector detector(400);
    detector.detect(temp, keypoints1);
    detector.detect(obj, keypoints2);
  	
	if (keypoints1.size()<5)
	{
		simpleMethod(temp,obj,resultNum,X1,Y1,X2,Y2,Alpha);
		
		return;
	}

    // 描述surf特征点
    SurfDescriptorExtractor surfDesc;
    Mat descriptros1,descriptros2;
    surfDesc.compute(temp,keypoints1,descriptros1);
    surfDesc.compute(obj,keypoints2,descriptros2);

    // 计算匹配点数
    BruteForceMatcher<L2<float>>matcher;
    vector<DMatch> matches;
    matcher.match(descriptros1,descriptros2,matches);

	while (!deleteErrors(matches,keypoints2)){}

	if (matches.size()>10)
	{
		std::nth_element(matches.begin(),matches.begin()+8,matches.end());
		matches.erase(matches.begin()+10,matches.end());
	}
	 
	vector<Mat> results;
	for (int a1 = 0;a1<matches.size()-3;a1++)
		for (int a2 = a1+1;a2<matches.size()-2;a2++)
			for (int a3 = a2+1;a3<matches.size()-1;a3++)
				for (int a4 = a3+1;a4<matches.size();a4++)
				{
					vector<Point2f> matchpoints1,matchpoints2;
					matchpoints1.push_back(keypoints1[matches[a1].queryIdx].pt);
					matchpoints2.push_back(keypoints2[matches[a1].trainIdx].pt);
					matchpoints1.push_back(keypoints1[matches[a2].queryIdx].pt);
					matchpoints2.push_back(keypoints2[matches[a2].trainIdx].pt);
					matchpoints1.push_back(keypoints1[matches[a3].queryIdx].pt);
					matchpoints2.push_back(keypoints2[matches[a3].trainIdx].pt);
					matchpoints1.push_back(keypoints1[matches[a4].queryIdx].pt);
					matchpoints2.push_back(keypoints2[matches[a4].trainIdx].pt);
					Mat result = cv::estimateRigidTransform(matchpoints1,matchpoints2,false);
					if (result.rows!=0)
						results.push_back(result);
				}
	
	while (!deleteErrors2(results)){}

	float p1 = 0,p2=0,p3=0,p4=0,p5=0,p6=0;
	for (int i=0;i<results.size();i++)
	{
		p1+=results[i].at<double>(0,0);
		p2+=results[i].at<double>(0,1);
		p3+=results[i].at<double>(0,2);
		p4+=results[i].at<double>(1,0);
		p5+=results[i].at<double>(1,1);
		p6+=results[i].at<double>(1,2);
	}
	
	p1/=results.size();p2/=results.size();p3/=results.size();p4/=results.size();p5/=results.size();p6/=results.size();

	resultNum = 1;
	float scale = p1*p1+p2*p2;
	Alpha[0] = acos(p1/scale);
	if (p2<0)
	{
		Alpha[0] = CV_PI*2-Alpha[0];
	}
	Alpha[0] = -Alpha[0];
	if (Alpha[0]<0) Alpha[0]+=CV_PI*2;

	X1[0] = temp.cols/2*p1+temp.rows/2*p2+p3;
	Y1[0] = temp.cols/2*p4+temp.rows/2*p5+p6;
	X2[0] = temp.cols*p1+temp.rows*p2+p3;
	Y2[0] = temp.cols*p4+temp.rows*p5+p6;
 
}





//int main2()
//{
//	Mat k=imread("template.bmp",0);
//	Mat f;
//	Mat k1=imread("object.bmp",0);
//	Mat f1;
//	//threshold(k,f,50,255,THRESH_BINARY);//对图像进行二值化
//	//threshold(k1,f1,50,255,THRESH_BINARY);
//	Canny(k,f,150,300);
//	Canny(k1,f1,150,300);
//	//threshold(k,f,100,255,THRESH_BINARY);//对图像进行二值化
//	//threshold(k1,f1,100,255,THRESH_BINARY);
//	Mat closerect=getStructuringElement(MORPH_RECT,Size(3,3)); //进行结构算子生成
//	morphologyEx(f,f,MORPH_CLOSE,closerect);
//	morphologyEx(f1,f1,MORPH_CLOSE,closerect);//进行形态学开运算
//	imshow("123",f);
//	imshow("1234",f1);
//	waitKey();
//	Mat dst = Mat::zeros(k.rows, k.cols, CV_8UC3);
//	Mat dst1 = Mat::zeros(k1.rows, k1.cols, CV_8UC3);
//	vector<vector<Point>> w,w1;
//	vector<Vec4i> hierarchy,hierarchy1 ;
//	
//	findContours(f,w,hierarchy,RETR_EXTERNAL,CHAIN_APPROX_NONE);//提取轮廓元素
//	findContours(f1,w1,hierarchy1,RETR_LIST,CHAIN_APPROX_NONE);
//	
//	//FileStorage fs("f.dat",FileStorage::WRITE);
//	//fs<<"f"<<w1[0];
//	int idx=0;
//	int max=0;
//
//	for (int i=0;i<w.size();i++)
//		if (w[i].size()>max && w[i].size()<100000)
//		{
//			max = w[i].size();
//			idx = i;
//		}
//	cout<<max<<'\t'<<idx<<endl;
//	Mat show=imread("object.bmp");
//
//	double maxf = 0;int jdx = 0;
//		for (int j=0;j<w1.size();j++)
//		
//			if (w1[j].size()<1000000 && w1[j].size()!=0)
//			{
//	double ffff=matchShapes(w[idx],w1[j],CV_CONTOURS_MATCH_I3,1.0);//进行轮廓匹配
//	if (ffff>0.1)
//		std::cout<<ffff<<'\t'<<j<<'\t'<<w1[j].size()<<std::endl;
//	if (ffff>0.5)
//		circle(show,w1[j][0],5,Scalar(255,0,0));
//	if (ffff>maxf)
//	{
//		maxf = ffff;
//		jdx = j;
//	}
//		}
//	cout<<"jdx\t"<<w1[jdx][0].x<<'\t'<<w1[jdx][0].y<<endl;
//	//system("pause");
//	
//	imshow("show",show);
//	waitKey();
//	return 0;
//}
//
//void main()
//{
//	int resultNum = 0;
//	float* X1=new float[1000];
//	float* X2 = new float[1000];
//	float * Y1 = new float[1000];
//	float * Y2= new float[1000];
//	X1[0] = (878+1102)/2;
//	X2[0] = 1102;
//	Y1[0] = (800+1012)/2;
//	Y2[0] = 1012;
//	float * Alpha = new float[1000];
//	templateMatchDLL("1.bmp","4.bmp",resultNum,X1,Y1,X2,Y2,Alpha);
//
//	cout<<resultNum;
//}