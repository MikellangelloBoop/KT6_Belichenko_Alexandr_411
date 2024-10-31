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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Drawing;


namespace PetShopKT.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {

        private int _failedAttempts = 0; 
        private string correctCaptcha;  

        private DispatcherTimer blockTimer;
        private int _blockTimeLeft = 10; 

        public Login()
        {
            InitializeComponent();
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new Pages.ViewProductPage());
        }
        private void BlockTimer_Tick(object sender, EventArgs e)
        {
            _blockTimeLeft--; 

            if (_blockTimeLeft <= 0)
            {
                LoginButton.IsEnabled = true;
                TimerLabel.Visibility = Visibility.Collapsed; 
                blockTimer.Stop(); 
            }
            else
            {
                TimerLabel.Content = $"Попробуйте снова через {_blockTimeLeft} секунд"; 
            }
        }


        private void GenerateCaptcha()
        {
            string captchaText = GenerateRandomCaptchaText();
            correctCaptcha = captchaText;

            using (Bitmap bitmap = new Bitmap(200, 80))
            using (Graphics g = Graphics.FromImage(bitmap))
            using (MemoryStream ms = new MemoryStream())
            {
                Random rnd = new Random();
                g.Clear(System.Drawing.Color.White);
                Font font = new Font("Arial", 20, System.Drawing.FontStyle.Bold);

                for (int i = 0; i < captchaText.Length; i++)
                {
                    float angle = rnd.Next(-30, 30);
                    g.TranslateTransform(40 + i * 30, 40);
                    g.RotateTransform(angle);
                    g.DrawString(captchaText[i].ToString(), font, System.Drawing.Brushes.Black, 0, 0);
                    g.ResetTransform();
                }

                for (int i = 0; i < 5; i++)
                {
                    g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Gray, 2),
                        rnd.Next(0, bitmap.Width), rnd.Next(0, bitmap.Height),
                        rnd.Next(0, bitmap.Width), rnd.Next(0, bitmap.Height));
                }

                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = new MemoryStream(ms.ToArray());
                image.EndInit();

                CaptchaImage.Source = image;
                CaptchaImage.Visibility = Visibility.Visible;
                CaptchaTextBox.Visibility = Visibility.Visible;
            }
        }

        private string GenerateRandomCaptchaText()
        {
            Random random = new Random();
            string chars = "qwertyuiopasdfghjklzxcvbnm0123456789";
            return new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }



        private async void LoginButton_Click(object sender, RoutedEventArgs e)
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

                    if (CaptchaTextBox.Visibility == Visibility.Visible)
                    {
                        if (string.IsNullOrEmpty(CaptchaTextBox.Text))
                        {
                            errors.AppendLine("Введите CAPTCHA");
                        }
                        else if (CaptchaTextBox.Text != correctCaptcha)
                        {
                            errors.AppendLine("Неверная CAPTCHA");
                        }
                    }


                    if (errors.Length > 0)
                    {
                        MessageBox.Show(errors.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);

                        if (_failedAttempts > 0)
                        {
                            LoginButton.IsEnabled = false;


                            _blockTimeLeft = 10; 
                            TimerLabel.Visibility = Visibility.Visible;
                            TimerLabel.Content = $"Попробуйте снова через {_blockTimeLeft} секунд";

                            if (blockTimer == null)
                            {
                                blockTimer = new DispatcherTimer();
                                blockTimer.Interval = TimeSpan.FromSeconds(1);
                                blockTimer.Tick += BlockTimer_Tick;
                            }
                            blockTimer.Start();

                            await Task.Delay(10000);
                            GenerateCaptcha(); 
                        }

                        return;
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
                        CaptchaImage.Visibility = Visibility.Collapsed;
                        CaptchaTextBox.Visibility = Visibility.Collapsed;
                        _failedAttempts = 0;
                    

                }
                    else
                    {
                        MessageBox.Show("Некорректный логин/пароль", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        _failedAttempts++;

                        if (_failedAttempts == 1)
                        {
                            GenerateCaptcha();
                        }

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
