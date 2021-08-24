using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PaintApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LocalBlobDbEntities db = new LocalBlobDbEntities();
            var strokes = from s in db.Tables
                          select s;
            foreach (var item in strokes)
            {
                Console.WriteLine(item.Id);
            }

            this.gridCanvas.ItemsSource = strokes.ToList();
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            var radiobutton = sender as RadioButton;
            string radioBpressed = radiobutton.Content.ToString();
            if (radioBpressed == "Draw")
            {
                this.DrawingCanvas.EditingMode = InkCanvasEditingMode.Ink;
            }
            else if (radioBpressed == "Erase")
            {
                this.DrawingCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
            }

        }
        private void LoadIMGButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = new BitmapImage(new Uri(op.FileName));
                DrawingCanvas.Background = imageBrush;
            }
        }
        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            var radiobutton = sender as RadioButton;
            string radioBpressed = radiobutton.Content.ToString();

            if (radioBpressed == "AirBrush")
            {
                strokeAttribute.StylusTipTransform = new Matrix(1, 0, 0.5, 1, 1, 4);
            }
            else if (radioBpressed == "Pen")
            {
                strokeAttribute.IsHighlighter = false;
                strokeAttribute.StylusTipTransform = new Matrix(1, 0, 0, 1, 0, 0);
            }
            else if (radioBpressed == "Marker")
            {
                strokeAttribute.StylusTipTransform = new Matrix(1, 0, 0, 5, 0, 0);
            }
            else if (radioBpressed == "Highlighter")
            {
                strokeAttribute.IsHighlighter = true;
                strokeAttribute.StylusTip = StylusTip.Ellipse;
            }
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedEventArgs e)
        {
            strokeAttribute.Color = (Color)_colorPicker.SelectedColor;
        }

        //fix this later
        private void NumericUpDown_BrushSizeChanged(object sender, RoutedEventArgs e)
        {
            strokeAttribute.Width = (double)numUpDown.Value;
            strokeAttribute.Height = (double)numUpDown.Value;
         
        }

        //saves new image with ink
        private void SaveBitMapButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                  "Portable Network Graphic (*.png)|*.png";

            if (saveFileDialog1.ShowDialog() == true)
            {
                FileStream fs = new FileStream(saveFileDialog1.FileName,
                                               FileMode.Create);
                int marg = int.Parse(DrawingCanvas.Margin.Left.ToString());
                RenderTargetBitmap rtb =
                        new RenderTargetBitmap((int)DrawingCanvas.ActualWidth - marg,
                                (int)DrawingCanvas.ActualHeight - marg, 0, 0,
                            PixelFormats.Default);
                rtb.Render(DrawingCanvas);
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));

                encoder.Save(fs);
                fs.Close();
            }
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Strokes.Clear();
        }

        //returns byte array of the inkcanvas to store in a DB
        private byte[] getByteArray()
        {
            MemoryStream ms = new MemoryStream();
            DrawingCanvas.Strokes.Save(ms);
            byte[] bytes = ms.ToArray();
            return bytes;
        }

        //saves only the ink /CREATE
        private void SaveInkButton_Click(object sender, RoutedEventArgs e)
        {
            LocalBlobDbEntities db = new LocalBlobDbEntities();

            //accidentally made the table name "table", table here refers to a 
            //new INK object. (I'm scared to touch the DB since its already created)
            Table newInk = new Table()
            {
                CanvasStrokes = getByteArray(),
                CreatedUtc = DateTime.Now
            };
            db.Tables.Add(newInk);
            db.SaveChanges();
        }

        //loads DATA for data grid
        private void LoadInkDataButton_Click(object sender, RoutedEventArgs e)
        {
            LocalBlobDbEntities db = new LocalBlobDbEntities();
            this.gridCanvas.ItemsSource = db.Tables.ToList();
        }

        private void gridCanvas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Table ink = (Table) this.gridCanvas.SelectedItem;
            byte[] bytes = ink.CanvasStrokes;
            Stream stream = new MemoryStream(bytes);
            DrawingCanvas.Strokes = new System.Windows.Ink.StrokeCollection(stream);

        }

        //update
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            LocalBlobDbEntities db = new LocalBlobDbEntities();
        }

      
        //the following code is still a WIP

        /*the idea here was to create a "spray paint" effect by coloring random dots within a radius, I created the method that calculates
        where to put the random dots however I'm struggling to figure out how to edit the stroke attribute to represent the effect*/
        private void SprayPaintButton_Click(object sender, RoutedEventArgs e)
        {
            //double radius = strokeAttribute.Width/2;
            //double area = radius * radius * Math.PI;
            //double dotsPerTick = Math.Ceiling(area / 30);
            //for (int i = 0; i < dotsPerTick; i++)
            //{
            //    double[] offset = randomPointInRadius(radius);
            //}

            // DrawingAttributes spray = new DrawingAttributes();
        }

        private double[] randomPointInRadius(double radius)
        {
            Random rnd = new Random();
            double x = rnd.NextDouble() * radius * 2;
            double y = rnd.NextDouble() * radius * 2;
            if (x * x + y * y <= 1)
            {
                x = x * radius;
                y = y * radius;
            }
            double[] myArray = new double[] { x, y };
            return myArray;
        }

        private void RightMouseUpHandler(object sender,
                             System.Windows.Input.MouseButtonEventArgs e)
        {
            Matrix m = new Matrix();
            m.Scale(1.1d, 1.1d);
            ((InkCanvas)sender).Strokes.Transform(m, true);
        }

        private void ZoomButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string buttonPressed = button.Content.ToString();
            if (buttonPressed == "+")
            {
                DrawingCanvas.Height += 5;
                DrawingCanvas.Width += 5;

            }
            if (buttonPressed == "-")
            {
                DrawingCanvas.Height -= 5;
                DrawingCanvas.Width -= 5;
            }
        }

     

    }
}

