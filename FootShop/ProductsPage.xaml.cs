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
using System.IO;
using FootShop.DatabaseModels;

namespace FootShop
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        private Entities db = DatabaseContext.GetContext();
        private Users _currentUser;

        private string _searchText = "";
        private int _selectedProviderId = 0;
        private string _sortOrder = "Без сортировки";

        public ProductsPage(Users user)
        {
            InitializeComponent();
            _currentUser = user;

            SetupRoleAccess();
            LoadProviders();
            LoadProducts();
        }

        private void SetupRoleAccess()
        {
            if (_currentUser == null || (_currentUser.RoleID != 1 && _currentUser.RoleID != 2))
            {
                ControlPanel.Visibility = Visibility.Collapsed;
                AdminPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                ControlPanel.Visibility = Visibility.Visible;

                if (_currentUser.RoleID == 1)
                    AdminPanel.Visibility = Visibility.Visible;
                else
                    AdminPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadProviders()
        {
            var providers = db.Providers.ToList();

            ProviderFilter.Items.Clear();
            ProviderFilter.Items.Add("Все поставщики");

            foreach (var p in providers)
                ProviderFilter.Items.Add(p.ProviderName);

            ProviderFilter.SelectedIndex = 0;
        }

        private void LoadProducts()
        {
            var list = db.Products
                .Select(p => new
                {
                    p.ProductID,
                    p.PhotoFileName,
                    ProductName = p.ProductNames.ProductName,
                    CategoryName = p.Categories.CategoryName,
                    Description = p.Description,
                    ManufacturerName = p.Manufacturers.ManufacturerName,
                    ProviderName = p.Providers.ProviderName,
                    Unit = p.Unit,
                    Price = p.Price,
                    Discount = p.Discount,
                    FinalPrice = p.Discount > 0 ? (p.Price - (p.Price * p.Discount / 100)) : p.Price,
                    QuantityInStock = p.QuantityInStok,
                    ProviderID = p.ProviderID
                })
                .ToList();

            string imagesFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

            // ВАЖНО: Загружаем изображения с BitmapCacheOption.OnLoad
            var result = list.Select(item => new
            {
                item.ProductID,
                item.ProductName,
                item.CategoryName,
                item.Description,
                item.ManufacturerName,
                item.ProviderName,
                item.Unit,
                item.Price,
                item.Discount,
                item.FinalPrice,
                item.QuantityInStock,
                item.ProviderID,

                // Загружаем BitmapImage вместо пути
                PhotoPath = LoadImageWithCache(item.PhotoFileName)
            }).ToList();

            if (_selectedProviderId != 0)
                result = result.Where(p => p.ProviderID == _selectedProviderId).ToList();

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                var searchWords = _searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                result = result.Where(p =>
                {
                    string allText = string.Join(" ",
                        p.ProductName ?? "",
                        p.CategoryName ?? "",
                        p.Description ?? "",
                        p.ManufacturerName ?? "",
                        p.ProviderName ?? "",
                        p.Unit ?? "",
                        p.Price.ToString(),
                        p.Discount.ToString(),
                        p.FinalPrice.ToString(),
                        p.QuantityInStock.ToString()
                    ).ToLower();

                    return searchWords.All(word => allText.Contains(word));
                }).ToList();
            }

            switch (_sortOrder)
            {
                case "По возрастанию":
                    result = result.OrderBy(p => p.QuantityInStock).ToList();
                    break;

                case "По убыванию":
                    result = result.OrderByDescending(p => p.QuantityInStock).ToList();
                    break;
            }

            ProductsList.ItemsSource = result;
        }

        // Загружает изображение с освобождением файла
        private BitmapImage LoadImageWithCache(string fileName)
        {
            string imgPath;

            if (string.IsNullOrEmpty(fileName))
            {
                imgPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "picture.png");
            }
            else
            {
                imgPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);
                if (!File.Exists(imgPath))
                    imgPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "picture.png");
            }

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad; // КРИТИЧЕСКИ ВАЖНО!
            bitmap.UriSource = new Uri(imgPath, UriKind.Absolute);
            bitmap.EndInit();
            bitmap.Freeze(); // Делаем immutable для многопоточности

            return bitmap;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchText = SearchBox.Text.ToLower();
            LoadProducts();
        }

        private void ProviderFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProviderFilter.SelectedIndex == 0)
                _selectedProviderId = 0;
            else
            {
                _selectedProviderId = db.Providers
                    .ToList()[ProviderFilter.SelectedIndex - 1]
                    .ProviderID;
            }

            LoadProducts();
        }

        private void SortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortBox.SelectedItem is ComboBoxItem item)
                _sortOrder = item.Content.ToString();
            else
                _sortOrder = "Без сортировки";

            LoadProducts();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddEditProductWindow(null);
            if (win.ShowDialog() == true)
                LoadProducts();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = ProductsList.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("Выберите товар.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int id = selected.ProductID;
            Products p = db.Products.FirstOrDefault(x => x.ProductID == id);

            var win = new AddEditProductWindow(p);
            if (win.ShowDialog() == true)
                LoadProducts();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = ProductsList.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("Выберите товар.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int id = selected.ProductID;
            Products p = db.Products.FirstOrDefault(x => x.ProductID == id);

            if (p.OrderItems.Any())
            {
                MessageBox.Show("Этот товар есть в заказах и не может быть удалён.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("Удалить товар?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    // СОХРАНЯЕМ имя фото ДО удаления товара
                    string photoToDelete = p.PhotoFileName;

                    // Удаляем товар из базы
                    db.Products.Remove(p);
                    db.SaveChanges();

                    // ПОСЛЕ удаления проверяем, используется ли фото другими товарами
                    if (!string.IsNullOrEmpty(photoToDelete))
                    {
                        DeleteImageIfUnused(photoToDelete);
                    }

                    LoadProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message, "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Удаляет изображение, если оно не используется другими товарами
        private void DeleteImageIfUnused(string fileName)
        {
            // Проверяем, используется ли это изображение другими товарами
            int usageCount = db.Products.Count(p => p.PhotoFileName == fileName);

            // Если никто больше не использует это изображение
            if (usageCount == 0)
            {
                string imgPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);
                if (File.Exists(imgPath))
                {
                    try
                    {
                        // Освобождаем файл перед удалением
                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        File.Delete(imgPath);
                    }
                    catch
                    {
                        // Игнорируем ошибку, если файл заблокирован
                    }
                }
            }
        }

        private void ProductsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_currentUser.RoleID != 1) return;

            EditButton_Click(sender, e);
        }
    }
}
