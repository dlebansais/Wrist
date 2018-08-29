using DatabaseManager;
using Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppCSHtml5
{
    public class Login : ILogin, INotifyPropertyChanged
    {
        public Login()
        {
            Name = Persistent.GetValue("name", null);
            Email = Persistent.GetValue("email", null);
            RecoveryQuestion = Persistent.GetValue("question", null);
            Remember = (Persistent.GetValue("remember", null) != null);
            LoginState = (Name != null ? LoginStates.SignedIn : LoginStates.LoggedOff);

            InitSimulation();
        }

        public ILanguage GetLanguage { get { return App.GetLanguage; } }
        public ILogin GetLogin { get { return App.GetLogin; } }

        public LoginStates LoginState { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string RecoveryQuestion { get; set; }
        public string RecoveryAnswer { get; set; }
        public bool Remember { get; set; }

        #region Login
        public void On_Login(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            if (!Remember)
            {
                Persistent.SetValue("name", null);
                Persistent.SetValue("email", null);
                Persistent.SetValue("question", null);
                Persistent.SetValue("remember", null);
            }

            if (string.IsNullOrEmpty(Name))
                destinationPageName = "login failed";

            else if (string.IsNullOrEmpty(Password))
                destinationPageName = "login failed";

            else
            {
                StartLogin(Name, Password, Remember);
                destinationPageName = null;
            }

            Password = null;
        }

        private void StartLogin(string name, string testPassword, bool remember)
        {
            EncryptPassword(testPassword, name, (bool encryptSuccess, object encryptResult) => Login_OnTestPasswordEncrypted(encryptSuccess, encryptResult, name, remember));
        }

        private void Login_OnTestPasswordEncrypted(bool success, object result, string name, bool remember)
        {
            if (success)
            {
                string EncryptedTestPassword = (string)result;
                CheckPassword(name, (bool checkSuccess, object checkResult) => Login_OnCurrentPasswordReceived(checkSuccess, checkResult, EncryptedTestPassword, remember));
            }
            else
                (App.Current as App).GoTo("login failed");
        }

        private void Login_OnCurrentPasswordReceived(bool success, object result, string encryptedTestPassword, bool remember)
        {
            if (success)
            {
                Dictionary<string, string> CheckPasswordResult = (Dictionary<string, string>)result;
                string EncryptedCurrentPassword = CheckPasswordResult["password"];

                if (encryptedTestPassword == EncryptedCurrentPassword)
                {
                    Email = CheckPasswordResult["email"];
                    RecoveryQuestion = CheckPasswordResult["question"];

                    if (remember)
                    {
                        Persistent.SetValue("name", Name);
                        Persistent.SetValue("email", Email);
                        Persistent.SetValue("question", RecoveryQuestion);
                        Persistent.SetValue("remember", "1");
                    }

                    LoginState = LoginStates.SignedIn;

                    NotifyPropertyChanged(nameof(Email));
                    NotifyPropertyChanged(nameof(RecoveryQuestion));
                    NotifyPropertyChanged(nameof(LoginState));

                    (App.Current as App).GoTo("account");
                }
                else
                    (App.Current as App).GoTo("login failed");
            }
            else
                (App.Current as App).GoTo("login failed");
        }
        #endregion

        #region Logout
        public void On_Logout(string pageName, string sourceName, string sourceContent)
        {
            LoginState = LoginStates.LoggedOff;
            Name = null;
            Email = null;
            RecoveryQuestion = null;

            Persistent.SetValue("name", null);
            Persistent.SetValue("email", null);
            Persistent.SetValue("question", null);
        }
        #endregion

        #region Change Password
        public void On_ChangePassword(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            if (string.IsNullOrEmpty(Password))
                destinationPageName = "change password failed #1";

            else if (string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
                destinationPageName = "change password failed #2";

            else if (NewPassword != ConfirmPassword)
                destinationPageName = "change password failed #3";

            else
            {
                StartUpdatePassword(Name, Password, NewPassword);
                destinationPageName = null;
            }

            Password = null;
            NewPassword = null;
            ConfirmPassword = null;
        }

        private void StartUpdatePassword(string name, string testPassword, string newPassword)
        {
            EncryptPassword(testPassword, name, (bool encryptSuccess, object encryptResult) => ChangePassword_OnTestPasswordEncrypted(encryptSuccess, encryptResult, name, newPassword));
        }

        private void ChangePassword_OnTestPasswordEncrypted(bool success, object result, string name, string newPassword)
        {
            if (success)
            {
                string EncryptedTestPassword = (string)result;
                CheckPassword(name, (bool checkSuccess, object checkResult) => ChangePassword_OnCurrentPasswordReceived(checkSuccess, checkResult, EncryptedTestPassword, name, newPassword));
            }
            else
                (App.Current as App).GoTo("change password failed #4");
        }

        private void ChangePassword_OnCurrentPasswordReceived(bool success, object result, string encryptedTestPassword, string name, string newPassword)
        {
            if (success)
            {
                Dictionary<string, string> CheckPasswordResult = (Dictionary<string, string>)result;
                string EncryptedCurrentPassword = CheckPasswordResult["password"];
                
                if (encryptedTestPassword == EncryptedCurrentPassword)
                    EncryptPassword(newPassword, name, (bool encryptSuccess, object encryptResult) => ChangePassword_OnNewPasswordEncrypted(encryptSuccess, encryptResult, name));
                else
                    (App.Current as App).GoTo("change password failed #5");
            }
            else
                (App.Current as App).GoTo("change password failed #4");
        }

        private void ChangePassword_OnNewPasswordEncrypted(bool success, object result, string name)
        {
            if (success)
            {
                string EncryptedNewPassword = (string)result;
                ChangePassword(name, EncryptedNewPassword, ChangePassword_OnPasswordChanged);
            }
            else
                (App.Current as App).GoTo("change password failed #4");
        }

        private void ChangePassword_OnPasswordChanged(bool success, object result)
        {
            if (success)
                (App.Current as App).GoTo("change password success");
            else
                (App.Current as App).GoTo("change password failed #4");
        }
        #endregion

        #region Operations
        private void CheckPassword(string name, Action<bool, object> callback)
        {
            Database.Completed += OnCheckPasswordCompleted;
            Database.Query(new DatabaseQueryOperation("check password", "query_1.php", new Dictionary<string, string>() { { "name", HtmlString.Entities(name) } }, callback));
        }

        private void OnCheckPasswordCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnCheckPasswordCompleted notified");
            Database.Completed -= OnCheckPasswordCompleted;

            Action<bool, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "id", "password", "email", "question" })) != null)
            {
                string Id = Result["id"];
                string EncryptedPassword = Result["password"];
                string Email = Result["email"];
                string RecoveryQuestion = Result["question"];
                Debug.WriteLine($"Account {Id}: password={EncryptedPassword}, email={Email}, question={RecoveryQuestion}");

                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(true, Result));
            }
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(false, null));
        }

        private void EncryptPassword(string plainText, string key, Action<bool, object> callback)
        {
            Database.Completed += OnEncryptPasswordCompleted;
            Database.Encrypt(new DatabaseEncryptOperation("encrypt password", "encrypt.php", "site/key", $"numbatsoft/{key}", "pt", plainText, callback));
        }

        private void OnEncryptPasswordCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnEncryptPasswordCompleted notified");
            Database.Completed -= OnEncryptPasswordCompleted;

            Action<bool, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "encrypted" })) != null)
            {
                string EncryptedPassword = Result["encrypted"];
                Debug.WriteLine($"Encrypted Password: {EncryptedPassword}");

                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(true, EncryptedPassword));
            }
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(false, null));
        }

        private void ChangePassword(string name, string encryptedNewPassword, Action<bool, object> callback)
        {
            Database.Completed += OnChangePasswordCompleted;
            Database.Update(new DatabaseUpdateOperation("change password", "update_1.php", new Dictionary<string, string>() { { "name", HtmlString.Entities(name) }, { "password", HtmlString.Entities(encryptedNewPassword) } }, callback));
        }

        private void OnChangePasswordCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnChangePasswordCompleted notified");
            Database.Completed -= OnChangePasswordCompleted;

            Action<bool, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
            {
                string ChangePasswordResult = Result["result"];

                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ChangePasswordResult == "1", null));
            }
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(false, null));
        }

        private Database Database = Database.Current;
        #endregion

        #region Simulation
        private void InitSimulation()
        {
            OperationHandler.Add(new OperationHandler("/request/encrypt.php", OnEncrypt));
            OperationHandler.Add(new OperationHandler("/request/query_1.php", OnLoginMatchRequest));
        }

        private List<Dictionary<string, string>> OnEncrypt(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string Site = null;
            string Username = null;
            if (parameters.ContainsKey("site/key"))
            {
                string EncodeInfo = parameters["site/key"];
                string[] Splitted = EncodeInfo.Split('/');
                if (Splitted.Length >= 2)
                {
                    Site = Splitted[0].Trim();
                    Username = Splitted[1];
                    for (int i = 2; i < Splitted.Length; i++)
                        Username += "/" + Splitted[i];
                    Username = Username.Trim();
                }
                else
                {
                    Site = null;
                    Username = null;
                }
            }
            else
            {
                Site = null;
                Username = null;
            }

            string PlainText;
            if (parameters.ContainsKey("pt"))
                PlainText = parameters["pt"];
            else
                PlainText = null;

            if (Site == null || Username == null || PlainText == null)
                return Result;

            Result.Add(new Dictionary<string, string>()
            {
                { "encrypted", Convert.ToBase64String(Encoding.UTF8.GetBytes(PlainText)) },
            });

            return Result;
        }

        private List<Dictionary<string, string>> OnLoginMatchRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string Username;
            if (parameters.ContainsKey("name"))
                Username = parameters["name"];
            else
                Username = null;

            if (Username == null)
                return Result;

            foreach (Dictionary<string, string> Line in KnownUserTable)
            {
                if (Line.ContainsKey("id") && Line["id"] == Username)
                    Result.Add(new Dictionary<string, string>()
                    {
                        { "id", Line["id"]},
                        { "password", Line["password"] },
                        { "email", Line["email"] },
                        { "question", Line["question"] },
                    });
            }

            return Result;
        }

        private List<Dictionary<string, string>> KnownUserTable = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string>()
            {
                { "id", "test" },
                { "password", Convert.ToBase64String(Encoding.UTF8.GetBytes("toto")) },
                { "email", "test@test.com" },
                { "question", "no question" },
            }
        };
        #endregion

        #region Implementation of INotifyPropertyChanged
        /// <summary>
        ///     Implements the PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameter is mandatory with [CallerMemberName]")]
        internal void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
