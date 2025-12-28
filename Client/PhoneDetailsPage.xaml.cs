using Client.Models;
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
            if (File.Exists(phone.Image))
            {
                var bitmap = new BitmapImage();
                using (var stream = File.OpenRead(phone.Image))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }
                PhoneImage.Source = bitmap;
            }
            //else
            //{
            //    PhoneImage = "Resources\noimage.jpg";
            //}
        }


        private void BackClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(null);
            Navigation.MainFrame.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
