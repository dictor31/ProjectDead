using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Threading;
using WebDead.Model;

namespace WpfDead
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<User> Users { get; set; } = new();
        HttpClient client = new();
        private User selectedUser;

        public User SelectedUser 
        {
            get => selectedUser;
            set 
            {
                selectedUser = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SelectedUser"));
            }
        }
        
        public AdminWindow()
        {
            InitializeComponent();
            client.BaseAddress = new Uri("https://localhost:7012/api/");

            //DispatcherTimer timer = new();
            //timer.Interval = TimeSpan.FromSeconds(5);
            //timer.Tick += GetUsers;
            //timer.Start();

            GetUsers(null, null);
            DataContext = this;
        }
        private async void GetUsers(object? sender, EventArgs e)
        {
            Users.Clear();
            var responce = await client.GetAsync("DB/GetUsers");
            var responceBody = await responce.Content.ReadAsStringAsync();
            ObservableCollection<User> users = JsonConvert.DeserializeObject<ObservableCollection<User>>(responceBody);
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        private void AddUser(object sender, RoutedEventArgs e)
        {
            EditorWindow editor = new();
            editor.Show();
        }

        private void EditUser(object sender, RoutedEventArgs e)
        {
            EditorWindow editor = new(SelectedUser);
            editor.Show();
        }
    }
}
