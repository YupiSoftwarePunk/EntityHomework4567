using Client.Models;
using Client.Services;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClientService httpService;
        public MainWindow()
        {
            InitializeComponent();
            Navigation.MainFrame = MainFrame;

            httpService = new HttpClientService("http://localhost:5050/api");
            _ = RefreshTable();
        }


        public async void EditClick(object sender, RoutedEventArgs e)
        {
            HideErrorMessage();

            if (mainDataGridView.SelectedItem is not Phone phone)
            {
                ShowErrorMessage("Выберите элемент для изменения");
                return;
            }

            var edit = new EditWindow(phone, httpService);
            edit.Owner = this;
            edit.ShowDialog();

            await RefreshTable();
        }


        private void SelectRowOnClick(object sender, RoutedEventArgs e)
        {
            var row = ItemsControl.ContainerFromElement(mainDataGridView, (DependencyObject)e.OriginalSource) as DataGridRow;
            if (row != null)
            {
                mainDataGridView.SelectedItem = row.Item;
            }
        }

        public async void AddClick(object sender, RoutedEventArgs e)
        {
            HideErrorMessage();

            AddWindow addWindow = new AddWindow(httpService, this);
            addWindow.Owner = this;
            addWindow.ShowDialog();
            await RefreshTable();
        }


        public async void BinClick(object sender, RoutedEventArgs e)
        {
            HideErrorMessage();

            if (mainDataGridView.SelectedItem is Phone selected)
            {
                await httpService.DeletePhones(selected.Id);
                //await LoadPhones();
                await RefreshTable();
            }
        }


        public void ShowErrorMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                ErrorMessage.Text = message;
                ErrorBorder.Visibility = Visibility.Visible;
            });
        }


        private void HideErrorMessage()
        {
            Dispatcher.Invoke(() =>
            {
                ErrorMessage.Text = string.Empty;
                ErrorBorder.Visibility = Visibility.Collapsed;
            });
        }


        public async Task RefreshTable()
        {
            HideErrorMessage();

            mainDataGridView.ItemsSource = null;
            var phones = await httpService.GetPhones();

            mainDataGridView.ItemsSource = phones;
        }


        public void DetailsClick(object sender, EventArgs e)
        {
            if (mainDataGridView.SelectedItem is not Phone phone)
            {
                ShowErrorMessage("Выберите элемент");
                return;
            }

            Navigation.MainFrame.Visibility = Visibility.Visible;
            Navigation.MainFrame.Navigate(new PhoneDetailsPage(phone));
        }

    }
}