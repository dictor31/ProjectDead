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
using WebDead.Model;

namespace WpfDead
{
    /// <summary>
    /// Логика взаимодействия для EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        HttpClient client = new();
        public User User { get; set; }

        public EditorWindow()
        {
            InitializeComponent();

            User = new();
            DataContext = this;
        }
        public EditorWindow(User user)
        {
            InitializeComponent();

            User = user;
            DataContext = this;
        }

        private void Save(object sender, RoutedEventArgs e)
        {

        }
    }
}
