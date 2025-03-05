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
            FIOTB_Order.Text = fio;
            var pickuppoints = Abdeev41Entities.GetContext().PickUpPoint.ToList().Select(p => p.PickUpPointAddress).ToList();
            PickPointComboBox.ItemsSource = pickuppoints;

            OrderIDTB.Text = selectedOrderProducts.First().OrderID.ToString();

            ProductOrderListView.ItemsSource = selectedProducts;

            foreach (Product p in selectedProducts)
            {
                p.ProductQuantityInStock = 1;
                foreach (OrderProduct q in selectedOrderProducts)
                {
                    if (p.ProductArticleNumber == q.ProductArticleNumber)
                        p.ProductQuantityInStock = q.ProductCount;
                }
            }

            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            currentOrder.OrderClientID = 1; // ПЕРЕДЕЛАТЬ!
            //currentOrder.PickUpPoint = PickPointComboBox.SelectedIndex.ToString();
            //currentOrder.OrderDate = DateFormOrder.;
        }
    }
}
