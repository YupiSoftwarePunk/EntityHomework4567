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
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        HttpClientService httpService;
        Phone _tempPhone;
        MainWindow mainWindow;

        private readonly string imageFolder = System.IO.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Images");

        private Microsoft.Win32.OpenFileDialog _imgDialog;
        private string _selectedImagePath;


        public EditWindow(Phone phone, HttpClientService http)
        {
            InitializeComponent();

            _tempPhone = phone;
            httpService = http;

            _ = LoadCompanies();

            NameInput.Text = phone.Title;
            CompanyInput.SelectedValue = phone.CompanyEntity.Id;
            PriceInput.Text = phone.Price.ToString();
            DescriptionInput.Text = phone.Description;

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
                PreviewImage.Source = bitmap;
                _selectedImagePath = phone.Image;
            }
        }


        private async Task LoadCompanies()
        {
            var companies = await httpService.GetCompanies();

            CompanyInput.ItemsSource = companies;
            CompanyInput.DisplayMemberPath = "Title";
            CompanyInput.SelectedValuePath = "Id";
            CompanyInput.SelectedValue = _tempPhone.CompanyId;
        }


        public async void EditButtonClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput(out string errorMessage))
            {
                (this.Owner as MainWindow)?.ShowErrorMessage(errorMessage); 
                return;
            }

            string finalImagePath = _tempPhone.Image;

            if (_imgDialog != null)
            {
                if (File.Exists(_tempPhone.Image))
                    File.Delete(_tempPhone.Image);

                finalImagePath = System.IO.Path.Combine(imageFolder, _imgDialog.SafeFileName);

                File.Copy(_imgDialog.FileName, finalImagePath, overwrite: true);
            }


            await httpService.EditPhone(new Phone
            {
                Id = _tempPhone.Id,
                Title = NameInput.Text,
                CompanyId = (int)CompanyInput.SelectedValue,
                Price = Convert.ToDecimal(PriceInput.Text),
                Description = DescriptionInput.Text,
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

            if (CompanyInput.SelectedItem == null)
            {
                errorMessage = "Выберите компанию из списка";
                CompanyInput.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(DescriptionInput.Text))
            {
                errorMessage = "Поле 'Описание' не может быть пустым";
                DescriptionInput.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(_selectedImagePath))
            {
                errorMessage = "Выберите изображение";
                SelectImageButton.Focus();
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
