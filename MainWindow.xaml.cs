using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Text.Json;
using WebDead.Model;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace WpfDead
{
    public partial class MainWindow : Window
    {
        string lastLogin;

        public User User { get; set; } = new();
        int count = 3;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        private async void Enter(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(User.Login) || String.IsNullOrEmpty(User.Password))
            {
                MessageBox.Show("Не все поля заполнены");
                return;
            }
            var responce = await Client.HttpClient.GetAsync($"DB/SearchUser?login={User.Login}");
            var responceBody = await responce.Content.ReadAsStringAsync();
            if (responce.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                MessageBox.Show("Пользователь не найден");
                return;
            }
            else if (responce.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                MessageBox.Show("Введены неверные данные");
                return;
            }
            User find = JsonConvert.DeserializeObject<User>(responceBody);
            if ((DateTime.Today - find.LastLogin) > new TimeSpan(31, 0, 0, 0) && !find.Ban)
            {
                find.Ban = true;
                find.LastLogin = DateTime.Now;
                string json = System.Text.Json.JsonSerializer.Serialize(find);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await Client.HttpClient.PutAsync("DB/PutUser", content);
                MessageBox.Show("Аккаунт был заблокирован из-за длительного отсутствия");
                return;
            }
            else if (find.Login == User.Login && find.Password == User.Password && !find.Admin && !find.Ban)
            {
                MessageBox.Show("Вход успешен");
                find.LastLogin = DateTime.Now;
                string json = System.Text.Json.JsonSerializer.Serialize(find);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await Client.HttpClient.PutAsync("DB/PutUser", content);
                return;
            }
            else if (find.Login == User.Login && find.Password == User.Password && find.Admin && !find.Ban)
            {
                if (find.LastLogin == null)
                {
                    PasswordWindow passwordWindow = new(find);
                    passwordWindow.ShowDialog();
                }
                find.LastLogin = DateTime.Now;
                string json = System.Text.Json.JsonSerializer.Serialize(find);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await Client.HttpClient.PutAsync("DB/PutUser", content);

                TokenRole tokenRole = await Client.HttpClient.GetFromJsonAsync<TokenRole>($"Jwt/login?login={find.Login}&password={find.Password}");
                Client.SetToken(tokenRole.Token);

                AdminWindow adminWindow = new AdminWindow();
                adminWindow.Show();
                Close();
                return;
            }
            else if (find.Login == User.Login && find.Ban)
            {
                MessageBox.Show("Пользователь заблокирован");
                return;
            }
            else if (find.Login == User.Login && find.Password != User.Password)
            {
                if (lastLogin != find.Login)
                {
                    count = 3;
                }
                lastLogin = find.Login;
                count -= 1;
                MessageBox.Show($"Пароль введён неверно. Осталось попыток: {count}");
                if (count < 1)
                {
                    find.Ban = true;
                    string json = System.Text.Json.JsonSerializer.Serialize(find);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var res = await Client.HttpClient.PutAsync("DB/PutUser", content);
                    MessageBox.Show("Аккаунт был заблокирован");
                    count = 3;
                }
                return;
            }
        }
    }
}