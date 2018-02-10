﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using User;
using User.SoftWare;
using User.UI;

namespace Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Lbl1.SetBinding(ContentProperty, new Binding("SlideValueInt") { Source = SliderBar1 });
            ColorPicker1.Value = new ColorP(Colors.OrangeRed);
        }
        private void SlideBar_SlideValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Lbl1.Content = (int)e.NewValue;
        }
    }

    public class Area
    {
        public USettings uSettings = new USettings(",", "sdfasdfas");
        public USettingsProperty<int> p;
        public void A()
        {
            p = uSettings.Register("t", 2);
        }

    }
}
