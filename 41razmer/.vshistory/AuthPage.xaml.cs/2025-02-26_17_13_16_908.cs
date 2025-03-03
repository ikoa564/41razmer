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

namespace _41razmer
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
            CaptchaTB.Visibility = Visibility.Hidden;

        }
        private void GuestBtn_Click(object sender, RoutedEventArgs e)
        {
            User user = null;
            Manager.MainFrame.Navigate(new ProductPage(user));
        }

        private async void SignInBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text;
            string password = PasswordTB.Text;
            int countErrorAuth = 0;

            if (login == "" || password == "")
            {
                StringBuilder errors = new StringBuilder();
                errors.AppendLine("Введите логин и пароль!");
                if (CaptchaTB.Visibility == Visibility.Visible && CaptchaTB.Text == "")
                {
                    errors.AppendLine("Введите капчу!");
                    //errors.AppendLine(countErrorAuth.ToString());
                }
                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString());
                    return;
                }
            }

            User user = Abdeev41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (user != null)
            {
                if (CaptchaTB.Visibility == Visibility.Visible && CaptchaTB.Text != "")
                {
                    string Captcha = captchaOneChar.Text + captchaTwoChar.Text + captchaThreeChar.Text + captchaFourChar.Text;
                    if (CaptchaTB.Text != Captcha)
                    {
                        MessageBox.Show("Неверная капча!");
                        return;
                    }
                }

                else if (CaptchaTB.Text == "")
                {
                    MessageBox.Show("Введите капчу!");
                    return;
                }

                Manager.MainFrame.Navigate(new ProductPage(user));
                ResetForm();
            }
            else
            {
                countErrorAuth++;
                MessageBox.Show("Введены неверные данные");
                if (countErrorAuth > 0 && CaptchaTB.Text == "")
                {
                    MessageBox.Show("Введите капчу!");
                    return;
                }

                if (countErrorAuth == 1)
                {
                    GenerateCaptcha();
                    CaptchaTB.Visibility = Visibility.Visible;
                }

                if (countErrorAuth >= 2)
                {
                    SignInBtn.IsEnabled = false;
                    GenerateCaptcha();
                    await Task.Delay(10000);
                    SignInBtn.IsEnabled = true;
                }

            }

        }

        private void GenerateCaptcha()
        {
            string symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random rnd = new Random();

            captchaOneChar.Text = symbols[rnd.Next(symbols.Length)].ToString();
            captchaTwoChar.Text = symbols[rnd.Next(symbols.Length)].ToString();
            captchaThreeChar.Text = symbols[rnd.Next(symbols.Length)].ToString();
            captchaFourChar.Text = symbols[rnd.Next(symbols.Length)].ToString();
        }

        private void ResetForm()
        {
            LoginTB.Text = "";
            PasswordTB.Text = "";
            CaptchaTB.Text = "";
            CaptchaTB.Visibility = Visibility.Hidden;
        }
    }
}
