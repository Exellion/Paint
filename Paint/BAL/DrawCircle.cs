using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Paint.BAL
{
    class DrawCircle : DrawShape
    {
        Ellipse ellipse;
        Point center;

        const double angle = 0.785398; //angle in radians, correcponding to 45 degrees

        public override void StartDraw(Canvas canvas)
        {
            center = Mouse.GetPosition(canvas);

            ellipse = new Ellipse();
            ellipse.Stroke = State.StrokeBrush;
            ellipse.Fill = State.FillBrush;
            Canvas.SetLeft(ellipse, center.X);
            Canvas.SetTop(ellipse, center.Y);
            canvas.Children.Add(ellipse);
        }
        
        public override void ProcessDraw(Canvas canvas)
        {
            if (ellipse != null)
            {
                Point cursor = Mouse.GetPosition(canvas);
                double radius = GetRadius(cursor);
                if (center.X - radius < 0 || center.Y - radius < 0 
                    || center.X + radius > canvas.ActualWidth || center.Y + radius > canvas.ActualHeight)
                    return;
                Canvas.SetLeft(ellipse, center.X - radius);
                Canvas.SetTop(ellipse, center.Y - radius);
                double diameter = radius * 2;
                ellipse.Height = diameter;
                ellipse.Width = diameter;                
            }
        }

        public override void EndDraw(Canvas canvas)
        {
            ellipse = null;
        }

        private double GetRadius(Point cursor)
        {
            return Math.Pow(Math.Pow(center.X - cursor.X, 2) + Math.Pow(center.Y - cursor.Y, 2), 0.5);
        }
    }
}
