using Client.Models;
using Client.Services;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        HttpClientService httpClient;
        private MainWindow mainWindow;

        private readonly string imageFolder = System.IO.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Images");

        private Microsoft.Win32.OpenFileDialog _imgDialog;
        private string _selectedImagePath;


        public AddWindow(HttpClientService http, MainWindow owner)
        {
            InitializeComponent();
            //NameInput.Text = phone.Title;
            //CompanyInput.SelectedValue = phone.CompanyEntity.Id;
            //PriceInput.Text = phone.Price.ToString();
            httpClient = http;
            mainWindow = owner;

            _ = LoadCompanies();
        }

        public async void AddButtonClick(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }

            string finalImagePath = System.IO.Path.Combine(
                imageFolder,
                _imgDialog != null
                ? Guid.NewGuid().ToString() + System.IO.Path.GetExtension(_imgDialog.FileName) 
                : "noimage.jpg"
            );

            if (_imgDialog != null)
            {
                File.Copy(_imgDialog.FileName, finalImagePath, overwrite: true);
            }

            string description = string.IsNullOrWhiteSpace(DescriptionInput.Text)
                ? "Описание отсутствует" : DescriptionInput.Text;


            if (!ValidateInput(out string errorMessage))
            {
                mainWindow.ShowErrorMessage(errorMessage);
                return;
            }

            await httpClient.GetCompanies();

            await httpClient.AddPhones(new Phone
            {
                Title = NameInput.Text,
                CompanyId = (int)CompanyInput.SelectedValue,
                Price = Convert.ToDecimal(PriceInput.Text),
                Description = description,
                Image = finalImagePath
            });
            await (this.Owner as MainWindow).RefreshTable();
            this.Close();
        }


        private void SelectImageClick(object sender, RoutedEventArgs e)
        {
            _imgDialog = new Microsoft.Win32.OpenFileDialog();
            _imgDialog.Filter = "Изображения (*.jpg;*.png)|*.jpg;*.png";

            if (_imgDialog.ShowDialog() == true)
            {
                _selectedImagePath = _imgDialog.FileName;
                PreviewImage.Source = new BitmapImage(new Uri(_selectedImagePath));
            }
        }



        private async Task LoadCompanies()
        {
            var companies = await httpClient.GetCompanies();

            CompanyInput.ItemsSource = companies;
            CompanyInput.DisplayMemberPath = "Title";   
            CompanyInput.SelectedValuePath = "Id";      
        }


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
