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
using FootShop.DatabaseModels;

namespace FootShop
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        Entities db = DatabaseContext.GetContext();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var user = db.Users
                .FirstOrDefault(u => u.Login == LoginBox.Text &&
                                     u.Password == PasswordBox.Password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.");
                return;
            }

            MainWindow.Instance.SetUser(user);
            MainWindow.Instance.MainFrame.Navigate(new ProductsPage(user));
        }

        private void Guest_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.SetUser(null);
            MainWindow.Instance.MainFrame.Navigate(new ProductsPage(null));
        }
    }
}
