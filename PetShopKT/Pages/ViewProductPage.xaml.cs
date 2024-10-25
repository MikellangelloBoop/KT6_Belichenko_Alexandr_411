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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetShopKT.Pages
{
    /// <summary>
    /// Логика взаимодействия для ViewProductPage.xaml
    /// </summary>
    public partial class ViewProductPage : Page
    {
        public ViewProductPage()
        {
            InitializeComponent();
            Init();
            ProductListView.ItemsSource = Data.ShopEntities.GetContext().Product.ToList();
            LoadProducts();

        }

        private void LoadProducts()
        {
            ProductListView.ItemsSource = Data.ShopEntities.GetContext().Product.ToList();
        }

        public void Init()
        {
            ProductListView.ItemsSource = Data.ShopEntities.GetContext().Product.ToList();
            var manufactList = Data.ShopEntities.GetContext().Manufacturer.ToList();
            manufactList.Insert(0, new Data.Manufacturer { Name = "Все производители" });
            ManufacturerComboBox.ItemsSource = manufactList;
            ManufacturerComboBox.SelectedIndex = 0;

            if (Classes.Manager.CurrentUser != null)
            {
                FIOLabel.Visibility = Visibility.Visible;
                FIOLabel.Content = $"{Classes.Manager.CurrentUser.UserSurname}" +
                    $"{Classes.Manager.CurrentUser.UserName}" +
                    $"{Classes.Manager.CurrentUser.UserPatronymic}";

            }
            else
            {
                FIOLabel.Visibility = Visibility.Hidden;
            }

            CountOfLabel.Content = $"{Data.ShopEntities.GetContext().Product.Count()}/" +
                $"{Data.ShopEntities.GetContext().Product.Count()}";
        }

        private void UpdateCountLabel()
        {
            int totalCount = Data.ShopEntities.GetContext().Product.Count();
            int currentCount = _currentProduct.Count;
            CountOfLabel.Content = $"{currentCount}/{totalCount}";
        }


        public List<Data.Product> _currentProduct = Data.ShopEntities.GetContext().Product.ToList();
        public void Update()
        {
            try
            {
                _currentProduct = Data.ShopEntities.GetContext().Product.ToList();

                _currentProduct = (from item in Data.ShopEntities.GetContext().Product
                                   where item.ProductName.Name.ToLower().Contains(SearchTextBox.Text) ||
                                   item.ProductDescription.ToLower().Contains(SearchTextBox.Text) ||
                                   item.Manufacturer.Name.ToLower().Contains(SearchTextBox.Text) ||
                                   item.ProductCost.ToString().ToLower().Contains(SearchTextBox.Text) ||
                                   item.ProductQuantityInStock.ToString().ToLower().Contains(SearchTextBox.Text)
                                   select item).ToList();
                if (SortUpRadioButton.IsChecked == true)
                {
                    _currentProduct = _currentProduct.OrderBy(d => d.ProductCost).ToList();
                }
                if (SortDownRadioButton.IsChecked == true)
                {
                    _currentProduct = _currentProduct.OrderByDescending(d => d.ProductCost).ToList();
                }
                var selected = ManufacturerComboBox.SelectedItem as Data.Manufacturer;
                if (selected != null && selected.Name != "Все производители")
                {
                    _currentProduct = _currentProduct.Where(d => d.Manufacturer.Id == selected.Id).ToList();
                }


                CountOfLabel.Content = $"{_currentProduct.Count}/{Data.ShopEntities.GetContext().Product.Count()}";

                ProductListView.ItemsSource = _currentProduct;
            }
            catch (Exception)
            {

            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update();
        }

        private void SortUpRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void SortDownRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Classes.Manager.MainFrame.CanGoBack)
            {
                Classes.Manager.MainFrame.GoBack();
            }
        }


        private void ManufacturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }
    }
}
