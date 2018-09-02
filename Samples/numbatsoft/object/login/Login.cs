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

            Database.DebugWriteResponse = true;

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
        public string NewQuestion { get; set; }
        public string RecoveryAnswer { get; set; }
        public string ConfirmAnswer { get; set; }
        public bool Remember { get; set; }

        #region Register
        public void On_Register(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Email) || !Email.Contains("@"))
                destinationPageName = PageNames.registration_failed_01Page;

            else if (!string.IsNullOrEmpty(RecoveryQuestion) && string.IsNullOrEmpty(RecoveryAnswer))
                destinationPageName = PageNames.registration_failed_02Page;

            else if (string.IsNullOrEmpty(RecoveryQuestion) && !string.IsNullOrEmpty(RecoveryAnswer))
                destinationPageName = PageNames.registration_failed_03Page;

            else
            {
                if (string.IsNullOrEmpty(RecoveryQuestion) || string.IsNullOrEmpty(RecoveryAnswer))
                    StartRegister(Name, Password, Email, "", "");
                else
                    StartRegister(Name, Password, Email, RecoveryQuestion, RecoveryAnswer);

                destinationPageName = PageNames.CurrentPage;
            }

            Password = null;
            RecoveryAnswer = null;
        }

        private void StartRegister(string name, string password, string email, string question, string answer)
        {
            GetUserInfo(name, (bool checkSuccess, object checkResult) => Register_OnNameChecked(checkSuccess, checkResult, name, password, email, question, answer));
        }

        private void Register_OnNameChecked(bool success, object result, string name, string password, string email, string question, string answer)
        {
            if (!success)
            {
                CheckIfEmailTaken(email, (bool checkSuccess, object checkResult) => Register_OnEmailChecked(checkSuccess, checkResult, name, password, email, question, answer));
            }
            else
                (App.Current as App).GoTo(PageNames.registration_failed_07Page);
        }

        private void Register_OnEmailChecked(bool success, object result, string name, string password, string email, string question, string answer)
        {
            if (!success)
            {
                EncryptPassword(password, name, (bool encryptSuccess, object encryptResult) => Register_OnTestPasswordEncrypted(encryptSuccess, encryptResult, name, email, question, answer));
            }
            else
                (App.Current as App).GoTo(PageNames.registration_failed_08Page);
        }

        private void Register_OnTestPasswordEncrypted(bool success, object result, string name, string email, string question, string answer)
        {
            if (success)
            {
                string EncryptedPassword = (string)result;
                EncryptPassword(answer, name, (bool encryptSuccess, object encryptResult) => Register_OnRecoveryAnswerEncrypted(encryptSuccess, encryptResult, name, EncryptedPassword, email, question));
            }
            else
                (App.Current as App).GoTo(PageNames.registration_failed_04Page);
        }

        private void Register_OnRecoveryAnswerEncrypted(bool success, object result, string name, string encryptedPassword, string email, string question)
        {
            if (success)
            {
                string EncryptedAnswer = (string)result;
                RegisterAndSendEmail(name, encryptedPassword, email, question, EncryptedAnswer, (bool checkSuccess, object checkResult) => Register_OnEmailSent(checkSuccess, checkResult));
            }
            else
                (App.Current as App).GoTo(PageNames.registration_failed_04Page);
        }

        private void Register_OnEmailSent(bool success, object result)
        {
            if (success)
            {
                (App.Current as App).GoTo(PageNames.registration_startedPage);
            }
            else
                (App.Current as App).GoTo(PageNames.registration_failed_04Page);
        }
        #endregion

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
                GetUserInfo(name, (bool checkSuccess, object checkResult) => Login_OnCurrentPasswordReceived(checkSuccess, checkResult, EncryptedTestPassword, remember));
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
                    RecoveryQuestion = DecodedRecoveryQuestion(CheckPasswordResult["question"]);

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
                GetUserInfo(name, (bool checkSuccess, object checkResult) => ChangePassword_OnCurrentPasswordReceived(checkSuccess, checkResult, EncryptedTestPassword, name, newPassword));
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
                GetUserInfo(name, (bool checkSuccess, object checkResult) => ChangeEmail_OnCurrentPasswordReceived(checkSuccess, checkResult, EncryptedTestPassword, name, newEmail));
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

        #region Change Recovery
        public void On_ChangeRecovery(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (string.IsNullOrEmpty(Password))
                destinationPageName = PageNames.change_recovery_failed_1Page;

            else if (string.IsNullOrEmpty(NewQuestion) && string.IsNullOrEmpty(RecoveryAnswer) && string.IsNullOrEmpty(ConfirmAnswer))
            {
                StartUpdateRecovery(Name, Password, "", "");
                destinationPageName = PageNames.CurrentPage;
            }

            else if (string.IsNullOrEmpty(NewQuestion) || string.IsNullOrEmpty(RecoveryAnswer) || string.IsNullOrEmpty(ConfirmAnswer))
                destinationPageName = PageNames.change_recovery_failed_2Page;

            else if (RecoveryAnswer != ConfirmAnswer)
                destinationPageName = PageNames.change_recovery_failed_3Page;

            else
            {
                StartUpdateRecovery(Name, Password, NewQuestion, RecoveryAnswer);
                destinationPageName = PageNames.CurrentPage;
            }

            Password = null;
            NewQuestion = null;
            RecoveryAnswer = null;
            ConfirmAnswer = null;
        }

        private void StartUpdateRecovery(string name, string testPassword, string newQuestion, string newAnswer)
        {
            EncryptPassword(testPassword, name, (bool encryptSuccess, object encryptResult) => ChangeRecovery_OnTestPasswordEncrypted(encryptSuccess, encryptResult, name, newQuestion, newAnswer));
        }

        private void ChangeRecovery_OnTestPasswordEncrypted(bool success, object result, string name, string newQuestion, string newAnswer)
        {
            if (success)
            {
                string EncryptedTestPassword = (string)result;
                GetUserInfo(name, (bool checkSuccess, object checkResult) => ChangeRecovery_OnCurrentPasswordReceived(checkSuccess, checkResult, EncryptedTestPassword, name, newQuestion, newAnswer));
            }
            else
                (App.Current as App).GoTo(PageNames.change_recovery_failed_4Page);
        }

        private void ChangeRecovery_OnCurrentPasswordReceived(bool success, object result, string encryptedTestPassword, string name, string newQuestion, string newAnswer)
        {
            if (success)
            {
                Dictionary<string, string> CheckPasswordResult = (Dictionary<string, string>)result;
                string EncryptedCurrentPassword = CheckPasswordResult["password"];

                if (encryptedTestPassword == EncryptedCurrentPassword)
                    ChangeRecovery(name, newQuestion, newAnswer, (bool changeSuccess, object changeResult) => ChangeRecovery_OnRecoveryChanged(changeSuccess, changeResult, newQuestion));
                else
                    (App.Current as App).GoTo(PageNames.change_recovery_failed_5Page);
            }
            else
                (App.Current as App).GoTo(PageNames.change_recovery_failed_4Page);
        }

        private void ChangeRecovery_OnRecoveryChanged(bool success, object result, string newQuestion)
        {
            if (success)
            {
                RecoveryQuestion = newQuestion;
                (App.Current as App).GoTo(PageNames.change_recovery_successPage);
            }
            else
                (App.Current as App).GoTo(PageNames.change_recovery_failed_4Page);
        }
        #endregion

        #region Operations
        private void GetUserInfo(string name, Action<bool, object> callback)
        {
            Database.Completed += OnGetUserInfoCompleted;
            Database.Query(new DatabaseQueryOperation("get user info", "query_1.php", new Dictionary<string, string>() { { "name", HtmlString.Entities(name) } }, callback));
        }

        private void OnGetUserInfoCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnGetUserInfoCompleted notified");
            Database.Completed -= OnGetUserInfoCompleted;

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

        private void ChangeRecovery(string name, string newQuestion, string newAnswer, Action<bool, object> callback)
        {
            Database.Completed += OnChangeRecoveryCompleted;
            Database.Update(new DatabaseUpdateOperation("change recovery", "update_3.php", new Dictionary<string, string>() { { "name", HtmlString.Entities(name) }, { "question", HtmlString.Entities(newQuestion) }, { "answer", HtmlString.Entities(newAnswer) } }, callback));
        }

        private void OnChangeRecoveryCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnChangeRecoveryCompleted notified");
            Database.Completed -= OnChangeRecoveryCompleted;

            Action<bool, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
            {
                string ChangeRecoveryResult = Result["result"];

                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ChangeRecoveryResult == "1", null));
            }
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(false, null));
        }

        private void CheckIfEmailTaken(string email, Action<bool, object> callback)
        {
            Database.Completed += OnIsEmailAvailableChecked;
            Database.Query(new DatabaseQueryOperation("check email", "query_3.php", new Dictionary<string, string>() { { "email", HtmlString.Entities(email) } }, callback));
        }

        private void OnIsEmailAvailableChecked(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnIsEmailAvailableChecked notified");
            Database.Completed -= OnIsEmailAvailableChecked;

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

        private void RegisterAndSendEmail(string name, string encryptedPassword, string email, string question, string answer, Action<bool, object> callback)
        {
            Database.Completed += OnRegisterSendEmailCompleted;
            Database.Update(new DatabaseUpdateOperation("start register", "insert_1.php", new Dictionary<string, string>() { { "name", HtmlString.Entities(name) }, { "password", HtmlString.Entities(encryptedPassword) }, { "email", HtmlString.Entities(email) }, { "question", HtmlString.Entities(question) }, { "answer", HtmlString.Entities(answer) }, { "language", ((int)GetLanguage.LanguageState).ToString() } }, callback));
        }

        private void OnRegisterSendEmailCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnRegisterSendEmailCompleted notified");
            Database.Completed -= OnRegisterSendEmailCompleted;

            Action<bool, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
            {
                string EmailSentResult = Result["result"];
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(EmailSentResult == "1", null));
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
            OperationHandler.Add(new OperationHandler("/request/update_3.php", OnChangeRecoveryRequest));
            OperationHandler.Add(new OperationHandler("/request/query_3.php", OnCheckEmailMatchRequest));
            OperationHandler.Add(new OperationHandler("/request/insert_1.php", OnSignUpRequest));
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

        private List<Dictionary<string, string>> OnChangeRecoveryRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string Username;
            if (parameters.ContainsKey("name"))
                Username = parameters["name"];
            else
                Username = null;

            string NewQuestion;
            if (parameters.ContainsKey("question"))
                NewQuestion = parameters["question"];
            else
                NewQuestion = null;

            string NewAnswer;
            if (parameters.ContainsKey("answer"))
                NewAnswer = parameters["answer"];
            else
                NewAnswer = null;

            if (Username == null || NewQuestion == null || NewAnswer == null)
                return Result;

            foreach (Dictionary<string, string> Line in KnownUserTable)
                if (Line.ContainsKey("id") && Line["id"] == Username && Line.ContainsKey("question") && Line.ContainsKey("answer"))
                {
                    Line["question"] = NewQuestion;
                    Line["answer"] = NewAnswer;

                    Result.Add(new Dictionary<string, string>()
                    {
                        { "result", "1"},
                    });

                    break;
                }

            return Result;
        }

        private List<Dictionary<string, string>> OnCheckEmailMatchRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string Email;
            if (parameters.ContainsKey("email"))
                Email = parameters["email"];
            else
                Email = null;

            if (Email == null)
                return Result;

            foreach (Dictionary<string, string> Line in KnownUserTable)
            {
                if (Line.ContainsKey("email") && Line["email"] == Email)
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

        private List<Dictionary<string, string>> OnSignUpRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string Username;
            if (parameters.ContainsKey("name"))
                Username = parameters["name"];
            else
                Username = null;

            string EncryptedPassword;
            if (parameters.ContainsKey("password"))
                EncryptedPassword = parameters["password"];
            else
                EncryptedPassword = null;

            string Email;
            if (parameters.ContainsKey("email"))
                Email = parameters["email"];
            else
                Email = null;

            string Question;
            if (parameters.ContainsKey("question"))
                Question = parameters["question"];
            else
                Question = null;

            string Answer;
            if (parameters.ContainsKey("answer"))
                Answer = parameters["answer"];
            else
                Answer = null;

            string Language;
            if (parameters.ContainsKey("language"))
                Language = parameters["language"];
            else
                Language = "0";

            if (Username == null || EncryptedPassword == null || Email == null || Question == null || Answer == null)
                return Result;

            foreach (Dictionary<string, string> Line in KnownUserTable)
                if ((Line.ContainsKey("id") && Line["id"] == Username) || (Line.ContainsKey("email") && Line["email"] == Email))
                    return Result;

            Dictionary<string, string> NewLine = new Dictionary<string, string>();
            NewLine.Add("id", Username);
            NewLine.Add("password", EncryptedPassword);
            NewLine.Add("email", Email);
            NewLine.Add("question", EncodedRecoveryQuestion(Question));
            NewLine.Add("answer", Answer);
            KnownUserTable.Add(NewLine);

            Result.Add(new Dictionary<string, string>()
            {
                { "result", "1"},
            });

            return Result;
        }

        private static string EncodedRecoveryQuestion(string text)
        {
            string Result = "";
            foreach (char c in text)
                Result += (char)(((int)c ^ 204 ) + 40);

            return Result;
        }

        private static string DecodedRecoveryQuestion(string text)
        {
            string Result = "";
            foreach (char c in text)
                Result += (char)(((int)c - 40) ^ 204);

            return Result;
        }

        private List<Dictionary<string, string>> KnownUserTable = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string>()
            {
                { "id", "test" },
                { "password", Convert.ToBase64String(Encoding.UTF8.GetBytes("toto")) },
                { "email", "test@test.com" },
                { "question", EncodedRecoveryQuestion("foo") },
                { "answer", Convert.ToBase64String(Encoding.UTF8.GetBytes("not foo")) },
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
