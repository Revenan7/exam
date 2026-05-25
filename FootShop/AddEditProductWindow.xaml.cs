using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using FootShop.DatabaseModels;

namespace FootShop
{
    /// <summary>
    /// Логика взаимодействия для AddEditProductWindow.xaml
    /// </summary>
    public partial class AddEditProductWindow : Window
    {
        private Entities _db = DatabaseContext.GetContext();
        private Products _product;
        private bool _isNew;
        private string _oldPhotoFileName; 

        public AddEditProductWindow(Products product)
        {
            InitializeComponent();

            if (product == null)
            {
                _isNew = true;
                _product = new Products();
                Title = "Добавление товара";
                IdBox.Text = "(новый)";
                _oldPhotoFileName = null;
            }
            else
            {
                _isNew = false;
                _product = _db.Products.First(p => p.ProductID == product.ProductID);
                Title = "Редактирование товара";
                IdBox.Text = _product.ProductID.ToString();
                _oldPhotoFileName = _product.PhotoFileName; 
            }

            LoadComboboxes();
            LoadProductData();
        }

        private void LoadComboboxes()
        {
            ProductNameBox.ItemsSource = _db.ProductNames.ToList();
            ProductNameBox.DisplayMemberPath = "ProductName";
            ProductNameBox.SelectedValuePath = "ProductNameID";

            CategoryBox.ItemsSource = _db.Categories.ToList();
            CategoryBox.DisplayMemberPath = "CategoryName";
            CategoryBox.SelectedValuePath = "CategoryID";

            ManufacturerBox.ItemsSource = _db.Manufacturers.ToList();
            ManufacturerBox.DisplayMemberPath = "ManufacturerName";
            ManufacturerBox.SelectedValuePath = "ManufacturerID";

            ProviderBox.ItemsSource = _db.Providers.ToList();
            ProviderBox.DisplayMemberPath = "ProviderName";
            ProviderBox.SelectedValuePath = "ProviderID";
        }

        private BitmapImage LoadImage(string fileName)
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

            var bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(imgPath, UriKind.Absolute);
            bi.CacheOption = BitmapCacheOption.OnLoad; 
            bi.EndInit();

            return bi;
        }

        private void LoadProductData()
        {
            if (!_isNew)
            {
                ArticleBox.Text = _product.Article;
                ProductNameBox.SelectedValue = _product.ProductNameID;
                CategoryBox.SelectedValue = _product.CategoryID;
                ManufacturerBox.SelectedValue = _product.ManufacturerID;
                ProviderBox.SelectedValue = _product.ProviderID;
                PriceBox.Text = _product.Price.ToString();
                UnitBox.Text = _product.Unit;
                QuantityBox.Text = _product.QuantityInStok.ToString();
                DiscountBox.Text = _product.Discount?.ToString() ?? "0";
                DescriptionBox.Text = _product.Description;
            }

            ProductImage.Source = LoadImage(_product.PhotoFileName);
        }

        private void ChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Изображения|*.png;*.jpg;*.jpeg"
            };

            if (dlg.ShowDialog() == true)
            {
                string imagesFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                string fileHash = GetFileHash(dlg.FileName);
                string newName = fileHash + ".jpg";
                string finalPath = System.IO.Path.Combine(imagesFolder, newName);

                if (!File.Exists(finalPath))
                {
                    ResizeAndSaveImage(dlg.FileName, finalPath, 300, 200);
                }

                _product.PhotoFileName = newName;
                ProductImage.Source = LoadImage(newName);
            }
        }

        private string GetFileHash(string filePath)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLower();
                }
            }
        }

        private void ResizeAndSaveImage(string sourcePath, string destinationPath, int targetWidth, int targetHeight)
        {
            BitmapImage original = new BitmapImage();

            using (FileStream fs = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            {
                original.BeginInit();
                original.CacheOption = BitmapCacheOption.OnLoad;
                original.StreamSource = fs;
                original.EndInit();
                original.Freeze();
            }

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(original, new Rect(0, 0, targetWidth, targetHeight));
            }

            var resized = new RenderTargetBitmap(targetWidth, targetHeight, 96, 96, PixelFormats.Pbgra32);
            resized.Render(drawingVisual);

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(resized));

            using (FileStream fs = new FileStream(destinationPath, FileMode.Create))
            {
                encoder.Save(fs);
            }
        }

        private bool ValidateData()
        {
            if (string.IsNullOrWhiteSpace(ArticleBox.Text))
            {
                MessageBox.Show("Укажите артикул.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (ProductNameBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите наименование.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (CategoryBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (ManufacturerBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите производителя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (ProviderBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите поставщика.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!decimal.TryParse(PriceBox.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Цена должна быть неотрицательным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!int.TryParse(QuantityBox.Text, out int qty) || qty < 0)
            {
                MessageBox.Show("Количество должно быть неотрицательным целым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!int.TryParse(DiscountBox.Text, out int disc) || disc < 0)
            {
                MessageBox.Show("Скидка должна быть неотрицательным целым числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(UnitBox.Text))
            {
                MessageBox.Show("Укажите единицу измерения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateData())
                    return;

                _product.Article = ArticleBox.Text;
                _product.ProductNameID = (int)ProductNameBox.SelectedValue;
                _product.CategoryID = (int)CategoryBox.SelectedValue;
                _product.ManufacturerID = (int)ManufacturerBox.SelectedValue;
                _product.ProviderID = (int)ProviderBox.SelectedValue;
                _product.Price = decimal.Parse(PriceBox.Text);
                _product.Unit = UnitBox.Text;
                _product.QuantityInStok = int.Parse(QuantityBox.Text);
                _product.Discount = int.Parse(DiscountBox.Text);
                _product.Description = DescriptionBox.Text;

                if (_isNew)
                {
                    int maxId = _db.Products.Any() ? _db.Products.Max(p => p.ProductID) : 0;
                    _product.ProductID = maxId + 1;
                    _db.Products.Add(_product);
                }

                _db.SaveChanges();

                if (!_isNew &&
                    !string.IsNullOrEmpty(_oldPhotoFileName) &&
                    _oldPhotoFileName != _product.PhotoFileName)
                {
                    DeleteImageIfUnused(_oldPhotoFileName);
                }

                DialogResult = true;
                Close();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                string msg = "";
                foreach (var eve in ex.EntityValidationErrors)
                    foreach (var ve in eve.ValidationErrors)
                        msg += $"{ve.PropertyName}: {ve.ErrorMessage}\n";

                MessageBox.Show(msg, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении товара: " + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteImageIfUnused(string fileName)
        {
            _db = new Entities();

            int usageCount = _db.Products.Count(p => p.PhotoFileName == fileName);

            if (usageCount == 0)
            {
                string imgPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);
                if (File.Exists(imgPath))
                {
                    try
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        File.Delete(imgPath);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
