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
using User.SoftWare;

namespace User.UI
{
    /// <summary>
    /// ColorPicker.xaml 的交互逻辑
    /// </summary>
    public partial class ColorPicker : UControl
    {
        public ColorPicker()
        {
            InitializeComponent();
            this.SlideBarL.SlideValueChanged += SlideBarL_SlideValueChanged;
            this.SlideBarA.SlideValueChanged += SlideBarA_SlideValueChanged;
            tbx[0] = TbxA;
            tbx[1] = TbxR;
            tbx[2] = TbxG;
            tbx[3] = TbxB;
            foreach (var item in tbx)
            {
                item.GotFocus += Tbx_GotFocus;
                item.SelectionChanged += Tbx_SelectionChanged;
            }
        }

        private void Tbx_GotFocus(object sender, RoutedEventArgs e)
        {
            for (int i =0;i< tbx.Length; i++)
            {
                if (sender.Equals(tbx[i]))
                {
                    tbxfocus[i] = true;
                    return;
                }
            }
        }
        private void Tbx_SelectionChanged(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < tbx.Length; i++)
            {
                if (sender.Equals(tbx[i]) && tbxfocus[i])
                {
                    tbxfocus[i] = false;
                    tbx[i].SelectAll();
                    return;
                }
            }
        }
        private bool isLeftMouseDown = false;
        private ColorP valueOld = new ColorP(new Point(), 255, 255);
        private TextBox[] tbx = new TextBox[4];
        private bool[] tbxfocus = new bool[4];
        public ColorP Value
        {
            get { return (ColorP)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(ColorP), typeof(ColorPicker), new PropertyMetadata(new ColorP(Colors.Red), new PropertyChangedCallback(Value_Changed)));
        public event PropertyChangedEventHander<ColorP> ChooseOkOrCancel;
        public event PropertyChangedEventHander<ColorP> ValueChanged;

        private void SlideBarL_SlideValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Value = new ColorP(Value.Location, (byte)e.NewValue, Value.Alpha);
        }
        private void SlideBarA_SlideValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Value = new ColorP(Value.Location, Value.Light, (byte)e.NewValue);
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isLeftMouseDown = true;
                Size size = new Size(this.Width - 50, this.Height - 100);
                Point p = new Point(Tools.Checkdouble((e.GetPosition(GridColors).X) / size.Width * 6.0, 0, 6.0), Tools.Checkdouble(e.GetPosition(GridColors).Y / size.Height, 0, 1.0));
                Value = new ColorP(p, Value.Light, Value.Alpha);
            }
        }
        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && isLeftMouseDown)
            {
                Size size = new Size(this.Width - 50, this.Height - 100);
                Point p = new Point(Tools.Checkdouble((e.GetPosition(GridColors).X) / size.Width * 6.0, 0, 6.0), Tools.Checkdouble(e.GetPosition(GridColors).Y / size.Height, 0, 1.0));
                Value = new ColorP(p, Value.Light, Value.Alpha);
            }
        }
        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isLeftMouseDown = false;
        }
        private void BdrColorCurrent_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                SetColorNew(); 
            }
        }
        private void BdrColorOld_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (    e.ChangedButton ==  MouseButton.Left)
            {
                SetColorOld(); 
            }
        }
        private void TbxOk_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                SetColorNew();
            }
        }
        private void TbxCancel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                SetColorOld();
            }
        }
        protected void OnValueChanged()
        {
            DrawSquare();
            MoveSquare();
            Draw();
            Color color = Value.GetColor();
            TbxA.Text = color.A.ToString();
            TbxR.Text = color.R.ToString();
            TbxG.Text = color.G.ToString();
            TbxB.Text = color.B.ToString();
            BdrColorCurrent.Background = new SolidColorBrush(color);
            ValueChanged?.Invoke(this, new PropertyChangedEventargs<ColorP>(ColorP.Empty, Value));
        }

        private static void Value_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker arg = (ColorPicker)d;
            arg.OnValueChanged();
        }

        private void DrawSquare()
        {
            Color[,] colors = Value.GetColorSquares();
            Border[] borders = new Border[20];
            foreach (var item in GridColors.Children)
            {
                if (item.GetType() == typeof(Border))
                {
                    var border = (Border)item;
                    int index = int.Parse(border.Name.Substring(6)) - 1;
                    borders[index] = border;
                }
            }
            for (int i = 0; i < borders.Length; i++)
            {
                GradientStop[] gradientstops = new GradientStop[7];
                for (int j = 0; j < gradientstops.Length; j++)
                {
                    gradientstops[j] = new GradientStop(colors[i, j % 6], j / 6.0);
                }
                LinearGradientBrush brush = new LinearGradientBrush()
                {
                    StartPoint = new Point(0, 0.5),
                    EndPoint = new Point(1, 0.5),
                    GradientStops = new GradientStopCollection(gradientstops)
                };
                borders[i].Background = brush;
            }
        }
        private void MoveSquare()
        {
            Color color = Value.GetColor();
            if (color.A + color.R + color.G + color.B > 630)
            {
                Elp.Stroke = Brushes.Black;
            }
            else
            {
                Elp.Stroke = Brushes.White;
            }
            Size size = new Size(this.Width - 50, this.Height - 100);
            Elp.Margin = new Thickness(5 + Value.Location.X / 6 * size.Width, 5 + Value.Location.Y * size.Height, 0, 0);
        }
        private void Draw()
        {
            Color color = Value.GetColor();
            Color colorL1 = ColorP.GetMaxColor(color, out double bi);
            colorL1.A = Value.Alpha;
            Color colorL0 = Color.FromArgb(Value.Alpha, 0, 0, 0);
            GradientStop[] gradientstops1 = new GradientStop[2];
            gradientstops1[0] = new GradientStop(colorL0, 0);
            gradientstops1[1] = new GradientStop(colorL1, 1);
            LinearGradientBrush brush1 = new LinearGradientBrush()
            {
                StartPoint = new Point(0, 0.5),
                EndPoint = new Point(1, 0.5),
                GradientStops = new GradientStopCollection(gradientstops1)
            };
            SlideBarL.SlideBrush = brush1;
            SlideBarL.SlideValue = Value.Light;

            Color colorA1 = Color.FromArgb(255, color.R, color.G, color.B);
            Color colorA0 = Color.FromArgb(0, color.R, color.G, color.B);
            GradientStop[] gradientstops2 = new GradientStop[2];
            gradientstops2[0] = new GradientStop(colorA0, 0);
            gradientstops2[1] = new GradientStop(colorA1, 1);
            LinearGradientBrush brush2 = new LinearGradientBrush()
            {
                StartPoint = new Point(0, 0.5),
                EndPoint = new Point(1, 0.5),
                GradientStops = new GradientStopCollection(gradientstops2)
            };
            SlideBarA.SlideBrush = brush2;
            SlideBarA.SlideValue = Value.Alpha;
        }
        private void SetColorNew()
        {
            ChooseOkOrCancel?.Invoke(this, new PropertyChangedEventargs<ColorP>(valueOld, Value));
            BdrColorOld.Background = BdrColorCurrent.Background.Clone();
            valueOld = Value;
        }
        private void SetColorOld()
        {
            ChooseOkOrCancel?.Invoke(this, new PropertyChangedEventargs<ColorP>(Value, valueOld));
            BdrColorCurrent.Background = BdrColorOld.Background.Clone();
            Value = valueOld;
        }

    }
}
