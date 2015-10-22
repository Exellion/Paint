using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Paint.BAL
{
    class DrawPolyline : DrawShape
    {
        Polyline pLine;

        public override void StartDraw(Canvas canvas)
        {
            pLine = new Polyline();
            pLine.Stroke = State.StrokeBrush;
            pLine.Points.Add(Mouse.GetPosition(canvas));
            canvas.Children.Add(pLine);
        }
        
        public override void ProcessDraw(Canvas canvas)
        {
            if (pLine != null)
            {
                pLine.Points.Add(Mouse.GetPosition(canvas));
            }
        }

        public override void EndDraw(Canvas canvas)
        {
            pLine = null;
        }
    }
}
