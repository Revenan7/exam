using FootShop.DatabaseModels;
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

namespace FootShop
{
    /// <summary>
    /// Логика взаимодействия для AddEditOrderWindow.xaml
    /// </summary>
    public partial class AddEditOrderWindow : Window
    {
        private Entities db = new Entities();
        private Orders _order;
        private Users _currentUser;

        // ✔ конструктор с параметром


        public AddEditOrderWindow(int? orderId, Users user)
        {
            InitializeComponent();

            _currentUser = user;

            StatusBox.ItemsSource = db.OrderStatuses.ToList();
            AddressBox.ItemsSource = db.Addresses.ToList();

            if (orderId == null)
            {
                _order = new Orders
                {
                    OrderDate = DateTime.Now,
                    DeliveryDate = DateTime.Now,
                    UserID = user.UserID,           // 🔥 ОБЯЗАТЕЛЬНО
                    ReceiptCode = GenerateCode()    // 🔥 ОБЯЗАТЕЛЬНО
                };
            }
            else
            {
                _order = db.Orders.Find(orderId);

                StatusBox.SelectedItem = _order.OrderStatuses;
                AddressBox.SelectedItem = _order.Addresses;
                OrderDatePicker.SelectedDate = _order.OrderDate;
                DeliveryDatePicker.SelectedDate = _order.DeliveryDate;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (StatusBox.SelectedItem == null || AddressBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _order.StatusID = ((OrderStatuses)StatusBox.SelectedItem).StatusID;
            _order.AddressID = ((Addresses)AddressBox.SelectedItem).AddressID;
            _order.OrderDate = OrderDatePicker.SelectedDate.Value;
            _order.DeliveryDate = DeliveryDatePicker.SelectedDate.Value;

            if (_order.OrderID == 0)
                db.Orders.Add(_order);

            db.SaveChanges();
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private int GenerateCode()
        {
            return new Random().Next(100000, 999999);
        }

    }
}
