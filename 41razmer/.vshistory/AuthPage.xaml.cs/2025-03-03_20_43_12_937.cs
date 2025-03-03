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



        private async void SignInBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text;
            string password = PasswordTB.Text;

            if (login == "" || password == "")
            {
                StringBuilder errors = new StringBuilder();
                errors.AppendLine("Введите логин и пароль!");
                if (CaptchaTB.Visibility == Visibility.Visible && CaptchaTB.Text == "")
                    errors.AppendLine("Введите капчу!");
                MessageBox.Show(errors.ToString());
                return;
            }

            User user = Abdeev41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (user != null)
            {
                MessageBox.Show(countErrorAuth.ToString());
                if (countErrorAuth > 0 && CaptchaTB.Text == "")
                {
                    MessageBox.Show("Введите капчу!");
                    return;
                }
                Manager.MainFrame.Navigate(new ProductPage(user));
                ResetForm();
            }
            else
            {
                MessageBox.Show(countErrorAuth.ToString());
                MessageBox.Show("Введены неверные данные");
                if (countErrorAuth > 0 && CaptchaTB.Text == "")
                {
                    MessageBox.Show("Введите капчу!");
                    return;
                }
                countErrorAuth++;
                string symbolsCaptcha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                CaptchaTB.Visibility = Visibility.Visible;
                Random rnd = new Random();
                if (countErrorAuth == 1)
                {
                    captchaOneChar.Text = symbolsCaptcha[rnd.Next(0, symbolsCaptcha.Length)].ToString();
                    captchaTwoChar.Text = symbolsCaptcha[rnd.Next(0, symbolsCaptcha.Length)].ToString();
                    captchaThreeChar.Text = symbolsCaptcha[rnd.Next(0, symbolsCaptcha.Length)].ToString();
                    captchaFourChar.Text = symbolsCaptcha[rnd.Next(0, symbolsCaptcha.Length)].ToString();
                    string captcha = captchaOneChar.Text + captchaTwoChar.Text + captchaThreeChar.Text + captchaFourChar.Text;

                    if (captcha == CaptchaTB.Text)
                    {
                        Manager.MainFrame.Navigate(new ProductPage(user));
                        ResetForm();
                    }
                }
                else
                {
                    SignInBtn.IsEnabled = false;
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

    }
}

