using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;

namespace library2
{
    //进行图像匹配
    public class Match
    {
        public Match()
        {
        }


        public Match(Bitmap img)
        {
            image = new Bitmap(img);
            currentSiftFeature = new SiftFeat(img);
        }


	    public bool FindImage(int row,int col)
        {
            if (preciousSiftFeature.FindImage(row, col))
                return true;

            return false;
        }

        //匹配特征点
	    public int MatchFeatures()
        {
            //记录图像的特征点及其数量
            SetFeature();

            //对特征点进行匹配
            int numOfMatchFeature = 0;
            for (int i = 0; i < (numOfCurrentFeature < numOfPreciousFeature ? numOfCurrentFeature : numOfPreciousFeature); ++i)
            {
                if (IsFeatureMatch(featureOfCurrentImage[i], featureOfPreciousImage[i]))
                    ++numOfMatchFeature;
            }

            //图像匹配则返回1，否则返回0
            if (numOfMatchFeature > 2 * (numOfCurrentFeature > numOfPreciousFeature ? numOfCurrentFeature : numOfPreciousFeature) / 3)
                return 1;
            return 0;
        }


        //对特征点中的单个特征向量进行匹配
        private bool IsFeatureMatch(Feature firstFeature, Feature secondFeature)
        {
            int numOfMatchFeatureVector = 0;
            double difference;
            for (int i = 0; i < firstFeature.FeatureVector.Length; ++i)
                for (int j = 0; j < secondFeature.FeatureVector.Length; ++j)
                {
                    difference = Math.Abs(firstFeature.FeatureVector[i] - secondFeature.FeatureVector[j]);
                    if (difference < (firstFeature.FeatureVector[i] < secondFeature.FeatureVector[j] ? firstFeature.FeatureVector[i] : secondFeature.FeatureVector[j]) / 5)
                    {
                        ++numOfMatchFeatureVector;
                    }
                }
            //特征向量匹配则返回true，否则返回false
            if (numOfMatchFeatureVector > 2 * (firstFeature.FeatureVector.Length > secondFeature.FeatureVector.Length ? firstFeature.FeatureVector.Length : secondFeature.FeatureVector.Length) / 3)
                return true;
             return false;
        }

        //记录图片的特征点及其数量
        private void SetFeature()
        {
            //使用sift算法提取特征点
            currentSiftFeature.SiftFeatures();
            preciousSiftFeature.SiftFeatures();

            featureOfCurrentImage = currentSiftFeature.GetFeatures();
            numOfCurrentFeature = currentSiftFeature.GetNumOfFeatures();

            featureOfPreciousImage = preciousSiftFeature.GetFeatures();
            numOfPreciousFeature = preciousSiftFeature.GetNumOfFeatures();
        }

        private Bitmap image;
	    private SiftFeat currentSiftFeature = new SiftFeat();
        private int numOfCurrentFeature;
        private SiftFeat preciousSiftFeature = new SiftFeat();
        private int numOfPreciousFeature;
        private Feature[] featureOfCurrentImage, featureOfPreciousImage;
    }
}
