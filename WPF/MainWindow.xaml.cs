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

        private readonly HttpClient _client = new HttpClient();

        //private async Task LogoutAsync()
        //{
        //    var response = await _client.GetAsync("https://gpslocation.fcomms.website/api/logoutWPF.php");

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var cookieManager = MyWebView.CoreWebView2.CookieManager;

        //        var cookies = await cookieManager.GetCookiesAsync("https://gpslocation.fcomms.website");
        //        foreach (var cookie in cookies)
        //        {
        //            if (cookie.Name == "PHPSESSID")
        //            {
        //                cookieManager.DeleteCookie(cookie);
        //            }
        //        }

        //        this.Close();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Logout failed.");
        //    }
        //}

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(User user, string sessionId)
        {
            InitializeComponent();
            _user = user;

            UserTestText.Text = $"Zalogowany jako: {_user.Username} ({_user.Email})";
            InitializeAsync(sessionId);
        }

        async void InitializeAsync(string sessionId)
        {
            await MyWebView.EnsureCoreWebView2Async();

            var cookie = MyWebView.CoreWebView2.CookieManager.CreateCookie(
                "PHPSESSID", sessionId, "gpslocation.fcomms.website", "/");
            MyWebView.CoreWebView2.CookieManager.AddOrUpdateCookie(cookie);

            MyWebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;

            MyWebView.CoreWebView2.Navigate("https://gpslocation.fcomms.website/index.php");
        }

        //private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        //{
        //    await LogoutAsync();
        //}
        private async void CoreWebView2_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                if (e.IsSuccess)
                {
                    string currentUrl = MyWebView.CoreWebView2.Source;

                    if (currentUrl.Contains("login.php") || currentUrl.Contains("logout") || currentUrl.Contains("session_expired"))
                    {
                        MessageBox.Show("Użytkownik został Wylogowany.");
                        Application.Current.Shutdown();
                    }
                }
                else
                {
                    // Błąd ładowania — może oznaczać, że sesja już nie istnieje
                    MessageBox.Show("Błąd ładowania strony lub utrata sesji. Aplikacja zostanie zamknięta.");
                    Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd przy sprawdzaniu sesji: {ex.Message}");
            }
        }
    }
}



