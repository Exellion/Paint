using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Paint.BAL
{
    static class State
    {
        public static bool isDrawing { get; set; }

        public static Brush StrokeBrush { get; set; }
        public static Brush FillBrush { get; set; }

        static Button currentBtn;       //reference to selected button

        public static void ButtonsManage(Button button)     //receive a button which was clicked
        {
            if (button == null)         //will be true when there will be a click on an empty place of grid
            {
                if (currentBtn != null)
                    currentBtn.Background = null;

                currentBtn = null;
            }
            else
            {
                if (currentBtn == button)
                    return;

                if (currentBtn != null)
                    currentBtn.Background = null;

                currentBtn = button;
                currentBtn.Background = Brushes.Azure;
            }             
        }
    }
}
