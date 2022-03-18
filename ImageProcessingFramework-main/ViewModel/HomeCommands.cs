using Emgu.CV;
using Emgu.CV.Structure;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ImageProcessingAlgorithms.Tools;
using ImageConverter = ImageProcessingFramework.Model.ImageConverter;
using static ImageProcessingFramework.Model.DataProvider;
using ImageProcessingFramework.View;
using System.Collections.Generic;
using ImageProcessingFramework.Model;
using System;

namespace ImageProcessingFramework.ViewModel
{
    class HomeCommands : INotifyPropertyChanged
    {
        public ImageSource InitialImage { get; set; }

        public ImageSource ProcessedImage { get; set; }



        private ICommand m_rowDisplay;
        private ICommand m_loadColorImage;
        private ICommand m_loadGrayImage;
        private ICommand m_exitWindow;
        private ICommand m_resetButton;
        private ICommand m_saveAsOriginalImage;
        private ICommand m_invertImage;
        private ICommand m_saveImage;
        private ICommand m_copyImage;
        private ICommand m_convertToGrayImage;
        private ICommand m_magnifier;
        private ICommand m_removeAllElements;
        private bool m_isColorImage;
        private bool m_isPressedConvertButton;

        //my commands
        private ICommand m_bynariseImage;
        private ICommand m_mirrorImage;
        private ICommand m_cropImage;
        private ICommand m_afinOperator;
        //
        //my methods

        public void AfinOperator(object parameter)
        {
            if (GrayInitialImage != null || ColorInitialImage != null)
            {
                DialogBox dialogBox = new DialogBox();
                List<string> props = new List<string>();
                props.Add("r1");
                props.Add("s1");
                props.Add("r2");
                props.Add("s2");

                dialogBox.CreateDialogBox(props);

                dialogBox.ShowDialog();

                List<double> response = dialogBox.GetResponseTexts();

                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = Tools.AfinOperator(GrayInitialImage,
                        (byte)response[0], (byte)response[1], (byte)response[2], (byte)response[3]);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    OnPropertyChanged("ProcessedImage");
                }
                else if(ColorInitialImage !=null)
                {
                    ColorProcessedImage = Tools.AfinOperator(ColorInitialImage,
                        (byte)response[0], (byte)response[1], (byte)response[2], (byte)response[3]);
                    ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                    OnPropertyChanged("ProcessedImage");
                }
            }
            else
            {
                MessageBox.Show("Please add an image");
            }
        }
        public void MirrorImage(object parameter)
        {
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Mirror(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Mirror(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else
                MessageBox.Show("Please add an image!");
        }
        public void BynariseImage(object parameter)
        {

            if (GrayInitialImage != null)
            {
                DialogBox dialogBox = new DialogBox();
                List<string> props = new List<string>();
                props.Add("threshhold");
                dialogBox.CreateDialogBox(props);

                dialogBox.ShowDialog();

                var response = dialogBox.GetResponseTexts();

                int threshhold = (int)response[0];

                GrayProcessedImage = Tools.Bynarise(GrayInitialImage, threshhold);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else if (ColorInitialImage != null)
            {
                MessageBox.Show("Image must be grayscale");
            }
            else
            {
                MessageBox.Show("Please add an image!");
            }
        }

        public void CropImage(object parameter)
        {
            if (VectorOfMousePosition.Count < 2)
            {
                MessageBox.Show("Select two points");
            }
            else if (GrayInitialImage != null || ColorInitialImage != null)
            {
                int leftTopX = (int)VectorOfMousePosition[VectorOfMousePosition.Count - 2].X;
                int leftTopY = (int)VectorOfMousePosition[VectorOfMousePosition.Count - 2].Y;
                int rightBottomX = (int)VectorOfMousePosition[VectorOfMousePosition.Count - 1].X;
                int rightBottomY = (int)VectorOfMousePosition[VectorOfMousePosition.Count - 1].Y;

                if (leftTopX > rightBottomX || leftTopY > rightBottomY)
                {
                    MessageBox.Show("Please select top left and bottom right points");
                }

                else
                {
                    VectorOfRectangles.Add(DrawHelper.DrawRectangle
                    (InitialCanvas, leftTopX, leftTopY, rightBottomX, rightBottomY, 2, Brushes.Red));

                    if (GrayInitialImage != null)
                    {

                        GrayProcessedImage = Tools.Crop(GrayInitialImage, leftTopX, leftTopY, rightBottomX, rightBottomY);
                        ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                        OnPropertyChanged("ProcessedImage");

                        double mean = Tools.Mean(GrayProcessedImage);
                        double stDev = Tools.StandardDeviation(GrayProcessedImage, mean);

                        MessageBox.Show($"Mean : {mean}" + Environment.NewLine + $"Standard deviation: {stDev}");
                    }
                    else if (ColorInitialImage != null)
                    {
                        ColorProcessedImage = Tools.Crop(ColorInitialImage, leftTopX, leftTopY, rightBottomX, rightBottomY);
                        ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                        OnPropertyChanged("ProcessedImage");

                        var bgrMean = Tools.Mean(ColorProcessedImage);
                        var bgrDev = Tools.StandardDeviation(ColorProcessedImage, bgrMean);




                        MessageBox.Show($"Mean: R:{String.Format("{0:0.00}", bgrMean.Item3)} " +
                            $"G:{String.Format("{0:0.00}", bgrMean.Item2)} " +
                            $"B:{String.Format("{0:0.00}", bgrMean.Item1)}" + Environment.NewLine +
                            $"Standard Deviation : R:{String.Format("{0:0.00}", bgrDev.Item3)} " +
                            $"G:{String.Format("{0:0.00}", bgrDev.Item2)} " +
                            $"B:{String.Format("{0:0.00}", bgrDev.Item1)}");

                    }
                }

            }
            else
            {
                MessageBox.Show("Please select an image!");
            }
        }

        /////////////////////
        public ICommand Bynarise
        {
            get
            {
                if (m_bynariseImage == null)
                    m_bynariseImage = new RelayCommand(BynariseImage);

                return m_bynariseImage;
            }
        }


        public ICommand Afin
        {
            get
            {
                if (m_afinOperator == null)
                {
                    m_afinOperator = new RelayCommand(AfinOperator);
                }
                return m_afinOperator;
            }
        }
        public ICommand Mirror
        {
            get
            {
                if (m_mirrorImage == null)
                {
                    m_mirrorImage = new RelayCommand(MirrorImage);
                }
                return m_mirrorImage;
            }
        }

        public ICommand Crop
        {
            get
            {
                if (m_cropImage == null)
                {
                    m_cropImage = new RelayCommand(CropImage);
                }
                return m_cropImage;
            }
        }

        ////////////////////


        public ICommand RemoveAllElements
        {
            get
            {
                if (m_removeAllElements == null)
                    m_removeAllElements = new RelayCommand(RemoveAllDrawnElements);
                return m_removeAllElements;
            }
        }


        public void RemoveAllDrawnElements(object parameter)
        {
            UiHelper.RemoveAllDrawnLines(InitialCanvas, ProcessedCanvas, VectorOfLines);
            UiHelper.RemoveAllDrawnRectangles(InitialCanvas, ProcessedCanvas, VectorOfRectangles);
            UiHelper.RemoveAllDrawnEllipses(InitialCanvas, ProcessedCanvas, VectorOfEllipses);
            UiHelper.RemoveAllDrawnPolygons(InitialCanvas, ProcessedCanvas, VectorOfPolygons);
        }

        private void ResetUiToInitial(object parameter)
        {
            ColorInitialImage = null;
            ColorProcessedImage = null;
            GrayInitialImage = null;
            GrayProcessedImage = null;
            InitialImage = null;
            ProcessedImage = null;
            m_isPressedConvertButton = false;
            MagnifierOn = false;
            GLevelsrowOn = false;
            OnPropertyChanged("InitialImage");
            OnPropertyChanged("ProcessedImage");

            ResetZoom(parameter);

            CloseAllWindows();
        }

        private static void CloseAllWindows()
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 1; intCounter--)
                App.Current.Windows[intCounter].Close();
        }

        public void LoadColorImage(object parameter)
        {
            ResetUiToInitial(parameter);

            var op = new OpenFileDialog
            {
                Title = "Select a picture",
                Filter = "Image files(*.jpg, *.jpeg, *.jpe, *.bmp, *.png) | *.jpg; *.jpeg; *.jpe; *.bmp; *.png"
            };
            op.ShowDialog();
            if (op.FileName.CompareTo("") == 0)
                return;

            ColorInitialImage = new Image<Bgr, byte>(op.FileName);
            InitialImage = ImageConverter.Convert(ColorInitialImage);
            OnPropertyChanged("InitialImage");
            m_isColorImage = true;
        }

        public void LoadGrayImage(object parameter)
        {
            ResetUiToInitial(parameter);

            var op = new OpenFileDialog
            {
                Title = "Select a picture",
                Filter = "Image files(*.jpg, *.jpeg, *.jpe, *.bmp, *.png) | *.jpg; *.jpeg; *.jpe; *.bmp; *.png"
            };
            op.ShowDialog();
            if (op.FileName.CompareTo("") == 0)
                return;

            GrayInitialImage = new Image<Gray, byte>(op.FileName);
            InitialImage = ImageConverter.Convert(GrayInitialImage);
            OnPropertyChanged("InitialImage");
            m_isColorImage = false;
        }


        public void InvertImage(object parameter)
        {
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Invert(GrayInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                OnPropertyChanged("ProcessedImage");
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Invert(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                OnPropertyChanged("ProcessedImage");


            }
            else
                MessageBox.Show("Please add an image!");
        }

        public void SaveImage(object parameter)
        {
            if (GrayProcessedImage == null && ColorProcessedImage == null)
            {
                MessageBox.Show("If you want to save your processed image, please add and process an image first!");
                return;
            }

            SaveFileDialog saveFile = new SaveFileDialog
            {
                FileName = "image.jpg",
                Filter = "Image files(*.jpg, *.jpeg, *.jpe, *.bmp, *.png) | *.jpg; *.jpeg; *.jpe; *.bmp; *.png"
            };

            saveFile.ShowDialog();

            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(
                System.Drawing.Imaging.Encoder.Quality,
                (long)100
            );

            var jpegCodec = (from codec in ImageCodecInfo.GetImageEncoders()
                             where codec.MimeType == "image/jpeg"
                             select codec).Single();

            if (GrayProcessedImage != null)
                GrayProcessedImage.Bitmap.Save(saveFile.FileName, jpegCodec, encoderParams);

            if (ColorProcessedImage != null)
                ColorProcessedImage.Bitmap.Save(saveFile.FileName, jpegCodec, encoderParams);
        }

        public void ExitWindow(object parameter)
        {
            System.Environment.Exit(0);
        }

        public void ResetZoom(object parameter)
        {
            if (parameter is Slider slider)
                slider.Value = 1;
            OnPropertyChanged("buttonResetZoom");
        }

        public void SaveAsOriginal(object parameter)
        {
            switch (m_isColorImage)
            {
                case true:
                    {
                        if (ColorProcessedImage == null && GrayProcessedImage == null)
                        {
                            System.Windows.MessageBox.Show("Doesn't exist processed image.");
                            return;
                        }

                        if (m_isPressedConvertButton == true)
                        {
                            GrayInitialImage = GrayProcessedImage;
                            InitialImage = ImageConverter.Convert(GrayInitialImage);
                            GrayProcessedImage = null;
                        }
                        else
                        {
                            ColorInitialImage = ColorProcessedImage;
                            InitialImage = ImageConverter.Convert(ColorInitialImage);
                            ColorProcessedImage = null;
                        }

                        ProcessedImage = null;
                        OnPropertyChanged("InitialImage");
                        OnPropertyChanged("ProcessedImage");
                        break;
                    }

                case false:
                    {
                        if (GrayProcessedImage == null)
                        {
                            System.Windows.MessageBox.Show("Doesn't exist processed image.");
                            return;
                        }

                        GrayInitialImage = GrayProcessedImage;
                        InitialImage = ImageConverter.Convert(GrayInitialImage);
                        GrayProcessedImage = null;
                        ProcessedImage = null;
                        OnPropertyChanged("InitialImage");
                        OnPropertyChanged("ProcessedImage");
                        break;
                    }
            }
        }

        public void CopyImage(object parameter)
        {
            if (m_isColorImage == true)
            {
                ColorProcessedImage = Tools.Copy(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(ColorProcessedImage);
                OnPropertyChanged("ProcessedImage");
                return;
            }
            else
            {
                if (GrayInitialImage != null)
                {
                    GrayProcessedImage = Tools.Copy(GrayInitialImage);
                    ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                    OnPropertyChanged("ProcessedImage");
                    return;
                }
            }
            System.Windows.MessageBox.Show("Please add an image.");
        }

        public void ConvertToGray(object parameter)
        {
            if (m_isColorImage == true)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                ProcessedImage = ImageConverter.Convert(GrayProcessedImage);
                ColorProcessedImage = null;
                OnPropertyChanged("ProcessedImage");
                m_isPressedConvertButton = true;
                m_isColorImage = false;
                return;
            }

            System.Windows.MessageBox.Show(ColorInitialImage != null
               ? "It is possible to copy only colored images."
               : "Please add a colored image first.");
        }

        public void GrayLevelsRow(object parameter)
        {
            if (GLevelsrowOn == true) return;
            var rowDisplayWindow = new RowDisplayWindow();
            rowDisplayWindow.Show();
            GLevelsrowOn = true;
        }

        public void MagnifierShow(object parameter)
        {
            if (MagnifierOn == true) return;
            var magnifierWindow = new MagnifierWindow();
            magnifierWindow.Show();
            MagnifierOn = true;
        }

        public ICommand AddColorImage
        {
            get
            {
                if (m_loadColorImage == null)
                    m_loadColorImage = new RelayCommand(LoadColorImage);
                return m_loadColorImage;
            }
        }



        public ICommand AddGrayImage
        {
            get
            {
                if (m_loadGrayImage == null)
                    m_loadGrayImage = new RelayCommand(LoadGrayImage);
                return m_loadGrayImage;
            }
        }

        public ICommand Exit
        {
            get
            {
                if (m_exitWindow == null)
                    m_exitWindow = new RelayCommand(ExitWindow);
                return m_exitWindow;
            }

        }

        public ICommand Reset
        {
            get
            {
                if (m_resetButton == null)
                    m_resetButton = new RelayCommand(ResetZoom);
                return m_resetButton;
            }
        }

        public ICommand SaveAsOriginalImage
        {
            get
            {
                if (m_saveAsOriginalImage == null)
                    m_saveAsOriginalImage = new RelayCommand(SaveAsOriginal);
                return m_saveAsOriginalImage;
            }
        }

        public ICommand Invert
        {
            get
            {
                if (m_invertImage == null)
                    m_invertImage = new RelayCommand(InvertImage);
                return m_invertImage;
            }
        }

        public ICommand Copy
        {
            get
            {
                if (m_copyImage == null)
                    m_copyImage = new RelayCommand(CopyImage);
                return m_copyImage;
            }
        }

        public ICommand Save
        {
            get
            {
                if (m_saveImage == null)
                    m_saveImage = new RelayCommand(SaveImage);
                return m_saveImage;
            }
        }

        public ICommand ConvertToGrayImage
        {
            get
            {
                if (m_convertToGrayImage == null)
                    m_convertToGrayImage = new RelayCommand(ConvertToGray);
                return m_convertToGrayImage;
            }
        }

        public ICommand RowDisplay
        {
            get
            {
                if (m_rowDisplay == null)
                    m_rowDisplay = new RelayCommand(GrayLevelsRow);
                return m_rowDisplay;
            }
        }

        public ICommand Magnifier
        {
            get
            {
                if (m_magnifier == null)
                    m_magnifier = new RelayCommand(MagnifierShow);
                return m_magnifier;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}