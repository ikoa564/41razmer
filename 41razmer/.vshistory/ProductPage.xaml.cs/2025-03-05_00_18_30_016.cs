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
    /// 

    public partial class ProductPage : Page
    {
        List<Product> selectedProducts = new List<Product>();
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        int CountRecords;
        private User _currentUser;

        public ProductPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            if (user != null)
            {
                FIOTB.Text = user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;
                switch (user.UserRole)
                {
                    case 1:
                        RoleTB.Text = "Клиент";
                        break;
                    case 2:
                        RoleTB.Text = "Менеджер";
                        break;
                    case 3:
                        RoleTB.Text = "Администратор";
                        break;
                }
            }
            else
            {
                FIOTB.Text = "";
                BeforeFIOTB.Text = "";
                RoleTB.Text = "Гость";
            }
            OrderBtn.Visibility = Visibility.Hidden;
            var currentProducts = Abdeev41Entities.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProducts;
            ComboDiscount.SelectedIndex = 0;
            UpdateProduct();
            CountRecords = Abdeev41Entities.GetContext().Product.ToList().Count;
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

            TBCount.Text = currentProducts.Count.ToString();
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(ProductListView.SelectedIndex >= 0)
            {
                var prod = ProductListView.SelectedItem as Product;
                selectedProducts.Add(prod);

                var newOrderProd = new OrderProduct();
                //newOrderProd.OrderID = newOrderID;

                newOrderProd.ProductArticleNumber = prod.ProductArticleNumber;
                newOrderProd.ProductCount = 1;

                var selOP = selectedProducts.Where(p => Equals(p.ProductArticleNumber, prod.ProductArticleNumber));
                if (selOP.Count() == 0)
                {
                    selectedOrderProducts.Add(newOrderProd);
                }
                else
                {
                    foreach(OrderProduct p in selectedOrderProducts)
                    {
                        if (p.ProductArticleNumber == prod.ProductArticleNumber)
                            p.ProductCount++;
                    }
                }

                OrderBtn.Visibility = Visibility.Visible;
                ProductListView.SelectedIndex = -1;
            }
        }

        //private void OrderBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    selectedProducts = selectedProducts.Distinct().ToList();
        //    OrderWindow orderWindow = new OrderWindow(selectedOrderProducts, selectedProducts, _currentUser);
        //    orderWindow.ShowDialog();
        //}

        private void OrderBtn_Click(object sender, RoutedEventArgs e)
        {
            selectedProducts = selectedProducts.Distinct().ToList();

            // Добавьте этот код для инициализации Quantity в Product
            foreach (var product in selectedProducts)
            {
                // Находим соответствующий OrderProduct для текущего Product
                var orderProduct = selectedOrderProducts.FirstOrDefault(op =>
                    op.ProductArticleNumber == product.ProductArticleNumber);

                if (orderProduct != null)
                {
                    // Устанавливаем Quantity в Product на основе ProductCount из OrderProduct
                    product.Quantity = orderProduct.ProductCount;
                }
                else
                {
                    // Если OrderProduct не найден (хотя это маловероятно), устанавливаем значение по умолчанию
                    product.Quantity = 1;
                }
            }

            OrderWindow orderWindow = new OrderWindow(selectedOrderProducts, selectedProducts, _currentUser);
            orderWindow.ShowDialog();
        }


        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Manager.MainFrame.Navigate(new AddEditPage());
        //}
    }
}
