using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Paint.BAL
{
    abstract class DrawShape
    {
        public abstract void StartDraw(Canvas canvas);
        public abstract void ProcessDraw(Canvas canvas);
        public abstract void EndDraw(Canvas canvas);
    }
}
