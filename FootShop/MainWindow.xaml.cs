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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FootShop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;

        private Users _currentUser;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            MainFrame.Navigate(new LoginPage());
            MainFrame.Navigated += MainFrame_Navigated;
        }

        // Устанавливаем пользователя после входа
        public void SetUser(Users user)
        {
            _currentUser = user;

            // Гость
            if (user == null)
            {
                UserNameBlock.Text = "Гость";
                OrdersButton.Visibility = Visibility.Collapsed;
                return;
            }

            // Авторизованный пользователь
            UserNameBlock.Text = user.FullName;

            // Заказы доступны только менеджеру и администратору
            if (user.RoleID == 1 || user.RoleID == 2)
                OrdersButton.Visibility = Visibility.Visible;
            else
                OrdersButton.Visibility = Visibility.Collapsed;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _currentUser = null;
            UserNameBlock.Text = "";
            OrdersButton.Visibility = Visibility.Collapsed;

            MainFrame.Navigate(new LoginPage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }

        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrdersPage(_currentUser));
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (MainFrame.Content is LoginPage)
                BackButton.Visibility = Visibility.Collapsed;
            else
                BackButton.Visibility = MainFrame.CanGoBack
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
    }
}
