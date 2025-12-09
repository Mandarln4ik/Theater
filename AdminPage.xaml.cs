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
using Theater.Services;

namespace Theater
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public Services.NavigationService Nav = new();
        public AdminPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Nav.Add(SessionsPage);
            Nav.Add(PlaysPage);
        }

        private void Session_Click(object sender, RoutedEventArgs e)
        {
            Nav.NavigateTo(SessionsPage);
        }

        private void Plays_Click(object sender, RoutedEventArgs e)
        {
            Nav.NavigateTo(PlaysPage);
        }
    }
}
