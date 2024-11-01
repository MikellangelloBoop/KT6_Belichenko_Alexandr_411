using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetShopKT.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {

        public string FlagAddorEdit = "default";
        public Data.Product _currentproduct = new Data.Product();
        public AddEditPage(Data.Product product)
        {
            InitializeComponent();

            if (product != null)
            {
                _currentproduct = product;
                FlagAddorEdit = "edit";
            }
            else
            {
                FlagAddorEdit = "add";
            }
            DataContext = _currentproduct;

            Init();
        }


        public void Init()
        {
            try
            {
                CategoryComboBox.ItemsSource = Data.ShopEntities.GetContext().Category.ToList();
                if (FlagAddorEdit == "add")
                {
                    IdTextBox.Visibility = Visibility.Hidden;
                    IdLabel.Visibility = Visibility.Hidden;

                    CategoryComboBox.SelectedItem = null;
                    CountTextBox.Text = string.Empty;
                    UnitTextBox.Text = string.Empty;
                    NameTextBox.Text = string.Empty;
                    CostTextBox.Text = string.Empty;
                    SupplierTextBox.Text = string.Empty;
                    DescriptionTextBox.Text = string.Empty;
                    IdTextBox.Text = Data.ShopEntities.GetContext().Product.Max(d => d.Id + 1).ToString();
                }

                else if (FlagAddorEdit == "edit")
                {
                    IdTextBox.Visibility = Visibility.Visible;
                    IdLabel.Visibility = Visibility.Visible;

                    CategoryComboBox.SelectedItem = null;
                    CountTextBox.Text = _currentproduct.ProductQuantityInStock.ToString();
                    UnitTextBox.Text = _currentproduct.Units.NameOfUnits;
                    NameTextBox.Text = _currentproduct.ProductName.Name;
                    CostTextBox.Text = _currentproduct.ProductCost.ToString();
                    SupplierTextBox.Text = _currentproduct.ProductName.Name;
                    DescriptionTextBox.Text = _currentproduct.ProductDescription;

                    IdTextBox.Text = _currentproduct.Id.ToString();
                    CategoryComboBox.SelectedItem = Data.ShopEntities.GetContext().Category.Where(d => d.Id == _currentproduct.IdProductCategory).FirstOrDefault();
                    if (_currentproduct.ProductPhoto != null && _currentproduct.ProductPhoto.Length > 0)
                    {

                        BitmapImage bitmap = new BitmapImage();
                        using (var ms = new System.IO.MemoryStream(_currentproduct.ProductPhoto))
                        {
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = ms;
                            bitmap.EndInit();
                        }
                        ProductImage.Source = bitmap;


                    }

                }
            }
            catch (Exception)
            {

            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Classes.Manager.MainFrame.CanGoBack)
            {
                Classes.Manager.MainFrame.GoBack();
            }

        }




        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();
                if (string.IsNullOrEmpty(NameTextBox.Text))
                {
                    errors.AppendLine("Заполните наименование");
                }
                if (CategoryComboBox.SelectedItem == null)
                {
                    errors.AppendLine("Выберите категорию");
                }
                if (string.IsNullOrEmpty(CountTextBox.Text))
                {
                    errors.AppendLine("Заполните количество");
                }
                else
                {
                    var tryQuantity = Int32.TryParse(CountTextBox.Text, out var resultQuantity);
                    if (!tryQuantity)
                    {
                        errors.AppendLine("Количество - целое число");
                    }
                }
                if (string.IsNullOrEmpty(UnitTextBox.Text))
                {
                    errors.AppendLine("Заполните ед.измерения");
                }
                if (string.IsNullOrEmpty(SupplierTextBox.Text))
                {
                    errors.AppendLine("Заполните поставщика");
                }
                if (string.IsNullOrEmpty(CostTextBox.Text))
                {
                    errors.AppendLine("Заполните стоимость");
                }
                else
                {
                    var tryCost = Decimal.TryParse(CostTextBox.Text, out var resultCost);
                    if (!tryCost)
                    {
                        errors.AppendLine("Стоимость - дробное число");
                    }
                    else
                    {
                        int decimalSeparatorIndex = CostTextBox.Text.IndexOfAny(new char[] { ',', '.' });

                        if (decimalSeparatorIndex != -1)
                        {
                            int decimalsCount = CostTextBox.Text.Length - decimalSeparatorIndex - 1;

                            if (decimalsCount > 2)
                            {
                                errors.AppendLine("Допускается не более двух знаков после запятой.");
                            }
                        }
                    }
                    if (tryCost && resultCost < 0)
                    {
                        errors.AppendLine("Стоимость не может быть отрицательной");
                    }
                }
                if (string.IsNullOrEmpty(DescriptionTextBox.Text))
                {
                    errors.AppendLine("Заполните описание");
                }

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedCategory = CategoryComboBox.SelectedItem as Data.Category;

                _currentproduct.IdProductCategory = Data.ShopEntities.GetContext().Category.Where(d => d.Id == selectedCategory.Id).FirstOrDefault().Id;
                _currentproduct.ProductQuantityInStock = Convert.ToInt32(CountTextBox.Text);
                _currentproduct.ProductCost = Convert.ToDecimal(CostTextBox.Text);
                _currentproduct.ProductDescription = DescriptionTextBox.Text;
                var searchUnit = (from item in Data.ShopEntities.GetContext().Units
                                  where item.NameOfUnits == UnitTextBox.Text
                                  select item).FirstOrDefault();
                if (searchUnit != null)
                {
                    _currentproduct.Idunits = searchUnit.Id;
                }
                else
                {
                    Data.Units units = new Data.Units()
                    {
                        NameOfUnits = UnitTextBox.Text
                    };

                    Data.ShopEntities.GetContext().Units.Add(units);
                    Data.ShopEntities.GetContext().SaveChanges();
                    _currentproduct.Idunits = units.Id;
                }


                var searchProductName = (from item in Data.ShopEntities.GetContext().ProductName
                                         where item.Name == NameTextBox.Text
                                         select item).FirstOrDefault();
                if (searchProductName != null)
                {
                    _currentproduct.IdProductName = searchProductName.Id;
                }
                else
                {
                    Data.ProductName ProductName = new Data.ProductName()
                    {
                        Name = NameTextBox.Text
                    };

                    Data.ShopEntities.GetContext().ProductName.Add(ProductName);
                    Data.ShopEntities.GetContext().SaveChanges();
                    _currentproduct.IdProductName = ProductName.Id;
                }

                var searchProducterName = (from item in Data.ShopEntities.GetContext().Producter
                                           where item.Name == SupplierTextBox.Text
                                           select item).FirstOrDefault();
                if (searchProducterName != null)
                {
                    _currentproduct.IdProducterName = searchProducterName.Id;
                }
                else
                {
                    Data.Producter ProducterName = new Data.Producter()
                    {
                        Name = SupplierTextBox.Text
                    };

                    Data.ShopEntities.GetContext().Producter.Add(ProducterName);

                    Data.ShopEntities.GetContext().SaveChanges();
                    _currentproduct.IdProducterName = ProducterName.Id;

                }






                if (FlagAddorEdit == "add")
                {
                    Data.ShopEntities.GetContext().Product.Add(_currentproduct);
                    Data.ShopEntities.GetContext().SaveChanges();
                    MessageBox.Show("Успешно добавлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (FlagAddorEdit == "edit")
                {
                    Data.ShopEntities.GetContext().SaveChanges();
                    MessageBox.Show("Успешно сохранено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                Classes.Manager.MainFrame.Navigate(new AdminPage());



            }




            catch (DbEntityValidationException ex)
            {
                StringBuilder errorMessages = new StringBuilder();
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorMessages.AppendLine($"Свойство: {validationError.PropertyName} Ошибка: {validationError.ErrorMessage}");
                    }
                }
                MessageBox.Show(errorMessages.ToString(), "Ошибка валидации данных");
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.InnerException;
                if (innerException != null)
                {
                    MessageBox.Show(innerException.Message, "Ошибка сохранения данных");
                }
                else
                {
                    MessageBox.Show(ex.Message, "Ошибка сохранения данных");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка сохранения данных");
            }

            Data.ShopEntities.GetContext().Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

        }

        private void ProductImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));

                if (bitmap.PixelWidth > 300 || bitmap.PixelHeight > 200)
                {
                    MessageBox.Show("Размер изображения не должен превышать 300x200 пикселей.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                ProductImage.Source = bitmap;

                _currentproduct.ProductPhoto = BitmapImageToByteArray(bitmap);

                Data.ShopEntities.GetContext().SaveChanges();
            }




        }

        private byte[] BitmapImageToByteArray(BitmapImage image)
        {
            byte[] data;
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}