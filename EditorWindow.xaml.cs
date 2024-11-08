using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WebDead.Model;

namespace WpfDead
{
    public partial class EditorWindow : Window
    {
        HttpClient client = new();
        public User User { get; set; }

        public EditorWindow()
        {
            InitializeComponent();

            User = new();
            client.BaseAddress = new Uri("https://localhost:7012/api/");
            DataContext = this;
        }
        public EditorWindow(User find)
        {
            InitializeComponent();

            client.BaseAddress = new Uri("https://localhost:7012/api/");
            User = find;
            DataContext = this;
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            if (User.Id == 0)
            {
                User.Ban = Ban_Y.IsChecked == true ? true : false;
                User.Admin = Admin_Y.IsChecked == true ? true : false;

                string json = JsonSerializer.Serialize(User);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var responce = await client.PostAsync("DB/CreateUser", content);
                if (responce.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Пользователь существует");
                    return;
                }
                MessageBox.Show("Пользователь создан");
            }
            else
            {
                User.Ban = Ban_Y.IsChecked == true ? true : false;
                User.Admin = Admin_Y.IsChecked == true ? true : false;
                string json = JsonSerializer.Serialize(User);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var responce = await client.PutAsync("DB/PutUser", content);
            }
            Close();
        }
    }
}
