using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Paint.BAL
{
    class ImageHandler
    {
        Canvas canvas;
        ProgressBar progress;
        BackgroundWorker worker;        //will be run invert in separate thread

        public ImageHandler(Canvas canvas, ProgressBar progress)
        {
            this.canvas = canvas;
            this.progress = progress;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = false;

            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        public bool IsInvertActive
        {
            get { return worker.IsBusy; }
        }

        public void SaveImage()
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.FileName = "Image";
            svd.DefaultExt = "*.png";
            svd.Filter = "Bitmap Image (.bmp)|*.bmp|PNG Image (.png)|*.png|JPEG Image (.jpeg)|*.jpeg";

            bool? result = svd.ShowDialog();

            string fileName = "";
            string extension = "";
            if (result != null)
            {
                fileName = svd.FileName;
                extension = System.IO.Path.GetExtension(fileName);
            }
                
            if (fileName == "" || result == false)
                return;

            Thickness margin = canvas.Margin;
            canvas.Margin = new Thickness(0);

            RenderTargetBitmap canvasBitmap = GetCanvasBitmap();

            BitmapEncoder encoder = GetEncoderByExtension(extension);
            if (encoder != null)
            {
                using (FileStream outStream = new FileStream(fileName, FileMode.Create))
                {
                    encoder.Frames.Add(BitmapFrame.Create(canvasBitmap));
                    encoder.Save(outStream);
                }
            }
            
            canvas.Margin = margin;
        }

        public void LoadImage()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Bitmap Image (.bmp)|*.bmp|PNG Image (.png)|*.png|JPEG Image (.jpeg)|*.jpeg|JPG Image (.jpg)|*.jpg";

            bool? result = ofd.ShowDialog();

            string fileName = "";
            if (result != null)
                fileName = ofd.FileName;

            if (fileName == "")
                return;

            canvas.Children.Clear();
            BitmapImage image;
            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                stream.Position = 0;
                image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
            }
            canvas.Background = new ImageBrush(image);
        }

        public void Invert()
        {
            Thickness margin = canvas.Margin;
            canvas.Margin = new Thickness(0);

            RenderTargetBitmap canvasBitmap = GetCanvasBitmap();

            System.Drawing.Bitmap bitmap;
            using (MemoryStream stream = new MemoryStream())        //get Bitmap object from RenderTargetBitmap object
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(canvasBitmap));
                encoder.Save(stream);
                bitmap = new System.Drawing.Bitmap(stream);
            }

            canvas.Margin = margin;
            progress.Value = 0;
            progress.Visibility = Visibility.Visible;
            worker.RunWorkerAsync(bitmap);                          //run separate thread
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {           
            try
            {
                System.Drawing.Bitmap bitmap = e.Argument as System.Drawing.Bitmap;
                BackgroundWorker worker = sender as BackgroundWorker;
                int x;
                for (x = 0; x < bitmap.Width; x++)      //invert color for each pixel on bitmap
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        System.Drawing.Color oldColor = bitmap.GetPixel(x, y);
                        System.Drawing.Color newColor;
                        newColor = System.Drawing.Color.FromArgb(oldColor.A, 255 - oldColor.R, 255 - oldColor.G, 255 - oldColor.B);
                        bitmap.SetPixel(x, y, newColor);
                    }
                    worker.ReportProgress((x * 100) / bitmap.Width);
                    Thread.Sleep(10);
                }

                e.Result = bitmap;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > progress.Minimum && e.ProgressPercentage < progress.Maximum)
                progress.Value = e.ProgressPercentage;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            System.Drawing.Bitmap bitmap = e.Result as System.Drawing.Bitmap;

            if (bitmap != null)
            {
                BitmapImage newImage;

                using (MemoryStream stream = new MemoryStream())        //convert Bitmap to BitmapImage
                {
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Position = 0;
                    newImage = new BitmapImage();
                    newImage.BeginInit();
                    newImage.StreamSource = stream;
                    newImage.CacheOption = BitmapCacheOption.OnLoad;
                    newImage.EndInit();
                }

                canvas.Background = new ImageBrush(newImage);

                if (canvas.Children.Count > 0)
                {
                    foreach (Shape shape in canvas.Children)            //invert brushes for all canvas children
                    {
                        SolidColorBrush stroke = shape.Stroke as SolidColorBrush;
                        if (stroke != null)
                        {
                            shape.Stroke = InvertBrush(stroke);
                        }

                        SolidColorBrush fill = shape.Fill as SolidColorBrush;
                        if (fill != null)
                        {
                            shape.Fill = InvertBrush(fill);
                        }
                    }
                }

                progress.Visibility = Visibility.Hidden;
            }          
        }      

        private SolidColorBrush InvertBrush(SolidColorBrush oldBrush)
        {
            Color oldColor = oldBrush.Color;
            Color newColor = Color.FromArgb(oldColor.A, Convert.ToByte(255 - oldColor.R), Convert.ToByte(255 - oldColor.G), Convert.ToByte(255 - oldColor.B));
            return new SolidColorBrush(newColor);
        }

        private RenderTargetBitmap GetCanvasBitmap()
        {
            try
            {
                System.Windows.Size size = new System.Windows.Size(canvas.ActualWidth, canvas.ActualHeight);
                canvas.Measure(size);
                canvas.Arrange(new Rect(size));

                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                    (int)size.Width,
                    (int)size.Height,
                    96d,
                    96d,
                    PixelFormats.Pbgra32);
                renderBitmap.Render(canvas);
                return renderBitmap;
            }            
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private BitmapEncoder GetEncoderByExtension(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".bmp":
                    return new BmpBitmapEncoder();
                case ".png":
                    return new PngBitmapEncoder();
                case ".jpeg":
                    return new JpegBitmapEncoder();
                default:
                    return null;
            }
        }
    }
}