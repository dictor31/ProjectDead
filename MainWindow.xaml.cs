using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using System.Windows;
using WebDead.Model;

namespace WpfDead
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client = new();

        public ObservableCollection<User> Users { get; set; }
        public User User { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();

            client.BaseAddress = new Uri("https://localhost:7012/api/");
            DataContext = this;
        }

        private async void Enter(object sender, RoutedEventArgs e)
        {
            var responce = await client.GetAsync("DB/GetUsers");
            var responceBody = await responce.Content.ReadAsStringAsync();
            Users = JsonConvert.DeserializeObject<ObservableCollection<User>>(responceBody);
            foreach (var user in Users)
            {
                if (user.Login == User.Login && user.Password == User.Password && !user.Admin && !user.Ban)
                {
                    MessageBox.Show("Вход успешен");
                    return;
                }
                else if (user.Login == User.Login && user.Password == User.Password && user.Admin && !user.Ban)
                {
                    AdminWindow adminWindow = new AdminWindow();
                    adminWindow.Show();
                    Close();
                    return;
                }
                else if (user.Login == User.Login && user.Password == User.Password && !user.Admin && user.Ban)
                {
                    MessageBox.Show("Пользователь заблокирован");
                    return;
                }
            }
            MessageBox.Show("Такого пользователя нет");
        }
    }
}