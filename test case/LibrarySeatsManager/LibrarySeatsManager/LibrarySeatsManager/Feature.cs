using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace library2
{
    //记录图片的特征
    public class Feature
    {
        public Feature()
        {
        }


        public Feature(ImageMap image, double x, double y, double imgScale,
            double kpScale, double orientation)
        {
            this.image = image;
            this.x = x;
            this.y = y;
            this.imageScale = imgScale;
            this.scale = kpScale;
            this.orientation = orientation;
        }


        //特征点所属的图片
        ImageMap image;
        public ImageMap Image
        {
            get
            {
                return (image);
            }
        }


        //特征点的coord坐标、与原始图片的比例等参数
        double x, y;
        double imageScale;	
        double scale;
        double orientation;
        public double X
        {
            get
            {
                return (x);
            }
            set
            {
                x = value;
            }
        }
        public double Y
        {
            get
            {
                return (y);
            }
            set
            {
                y = value;
            }
        }
        public double ImgScale
        {
            get
            {
                return (imageScale);
            }
            set
            {
                imageScale = value;
            }
        }
        public double Scale
        {
            get
            {
                return (scale);
            }
            set
            {
                scale = value;
            }
        }
        public double Orientation
        {
            get
            {
                return (orientation);
            }
            set
            {
                orientation = value;
            }
        }


        //判断是否有特征向量
        bool hasFV = false;
        public bool HasFeatureVector
        {
            get
            {
                return (hasFV);
            }
            set
            {
                hasFV = value;
            }
        }


        //特征向量
        double[] featureVector;
        public double[] FeatureVector
        {
            get
            {
                return (featureVector);
            }
            set
            {
                featureVector = value;
            }
        }


        //自定义的特征向量的提取与赋值
        public double GetFeatureVector(int xI, int yI, int oI)
        {
            return (featureVector[(xI * yDim * oDim) + (yI * oDim) + oI]);
        }
        public void SetFeatureVector(int xI, int yI, int oI, double value)
        {
            featureVector[(xI * yDim * oDim) + (yI * oDim) + oI] = value;
        }


        //特征向量的维数
        public int FeatureVectorLinearDimension
        {
            get
            {
                return (featureVector.Length);
            }
        }


        //特征向量中某一维的数值的提取与赋值
        public double GetFeatureVectorLinear(int idx)
        {
            return (featureVector[idx]);
        }
        public void SetFeatureVectorLinear(int idx, double value)
        {
            featureVector[idx] = value;
        }
        public double GetDimensionElement(int dim)
        {
            return (GetFeatureVectorLinear(dim));
        }


        //自定义维数的特征向量
        public void CreateLinearVector(int dim)
        {
            featureVector = new double[dim];
        }


        //自定义的coord坐标特征向量
        private int xDim, yDim, oDim;
        public void CreateVector(int xDim, int yDim, int oDim)
        {
            hasFV = true;
            this.xDim = xDim;
            this.yDim = yDim;
            this.oDim = oDim;
            featureVector = new double[yDim * xDim * oDim];
        }


        

        public int DimensionCount
        {
            get
            {
                return (FeatureVectorLinearDimension);
            }
        }

        
    }
}
