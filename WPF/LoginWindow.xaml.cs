using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace WPF
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _client = new HttpClient();
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailInput.Text.Trim();
            string password = PasswordInput.Password.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }


            var user = await TryLoginAsync(email, password);

            if (user != null)
            {
                var main = new MainWindow(user);
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Login failed. Invalid username or password.");
            }
        }

        private async Task<User?> TryLoginAsync(string email, string password)
        {
            string url = "https://gpslocation.fcomms.website/api/login.php"; 

            var data = new
            {
                email = email,
                password = password
            };

            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _client.PostAsync(url, content);
                string result = await response.Content.ReadAsStringAsync();

                //MessageBox.Show(result);

                if (!response.IsSuccessStatusCode)
                {
                    //MessageBox.Show($"Server returned HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
                    return null;
                }

                dynamic jsonResponse = JsonConvert.DeserializeObject(result);

                if (jsonResponse.success == true)
                {
                    return new User
                    {
                        Username = jsonResponse.user.username,
                        Email = jsonResponse.user.email
                    };
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Connection error: " + ex.Message);
            }

            return null;
        }
    }
    public class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}