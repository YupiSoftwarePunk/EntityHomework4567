using Client.Models;
using Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        HttpClientService httpService;
        Phone _tempPhone;
        MainWindow mainWindow;
        public EditWindow(Phone phone, HttpClientService http)
        {
            InitializeComponent();
            _tempPhone = phone;
            mainWindow = new MainWindow();
            httpService = http;

            CompanyInput.ItemsSource = (System.Collections.IEnumerable)httpService.GetCompanies();
            NameInput.Text = phone.Title;
            CompanyInput.SelectedValue = phone.CompanyEntity.Id;
            PriceInput.Text = phone.Price.ToString();
        }


        public async void EditButtonClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput(out string errorMessage))
            {
                mainWindow.ShowErrorMessage(errorMessage);
                return;
            }

            await httpService.EditPhone(new Phone
            {
                Title = NameInput.Text,
                CompanyId = (int)CompanyInput.SelectedValue,
                Price = Convert.ToDecimal(PriceInput.Text)
            });
            await (this.Owner as MainWindow).RefreshTable();
            this.Close();
        }



        //private async Task LoadCompanies()
        //{
        //    var companies = await httpClient.GetCompanies();

        //    CompanyInput.ItemsSource = companies;
        //    CompanyInput.DisplayMemberPath = "Title";
        //    CompanyInput.SelectedValuePath = "Id";
        //}



        private bool ValidateInput(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(NameInput.Text))
            {
                errorMessage = "Поле 'Название' не может быть пустым";
                NameInput.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(PriceInput.Text))
            {
                errorMessage = "Поле 'Цена' не может быть пустым";
                PriceInput.Focus();
                return false;
            }

            if (!decimal.TryParse(PriceInput.Text, out decimal price))
            {
                errorMessage = "Введите корректное числовое значение для цены";
                PriceInput.Focus();
                PriceInput.SelectAll();
                return false;
            }

            if (price <= 0)
            {
                errorMessage = "Цена должна быть больше нуля";
                PriceInput.Focus();
                PriceInput.SelectAll();
                return false;
            }

            if (string.IsNullOrWhiteSpace(CompanyInput.Text))
            {
                errorMessage = "Поле 'Компания' не может быть пустым";
                CompanyInput.Focus();
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
