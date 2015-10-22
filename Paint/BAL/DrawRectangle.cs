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
    class DrawRectangle : DrawShape
    {
        Rectangle rectangle;
        Point firstVertex;

        public override void StartDraw(Canvas canvas)
        {
            firstVertex = Mouse.GetPosition(canvas);

            rectangle = new Rectangle();
            rectangle.Stroke = State.StrokeBrush;
            rectangle.Fill = State.FillBrush;
            Canvas.SetLeft(rectangle, firstVertex.X);
            Canvas.SetTop(rectangle, firstVertex.Y);
            canvas.Children.Add(rectangle);
        }

        public override void ProcessDraw(Canvas canvas)
        {
            if (rectangle != null)
            {
                Point secondVertex = Mouse.GetPosition(canvas);
                double width = secondVertex.X - firstVertex.X;
                double height = secondVertex.Y - firstVertex.Y;
                rectangle.Width = width < 0 ? 0 : width;
                rectangle.Height = height < 0 ? 0 : height;
            }
        }

        public override void EndDraw(Canvas canvas)
        {
            rectangle = null;
        }
    }
}
