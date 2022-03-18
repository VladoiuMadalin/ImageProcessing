using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;

namespace ImageProcessingAlgorithms.Tools
{
    public static class Tools
    {

        public static double Mean(Image<Gray, byte> inputImage)
        {
            double mean = 0;
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    mean += inputImage.Data[y, x, 0];
                }
            }
            return mean / (inputImage.Height * inputImage.Width);
        }

        public static Tuple<double, double, double> Mean(Image<Bgr, byte> inputImage)
        {
            double bMean = 0;
            double gMean = 0;
            double rMean = 0;

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {

                    bMean += inputImage.Data[y, x, 0];
                    gMean += inputImage.Data[y, x, 1];
                    rMean += inputImage.Data[y, x, 2];
                }
            }
            bMean /= inputImage.Height * inputImage.Width;
            gMean /= inputImage.Height * inputImage.Width;
            rMean /= inputImage.Height * inputImage.Width;


            return Tuple.Create(bMean, gMean, rMean);
        }

        public static double StandardDeviation(Image<Gray, byte> inputImage, double mean)
        {
            double stDev = 0;

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    stDev += (inputImage.Data[y, x, 0] - mean) * (inputImage.Data[y, x, 0] - mean);
                }
            }
            stDev /= inputImage.Height * inputImage.Width - 1;
            stDev = Math.Sqrt(stDev);

            return stDev;
        }

        public static Tuple<double, double, double> StandardDeviation(Image<Bgr, byte> inputImage,
            Tuple<double, double, double> bgrMean)
        {
            double bDev = 0;
            double gDev = 0;
            double rDev = 0;

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    bDev += (inputImage.Data[y, x, 0] - bgrMean.Item1) * (inputImage.Data[y, x, 0] - bgrMean.Item1);
                    gDev += (inputImage.Data[y, x, 1] - bgrMean.Item2) * (inputImage.Data[y, x, 1] - bgrMean.Item2);
                    rDev += (inputImage.Data[y, x, 2] - bgrMean.Item3) * (inputImage.Data[y, x, 2] - bgrMean.Item3);
                }
            }

            bDev /= inputImage.Height * inputImage.Width - 1;
            gDev /= inputImage.Height * inputImage.Width - 1;
            rDev /= inputImage.Height * inputImage.Width - 1;

            bDev = Math.Sqrt(bDev);
            gDev = Math.Sqrt(gDev);
            rDev = Math.Sqrt(rDev);

            return Tuple.Create(bDev, gDev, rDev);
        }
        public static Image<Bgr, byte> AfinOperator(Image<Bgr, byte> inputImage, byte r1, byte s1, byte r2, byte s2)
        {
            Image<Bgr, byte> outputImage = new Image<Bgr, byte>(inputImage.Size);

            Dictionary<byte, byte> op = new Dictionary<byte, byte>();

            op[0] = 0;
            for (byte x = 1; x < r1; ++x)
            {
                op[x] = (byte)Math.Round((double)x * s1 / r1);
            }
            op[r1] = s1;

            for (byte x = (byte)(r1 + 1); x < r2; ++x)
            {
                op[x] = (byte)Math.Round(((double)(x - r1)) * (s2 - s1) / (r2 - r1) + s1);
            }
            op[r2] = s2;
            for (byte x = (byte)(r2 + 1); x < 255; ++x)
            {
                op[x] = (byte)Math.Round(((double)(x - r2)) * (255 - s2) / (255 - r2) + s2);
            }
            op[255] = 255;

            for (int y = 0; y < outputImage.Rows; ++y)
            {
                for (int x = 0; x < outputImage.Cols; ++x)
                {
                    outputImage.Data[y, x, 0] = op[inputImage.Data[y, x, 0]];
                }
            }

            for (int y = 0; y < outputImage.Rows; ++y)
            {
                for (int x = 0; x < outputImage.Cols; ++x)
                {
                    outputImage.Data[y, x, 0] = op[inputImage.Data[y, x, 0]];
                    outputImage.Data[y, x, 1] = op[inputImage.Data[y, x, 1]];
                    outputImage.Data[y, x, 2] = op[inputImage.Data[y, x, 2]];
                }
            }


            return outputImage;
        }
        public static Image<Gray, byte> AfinOperator(Image<Gray, byte> inputImage, byte r1, byte s1, byte r2, byte s2)
        {
            Image<Gray, byte> outputImage = new Image<Gray, byte>(inputImage.Size);

            Dictionary<byte, byte> op = new Dictionary<byte, byte>();

            op[0] = 0;
            for (byte x = 1; x < r1; ++x)
            {
                op[x] = (byte)Math.Round((double)x * s1 / r1);
            }
            op[r1] = s1;

            for (byte x = (byte)(r1 + 1); x < r2; ++x)
            {
                op[x] = (byte)Math.Round(((double)(x - r1)) * (s2 - s1) / (r2 - r1) + s1);
            }
            op[r2] = s2;
            for (byte x = (byte)(r2 + 1); x < 255; ++x)
            {
                op[x] = (byte)Math.Round(((double)(x - r2)) * (255 - s2) / (255 - r2) + s2);
            }
            op[255] = 255;

            for (int y = 0; y < outputImage.Rows; ++y)
            {
                for (int x = 0; x < outputImage.Cols; ++x)
                {
                    outputImage.Data[y, x, 0] = op[inputImage.Data[y, x, 0]];
                }
            }


            return outputImage;
        }

        public static Image<Gray, byte> Crop(Image<Gray, byte> inputImage,
            int leftTopX, int leftTopY, int bottomRightX, int bottomRightY)
        {
            Image<Gray, byte> outputImage = new Image<Gray, byte>(bottomRightX - leftTopX + 1, bottomRightY - leftTopY + 1);


            for (int y = leftTopY; y <= bottomRightY; y++)
            {
                for (int x = leftTopX; x <= bottomRightX; x++)
                {
                    outputImage[y - leftTopY, x - leftTopX] = inputImage[y, x];
                }
            }

            return outputImage;
        }
        public static Image<Bgr, byte> Crop(Image<Bgr, byte> inputImage,
            int leftTopX, int leftTopY, int bottomRightX, int bottomRightY)
        {
            Image<Bgr, byte> outputImage = new Image<Bgr, byte>(bottomRightX - leftTopX + 1, bottomRightY - leftTopY + 1);


            for (int y = leftTopY; y <= bottomRightY; y++)
            {
                for (int x = leftTopX; x <= bottomRightX; x++)
                {
                    outputImage[y - leftTopY, x - leftTopX] = inputImage[y, x];
                }
            }

            return outputImage;
        }


        public static Image<Gray, byte> Mirror(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> outputImage = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    outputImage[y, x] = inputImage[y, inputImage.Width - x - 1];
                }
            }
            return outputImage;
        }

        public static Image<Bgr, byte> Mirror(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> outputImage = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    outputImage.Data[y, x, 0] = inputImage.Data[y, inputImage.Width - x - 1, 0];
                    outputImage.Data[y, x, 1] = inputImage.Data[y, inputImage.Width - x - 1, 1];
                    outputImage.Data[y, x, 2] = inputImage.Data[y, inputImage.Width - x - 1, 2];
                }
            }
            return outputImage;
        }

        public static Image<Gray, byte> Bynarise(Image<Gray, byte> inputImage, int threshHold)
        {
            Image<Gray, byte> outputImage = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    if (inputImage.Data[y, x, 0] > threshHold)
                    {
                        outputImage.Data[y, x, 0] = 255;
                    }
                }
            }

            return outputImage;
        }

        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            var result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            var result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Copy(Image<Bgr, byte> image)
        {
            var result = new Image<Bgr, byte>(image.Size);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(image.Data[y, x, 0]);
                }
            }
            return image;
        }

        public static Image<Gray, byte> Copy(Image<Gray, byte> image)
        {
            var result = new Image<Gray, byte>(image.Size);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(image.Data[y, x, 0]);
                }
            }
            return image;
        }

        public static Image<Gray, byte> Convert(Image<Bgr, byte> coloredImage)
        {
            var grayImage = coloredImage.Convert<Gray, byte>();

            return grayImage;
        }
    }
}