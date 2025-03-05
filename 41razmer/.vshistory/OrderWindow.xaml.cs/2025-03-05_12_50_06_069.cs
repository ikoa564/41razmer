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
        private User _currentUser;

        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, User user)
        {
            InitializeComponent();
            _currentUser = user;
            //DateDeliveryOrder.IsEnabled = false;
            if (user != null)
                FIOTB_Order.Text = user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;
            else
                FIOTB_Order.Text = "";
            var pickuppoints = Abdeev41Entities.GetContext().PickUpPoint.ToList().Select(p => p.PickUpPointAddress).ToList();
            PickPointComboBox.ItemsSource = pickuppoints;

            //OrderIDTB.Text = selectedOrderProducts.First().OrderID.ToString();
            int nextOrderID = GetNextOrderID();
            OrderIDTB.Text = nextOrderID.ToString();

            // Инициализируем Quantity для каждого Product
            foreach (var product in selectedProducts)
            {
                var orderProduct = selectedOrderProducts.FirstOrDefault(op => op.ProductArticleNumber == product.ProductArticleNumber);
                if (orderProduct != null)
                {
                    product.Quantity = orderProduct.ProductCount;
                }
                else
                {
                    product.Quantity = 1; // Если не найден, устанавливаем 1 (опционально)
                }
            }

            ProductOrderListView.ItemsSource = selectedProducts;

            //foreach (Product p in selectedProducts)
            //{
            //    p.Quantity = 1;
            //    foreach (OrderProduct q in selectedOrderProducts)
            //    {
            //        if (p.ProductArticleNumber == q.ProductArticleNumber)
            //            p.Quantity = q.ProductCount;
            //    }
            //}

            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;
            DateFormOrder.Text = DateTime.Now.ToString();
            SetDeliveryDate();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (PickPointComboBox.SelectedIndex == -1)
                errors.AppendLine("Выберите пункт выдачи!");
            if (DateFormOrder.SelectedDate.Value == null)
                errors.AppendLine("Введите дату формирования заказа!");
            if (DateDeliveryOrder.SelectedDate.Value == null)
                errors.AppendLine("Введите дату доставки заказа!");
            if (selectedOrderProducts.Count == 0) // Новая проверка
                errors.AppendLine("Добавьте хотя бы один товар в заказ!");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (_currentUser == null)
                currentOrder.OrderClientID = null;
            else
                currentOrder.OrderClientID = _currentUser.UserID; 
            currentOrder.OrderPickupPointID = PickPointComboBox.SelectedIndex + 1;
            currentOrder.OrderDate = DateFormOrder.SelectedDate.Value;
            currentOrder.OrderDeliveryDate = DateDeliveryOrder.SelectedDate.Value;
            currentOrder.OrderStatus = "Новый";
            currentOrder.OrderCode = GenerateUniqueOrderCode();

            Abdeev41Entities.GetContext().Order.Add(currentOrder);
            Abdeev41Entities.GetContext().SaveChanges();
            OrderIDTB.Text = currentOrder.OrderID.ToString();

            foreach (var op in selectedOrderProducts)
            {
                op.OrderID = currentOrder.OrderID;
                Abdeev41Entities.GetContext().OrderProduct.Add(op);
            }
            Abdeev41Entities.GetContext().SaveChanges();
            MessageBox.Show($"Заказ №{currentOrder.OrderID} сохранен! Код: {currentOrder.OrderCode}");
            Close();
        }

        //private void PlusBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    var prod = (sender as Button).DataContext as Product;
        //    prod.Quantity++; //?
        //    var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
        //    int index = selectedOrderProducts.IndexOf(selectedOP);
        //    selectedOrderProducts[index].ProductCount++;
        //    SetDeliveryDate();
        //    ProductOrderListView.Items.Refresh();
        //}

        private void PlusBtn_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);

            if (selectedOP != null)
            {
                selectedOP.ProductCount++;
                prod.Quantity = selectedOP.ProductCount;
                SetDeliveryDate();
                ProductOrderListView.Items.Refresh();
            }
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);

            if (selectedOP != null)
            {
                if (selectedOP.ProductCount > 1)
                {
                    selectedOP.ProductCount--;
                    prod.Quantity = selectedOP.ProductCount; // Синхронизируем Quantity
                    SetDeliveryDate();
                    ProductOrderListView.Items.Refresh();
                }
                else
                {
                    // Удаляем OrderProduct из списка
                    selectedOrderProducts.Remove(selectedOP);

                    // Находим Product в selectedProducts по артикулу (чтобы избежать проблем с ссылками)
                    var productToRemove = selectedProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
                    if (productToRemove != null)
                    {
                        selectedProducts.Remove(productToRemove);
                    }

                    // Обновляем интерфейс
                    ProductOrderListView.Items.Refresh();
                    // Перепривязываем данные, чтобы обновить интерфейс
                    ProductOrderListView.ItemsSource = null;
                    ProductOrderListView.ItemsSource = selectedProducts;
                    ProductOrderListView.Items.Refresh();
                }
            }
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

        private int GenerateUniqueOrderCode()
        {
            var context = Abdeev41Entities.GetContext();
            Random random = new Random();
            int code;
            do
            {
                code = random.Next(100, 1000); // 100-999
            } while (context.Order.Any(o => o.OrderCode == code));

            return code;
        }
        private int GetNextOrderID()
        {
            using (var context = Abdeev41Entities.GetContext())
            {
                var sqlCommand = "SELECT IDENT_CURRENT('Order')";
                var nextID = context.Database.SqlQuery<int>(sqlCommand).FirstOrDefault();
                return nextID;
            }
        }
    }
}
