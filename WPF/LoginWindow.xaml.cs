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
    /// 

    public partial class LoginWindow : Window
    {
        private readonly HttpClient _client = new HttpClient();
        public LoginWindow()
        {
            InitializeComponent();
            AutoLoginIfPossible();
        }
        private async void AutoLoginIfPossible()
        {
            string session = Properties.Settings.Default.AutoSessionID;

            if (!string.IsNullOrEmpty(session))
            {
                // spróbuj wejść na stronę, żeby sprawdzić czy sesja żyje
                bool valid = await ValidateSession(session);

                if (valid)
                {
                    var main = new MainWindow(null!, session);
                    main.Show();
                    this.Close();
                    return;
                }
                else
                {
                    // sesja wygasła — czyścić i pokazać okno logowania
                    Properties.Settings.Default.AutoSessionID = "";
                    Properties.Settings.Default.Save();
                }
            }

            // jeśli są zapisane email+hasło → spróbuj automatycznego logowania
            if (!string.IsNullOrEmpty(Properties.Settings.Default.AutoEmail) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.AutoPassword))
            {
                var loginResult = await TryLoginAsync(
                    Properties.Settings.Default.AutoEmail,
                    Properties.Settings.Default.AutoPassword
                );

                if (loginResult != null)
                {
                    Properties.Settings.Default.AutoSessionID = loginResult.SessionID;
                    Properties.Settings.Default.Save();

                    var main = new MainWindow(loginResult.User, loginResult.SessionID);
                    main.Show();
                    this.Close();
                }
            }
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

            var loginResult = await TryLoginAsync(email, password);

            if (loginResult != null)
            {
                var main = new MainWindow(loginResult.User, loginResult.SessionID);
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Login failed. Invalid username or password.");
            }
        }

        private async Task<LoginResult?> TryLoginAsync(string email, string password)
        {
            string url = "https://gpslocation.fcomms.website/api/loginWPF.php";

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

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                dynamic jsonResponse = JsonConvert.DeserializeObject(result);

                if (jsonResponse.success == true)
                {
                    return new LoginResult
                    {
                        User = new User
                        {
                            Username = jsonResponse.user.username,
                            Email = jsonResponse.user.email
                        },
                        SessionID = jsonResponse.sessionID
                    };
                }
            }
            catch (Exception ex)
            {
                // Możesz dodać logowanie błędu lub komunikat
            }

            return null;
        }
    }
    public class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
    public class LoginResult
    {
        public User User { get; set; }
        public string SessionID { get; set; }
    }
}