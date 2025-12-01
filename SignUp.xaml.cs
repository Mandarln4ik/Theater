using MySql.Data.MySqlClient;
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
    public partial class SignUp : Window
    {
        public User User { get; set; }

        public SignUp()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string FirstName = FirstNameBox.Text;
            string LastName = LastNameBox.Text;
            string email = EmailBox.Text;
            if (!email.Contains("@") || !email.Contains("."))
            {
                EmailError.Text = "Введен некорректный Email!";
                return;
            }
            string pass = PasswordBox.Text;

            var conn = DBmanager.GetConnection();
            var cmd = new MySqlCommand($"SELECT id FROM `Users` WHERE email = '{email}'", conn);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    EmailError.Text = "Аккаунт с таким Email уже существует!";
                    return;
                }
            }

            cmd = new MySqlCommand($"INSERT INTO `Users` (first_name, last_name, email, password_hash) VALUES ('{FirstName}', '{LastName}', '{email}', '{PasswordHash.HashPassword(pass)}')", conn);
            cmd.ExecuteNonQuery();
            long userid = cmd.LastInsertedId;
            User = new User((int)userid);
            Settings.Default.LastLogIn = DateTime.Now;
            Settings.Default.Email = email;
            Settings.Default.PasswordHash = PasswordHash.HashPassword(pass);
            Settings.Default.Save();
            DialogResult = true;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
            App.GetApp().SignIn(); ;
            Close();
        }

        private void EmailBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EmailError.Text = "";
        }
    }
}
