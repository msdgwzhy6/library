using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace library2
{
    public class SiftFeat
    {
        public SiftFeat()
        {
        }

        public SiftFeat(Bitmap img)
        {
            currentImage = img;
        }

        public bool FindImage(int row, int col)
        {
            preciousImage = LoadImage();
            if(preciousImage != null)
                return true;

            return false;
        }

        public int GetNumOfFeatures()
        {
            return NumOfFeature;
        }

        public Feature[] GetFeatures()
        {
            return features;
        }

        private Bitmap LoadImage()
        {
            return new Bitmap("E:/SoftwareEngineering/LibrarySeatsManager/LibrarySeatsManager/a.jpg");
        }

        //使用sift算法提取特征点
        public void SiftFeatures()
        {
            BasicImagingInterface picture = new DisplayImage("E:/SoftwareEngineering/LibrarySeatsManager/LibrarySeatsManager/b.jpg");
            int pictureWidth = picture.Width;
            int pictureHeight = picture.Height;
            double startScale = 1.0;//图片之间的原始比例


            ImageMap pictureMap = picture.ConvertToImageMap(null);

            picture = null;

            //对内存垃圾回收
            GC.Collect();

            LoweFeatureDetector featureDetector = new LoweFeatureDetector();
            featureDetector.DetectFeatures(pictureMap);

            //记录提取的特征点及其数量
            SetFeature(featureDetector);
            
        }

        //记录提取的特征点及其数量
        private void SetFeature(LoweFeatureDetector featureDetector)
        {
            NumOfFeature = featureDetector.GlobalKeypoints.Count;
            features = new Feature[NumOfFeature];
            for (int i = 0; i < NumOfFeature; i++)
            {
                Keypoint keypoint = (Keypoint)featureDetector.GlobalKeypoints[i];

                features[i] = new Feature(keypoint.Image, keypoint.X, keypoint.Y, keypoint.ImgScale, keypoint.Scale, keypoint.Orientation);
                features[i].CreateVector(4, 4, 8);
                features[i].HasFeatureVector = keypoint.HasFV;
                features[i].FeatureVector = keypoint.FV;
            }
        }

        private Bitmap preciousImage, currentImage;
	    private int NumOfFeature;
	    private Feature[] features;
    }
}
