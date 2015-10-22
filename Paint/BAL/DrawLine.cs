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
    class DrawLine : DrawShape
    {
        Line line;

        public override void StartDraw(Canvas canvas)
        {
            line = new Line();
            line.Stroke = State.StrokeBrush;
            Point cursor = Mouse.GetPosition(canvas);
            line.X1 = cursor.X;
            line.Y1 = cursor.Y;
            canvas.Children.Add(line);
        }
        
        public override void ProcessDraw(Canvas canvas)
        {
            if (line != null)
            {
                Point cursor = Mouse.GetPosition(canvas);
                line.X2 = cursor.X;
                line.Y2 = cursor.Y;
            }
        }

        public override void EndDraw(Canvas canvas)
        {
            line = null;
        }
    }
}
