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
using Theater.View;
using Nav = Theater.Services.NavigationService;

namespace Theater
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            DataContext = App.GetApp();
        }

        private void ExitFromAccount_Click(object sender, RoutedEventArgs e)
        {
            App.GetApp().LogOut();
        }

        private void OpenPerformance_Click(object sender, RoutedEventArgs e)
        {
            OrderView ov = (sender as Button).DataContext as OrderView;
            App.GetApp().LoadPerformance(ov.Title);
            Nav.Instance.NavigateTo(2);
        }

        private void ShowQr_Click(object sender, RoutedEventArgs e)
        {
            int id = ((sender as Button).DataContext as OrderView).PerformanceId;
            BlurGrid.Visibility = Visibility.Visible;
            QRGrid.Visibility = Visibility.Visible;
            App.GetApp().GenerateQRcode(id);
        }

        private void CloseQR_Click(object sender, RoutedEventArgs e)
        {
            BlurGrid.Visibility = Visibility.Hidden;
            QRGrid.Visibility = Visibility.Hidden;
        }
    }
}
