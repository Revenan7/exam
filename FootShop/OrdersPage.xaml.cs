using FootShop.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace FootShop
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        private Entities db = new Entities();
        private Users _currentUser;

        public OrdersPage(Users user)
        {
            InitializeComponent();
            _currentUser = user;

            if (_currentUser.RoleID != 1) // не админ
                AdminPanel.Visibility = Visibility.Collapsed;

            LoadOrders();
        }

        private void LoadOrders()
        {
            OrdersList.ItemsSource = db.Orders.Select(o => new
            {
                o.OrderID,
                o.OrderDate,
                o.DeliveryDate,
                StatusName = o.OrderStatuses.StatusName,
                AddressName = o.Addresses.AddressName
            }).ToList();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            new AddEditOrderWindow(null, _currentUser).ShowDialog();
            LoadOrders();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersList.SelectedItem == null) return;

            dynamic order = OrdersList.SelectedItem;
            new AddEditOrderWindow(order.OrderID, _currentUser).ShowDialog();
            LoadOrders();
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersList.SelectedItem == null) return;

            dynamic item = OrdersList.SelectedItem;
            var order = db.Orders.Find(item.OrderID);

            if (MessageBox.Show("Удалить заказ?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                db.Orders.Remove(order);
                db.SaveChanges();
                LoadOrders();
            }
        }

        private void OrdersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_currentUser.RoleID != 1) return;
            Edit_Click(sender, e);
        }

    }
}
