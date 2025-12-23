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


        //private void EndEditing()
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        ModelInput.Clear();
        //        BrandInput.Clear();
        //        PriceInput.Clear();

        //        //ModelInput.Text = string.Empty;
        //        //BrandInput.Text = string.Empty;
        //        //PriceInput.Text = string.Empty;

        //        //mainListBox.IsEnabled = false;
        //        //Add.IsEnabled = false;
        //        //Edit.IsEnabled = false;
        //        //Bin.IsEnabled = false;

        //        Save.Visibility = Visibility.Collapsed;
        //        Cancel.Visibility = Visibility.Collapsed;
        //        Add.IsEnabled = true;
        //        Edit.IsEnabled = true;
        //        Bin.IsEnabled = true;
        //    });
        //}


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

            //Dispatcher.Invoke(() =>
            //{
            //    mainListBox.ItemsSource = null;
            //    mainListBox.ItemsSource = phones;
            //});


            //Dispatcher.Invoke(() =>
            //{
            //    ModelInput.Clear();
            //    BrandInput.Clear();
            //    PriceInput.Clear();
            //});
        }



        //private bool ValidateInput(out string errorMessage)
        //{
        //    if (string.IsNullOrWhiteSpace(BrandInput.Text))
        //    {
        //        errorMessage = "Поле 'Модель' не может быть пустым";
        //        BrandInput.Focus();
        //        return false;
        //    }

        //    if (string.IsNullOrWhiteSpace(ModelInput.Text))
        //    {
        //        errorMessage = "Поле 'Производитель' не может быть пустым";
        //        ModelInput.Focus();
        //        return false;
        //    }

        //    if (string.IsNullOrWhiteSpace(PriceInput.Text))
        //    {
        //        errorMessage = "Поле 'Цена' не может быть пустым";
        //        PriceInput.Focus();
        //        return false;
        //    }

        //    if (!decimal.TryParse(PriceInput.Text, out decimal price))
        //    {
        //        errorMessage = "Введите корректное числовое значение для цены";
        //        PriceInput.Focus();
        //        PriceInput.SelectAll();
        //        return false;
        //    }

        //    if (price <= 0)
        //    {
        //        errorMessage = "Цена должна быть больше нуля";
        //        PriceInput.Focus();
        //        PriceInput.SelectAll();
        //        return false;
        //    }

        //    errorMessage = string.Empty;
        //    return true;
        //}


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


        public void CancelClick(object sender, RoutedEventArgs e)
        {
            //EndEditing();


        }



        //public async void SaveClick(object sender, RoutedEventArgs e)
        //{
        //    HideErrorMessage();

        //    if (!ValidateInput(out string errorMessage))
        //    {
        //        ShowErrorMessage(errorMessage);
        //        return;
        //    }

        //    if (mainListBox.SelectedItem is not Phone selected)
        //    {
        //        return;
        //    }

        //    //var phone = new Phone
        //    //{
        //    //    Company = ModelInput.Text,
        //    //    Title = BrandInput.Text,
        //    //    Price = Convert.ToDecimal(PriceInput.Text)
        //    //};

        //    selected.Company = ModelInput.Text;
        //    selected.Title = BrandInput.Text;
        //    selected.Price = Convert.ToDecimal(PriceInput.Text);

        //    await httpService.EditPhone(selected);
        //    await LoadPhones();

        //    //mainListBox.ItemsSource = null;
        //    //mainListBox.ItemsSource = (System.Collections.IEnumerable?)phone;

        //    //EndEditing();
        //}



        //private async Task LoadPhones()
        //{
        //    try
        //    {
        //        phones = await httpService.GetPhones();
        //        Dispatcher.Invoke(() =>
        //        {
        //            mainDataGridView.ItemsSource = null;
        //            mainDataGridView.ItemsSource = phones;
        //        });

        //        //Dispatcher.Invoke(() =>
        //        //{
        //        //    ModelInput.Clear();
        //        //    BrandInput.Clear();
        //        //    PriceInput.Clear();
        //        //});

        //        HideErrorMessage();
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowErrorMessage($"Ошибка при загрузке данных: {ex.Message}");
        //    }
        //}


        public async Task RefreshTable()
        {
            HideErrorMessage();

            mainDataGridView.ItemsSource = null;
            var phones = await httpService.GetPhones();

            mainDataGridView.ItemsSource = phones;
        }
    }
}