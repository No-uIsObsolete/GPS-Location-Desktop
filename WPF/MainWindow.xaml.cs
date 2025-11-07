using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly User _user;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(User user)
        {
            InitializeComponent();
            _user = user;

            UserTestText.Text = $"Zalogowany jako: {_user.Username} ({_user.Email})";
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            await MyWebView.EnsureCoreWebView2Async();
            MyWebView.CoreWebView2.Navigate("https://gpslocation.fcomms.website/index.php");
        }
    }
}




// Rozwiązania mapki


// > 1 OsmSharp library
// > 3 JS Iframe

// X -2-devExpress-library- 