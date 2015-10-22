using Paint.BAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Paint
{
    class Controller
    {
        Canvas canvas;
        ProgressBar progress;
        DrawShape drawShape;        //object that contains logic for drawing selected shape
        ImageHandler imageHandler;  //object thas handles actions with images 
           
        public Controller(Canvas canvas, ProgressBar progress)
        {
            this.canvas = canvas;
            this.progress = progress;
            this.imageHandler = new ImageHandler(canvas, progress);          
        }

        public void SetDrawShape(string btnTag)
        {
            switch(btnTag)
            {
                case "pencil":
                    drawShape = new DrawPolyline();
                    break;
                case "line":
                    drawShape = new DrawLine();
                    break;
                case "rectangle":
                    drawShape = new DrawRectangle();
                    break;
                case "circle":
                    drawShape = new DrawCircle();
                    break;
                default:
                    drawShape = null;
                    break;
            }
        }

        public void StartDraw()
        {
            if (drawShape != null)
            {
                State.isDrawing = true;
                drawShape.StartDraw(canvas);
            }
        }

        public void ProcessDraw()
        {
            if (State.isDrawing)
            {
                drawShape.ProcessDraw(canvas);
            }
        }

        public void EndDraw()
        {
            State.isDrawing = false;
            if (drawShape != null)
                drawShape.EndDraw(canvas);
        }

        public void NewImage()
        {
            if (canvas.Children.Count > 0)
                canvas.Children.Clear();
            canvas.Background = System.Windows.Media.Brushes.White;
        }

        public void SaveImage()
        {
            if (imageHandler.IsInvertActive)
                MessageBox.Show("You can't save image when background proccess is active");
            else
                imageHandler.SaveImage();
        }

        public void LoadImage()
        {
            if (imageHandler.IsInvertActive)
                MessageBox.Show("You can't load image when background proccess is active");
            else
                imageHandler.LoadImage();
        }

        public void InvertImage()
        {
            if (imageHandler.IsInvertActive)
                MessageBox.Show("Invert already running");
            else
                imageHandler.Invert();
        }        
    }
}
