using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Nav = Theater.Services.NavigationService;

namespace Theater
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.Instance.NavigateTo(MainFrame);
        }

        private void TruppaBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.Instance.NavigateTo(TruppaFrame);
        }

        private void AdminBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.Instance.NavigateTo(AdminFrame);
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            DataContext = App.GetApp();
            Nav nav = new();
            Nav.Instance.Add(new List<Frame> { MainFrame, TruppaFrame, BookingFrame, ProfileFrame, AdminFrame });
            Nav.Instance.NavigateTo(MainFrame);
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            App app = App.GetApp();
            if (app.currentUser == null)
            {
                app.SignIn();
            }
            else
            {
                Nav.Instance.NavigateTo(ProfileFrame);
            }
        }
    }
}
