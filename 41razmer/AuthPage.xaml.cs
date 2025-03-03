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
        private int countErrorAuth = 0;
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

        string captcha;

        private async void SignInBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text;
            string password = PasswordTB.Text;

            if (login == "" || password == "")
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            if (countErrorAuth > 0)
            {
                if (string.IsNullOrEmpty(CaptchaTB.Text))
                {
                    MessageBox.Show("Введите капчу!");
                    return;
                }

                if (CaptchaTB.Text != captcha)
                {
                    MessageBox.Show("Капча введена неверно!");
                    return;
                }
            }

            User user = Abdeev41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (user != null)
            {
                countErrorAuth = 0;
                captcha = "";
                Manager.MainFrame.Navigate(new ProductPage(user));
                ResetForm();
            }
            else
            {
                MessageBox.Show("Введены неверные данные");
                countErrorAuth++;
                if (countErrorAuth == 1)
                {
                    captcha = GenerateCaptcha();
                    CaptchaTB.Visibility = Visibility.Visible;
                }
                else
                {
                    SignInBtn.IsEnabled = false;

                    captcha = GenerateCaptcha();

                    await Task.Delay(10000);
                    SignInBtn.IsEnabled = true;
                }

            }
        }

        private void ResetForm()
        {
            LoginTB.Text = "";
            PasswordTB.Text = "";
            CaptchaTB.Visibility = Visibility.Hidden;
            CaptchaTB.Text = "";
            captchaOneChar.Text = "";
            captchaTwoChar.Text = "";
            captchaThreeChar.Text = "";
            captchaFourChar.Text = "";
        }

        private string GenerateCaptcha()
        {
            string symbolsCaptcha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random rnd = new Random();
            captchaOneChar.Text = symbolsCaptcha[rnd.Next(0, symbolsCaptcha.Length)].ToString();
            captchaTwoChar.Text = symbolsCaptcha[rnd.Next(0, symbolsCaptcha.Length)].ToString();
            captchaThreeChar.Text = symbolsCaptcha[rnd.Next(0, symbolsCaptcha.Length)].ToString();
            captchaFourChar.Text = symbolsCaptcha[rnd.Next(0, symbolsCaptcha.Length)].ToString();
            captcha = captchaOneChar.Text + captchaTwoChar.Text + captchaThreeChar.Text + captchaFourChar.Text;
            return captcha;
        }
    }
}

