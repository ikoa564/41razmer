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
using System.Windows.Shapes;

namespace _41razmer
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();
        private Order currentOrder = new Order();
        private OrderProduct currentOrderProduct = new OrderProduct();

        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, string fio)
        {
            InitializeComponent();
            DateDeliveryOrder.IsEnabled = false;
            FIOTB_Order.Text = fio;
            var pickuppoints = Abdeev41Entities.GetContext().PickUpPoint.ToList().Select(p => p.PickUpPointAddress).ToList();
            PickPointComboBox.ItemsSource = pickuppoints;

            //OrderIDTB.Text = selectedOrderProducts.First().OrderID.ToString();

            ProductOrderListView.ItemsSource = selectedProducts;

            foreach (Product p in selectedProducts)
            {
                p.Quantity = 1;
                foreach (OrderProduct q in selectedOrderProducts)
                {
                    if (p.ProductArticleNumber == q.ProductArticleNumber)
                        p.Quantity = q.ProductCount;
                }
            }

            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;
            DateFormOrder.Text = DateTime.Now.ToString();
            SetDeliveryDate();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            currentOrder.OrderClientID = 1; // ПЕРЕДЕЛАТЬ!
            //currentOrder.PickUpPoint = PickPointComboBox.SelectedIndex.ToString();
            //currentOrder.OrderDate = DateFormOrder.;
        }

        private void PlusBtn_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.Quantity++; //?
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            int index = selectedOrderProducts.IndexOf(selectedOP);
            selectedOrderProducts[index].ProductCount++;
            SetDeliveryDate();
            ProductOrderListView.Items.Refresh();
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.Quantity--; //?
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            int index = selectedOrderProducts.IndexOf(selectedOP); //тут ошибка
            selectedOrderProducts[index].ProductCount--;
            SetDeliveryDate();
            ProductOrderListView.Items.Refresh();
        }

        private void SetDeliveryDate()
        {
            bool hasLowStock = false; // Флаг для проверки наличия товаров <3 шт.

            // Проверяем каждый продукт в заказе
            foreach (var product in selectedProducts)
            {
                if (product.ProductQuantityInStock < 3) // Если количество на складе <3
                {
                    hasLowStock = true;
                    break; // Выходим из цикла при первом нарушении
                }
            }

            // Вычисляем дату доставки
            DateTime deliveryDate = DateFormOrder.SelectedDate.Value;
            deliveryDate = hasLowStock
                ? deliveryDate.AddDays(6) // Если есть товары <3 шт. → +6 дней
                : deliveryDate.AddDays(3); // В противном случае → +3 дня

            // Устанавливаем дату в элемент DateDeliveryOrder (проверьте имя элемента в XAML)
            DateDeliveryOrder.SelectedDate = deliveryDate;
        }

        private void DateFormOrder_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDeliveryDate();
        }
    }
}
