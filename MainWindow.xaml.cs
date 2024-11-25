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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Paint;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        Point start;
        Point end;
        private bool isColorPickerActive = false;

        private bool createShape = false;
        private bool isDrawing = false;
        private Point startPoint;
        private enum MyShape
        {
            Line, Ellipse, Rectangle
        }
        private MyShape currShape = MyShape.Line;

        private bool rubber = false;
        private int diameter = (int)Sizes.small;

        private Brush brushcolor = Brushes.Black;
        private bool ispaint = false;
        private bool iserase = false;
        private bool isAddingText = false;
        private enum Sizes
        {
            small = 5,
            medium = 15,
            large = 30
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void paintcircle(Brush circlecolor, Point position)
        {
            Ellipse newellipse = new Ellipse();
            newellipse.Fill = circlecolor;
            newellipse.Width = SizeSlider.Value;
            newellipse.Height = SizeSlider.Value;
            Canvas.SetTop(newellipse, position.Y);
            Canvas.SetLeft(newellipse, position.X);
            paintcanvas.Children.Add(newellipse);
        }

        private DateTime lastDrawTime = DateTime.MinValue;
        private int delay = 1; 

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if ((ispaint || iserase) && !createShape)
            {
                DateTime now = DateTime.Now;
                if ((now - lastDrawTime).TotalMilliseconds > delay)
                {
                    Point currentMousePosition = e.GetPosition(paintcanvas);

                    int additionalCircles = 100;

                    double deltaX = currentMousePosition.X - start.X;
                    double deltaY = currentMousePosition.Y - start.Y;

                    double stepX = deltaX / additionalCircles;
                    double stepY = deltaY / additionalCircles;

                    for (int i = 0; i < additionalCircles; i++)
                    {
                        Point intermediatePosition = new Point(start.X + i * stepX, start.Y + i * stepY);

                        Brush color = (rubber || iserase) ? Brushes.White : brushcolor;

                        paintcircle(color, intermediatePosition);
                    }

                    paintcircle((rubber || iserase) ? Brushes.White : brushcolor, currentMousePosition);

                    start = currentMousePosition;

                    lastDrawTime = now;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int count = paintcanvas.Children.Count;

            if (count > 0)
            {
                paintcanvas.Children.RemoveAt(count - 1);
            }
        }

        private void paintcanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            if (isColorPickerActive)
            {
                Point clickPoint = e.GetPosition(paintcanvas);

                Color pickedColor = GetColorAtPoint(clickPoint);

                brushcolor = new SolidColorBrush(pickedColor);

                isColorPickerActive = false;

            }
            if (isAddingText)
            {
                Point clickPosition = e.GetPosition(paintcanvas);
                AddTextBoxToCanvas(clickPosition);
            }
            else
            {
                start = e.GetPosition(paintcanvas);
                ispaint = true;
                if (currShape != MyShape.Line && createShape == true)
                {
                    startPoint = e.GetPosition(paintcanvas);
                    isDrawing = true;
                }
            }
            

            
        }
        private void paintcanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (createShape)
            {
                end = e.GetPosition(paintcanvas);
                switch (currShape)
                {
                    case MyShape.Line:
                        DrawLine();
                        break;

                    case MyShape.Ellipse:
                        DrawEllipse();
                        break;

                    case MyShape.Rectangle:
                        DrawRectangle();
                        break;

                    default:
                        return;
                }

            }

            ispaint = false;
            isDrawing = false;
        }

        private void Apply_Color(object sender, RoutedEventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                string buttonName = ClickedButton.Name;

                switch (buttonName)
                {
                    case "Black":
                        brushcolor = Brushes.Black;
                        break;
                    case "DarkGray":
                        brushcolor = Brushes.DarkGray;
                        break;
                    case "DarkRed":
                        brushcolor = Brushes.DarkRed;
                        break;
                    case "Red":
                        brushcolor = Brushes.Red;
                        break;
                    case "Orange":
                        brushcolor = Brushes.Orange;
                        break;
                    case "Yellow":
                        brushcolor = Brushes.Yellow;
                        break;
                    case "Green":
                        brushcolor = Brushes.Green;
                        break;
                    case "Blue":
                        brushcolor = Brushes.Blue;
                        break;
                    case "DarkBlue":
                        brushcolor = Brushes.DarkBlue;
                        break;
                    case "Custom1":
                        brushcolor = Brushes.Purple;
                        break;
                    case "Custom2":
                        brushcolor = Brushes.Purple;
                        break;
                    case "Custom3":
                        brushcolor = Brushes.Purple;
                        break;
                    case "Custom4":
                        brushcolor = Brushes.Purple;
                        break;
                    case "Custom5":
                        brushcolor = Brushes.Purple;
                        break;
                    case "Custom6":
                        brushcolor = Brushes.Purple;
                        break;
                    case "Custom7":
                        brushcolor = Brushes.Purple;
                        break;
                    case "Custom8":
                        brushcolor = Brushes.Purple;
                        break;
                    case "Custom9":
                        brushcolor = Brushes.Purple;
                        break;
                    case "Custom10":
                        brushcolor = Brushes.Purple;
                        break;
                    default:
                        brushcolor = Brushes.Black;
                        break;
                }
            }
        }

        private void Active_Eraser(object sender, RoutedEventArgs e)
        {
            rubber = true;
            createShape = false;
            isAddingText = false;
            isColorPickerActive = false;

        }

        private void Active_Paintbrush(object sender, RoutedEventArgs e)
        {
            rubber = false;
            createShape = false;
            isAddingText = false;
            isColorPickerActive = false;

        }


        private Color GetColorAtPoint(Point point)
        {
            var hitTestResult = VisualTreeHelper.HitTest(paintcanvas, point);
            if (hitTestResult != null && hitTestResult.VisualHit is Shape shape)
            {
                var brush = shape.Fill as SolidColorBrush;
                if (brush != null)
                {
                    return brush.Color;
                }
            }

            return Colors.Transparent;
        }




        private void RedrawShapesOnTop()
        {
            foreach (var shape in paintcanvas.Children.OfType<Shape>())
            {
                paintcanvas.Children.Remove(shape);
                paintcanvas.Children.Add(shape);
            }
        }



        private Ellipse CreateFilledEllipse(Brush fillColor, Point position)
        {
            Ellipse newEllipse = new Ellipse()
            {
                Fill = fillColor,
                Width = 1,
                Height = 1
            };
            Canvas.SetTop(newEllipse, position.Y);
            Canvas.SetLeft(newEllipse, position.X);

            return newEllipse;
        }

        private void RectangleButton_Click(object sender, RoutedEventArgs e)
        {
            currShape = MyShape.Rectangle;
            createShape = true;
            isAddingText = false;
            isColorPickerActive = false;

        }

        private void EllipseButton_Click(object sender, RoutedEventArgs e)
        {
            currShape = MyShape.Ellipse;
            createShape = true;
            isAddingText = false;
            isColorPickerActive = false;

        }

        private void LineButton_Click(object sender, RoutedEventArgs e)
        {
            currShape = MyShape.Line;
            createShape = true;
            isAddingText = false;
            isColorPickerActive = false;
            

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            paintcanvas.Children.Clear();
        }

        private void DrawLine()
        {
            Line newLine = new Line()
            {
                Stroke = brushcolor,
                StrokeThickness = SizeSlider.Value,
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y
            };
            paintcanvas.Children.Add(newLine);
        }
        private void DrawEllipse()
        {
            Ellipse newEllipse = new Ellipse()
            {
                Stroke = brushcolor,
                StrokeThickness = SizeSlider.Value,
                Height = 10,
                Width = 10
            };

            if (end.X >= start.X)
            {
                newEllipse.SetValue(Canvas.LeftProperty, start.X);
                newEllipse.Width = end.X - start.X;
            }
            else
            {
                newEllipse.SetValue(Canvas.LeftProperty, end.X);
                newEllipse.Width = start.X - end.X;
            }


            if (end.Y >= start.Y)
            {
                newEllipse.SetValue(Canvas.TopProperty, start.Y);
                newEllipse.Height = end.Y - start.Y;
            }
            else
            {
                newEllipse.SetValue(Canvas.TopProperty, end.Y);
                newEllipse.Height = start.Y - end.Y;
            }


            if (checkbox_wypelnienie.IsChecked == false)
            {
                newEllipse.Fill = Brushes.Transparent;
            }
            else
            {
                newEllipse.Fill = brushcolor;
            }

            paintcanvas.Children.Add(newEllipse);
        }
        private void DrawRectangle()
        {
            Rectangle newRectangle = new Rectangle()
            {
                Stroke = brushcolor,
                StrokeThickness = SizeSlider.Value
            };

            if (end.X >= start.X)
            {
                newRectangle.SetValue(Canvas.LeftProperty, start.X);
                newRectangle.Width = end.X - start.X;
            }
            else
            {
                newRectangle.SetValue(Canvas.LeftProperty, end.X);
                newRectangle.Width = start.X - end.X;
            }

            if (end.Y >= start.Y)
            {
                newRectangle.SetValue(Canvas.TopProperty, start.Y);
                newRectangle.Height = end.Y - start.Y;
            }
            else
            {
                newRectangle.SetValue(Canvas.TopProperty, end.Y);
                newRectangle.Height = start.Y - end.Y;
            }
            if (checkbox_wypelnienie.IsChecked == false)
            {
                newRectangle.Fill = Brushes.Transparent;
            }
            else
            {
                newRectangle.Fill = brushcolor;
            }


            paintcanvas.Children.Add(newRectangle);
        }

        private void Nowy_Click(object sender, RoutedEventArgs e)
        {
            OnOpenCanvasSettings(sender, e);
        }

        private void OnOpenCanvasSettings(object sender, RoutedEventArgs e)
        {
            var canvasSettingsWindow = new CanvasSettingsWindow();
            if (canvasSettingsWindow.ShowDialog() == true)
            {
                int canvasWidth = canvasSettingsWindow.CanvasWidth;
                int canvasHeight = canvasSettingsWindow.CanvasHeight;
                bool isTransparent = canvasSettingsWindow.IsTransparent;

                paintcanvas.Width = canvasWidth;
                paintcanvas.Height = canvasHeight;
                paintcanvas.Children.Clear();
                if (isTransparent)
                {
                    paintcanvas.Background = Brushes.Transparent;
                }
                else
                {
                    paintcanvas.Background = Brushes.White;
                }
            }
        }

        private void Zakoncz_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ZapiszJako_Click(object sender, RoutedEventArgs e)
        {
            paintcanvas.Measure(new Size(paintcanvas.Width, paintcanvas.Height));
            paintcanvas.Arrange(new Rect(new Size(paintcanvas.Width, paintcanvas.Height)));

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)paintcanvas.Width, (int)paintcanvas.Height,
                96d, 96d, PixelFormats.Default);

            renderBitmap.Render(paintcanvas);

            BitmapEncoder encoder = null;

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Obrazek";
            dlg.DefaultExt = ".png";
            dlg.Filter = "Obrazy PNG (.png)|*.png|Obrazy JPEG (.jpeg)|*.jpeg|Obrazy BMP (.bmp)|*.bmp|Wszystkie pliki|*.*";

            if (dlg.ShowDialog() == true)
            {
                string selectedExt = System.IO.Path.GetExtension(dlg.FileName).ToLower();

                switch (selectedExt)
                {
                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;

                    case ".jpeg":
                        encoder = new JpegBitmapEncoder();
                        break;

                    case ".bmp":
                        encoder = new BmpBitmapEncoder();
                        break;

                    default:
                        MessageBox.Show("Nieobsługiwany format pliku");
                        return;
                }

                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create))
                {
                    encoder.Save(fs);
                }
            }
        }

        private void pelen_ekran(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void minimalizuj_ekran(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void AddTextBoxToCanvas(Point position)
        {
            TextBox textBox = new TextBox
            {
                Width = SizeSlider.Value*15,
                Height = SizeSlider.Value * 3.3,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1.5),
                Background = Brushes.Transparent,
                Foreground = brushcolor,
                FontSize = (SizeSlider.Value)*2,
                FontWeight = FontWeights.Normal,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            textBox.LostFocus += (sender, e) =>
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = textBox.Text,
                    Foreground = brushcolor,
                    FontSize = (SizeSlider.Value)*2,
                    FontWeight = FontWeights.Normal,
                    TextWrapping = TextWrapping.Wrap
                };

                Canvas.SetLeft(textBlock, position.X);
                Canvas.SetTop(textBlock, position.Y);

                paintcanvas.Children.Add(textBlock);
                paintcanvas.Children.Remove(textBox);
            };

            Canvas.SetLeft(textBox, position.X);
            Canvas.SetTop(textBox, position.Y);

            paintcanvas.Children.Add(textBox);
            textBox.Focus();
        }
        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            isAddingText = !isAddingText;
            isColorPickerActive = false;
            createShape = false;
            isDrawing = false;
            iserase = false;
        }
        private void ColorPickerButton_Click(object sender, RoutedEventArgs e)
        {
            
            isColorPickerActive = !isColorPickerActive;
            

            if (isColorPickerActive)
            {
                //To be continued... :)))))))))
            }
            else
            {
                
            }
        }

        private void zmniejsz_ekran(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }
    }
    

}

