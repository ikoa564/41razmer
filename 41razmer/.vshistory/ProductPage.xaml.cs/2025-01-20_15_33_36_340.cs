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

namespace _41razmer
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        List<Product> Tablelist;
        int CountRecords;

        public ProductPage()
        {
            InitializeComponent();
            var currentProducts = Abdeev41Entities.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProducts;
            ComboDiscount.SelectedIndex = 0;
            UpdateProduct();
            CountRecords = Abdeev41Entities.GetContext().Product.ToList().Count;
            int min = CountRecords;

            TBCount.Text = min.ToString();
            TBAllRecords.Text = " из " + CountRecords.ToString();
        }

        private void UpdateProduct()
        {
            var currentProducts = Abdeev41Entities.GetContext().Product.ToList();


            if (ComboDiscount.SelectedIndex == 1)
            {
                currentProducts = currentProducts.Where(p => p.ProductDiscountAmount >= 0 && p.ProductDiscountAmount < 10).ToList();
            }
            if (ComboDiscount.SelectedIndex == 2)
            {
                currentProducts = currentProducts.Where(p => p.ProductDiscountAmount >= 10 && p.ProductDiscountAmount < 15).ToList();
            }
            if (ComboDiscount.SelectedIndex == 3)
            {
                currentProducts = currentProducts.Where(p => p.ProductDiscountAmount >= 15).ToList();
            }
            currentProducts = currentProducts.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
            ProductListView.ItemsSource = currentProducts.ToList();

            if (RBtnDown.IsChecked.Value)
                ProductListView.ItemsSource = currentProducts.OrderByDescending(p => p.ProductCost).ToList();
            if (RBtnUp.IsChecked.Value)
                ProductListView.ItemsSource = currentProducts.OrderBy(p => p.ProductCost).ToList();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProduct();
        }

        private void RBtnUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProduct();
        }

        private void RBtnDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProduct();
        }

        private void ComboDiscount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProduct();
        }


        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Manager.MainFrame.Navigate(new AddEditPage());
        //}
    }
}
