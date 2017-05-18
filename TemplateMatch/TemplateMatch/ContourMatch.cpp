#include <cv.h>
#include <highgui.h>
#include <iostream>
#include <fstream>
#include <time.h>
#include <vector>

using namespace std;

using namespace cv;

struct EdgePoint
{
	Point pos;
	int dir;
};


Point convert(Point pt, float angle)
{
	float y = (pt.y*cos(angle)-pt.x*sin(angle));
	float x = (pt.y*sin(angle)+pt.x*cos(angle));
	return Point(x,y);
}

void promoteEdgeSet(vector<vector<EdgePoint>> &edgeSets, vector<EdgePoint> edgeSet, vector<float> angle)
{
	for (int j=0;j<angle.size();j++)
	{
		vector<EdgePoint> newEdgeSet;

		for (vector<EdgePoint>::iterator it = edgeSet.begin();it!=edgeSet.end();it++)
		{
			EdgePoint temp;
			temp.pos = convert(it->pos,angle[j]);
			temp.dir = (int)(it->dir+angle[j]*180/CV_PI+0.5);
			newEdgeSet.push_back(temp);
		}
		edgeSets.push_back(newEdgeSet);
	}
}

void saveTemplate(vector<vector<EdgePoint>> edgeSets, string filename, int level, vector<float> angle,float minRate, int questNum, int angleThr,int width,int height,int thr)
{	
	ofstream os(filename, ios::binary); 
	os.write((const char*)&level,sizeof(int));
	int size = angle.size();
	os.write((const char*)&size,sizeof(int));
	for (int i=0;i<size;i++)
		os.write((const char*)&angle[i],sizeof(float));
	os.write((const char*)&minRate,sizeof(float));
	os.write((const char*)&questNum,sizeof(int));
	os.write((const char*)&angleThr,sizeof(int));
	os.write((const char*)&width,sizeof(int));
	os.write((const char*)&height,sizeof(int));
	os.write((const char*)&thr,sizeof(int));

	for (vector<vector<EdgePoint>>::iterator it = edgeSets.begin();it!=edgeSets.end();it++)
	{
		int size = it->size();
		os.write((const char*)&size, 4); 
		os.write((const char*)&(*it)[0], size * sizeof(EdgePoint)); 
	}
	os.close(); 
}

vector<vector<EdgePoint>> loadTemplate(string filename, int& level, vector<float>& angle,float &minRate, int &questNum, int &angleThr,int &width,int &height,int &thr)
{
	angle.clear();
	ifstream is(filename, ios::binary); 
	vector<vector<EdgePoint>> edgeSets;
	is.read((char*)&level,sizeof(int));
	int size;
	is.read((char*)&size,sizeof(int));
	for (int i=0;i<size;i++)
	{
		float temp;
		is.read((char*)&temp,sizeof(float));
		angle.push_back(temp);
	}

	is.read((char*)&minRate,sizeof(float));
	is.read((char*)&questNum,sizeof(int));
	is.read((char*)&angleThr,sizeof(int));
	is.read((char*)&width,sizeof(int));
	is.read((char*)&height,sizeof(int));
	is.read((char*)&thr,sizeof(int));
	for (int i = 0;i<level*angle.size();i++)
	{
		vector<EdgePoint> edgeSet;
		int size;
		is.read((char*)&size, 4); 
		edgeSet.resize(size); 
		is.read((char*)&edgeSet[0], size * sizeof(EdgePoint)); 
		edgeSets.push_back(edgeSet);
	}
	return edgeSets;
}

void templateMake(Mat temp, vector<float> angle, float thr, int level, float minRate, int questNum, int angleThr,string filename)
{
	vector<vector<EdgePoint>> edgeSets;


	for (int l = 1;l<=level;l++)
	{
		Mat resizedTemp;
		vector<EdgePoint> edgeSet;
		resize(temp,resizedTemp,Size(),pow(1.0/2.0,l-1),pow(1.0/2.0,l-1));
		Mat tempEdge(resizedTemp.rows,resizedTemp.cols,CV_8UC1);

		Mat sobelX,sobelY;
		Sobel(resizedTemp,sobelX,resizedTemp.depth(),1,0);
		Sobel(resizedTemp,sobelY,resizedTemp.depth(),0,1);

		for (int i=0;i<tempEdge.rows*tempEdge.cols;i++)
			tempEdge.data[i] = sqrt((sobelX.data[i]*sobelX.data[i]+sobelY.data[i]*sobelY.data[i])/2);

		//Canny(resizedTemp,tempEdge,thr1,thr2);
		for (int i=0;i<tempEdge.rows;i++)
			for (int j=0;j<tempEdge.cols;j++)
				//if (tempEdge.data[i*tempEdge.cols+j]==255)
				if (tempEdge.data[i*sobelY.cols+j] >= thr)
				{
					tempEdge.data[i*sobelY.cols+j] = 255;
					int count = 0;
					for (int p=-1;p<=1;p++)
						for (int q=-1;q<=1;q++)
							if (i+p>=0 && i+p<tempEdge.rows && j+q>=0 && j+q<tempEdge.cols)
								if (tempEdge.data[(i+p)*tempEdge.cols+j+q] >= thr)
									count++;
					if (count == 1)
						tempEdge.data[i*tempEdge.cols+j] = 0;
					else
					{
						EdgePoint ep;
						ep.pos = Point(i-tempEdge.rows/2,j-tempEdge.cols/2);
						if (sobelX.data[i*sobelX.cols+j] == 0)
							ep.dir = 90;
						else ep.dir = (int)(atan((double)sobelY.data[i*sobelY.cols+j]/(double)sobelX.data[i*sobelY.cols+j])+0.5);
						edgeSet.push_back(ep);
					}
				}
				else tempEdge.data[i*sobelY.cols+j] = 0;
		promoteEdgeSet(edgeSets, edgeSet, angle);
		//imshow("tempEdge",tempEdge);
		//waitKey();
	}

	saveTemplate(edgeSets,filename,level, angle, minRate,  questNum,  angleThr,temp.cols,temp.rows,thr);

}

struct MatchResult
{
	vector<Point> pos;
	vector<int> angle;
	vector<double> rate;
};

MatchResult pointMatch(Mat obj, vector<Point> pts, vector<int> angle, vector<vector<EdgePoint>> edgeSets, int level, int thr1, int thr2, int lowAngle, int highAngle,double minRate,int questNum,int angleVar,int tempWidth, int tempHeight)
{
	Mat newObj;
	double scale = pow(1.0/2.0,level-1);
	resize(obj,newObj,Size(),scale,scale);

	Mat objEdge(newObj.rows,newObj.cols,CV_8UC1);
	//Canny(newObj,objEdge,thr1,thr2);
	Mat sobelX(newObj.rows,newObj.cols,CV_16SC1),sobelY(newObj.rows,newObj.cols,CV_16SC1);
	Sobel(newObj,sobelX,sobelX.depth(),1,0);
	Sobel(newObj,sobelY,sobelX.depth(),0,1);
	convertScaleAbs(sobelX, sobelX);  
    convertScaleAbs(sobelY, sobelY); 

	for (int i=0;i<objEdge.rows*objEdge.cols;i++)
		objEdge.data[i] = sqrt(((sobelX.data)[i]*(sobelX.data)[i]+(sobelY.data)[i]*(sobelY.data)[i])/2);
	threshold(objEdge,objEdge,thr1,255,THRESH_BINARY);

	//imshow("objEdge",objEdge);
		//waitKey();

	int *dir = new int[objEdge.rows*objEdge.cols];
	for (int i=0;i<objEdge.rows*objEdge.cols;i++)
		if (objEdge.data[i] == 255)
		{
			if ((sobelX.data)[i] == 0)
				dir[i]= 90;
			else dir[i] = (int)(atan((double)(sobelY.data)[i]/(double)(sobelX.data)[i])+0.5);
		}
	int size = pts.size();
	int *weight = new int[size*(2*angleVar+1)];
	memset(weight,0,size*(2*angleVar+1)*sizeof(int));
	int *tag = new int[size];
	memset(tag,0,size*sizeof(int));

	int max = 0;

	for (int i=0;i<size;i++)
	{
		//cout<<pts[i].x<<'\t'<<pts[i].y<<'\t'<<angle[i]<<'\t'<<angleVar<<endl;
		for (int j = angle[i] - angleVar;j<=angle[i]+angleVar;j++)
			if (j>=lowAngle && j<=highAngle)
			{
				int weightIndex = i * (2*angleVar+1) + j-(angle[i] - angleVar);
				int index = (level-1)*(highAngle-lowAngle+1) + j - lowAngle;
				//int num = 0;
				for (vector<EdgePoint>::iterator tempIt = edgeSets[index].begin();tempIt!=edgeSets[index].end();tempIt++)
				{
					//num++;
					//cout<<level<<'\t'<<i<<'\t'<<j<<'\t'<<angle[i]<<'\t'<<index<<'\t'<<edgeSets.size()<<endl;
					int xx = tempIt->pos.y + pts[i].x;
					int yy = tempIt->pos.x + pts[i].y;
					if (xx>=0 && xx<objEdge.cols && yy>=0 && yy<objEdge.rows)
						if (objEdge.data[yy*objEdge.cols+xx] == 255 && abs(dir[yy*objEdge.cols+xx]%180-tempIt->dir%180)<=thr2)
							weight[weightIndex]++;
					//if (iWeight[index]+10<num*greediness)
						//break;
				}
			}
		for (int j = 1;j<(2*angleVar+1);j++)
			if (weight[i * (2*angleVar+1) + j]>weight[i * (2*angleVar+1)])
			{
				weight[i * (2*angleVar+1)] = weight[i * (2*angleVar+1) + j];
				tag[i] = j;
			}
		if (weight[i * (2*angleVar+1)]>max)
			max = weight[i * (2*angleVar+1)];
	}
	


	//cvtColor(obj,obj,CV_GRAY2RGB);

	
		//Mat weightShow(obj.rows,obj.cols,CV_8UC1);
		//for (int i=0;i<obj.rows*obj.cols;i++)
		//	weightShow.data[i] = 0;
		//for (int i=0;i<size;i++)
		//	weightShow.data[pts[i].y*obj.cols+pts[i].x] = weight[i*(2*angleVar+1)]*255/max;
		//imshow("weight",weightShow);
		//waitKey();

		//for (int i=0;i<size;i++)
		//	if (weight[i*(2*angleVar+1)]>max*rate)
		//		rectangle(obj,Point(pts[i].x-10,pts[i].y-10),Point(pts[i].x+10,pts[i].y+10),Scalar(255,0,0),2);

		//imshow("obj",obj);
		//waitKey();

	if (level == 1)
	{
		MatchResult mr;
		while (questNum-->0)
		{
			int tempMax = 0;
			for (int i=0;i<size;i++)
				if (weight[i*(2*angleVar+1)] > tempMax)
					tempMax = weight[i*(2*angleVar+1)];
			for (int i=0;i<size;i++)
			{
				if (weight[i*(2*angleVar+1)] == tempMax)
				{
					mr.pos.push_back(Point(pts[i].x,pts[i].y));
					mr.angle.push_back(tag[i]-angleVar+angle[i]);
					mr.rate.push_back((double)tempMax/(double)max);
					for (int j=0;j<size;j++)
						if (abs(pts[i].x-pts[j].x)<tempWidth/2 && abs(pts[i].y-pts[j].y)<tempHeight/2)
						{
							weight[j*(2*angleVar+1)] = 0;
						}
					break;
				}
			}
		}
		return mr;
	}

		vector<Point> newPts;
	vector<int> newAngle;
	//for (int i=0;i<size;i++)
	//	if (weight[i*(2*angleVar+1)]>max*rate)
	//	{
	//		for (int p = -1;p<=1;p++)
	//			for (int q=-1;q<=1;q++)
	//				if (pts[i].x*2+p>=0 && pts[i].y*2+q>=0)
	//				{
	//					newPts.push_back(Point(pts[i].x*2+p,pts[i].y*2+q));
	//					newAngle.push_back(tag[i]-angleVar+angle[i]);
	//				}
	//	}
	
	double rate = 1;
	while (newPts.size()<questNum*level*4*10 && rate>minRate)
	{
		rate-=0.1;
		for (int i=0;i<size;i++)
		if (weight[i*(2*angleVar+1)]>max*rate && weight[i*(2*angleVar+1)]<=max*(rate+0.1))
		{
			for (int p = 0;p<=1;p++)
				for (int q=0;q<=1;q++)
					if (pts[i].x*2+p>=0 && pts[i].y*2+q>=0)
					{
						newPts.push_back(Point(pts[i].x*2+p,pts[i].y*2+q));
						newAngle.push_back(tag[i]-angleVar+angle[i]);
					}
		}
	}

	return pointMatch(obj,newPts,newAngle,edgeSets,level-1,thr1,thr2,lowAngle,highAngle,minRate,questNum,angleVar, tempWidth, tempHeight);
}


MatchResult edgeMatch(Mat obj, vector<vector<EdgePoint>> edgeSets, vector<int> indexs, int level, int thr1, int thr2, double minRate, int questNum, int lowAngle, int highAngle, int angleVar,int tempWidth, int tempHeight)
{
	clock_t t1,t2,t3,t4,t5;
	t1 = clock();
	Mat newObj;
	double scale = pow(1.0/2.0,level-1);
	resize(obj,newObj,Size(),scale,scale);

	int minX = 0, maxX = obj.cols-1;
	int minY = 0, maxY = obj.rows-1;

	Mat objEdge(newObj.rows,newObj.cols,CV_8UC1);
	//Canny(newObj,objEdge,thr1,thr2);
	Mat sobelX(newObj.rows,newObj.cols,CV_16SC1),sobelY(newObj.rows,newObj.cols,CV_16SC1);
	Sobel(newObj,sobelX,sobelX.depth(),1,0);
	Sobel(newObj,sobelY,sobelX.depth(),0,1);
	convertScaleAbs(sobelX, sobelX);  
    convertScaleAbs(sobelY, sobelY); 

	for (int i=0;i<objEdge.rows*objEdge.cols;i++)
	{
			//cout<<sqrt((((short*)sobelX.data)[i]*((short*)sobelX.data)[i]+((short*)sobelY.data)[i]*((short*)sobelY.data)[i])/2)/128<<'\t';
		objEdge.data[i] = sqrt(((sobelX.data)[i]*(sobelX.data)[i]+(sobelY.data)[i]*(sobelY.data)[i])/2);
	}
	threshold(objEdge,objEdge,thr1,255,THRESH_BINARY);

	int *dir = new int[objEdge.rows*objEdge.cols];
	for (int i=0;i<objEdge.rows*objEdge.cols;i++)
		if (objEdge.data[i] == 255)
		{
			if ((sobelX.data)[i] == 0)
				dir[i]= 90;
			else dir[i] = (int)(atan((double)(sobelY.data)[i]/(double)(sobelX.data)[i])+0.5);
		}
	
	vector<EdgePoint> objEdgeSet;
	for (int i=0;i<objEdge.rows;i++)
		for (int j=0;j<objEdge.cols;j++)
			if (objEdge.data[i*objEdge.cols+j] ==255)
			{
				int count = 0;
				for (int p=-1;p<=1;p++)
					for (int q=-1;q<=1;q++)
						if (i+p>=0 && i+p<objEdge.rows && j+q>=0 && j+q<objEdge.cols)
							if (objEdge.data[(i+p)*objEdge.cols+j+q] == 255)
								count++;
				if (count == 1)
					objEdge.data[i*objEdge.cols+j] = 0;
				else
				{
					EdgePoint ep;
					ep.pos = Point(i,j);
					ep.dir = dir[i*objEdge.cols+j];
					objEdgeSet.push_back(ep);
				}
			}

	int size = (maxX-minX+1)*(maxY-minY+1);
	int *weight = new int[indexs.size()*size];
	memset(weight,0,indexs.size()*size*sizeof(int));

	t2 = clock();
	for (int i = 0;i<indexs.size();i++)
	{
		for (vector<EdgePoint>::iterator objIt = objEdgeSet.begin();objIt!=objEdgeSet.end();objIt++)
		{
			if (objIt->pos.x>=minY && objIt->pos.x<=maxY && objIt->pos.y>=minX &&objIt->pos.y<=maxX)
			{
				for (vector<EdgePoint>::iterator tempIt = edgeSets[indexs[i]].begin();tempIt!=edgeSets[indexs[i]].end();tempIt++)
				{
					if (abs(objIt->dir%180-tempIt->dir%180)<=thr2)
					if (objIt->pos.x-tempIt->pos.x>=minY && objIt->pos.x-tempIt->pos.x<=maxY &&
						objIt->pos.y-tempIt->pos.y>=minX && objIt->pos.y-tempIt->pos.y<=maxX)
						weight[i*size + (objIt->pos.x-(int)(tempIt->pos.x+0.5)-minY)*(maxX-minX+1)+objIt->pos.y-(int)(tempIt->pos.y+0.5)-minX]++;
				}
			}
		}
	}
	t3 = clock();

	int max = 0;
	int *tag = new int[size];
	memset(tag,0,size*sizeof(int));

	for (int i=0;i<size;i++)
		for (int k = 1;k<indexs.size();k++)
		{
			if (weight[i]<weight[k*size+i])
			{
				weight[i] = weight[k*size+i];
				tag[i] = k;
			}
			if (weight[i]>max) max = weight[i];
		}

	
	if (level == 1)
	{
		MatchResult mr;
		while (questNum-->0)
		{
			int tempMax = 0;
			for (int i=0;i<size;i++)
				if (weight[i] > tempMax)
					tempMax = weight[i];
			for (int i=0;i<size;i++)
			{
				if (weight[i*(2*angleVar+1)] == tempMax)
				{
					mr.pos.push_back(Point(i%(maxX-minX+1),i/(maxX-minX+1)));
					mr.angle.push_back(tag[i]*angleVar+lowAngle);
					mr.rate.push_back((double)tempMax/(double)max);
					for (int p = -tempWidth/2+1;p<tempWidth/2;p++)
						for (int q = -tempHeight/2+1;q<tempHeight/2;q++)
							if (i/(maxX-minX+1)+q>=0 && i/(maxX-minX+1)+q<(maxY-minY+1) &&
								i%(maxX-minX+1)+p>=0 && i%(maxX-minX+1)+p<(maxX-minX+1))
								weight[(i/(maxX-minX+1)+q)*(maxX-minX+1)+(i%(maxX-minX+1)+p)] = 0;
					break;
				}
			}
		}
		return mr;
	}

	vector<Point> pts;
	vector<int> angle;
	//for (int i=0;i<size;i++)
	//	if (weight[i]>max*rate)
	//	{
	//		for (int p = -1;p<=1;p++)
	//			for (int q=-1;q<=1;q++)
	//				if (i%(maxX-minX+1)*2+p>=0 && i/(maxX-minX+1)*2+q>=0)
	//				{
	//					pts.push_back(Point(i%(maxX-minX+1)*2+p,i/(maxX-minX+1)*2+q));
	//					angle.push_back(tag[i]*angleVar+lowAngle);
	//				}
	//	}
	double rate = 1;
	while (pts.size()<questNum*level*4*10 && rate>minRate)
	{
		rate-=0.05;
		for (int i=0;i<size;i++)
		if (weight[i]>max*rate && weight[i]<=max*(rate+0.05))
		{
			for (int p = 0;p<=1;p++)
				for (int q=0;q<=1;q++)
					if (i%(maxX-minX+1)*2+p>=0 && i/(maxX-minX+1)*2+q>=0)
					{
						pts.push_back(Point(i%(maxX-minX+1)*2+p,i/(maxX-minX+1)*2+q));
						angle.push_back(tag[i]*angleVar+lowAngle);
					}
		}
	}

	Mat weightShow((maxY-minY+1),(maxX-minX+1),CV_8UC1);
	for (int i=0;i<size;i++)
		//weightShow.data[i] = weight[i]*255/max;
		if (weight[i]>max*rate) weightShow.data[i] = 255;
		else weightShow.data[i] = 0;
	//imshow("weight",weightShow);
	//waitKey();

	t4 = clock();
	MatchResult mr = pointMatch(obj,pts,angle,edgeSets,level-1,thr1,thr2,lowAngle,highAngle,minRate,questNum,angleVar, tempWidth, tempHeight);
	t5 = clock();
	cout<<t2-t1<<"ms\t"<<t3-t2<<"ms\t"<<t4-t3<<"ms\t"<<t5-t4<<"ms"<<endl;
	return mr;
}



//MatchResult edgeMatch(vector<Point> objEdgeSet, Rect ROI,vector<vector<Point>> edgeSets, vector<int> indexs)
//{
//	int size = ROI.width*ROI.height;
//	int *weight = new int[indexs.size()*size];
//	memset(weight,0,indexs.size()*size*sizeof(int));
//
//	for (int i = 0;i<indexs.size();i++)
//	{
//		for (vector<Point>::iterator objIt = objEdgeSet.begin();objIt!=objEdgeSet.end();objIt++)
//		{
//			if (objIt->x>=ROI.y && objIt->x<ROI.y+ROI.height && objIt->y>=ROI.x &&objIt->y<ROI.x+ROI.width)
//			{
//				for (vector<Point>::iterator tempIt = edgeSets[indexs[i]].begin();tempIt!=edgeSets[indexs[i]].end();tempIt++)
//					if (objIt->x-tempIt->x>=ROI.y && objIt->x-tempIt->x<ROI.y+ROI.height &&
//						objIt->y-tempIt->y>=ROI.x && objIt->y-tempIt->y<ROI.x+ROI.width)
//						weight[i*size + (objIt->x-tempIt->x-ROI.y)*ROI.width+objIt->y-tempIt->y-ROI.x]++;
//			}
//		}
//	}
//
//	int max = 0;
//	int *tag = new int[size];
//	memset(tag,0,size*sizeof(int));
//
//	for (int i=0;i<size;i++)
//		for (int k = 1;k<indexs.size();k++)
//		{
//			if (weight[i]<weight[k*size+i])
//			{
//				weight[i] = weight[k*size+i];
//				tag[i] = k;
//			}
//		}
//
//	for (int i=0;i<size;i++)
//	if (weight[i]>max) max = weight[i];
//		Mat resultM(ROI.height,ROI.width,CV_8UC1);
//	for (int i=0;i<size;i++)
//		resultM.data[i] = weight[i]*255/max;
//	
//	imshow("result",resultM);
//
//	MatchResult result;
//	for (int i=0;i<size;i++)
//	{
//		if (weight[i] == max)
//		{
//			result.pos = Point(i/ROI.width+ROI.y,i%ROI.width+ROI.x);
//			result.index = tag[i];
//		}
//	}
//	return result;
//}


//MatchResult reverseEdgeMatch(Mat objEdge, Rect ROI, vector<vector<Point>> edgeSets, vector<int> indexs, double greediness)
//{
//	//cvtColor(obj,obj,CV_RGB2GRAY);
//	//Mat objX,objY;
//	//Sobel(obj,objX,obj.depth(),1,0);
//	//Sobel(obj,objY,obj.depth(),0,1);
//	//Mat objEdge(obj.rows,obj.cols,CV_8UC1);
//	//for (int i = 0;i<obj.rows*obj.cols;i++)
//	//{
//	//	objEdge.data[i] = sqrt((objX.data[i]*objX.data[i]+objY.data[i]*objY.data[i])/2);
//	//	//if (objX.data[i]>100)
//	//	//{
//	//		//cout<<(int)objX.data[i]<<'\t'<<(int)objY.data[i]<<'\t'<<(int)objEdge.data[i]<<endl;
//	//	//}
//	//}
//		
//	int minX = ROI.x, maxX = (ROI.x + ROI.width - 1);
//	int minY = ROI.y, maxY = (ROI.y + ROI.height - 1);
//
//	int size = (maxX-minX+1)*(maxY-minY+1);
//	int *weight = new int[size];
//	memset(weight,0,size*sizeof(int));
//	int *tag = new int[size];
//	memset(tag,0,size*sizeof(int));
//
//	for (int i = 0;i<indexs.size();i++)
//	{
//		int *iWeight = new int[size];
//		memset(iWeight,0,size*sizeof(int));
//		for (int y=minY;y<=maxY;y++)
//			for (int x=minX;x<=maxX;x++)
//			{
//				int index = (y-minY)*(maxX-minX+1)+x-minX;
//				int num = 0;
//				for (vector<Point>::iterator tempIt = edgeSets[indexs[i]].begin();tempIt!=edgeSets[indexs[i]].end();tempIt++)
//				{
//					num++;
//					int xx = tempIt->y + x ;
//					int yy = tempIt->x + y;
//					if (xx>=0 && xx<objEdge.cols && yy>=0 && yy<objEdge.rows)
//						if (objEdge.data[yy*objEdge.cols+xx] == 255)
//							iWeight[index]++;
//					//if (iWeight[index]+10<num*greediness)
//						//break;
//				}
//			}
//		for (int j = 0;j<size;j++)
//			if (iWeight[j]>weight[j])
//			{
//				weight[j] = iWeight[j];
//				tag[j] = i;
//			}
//	}
//	int max = 0;
//	for (int i=0;i<size;i++)
//	if (weight[i]>max) max = weight[i];
//	Mat resultM(ROI.height,ROI.width,CV_8UC1);
//	for (int i=0;i<size;i++)
//		resultM.data[i] = weight[i]*255/max;
//	
//	imshow("result",resultM);
//	MatchResult result;
//	for (int i=0;i<size;i++)
//	{
//		if (weight[i] == max)
//		{
//			result.pos = Point(i/ROI.width+ROI.y,i%ROI.width+ROI.x);
//			result.index = tag[i];
//		}
//	}
//	return result;
//}


void __declspec(dllexport) templateFinetune(int width, int height, int imgSize, unsigned char *data, int thr, int& edgeNum, int *edgeX, int *edgeY)
{
	Mat pic;

	//计算图像通道数
	int channel = imgSize/width/height + 0.5;

	//根据图像通道数保存为对应的Mat类型
	switch (channel)
	{
	case 1:
		pic = Mat(height,width,CV_8UC1);
		for (int i=0;i<imgSize;i++)
			pic.data[i] = data[i];
		break;
	case 3:
		pic = Mat(height,width,CV_8UC3);
		for (int i=0;i<imgSize;i++)
			pic.data[i] = data[i];
		cvtColor(pic,pic,CV_RGB2GRAY);
		break;
	case 4:
		pic = Mat(height,width,CV_8UC4);
		for (int i=0;i<imgSize;i++)
			pic.data[i] = data[i];
		cvtColor(pic,pic,CV_RGBA2GRAY);
		break;
	default:
		return;
	};

	Mat tempEdge(pic.rows,pic.cols,CV_8UC1);

	Mat sobelX(tempEdge.rows,tempEdge.cols,CV_16SC1),sobelY(tempEdge.rows,tempEdge.cols,CV_16SC1);
	Sobel(pic,sobelX,sobelX.depth(),1,0);
	Sobel(pic,sobelY,sobelX.depth(),0,1);

	convertScaleAbs(sobelX, sobelX);  
    convertScaleAbs(sobelY, sobelY); 
	for (int i=0;i<tempEdge.rows*tempEdge.cols;i++)
		tempEdge.data[i] = sqrt(((sobelX.data)[i]*(sobelX.data)[i]+(sobelY.data)[i]*(sobelY.data)[i])/2);

	edgeNum = 0;
	for (int i=0;i<tempEdge.rows*tempEdge.cols;i++)
	{
		tempEdge.data[i] = sqrt(((sobelX.data)[i]*(sobelX.data)[i]+(sobelY.data)[i]*(sobelY.data)[i])/2);
		if (tempEdge.data[i]>=thr)
		{
			edgeX[edgeNum] = i%tempEdge.cols;
			edgeY[edgeNum++] = i/tempEdge.cols;
		}
	}
}

void __declspec(dllexport) templateSave(int width, int height, int imgSize, unsigned char *data, int thr, int lowAngle, int highAngle, int level, float minRate, int questNum, int angleThr, char* filename)
{
	Mat pic;

	//计算图像通道数
	int channel = imgSize/width/height + 0.5;

	//根据图像通道数保存为对应的Mat类型
	switch (channel)
	{
	case 1:
		pic = Mat(height,width,CV_8UC1);
		for (int i=0;i<imgSize;i++)
			pic.data[i] = data[i];
		break;
	case 3:
		pic = Mat(height,width,CV_8UC3);
		for (int i=0;i<imgSize;i++)
			pic.data[i] = data[i];
		cvtColor(pic,pic,CV_RGB2GRAY);
		break;
	case 4:
		pic = Mat(height,width,CV_8UC4);
		for (int i=0;i<imgSize;i++)
			pic.data[i] = data[i];
		cvtColor(pic,pic,CV_RGBA2GRAY);
		break;
	default:
		return;
	};

	vector<float> angleSet;
	for (int i = lowAngle;i<=highAngle;i+=1)
		angleSet.push_back(i*CV_PI/180);
	templateMake(pic,angleSet,thr,level,minRate,questNum,angleThr,filename);
}

void __declspec(dllexport) templateDetect(int width, int height, int imgSize, unsigned char *data, 
	char* filename, int &resultNum, int *resultX, int *resultY, int *dir, float *rate)
{
	Mat pic;

	//计算图像通道数
	int channel = imgSize/width/height + 0.5;

	//根据图像通道数保存为对应的Mat类型
	switch (channel)
	{
	case 1:
		pic = Mat(height,width,CV_8UC1);
		for (int i=0;i<imgSize;i++)
			pic.data[i] = data[i];
		break;
	case 3:
		pic = Mat(height,width,CV_8UC3);
		for (int i=0;i<imgSize;i++)
			pic.data[i] = data[i];
		cvtColor(pic,pic,CV_RGB2GRAY);
		break;
	case 4:
		pic = Mat(height,width,CV_8UC4);
		for (int i=0;i<imgSize;i++)
			pic.data[i] = data[i];
		cvtColor(pic,pic,CV_RGBA2GRAY);
		break;
	default:
		return;
	};

	//Mat pic = imread("1.bmp");
	//cvtColor(pic,pic,CV_RGB2GRAY);

	int level,thr,angleThr,questNum,tempWidth,tempHeight;
	float minRate;
	
	vector<float> angleSet;
	vector<vector<EdgePoint>> edgeSets;
	edgeSets = loadTemplate(filename, level,angleSet,minRate,questNum,angleThr,tempWidth,tempHeight,thr);

	vector<int> indexs;
	for (int j=0;j<angleSet.size();j+=5)
		indexs.push_back((level-1)*angleSet.size()+j);

	int lowAngle,highAngle;
	if (angleSet[0]<0) lowAngle = (int)(angleSet[0]*180/CV_PI-0.5);
	else lowAngle = (int)(angleSet[0]*180/CV_PI+0.5);
	if (angleSet[angleSet.size()-1]<0) highAngle = (int)(angleSet[angleSet.size()-1]*180/CV_PI-0.5);
	else highAngle = (int)(angleSet[angleSet.size()-1]*180/CV_PI+0.5);

	MatchResult mr = edgeMatch(pic,edgeSets,indexs,level,thr,angleThr,minRate,questNum,lowAngle,highAngle,5,tempWidth,tempHeight);

	//cout<<mr.pos.size()<<'\t'<<mr.angle.size()<<'\t'<<mr.rate.size()<<endl;
	resultNum = mr.pos.size();
	for (int i=0;i<mr.pos.size();i++)
	{
		resultX[i] = mr.pos[i].x;
		resultY[i] = mr.pos[i].y;
		dir[i] = mr.angle[i];
		rate[i] = mr.rate[i];
	}

}

void __declspec(dllexport) templateLoad(char* filename,int& level, float&lowAngle, float &highAngle,float &minRate, int &questNum, int &angleThr,int &width,int &height,int &thr)
{
	vector<float> angle;
	loadTemplate(filename,level, angle,minRate, questNum,angleThr,width,height,thr);
	lowAngle = (angle[0]);
	highAngle = (angle[angle.size()-1]);
}

void main()
{
	              int resultNum = 1000;
                int* resultX = new int[resultNum];
                int* resultY = new int[resultNum];
                int* dir = new int[resultNum];
                float* rate = new float[resultNum];
	//templateDetect("test.tr",resultNum,resultX,resultY,dir,rate);
	cout<<resultNum<<endl;
	cout<<resultX[0]<<'\t'<<resultY[0]<<'\t'<<dir[0]<<'\t'<<rate[0]<<endl;
	//Mat temp = imread("template2.bmp");
	Mat temp = imread("circleTemp.bmp");

	//Mat temp = imread("smallTemp.bmp");
	resize(temp,temp,Size(),1,1);
	cvtColor(temp,temp,CV_RGB2GRAY);
	Mat obj;
	Mat objColor = imread("1.bmp");
	resize(objColor,objColor,Size(),1,1);
	cvtColor(objColor,obj,CV_RGB2GRAY);
	
	Mat tempEdge;
	
	vector<float> sizeSet;
	vector<float> angleSet;
	int level = 2;
	int lowAngle = -50;
	int highAngle = 0;

	for (float i = lowAngle;i<=highAngle;i+=1)
		angleSet.push_back(i/180*CV_PI);
	templateMake(temp,angleSet,100,level,0.8,10,30,"template.tr");

	int thr,angleThr,questNum,width,height;
	float minRate;

	vector<vector<EdgePoint>> edgeSets;
	edgeSets = loadTemplate("template.tr", level,angleSet,minRate,questNum,angleThr,width,height,thr);

	vector<int> indexs;
	for (int j=0;j<angleSet.size();j+=5)
		indexs.push_back((level-1)*angleSet.size()+j);
	
	MatchResult mr = edgeMatch(obj,edgeSets,indexs,level,thr,angleThr,minRate,questNum,lowAngle,highAngle,5,width,height);
	//MatchResult mr = reverseEdgeMatch(objEdge,Rect(0,0,objEdge.cols,objEdge.rows),edgeSets,indexs,0.9);
	
	cout<<mr.pos.size()<<endl;
	cout<<mr.pos[0].x<<'\t'<<mr.pos[0].y<<'\t'<<mr.pos[1].x<<'\t'<<mr.pos[1].y<<endl;
	for (int i=0;i<mr.pos.size();i++)
		rectangle(objColor,Point(mr.pos[i].x-temp.cols/2,mr.pos[i].y-temp.rows/2),Point(mr.pos[i].x+temp.cols/2,mr.pos[i].y+temp.rows/2),Scalar(255,0,0));

	resize(objColor,objColor,Size(),0.5,0.5);
	imshow("obj",objColor);
	waitKey();
}
