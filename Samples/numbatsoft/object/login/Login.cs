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
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string NewEmail { get; set; }
        public string RecoveryQuestion { get; set; }
        public string RecoveryAnswer { get; set; }
        public bool Remember { get; set; }

        #region Login
        public void On_Login(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (!Remember)
            {
                Persistent.SetValue("name", null);
                Persistent.SetValue("email", null);
                Persistent.SetValue("question", null);
                Persistent.SetValue("remember", null);
            }

            if (string.IsNullOrEmpty(Name))
                destinationPageName = PageNames.login_failedPage;

            else if (string.IsNullOrEmpty(Password))
                destinationPageName = PageNames.login_failedPage;

            else
            {
                StartLogin(Name, Password, Remember);
                destinationPageName = PageNames.CurrentPage;
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
                (App.Current as App).GoTo(PageNames.login_failedPage);
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

                    (App.Current as App).GoTo(PageNames.accountPage);
                }
                else
                    (App.Current as App).GoTo(PageNames.login_failedPage);
            }
            else
                (App.Current as App).GoTo(PageNames.login_failedPage);
        }
        #endregion

        #region Logout
        public void On_Logout(PageNames pageName, string sourceName, string sourceContent)
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
        public void On_ChangePassword(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (string.IsNullOrEmpty(Password))
                destinationPageName = PageNames.change_password_failed_1Page;

            else if (string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
                destinationPageName = PageNames.change_password_failed_2Page;

            else if (NewPassword != ConfirmPassword)
                destinationPageName = PageNames.change_password_failed_3Page;

            else
            {
                StartUpdatePassword(Name, Password, NewPassword);
                destinationPageName = PageNames.CurrentPage;
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
                (App.Current as App).GoTo(PageNames.change_password_failed_4Page);
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
                    (App.Current as App).GoTo(PageNames.change_password_failed_5Page);
            }
            else
                (App.Current as App).GoTo(PageNames.change_password_failed_4Page);
        }

        private void ChangePassword_OnNewPasswordEncrypted(bool success, object result, string name)
        {
            if (success)
            {
                string EncryptedNewPassword = (string)result;
                ChangePassword(name, EncryptedNewPassword, ChangePassword_OnPasswordChanged);
            }
            else
                (App.Current as App).GoTo(PageNames.change_password_failed_4Page);
        }

        private void ChangePassword_OnPasswordChanged(bool success, object result)
        {
            if (success)
                (App.Current as App).GoTo(PageNames.change_password_successPage);
            else
                (App.Current as App).GoTo(PageNames.change_password_failed_4Page);
        }
        #endregion

        #region Change Email
        public void On_ChangeEmail(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (string.IsNullOrEmpty(Password))
                destinationPageName = PageNames.change_email_failed_1Page;

            else if (string.IsNullOrEmpty(NewEmail))
                destinationPageName = PageNames.change_email_failed_2Page;

            else if (!NewEmail.Contains("@"))
                destinationPageName = PageNames.change_email_failed_3Page;

            else
            {
                StartUpdateEmail(Name, Password, NewEmail);
                destinationPageName = PageNames.CurrentPage;
            }

            Password = null;
            NewEmail = null;
        }

        private void StartUpdateEmail(string name, string testPassword, string newEmail)
        {
            EncryptPassword(testPassword, name, (bool encryptSuccess, object encryptResult) => ChangeEmail_OnTestPasswordEncrypted(encryptSuccess, encryptResult, name, newEmail));
        }

        private void ChangeEmail_OnTestPasswordEncrypted(bool success, object result, string name, string newEmail)
        {
            if (success)
            {
                string EncryptedTestPassword = (string)result;
                CheckPassword(name, (bool checkSuccess, object checkResult) => ChangeEmail_OnCurrentPasswordReceived(checkSuccess, checkResult, EncryptedTestPassword, name, newEmail));
            }
            else
                (App.Current as App).GoTo(PageNames.change_email_failed_4Page);
        }

        private void ChangeEmail_OnCurrentPasswordReceived(bool success, object result, string encryptedTestPassword, string name, string newEmail)
        {
            if (success)
            {
                Dictionary<string, string> CheckPasswordResult = (Dictionary<string, string>)result;
                string EncryptedCurrentPassword = CheckPasswordResult["password"];

                if (encryptedTestPassword == EncryptedCurrentPassword)
                    ChangeEmail(name, newEmail, (bool changeSuccess, object changeResult) => ChangeEmail_OnEmailChanged(changeSuccess, changeResult, newEmail));
                else
                    (App.Current as App).GoTo(PageNames.change_email_failed_5Page);
            }
            else
                (App.Current as App).GoTo(PageNames.change_email_failed_4Page);
        }

        private void ChangeEmail_OnEmailChanged(bool success, object result, string newEmail)
        {
            if (success)
            {
                Email = newEmail;
                (App.Current as App).GoTo(PageNames.change_email_successPage);
            }
            else
                (App.Current as App).GoTo(PageNames.change_email_failed_4Page);
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

        private void ChangeEmail(string name, string newEmail, Action<bool, object> callback)
        {
            Database.Completed += OnChangeEmailCompleted;
            Database.Update(new DatabaseUpdateOperation("change email", "update_2.php", new Dictionary<string, string>() { { "name", HtmlString.Entities(name) }, { "email", HtmlString.Entities(newEmail) } }, callback));
        }

        private void OnChangeEmailCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnChangeEmailCompleted notified");
            Database.Completed -= OnChangeEmailCompleted;

            Action<bool, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
            {
                string ChangeEmailResult = Result["result"];

                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ChangeEmailResult == "1", null));
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
            OperationHandler.Add(new OperationHandler("/request/query_1.php", OnSignInMatchRequest));
            OperationHandler.Add(new OperationHandler("/request/update_1.php", OnChangePasswordRequest));
            OperationHandler.Add(new OperationHandler("/request/update_2.php", OnChangeEmailRequest));
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

        private List<Dictionary<string, string>> OnSignInMatchRequest(Dictionary<string, string> parameters)
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

        private List<Dictionary<string, string>> OnChangePasswordRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string Username;
            if (parameters.ContainsKey("name"))
                Username = parameters["name"];
            else
                Username = null;

            string Password;
            if (parameters.ContainsKey("password"))
                Password = parameters["password"];
            else
                Password = null;

            if (Username == null || Password == null)
                return Result;

            foreach (Dictionary<string, string> Line in KnownUserTable)
                if (Line.ContainsKey("id") && Line["id"] == Username && Line.ContainsKey("password"))
                {
                    Line["password"] = Password;

                    Result.Add(new Dictionary<string, string>()
                    {
                        { "result", "1"},
                    });

                    break;
                }

            return Result;
        }

        private List<Dictionary<string, string>> OnChangeEmailRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string Username;
            if (parameters.ContainsKey("name"))
                Username = parameters["name"];
            else
                Username = null;

            string Email;
            if (parameters.ContainsKey("email"))
                Email = parameters["email"];
            else
                Email = null;

            if (Username == null || Email == null)
                return Result;

            foreach (Dictionary<string, string> Line in KnownUserTable)
                if (Line.ContainsKey("id") && Line["id"] == Username && Line.ContainsKey("email"))
                {
                    Line["email"] = Email;

                    Result.Add(new Dictionary<string, string>()
                    {
                        { "result", "1"},
                    });

                    break;
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
