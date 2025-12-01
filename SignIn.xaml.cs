using MySql.Data.MySqlClient;
using System;
using System.CodeDom;
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
using System.Windows.Shapes;
using Theater.DataBase;
using Theater.Properties;
using Theater.Services;
using Theater.Services.DataBase;

namespace Theater
{
    /// <summary>
    /// Логика взаимодействия для SignUp.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        public User User { get; set; }

        public SignIn()
        {
            InitializeComponent();
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text;
            if (!EmailBox.Text.Contains("@") || !EmailBox.Text.Contains("."))
            {
                EmailError.Text = "Введен некорректный Email!";
                return;
            }
            string pass = PasswordBox.Text;
            string userpass = "";
            var conn = DBmanager.GetConnection();
            var cmd = new MySqlCommand($"SELECT password_hash FROM `Users` WHERE email = '{email}'", conn);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        userpass = reader.GetString(0);
                    }
                }
                else
                {
                    EmailError.Text = "Аккаунта с таким Email не существует!";
                    return;
                }
            }

            if (!PasswordHash.VerifyPassword(pass, userpass))
            {
                PasswordError.Text = "Неверный пароль!";
                return;
            }
            else
            {
                cmd = new MySqlCommand($"SELECT id FROM `Users` WHERE email = '{email}' AND password_hash = '{PasswordHash.HashPassword(pass)}'", conn);
                int id = -1;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = (reader.GetInt32(0));
                    }
                }
                if (id != -1)
                {
                    User = new User(id);
                    Settings.Default.LastLogIn = DateTime.Now;
                    Settings.Default.Email = email;
                    Settings.Default.PasswordHash = PasswordHash.HashPassword(pass);
                    Settings.Default.Save();
                    DialogResult = true;
                }
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
            App.GetApp().SignUp(); ;
            Close();
        }

        private void EmailBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ResetErrors();
        }

        private void PasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ResetErrors();
        }

        private void ResetErrors()
        {
            EmailError.Text = "";
            PasswordError.Text = "";
        }
    }
}
