using Paint.BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Controller controller;

        public MainWindow()
        {
            InitializeComponent();
            this.controller = new Controller(canvas, progressbar);
            State.StrokeBrush = new SolidColorBrush(strokeColor.SelectedColor ?? Brushes.Black.Color);
            State.FillBrush = new SolidColorBrush(fillColor.SelectedColor ?? Brushes.White.Color);
        }

        private void grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            State.ButtonsManage(null);      //remove selection from current button
            controller.SetDrawShape("");       
        }

        private void drawBtn_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            controller.SetDrawShape(button.Tag.ToString());

            State.ButtonsManage(button);    //set pressed button as selected
        }

        private void new_Click(object sender, RoutedEventArgs e)
        {
            controller.NewImage();
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            controller.SaveImage();
        }

        private void load_Click(object sender, RoutedEventArgs e)
        {
            controller.LoadImage();
        }

        private void invert_Click(object sender, RoutedEventArgs e)
        {
            controller.InvertImage();
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;               //stop event bubbling
            controller.StartDraw();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            controller.ProcessDraw();       
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            controller.EndDraw();
        }

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            State.isDrawing = false;
        }

        private void lineColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            ColorPicker picker = (ColorPicker)sender;
            State.StrokeBrush = new SolidColorBrush(picker.SelectedColor ?? Brushes.Black.Color);
        }

        private void backgroundColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            ColorPicker picker = (ColorPicker)sender;
            State.FillBrush = new SolidColorBrush(picker.SelectedColor ?? Brushes.White.Color);
        }
    }
}
