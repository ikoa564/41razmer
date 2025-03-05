﻿using System;
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
using System.Windows.Shapes;

namespace _41razmer
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, string fio)
        {
            InitializeComponent();
            FIOTB_Order.Text = fio;
            var pickuppoints = Abdeev41Entities.GetContext().PickUpPoint.Select(p => p.PickUpPointAddress).ToList();


            PickPointComboBox.ItemsSource = pickuppoints;
        }
    }
}
