using Client.Models;
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

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для PhoneDetailsPage.xaml
    /// </summary>
    public partial class PhoneDetailsPage : Page
    {
        public PhoneDetailsPage(Phone phone)
        {
            InitializeComponent();

            TitleField.Text = phone.Title; 
            CompanyField.Text = phone.CompanyEntity.Title; 
            PriceField.Text = phone.Price.ToString(); 
            DescriptionField.Text = phone.Description; 
            //PhoneImage.Source = new BitmapImage(new Uri(phone.Image));
        }


        private void BackClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(null);
            Navigation.MainFrame.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
