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
using System.Windows.Threading;

namespace PetShopKT.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new Pages.ViewProductPage());
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            {
                try
                {
                    StringBuilder errors = new StringBuilder();

                    if (string.IsNullOrEmpty(LoginTextBox.Text))
                    {
                        errors.AppendLine("Заполните логин");
                    }

                    if (string.IsNullOrEmpty(PasswordBox.Password))
                    {
                        errors.AppendLine("Заполните пароль");
                    }




                    if (Data.ShopEntities.GetContext().User
                        .Any(d => d.UserLogin == LoginTextBox.Text && d.UserPassword == PasswordBox.Password))
                    {
                        var user = Data.ShopEntities.GetContext().User
                            .FirstOrDefault(d => d.UserLogin == LoginTextBox.Text && d.UserPassword == PasswordBox.Password);

                        switch (user.Role.RoleName)
                        {
                            case "Администратор":
                                Classes.Manager.MainFrame.Navigate(new Pages.AdminPage());
                                break;
                            case "Клиент":
                                Classes.Manager.MainFrame.Navigate(new Pages.ViewProductPage());
                                break;
                            case "Менеджер":
                                Classes.Manager.MainFrame.Navigate(new Pages.ViewProductPage());
                                break;
                        }

                        MessageBox.Show("Успех!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    else
                    {
                        MessageBox.Show("Некорректный логин/пароль", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
    }
}
