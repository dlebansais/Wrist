using NetTools;
using Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.UI.Xaml;

namespace AppCSHtml5
{
    public class Login : ObjectBase, ILogin
    {
        private enum ErrorCodes
        {
            AnyError = -1,
            Success = 0,
            ErrorNotFound = 1,
            OperationFailed = 2,
            InvalidUsernameOrPassword = 3,
            InvalidUsernamePasswordOrAnswer = 4,
            ErrorNoQuestion = 5,
            InvalidUsernameOrAnswer = 6,
        }

        public Login()
        {
            Name = Persistent.GetValue("name", null);
            Email = Persistent.GetValue("email", null);
            RecoveryQuestion = Persistent.GetValue("question", null);
            Remember = (Persistent.GetValue("remember", null) != null);
            LoginState = (Name != null ? LoginStates.SignedIn : LoginStates.LoggedOff);

            Database.DebugLog = true;
            Database.DebugLogFullResponse = true;

            InitSimulation();
        }

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
        public bool HasQuestion { get { return !string.IsNullOrEmpty(RecoveryQuestion); } }

        #region Register
        public void On_Register(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Email) || !Email.Contains("@"))
                destinationPageName = PageNames.register_failed_1Page;

            else if (!string.IsNullOrEmpty(RecoveryQuestion) && string.IsNullOrEmpty(RecoveryAnswer))
                destinationPageName = PageNames.register_failed_2Page;

            else if (string.IsNullOrEmpty(RecoveryQuestion) && !string.IsNullOrEmpty(RecoveryAnswer))
                destinationPageName = PageNames.register_failed_3Page;

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
            GetUserInfo(name, (int checkError, object checkResult) => Register_OnNameChecked(checkError, checkResult, name, password, email, question, answer));
        }

        private void Register_OnNameChecked(int error, object result, string name, string password, string email, string question, string answer)
        {
            if (error == (int)ErrorCodes.ErrorNotFound)
            {
                CheckIfEmailTaken(email, (int checkError, object checkResult) => Register_OnEmailChecked(checkError, checkResult, name, password, email, question, answer));
            }
            else if (error == (int)ErrorCodes.Success)
                (App.Current as App).GoTo(PageNames.register_failed_5Page);
            else
                (App.Current as App).GoTo(PageNames.register_failed_4Page);
        }

        private void Register_OnEmailChecked(int error, object result, string name, string password, string email, string question, string answer)
        {
            if (error == (int)ErrorCodes.ErrorNotFound)
            {
                EncryptString(password, name, (int encryptError, object encryptResult) => Register_OnTestPasswordEncrypted(encryptError, encryptResult, name, email, question, answer));
            }
            else if (error == (int)ErrorCodes.Success)
                (App.Current as App).GoTo(PageNames.register_failed_6Page);
            else
                (App.Current as App).GoTo(PageNames.register_failed_4Page);
        }

        private void Register_OnTestPasswordEncrypted(int error, object result, string name, string email, string question, string answer)
        {
            if (error == (int)ErrorCodes.Success)
            {
                string EncryptedPassword = (string)result;
                EncryptString(answer, name, (int encryptError, object encryptResult) => Register_OnRecoveryAnswerEncrypted(encryptError, encryptResult, name, EncryptedPassword, email, question));
            }
            else
                (App.Current as App).GoTo(PageNames.register_failed_4Page);
        }

        private void Register_OnRecoveryAnswerEncrypted(int error, object result, string name, string encryptedPassword, string email, string question)
        {
            if (error == (int)ErrorCodes.Success)
            {
                string EncryptedAnswer = (string)result;
                RegisterAndSendEmail(name, encryptedPassword, email, question, EncryptedAnswer, (int checkError, object checkResult) => Register_OnEmailSent(checkError, checkResult));
            }
            else
                (App.Current as App).GoTo(PageNames.register_failed_4Page);
        }

        private void Register_OnEmailSent(int error, object result)
        {
            if (error == (int)ErrorCodes.Success)
            {
                (App.Current as App).GoTo(PageNames.registration_startedPage);
            }
            else
                (App.Current as App).GoTo(PageNames.register_failed_4Page);
        }

        public void On_RegisterEnd(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            Dictionary<string, string> QueryString = App.QueryString;
            if (QueryString != null && QueryString.Count > 0)
            {
                if (QueryString.ContainsKey("type") && QueryString["type"] == "register")
                {
                    string QueryName = QueryString.ContainsKey("username") ? QueryString["username"] : null;
                    string QueryEmail = QueryString.ContainsKey("email") ? QueryString["email"] : null;
                    string QueryQuestion = QueryString.ContainsKey("question") ? QueryString["question"] : null;

                    if (!string.IsNullOrEmpty(QueryName) && !string.IsNullOrEmpty(QueryEmail))
                    {
                        Name = QueryName;
                        Email = QueryEmail;
                        RecoveryQuestion = DecodedRecoveryQuestion(QueryQuestion);
                        destinationPageName = PageNames.registration_endPage;
                    }
                    else
                        destinationPageName = PageNames.invalid_operationPage;
                }
                else
                    destinationPageName = PageNames.invalid_operationPage;
            }
            else
                destinationPageName = pageName;
        }

        public void On_CompleteRegistration(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (!Remember)
            {
                Persistent.SetValue("name", null);
                Persistent.SetValue("email", null);
                Persistent.SetValue("question", null);
                Persistent.SetValue("remember", null);
            }

            if (string.IsNullOrEmpty(Password))
                destinationPageName = PageNames.registration_end_failed_1Page;

            else if (HasQuestion && string.IsNullOrEmpty(RecoveryAnswer))
                destinationPageName = PageNames.registration_end_failed_2Page;

            else
            {
                StartCompleteRegistration(Name, Password, Email, Remember, HasQuestion, RecoveryAnswer);
                destinationPageName = PageNames.CurrentPage;
            }

            Password = null;
        }

        private void StartCompleteRegistration(string name, string testPassword, string email, bool remember, bool hasQuestion, string answer)
        {
            EncryptString(testPassword, name, (int encryptError, object encryptResult) => CompleteRegistration_OnTestPasswordEncrypted(encryptError, encryptResult, name, email, remember, hasQuestion, answer));
        }

        private void CompleteRegistration_OnTestPasswordEncrypted(int error, object result, string name, string email, bool remember, bool hasQuestion, string answer)
        {
            if (error == (int)ErrorCodes.Success)
            {
                string EncryptedTestPassword = (string)result;

                if (hasQuestion)
                    EncryptString(answer, name, (int encryptError, object encryptResult) => CompleteRegistration_OnAnswerEncrypted(encryptError, encryptResult, name, email, EncryptedTestPassword, remember));
                else
                    ActivateAccountAndSendEmail(name, email, EncryptedTestPassword, null, (int checkError, object checkResult) => CompleteRegistration_OnGetUserInfo(checkError, checkResult, remember));
            }
            else
                (App.Current as App).GoTo(PageNames.registration_end_failed_3Page);
        }

        private void CompleteRegistration_OnAnswerEncrypted(int error, object result, string name, string email, string encryptedTestPassword, bool remember)
        {
            if (error == (int)ErrorCodes.Success)
            {
                string EncryptedAnswer = (string)result;
                ActivateAccountAndSendEmail(name, email, encryptedTestPassword, EncryptedAnswer, (int checkError, object checkResult) => CompleteRegistration_OnGetUserInfo(checkError, checkResult, remember));
            }
            else
                (App.Current as App).GoTo(PageNames.registration_end_failed_3Page);
        }

        private void CompleteRegistration_OnGetUserInfo(int error, object result, bool remember)
        {
            if (error == (int)ErrorCodes.Success)
            {
                Dictionary<string, string> CheckPasswordResult = (Dictionary<string, string>)result;
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

                (App.Current as App).GoTo(PageNames.homePage);
            }
            else if (error == (int)ErrorCodes.InvalidUsernameOrPassword)
                (App.Current as App).GoTo(PageNames.registration_end_failed_4Page);
            else if (error == (int)ErrorCodes.InvalidUsernamePasswordOrAnswer)
                (App.Current as App).GoTo(PageNames.registration_end_failed_5Page);
            else
                (App.Current as App).GoTo(PageNames.registration_end_failed_3Page);
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
            EncryptString(testPassword, name, (int encryptError, object encryptResult) => Login_OnTestPasswordEncrypted(encryptError, encryptResult, name, remember));
        }

        private void Login_OnTestPasswordEncrypted(int error, object result, string name, bool remember)
        {
            if (error == (int)ErrorCodes.Success)
            {
                string EncryptedTestPassword = (string)result;
                GetUserInfo(name, (int checkError, object checkResult) => Login_OnCurrentPasswordReceived(checkError, checkResult, EncryptedTestPassword, remember));
            }
            else
                (App.Current as App).GoTo(PageNames.login_failedPage);
        }

        private void Login_OnCurrentPasswordReceived(int error, object result, string encryptedTestPassword, bool remember)
        {
            if (error == (int)ErrorCodes.Success)
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

                    string OrganizationName = CheckPasswordResult["name"];
                    ((Eqmlp)GetEqmlp).Login(OrganizationName);

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
            ((Eqmlp)GetEqmlp).Logout();

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
            EncryptString(testPassword, name, (int encryptError, object encryptResult) => ChangePassword_OnTestPasswordEncrypted(encryptError, encryptResult, name, newPassword));
        }

        private void ChangePassword_OnTestPasswordEncrypted(int error, object result, string name, string newPassword)
        {
            if (error == (int)ErrorCodes.Success)
            {
                string EncryptedTestPassword = (string)result;
                GetUserInfo(name, (int checkError, object checkResult) => ChangePassword_OnCurrentPasswordReceived(checkError, checkResult, EncryptedTestPassword, name, newPassword));
            }
            else
                (App.Current as App).GoTo(PageNames.change_password_failed_4Page);
        }

        private void ChangePassword_OnCurrentPasswordReceived(int error, object result, string encryptedTestPassword, string name, string newPassword)
        {
            if (error == (int)ErrorCodes.Success)
            {
                Dictionary<string, string> CheckPasswordResult = (Dictionary<string, string>)result;
                string EncryptedCurrentPassword = CheckPasswordResult["password"];
                
                if (encryptedTestPassword == EncryptedCurrentPassword)
                    EncryptString(newPassword, name, (int encryptError, object encryptResult) => ChangePassword_OnNewPasswordEncrypted(encryptError, encryptResult, name));
                else
                    (App.Current as App).GoTo(PageNames.change_password_failed_5Page);
            }
            else
                (App.Current as App).GoTo(PageNames.change_password_failed_4Page);
        }

        private void ChangePassword_OnNewPasswordEncrypted(int error, object result, string name)
        {
            if (error == (int)ErrorCodes.Success)
            {
                string EncryptedNewPassword = (string)result;
                ChangePassword(name, EncryptedNewPassword, ChangePassword_OnPasswordChanged);
            }
            else
                (App.Current as App).GoTo(PageNames.change_password_failed_4Page);
        }

        private void ChangePassword_OnPasswordChanged(int error, object result)
        {
            if (error == (int)ErrorCodes.Success)
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
            EncryptString(testPassword, name, (int encryptError, object encryptResult) => ChangeEmail_OnTestPasswordEncrypted(encryptError, encryptResult, name, newEmail));
        }

        private void ChangeEmail_OnTestPasswordEncrypted(int error, object result, string name, string newEmail)
        {
            if (error == (int)ErrorCodes.Success)
            {
                string EncryptedTestPassword = (string)result;
                GetUserInfo(name, (int checkError, object checkResult) => ChangeEmail_OnCurrentPasswordReceived(checkError, checkResult, EncryptedTestPassword, name, newEmail));
            }
            else
                (App.Current as App).GoTo(PageNames.change_email_failed_4Page);
        }

        private void ChangeEmail_OnCurrentPasswordReceived(int error, object result, string encryptedTestPassword, string name, string newEmail)
        {
            if (error == (int)ErrorCodes.Success)
            {
                Dictionary<string, string> CheckPasswordResult = (Dictionary<string, string>)result;
                string EncryptedCurrentPassword = CheckPasswordResult["password"];

                if (encryptedTestPassword == EncryptedCurrentPassword)
                    ChangeEmail(name, newEmail, (int changeError, object changeResult) => ChangeEmail_OnEmailChanged(changeError, changeResult, newEmail));
                else
                    (App.Current as App).GoTo(PageNames.change_email_failed_5Page);
            }
            else
                (App.Current as App).GoTo(PageNames.change_email_failed_4Page);
        }

        private void ChangeEmail_OnEmailChanged(int error, object result, string newEmail)
        {
            if (error == (int)ErrorCodes.Success)
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
            EncryptString(testPassword, name, (int encryptError, object encryptResult) => ChangeRecovery_OnTestPasswordEncrypted(encryptError, encryptResult, name, newQuestion, newAnswer));
        }

        private void ChangeRecovery_OnTestPasswordEncrypted(int error, object result, string name, string newQuestion, string newAnswer)
        {
            if (error == (int)ErrorCodes.Success)
            {
                string EncryptedTestPassword = (string)result;
                GetUserInfo(name, (int checkError, object checkResult) => ChangeRecovery_OnCurrentPasswordReceived(checkError, checkResult, EncryptedTestPassword, name, newQuestion, newAnswer));
            }
            else
                (App.Current as App).GoTo(PageNames.change_recovery_failed_4Page);
        }

        private void ChangeRecovery_OnCurrentPasswordReceived(int error, object result, string encryptedTestPassword, string name, string newQuestion, string newAnswer)
        {
            if (error == (int)ErrorCodes.Success)
            {
                Dictionary<string, string> CheckPasswordResult = (Dictionary<string, string>)result;
                string EncryptedCurrentPassword = CheckPasswordResult["password"];

                if (encryptedTestPassword == EncryptedCurrentPassword)
                    ChangeRecovery(name, newQuestion, newAnswer, (int changeError, object changeResult) => ChangeRecovery_OnRecoveryChanged(changeError, changeResult, newQuestion));
                else
                    (App.Current as App).GoTo(PageNames.change_recovery_failed_5Page);
            }
            else
                (App.Current as App).GoTo(PageNames.change_recovery_failed_4Page);
        }

        private void ChangeRecovery_OnRecoveryChanged(int error, object result, string newQuestion)
        {
            if (error == (int)ErrorCodes.Success)
            {
                RecoveryQuestion = newQuestion;
                (App.Current as App).GoTo(PageNames.change_recovery_successPage);
            }
            else
                (App.Current as App).GoTo(PageNames.change_recovery_failed_4Page);
        }
        #endregion

        #region Recovery
        public void On_BeginRecovery(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (string.IsNullOrEmpty(Email))
                destinationPageName = PageNames.recovery_failed_1Page;

            else
            {
                StartRecovery(Email);
                destinationPageName = PageNames.CurrentPage;
            }
        }

        private void StartRecovery(string email)
        {
            CheckIfEmailTaken(email, (int checkError, object checkResult) => Recovery_OnEmailChecked(checkError, checkResult, email));
        }

        private void Recovery_OnEmailChecked(int error, object result, string email)
        {
            if (error == (int)ErrorCodes.Success)
            {
                if ((result is Dictionary<string, string> QueryResult) && QueryResult.ContainsKey("question"))
                    if (!string.IsNullOrEmpty(QueryResult["question"]))
                        BeginRecoveryAndSendEmail(email, (int checkError, object checkResult) => Recovery_OnEmailSent(checkError, checkResult));
                    else
                        (App.Current as App).GoTo(PageNames.recovery_failed_2Page);
                else
                    (App.Current as App).GoTo(PageNames.recovery_failed_3Page);
            }
            else
                (App.Current as App).GoTo(PageNames.recovery_failed_2Page);
        }

        private void Recovery_OnEmailSent(int error, object result)
        {
            if (error == (int)ErrorCodes.Success)
                (App.Current as App).GoTo(PageNames.recovery_startedPage);
            else
                (App.Current as App).GoTo(PageNames.recovery_failed_3Page);
        }

        public void On_RecoveryEnd(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            Dictionary<string, string> QueryString = App.QueryString;
            if (QueryString != null && QueryString.Count > 0)
            {
                if (QueryString.ContainsKey("type") && QueryString["type"] == "recovery")
                {
                    string QueryName = QueryString.ContainsKey("username") ? QueryString["username"] : null;
                    string QueryEmail = QueryString.ContainsKey("email") ? QueryString["email"] : null;
                    string QueryQuestion = QueryString.ContainsKey("question") ? QueryString["question"] : null;

                    if (!string.IsNullOrEmpty(QueryName) && !string.IsNullOrEmpty(QueryEmail) && !string.IsNullOrEmpty(QueryQuestion))
                    {
                        Name = QueryName;
                        Email = QueryEmail;
                        RecoveryQuestion = DecodedRecoveryQuestion(QueryQuestion);
                        destinationPageName = PageNames.recovery_endPage;
                    }
                    else
                        destinationPageName = PageNames.invalid_operationPage;
                }
                else
                    destinationPageName = PageNames.invalid_operationPage;
            }
            else
                destinationPageName = pageName;
        }

        public void On_CompleteRecovery(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (string.IsNullOrEmpty(RecoveryAnswer))
                destinationPageName = PageNames.recovery_end_failed_1Page;

            else if (string.IsNullOrEmpty(NewPassword))
                destinationPageName = PageNames.recovery_end_failed_2Page;

            else
            {
                StartCompleteRecovery(Name, RecoveryAnswer, NewPassword);
                destinationPageName = PageNames.CurrentPage;
            }

            NewPassword = null;
            RecoveryAnswer = null;
        }

        private void StartCompleteRecovery(string name, string answer, string newPassword)
        {
            EncryptString(answer, name, (int encryptError, object encryptResult) => CompleteRecovery_OnAnswerEncrypted(encryptError, encryptResult, name, newPassword));
        }

        private void CompleteRecovery_OnAnswerEncrypted(int error, object result, string name, string newPassword)
        {
            if (error == (int)ErrorCodes.Success)
            {
                string EncryptedAnswer = (string)result;
                EncryptString(newPassword, name, (int encryptError, object encryptResult) => CompleteRecovery_OnNewPasswordEncrypted(encryptError, encryptResult, name, EncryptedAnswer));
            }
            else
                (App.Current as App).GoTo(PageNames.recovery_end_failed_3Page);
        }

        private void CompleteRecovery_OnNewPasswordEncrypted(int error, object result, string name, string encryptedAnswer)
        {
            if (error == (int)ErrorCodes.Success)
            {
                string EncryptedNewPassword = (string)result;
                RecoverAccount(name, encryptedAnswer, EncryptedNewPassword, (int checkError, object checkResult) => CompleteRecovery_OnGetUserInfo(checkError, checkResult));
            }
            else
                (App.Current as App).GoTo(PageNames.recovery_end_failed_3Page);
        }

        private void CompleteRecovery_OnGetUserInfo(int error, object result)
        {
            if (error == (int)ErrorCodes.Success)
            {
                Dictionary<string, string> CheckPasswordResult = (Dictionary<string, string>)result;

                LoginState = LoginStates.SignedIn;
                NotifyPropertyChanged(nameof(LoginState));

                (App.Current as App).GoTo(PageNames.accountPage);
            }
            else if (error == (int)ErrorCodes.InvalidUsernameOrAnswer)
                (App.Current as App).GoTo(PageNames.recovery_end_failed_4Page);
            else
                (App.Current as App).GoTo(PageNames.recovery_end_failed_3Page);
        }
        #endregion

        #region Operations
        private void GetUserInfo(string name, Action<int, object> callback)
        {
            Database.Completed += OnGetUserInfoCompleted;
            Database.Query(new DatabaseQueryOperation("get user info", "query_1.php", new Dictionary<string, string>() { { "name", HtmlString.Entities(name) } }, callback));
        }

        private void OnGetUserInfoCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnGetUserInfoCompleted notified");
            Database.Completed -= OnGetUserInfoCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "id", "password", "email", "question", "name", "login_url", "meeting_url", "validation_url" })) != null)
            {
                string Id = Result["id"];
                string EncryptedPassword = Result["password"];
                string Email = Result["email"];
                string RecoveryQuestion = Result["question"];
                string Name = Result["name"];
                string LoginUrl = Result["login_url"];
                string MeetingUrl = Result["meeting_url"];
                string ValidationUrl = Result["validation_url"];
                Debug.WriteLine($"Account {Id}: password={EncryptedPassword}, email={Email}, question={RecoveryQuestion}");

                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.Success, Result));
            }
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.ErrorNotFound, null));
        }

        private void EncryptString(string plainText, string key, Action<int, object> callback)
        {
            Database.Completed += OnEncryptPasswordCompleted;
            Database.Encrypt(new DatabaseEncryptOperation("encrypt password", "encrypt.php", "site/key", $"numbatsoft/{key}", "pt", plainText, callback));
        }

        private void OnEncryptPasswordCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnEncryptPasswordCompleted notified");
            Database.Completed -= OnEncryptPasswordCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "encrypted" })) != null)
            {
                string EncryptedPassword = Result["encrypted"];
                Debug.WriteLine($"Encrypted Password: {EncryptedPassword}");

                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.Success, EncryptedPassword));
            }
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private void ChangePassword(string name, string encryptedNewPassword, Action<int, object> callback)
        {
            Database.Completed += OnChangePasswordCompleted;
            Database.Update(new DatabaseUpdateOperation("change password", "update_1.php", new Dictionary<string, string>() { { "name", HtmlString.Entities(name) }, { "password", HtmlString.Entities(encryptedNewPassword) } }, callback));
        }

        private void OnChangePasswordCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnChangePasswordCompleted notified");
            Database.Completed -= OnChangePasswordCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
            {
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), null));
            }
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(-1, null));
        }

        private void ChangeEmail(string name, string newEmail, Action<int, object> callback)
        {
            Database.Completed += OnChangeEmailCompleted;
            Database.Update(new DatabaseUpdateOperation("change email", "update_2.php", new Dictionary<string, string>() { { "name", HtmlString.Entities(name) }, { "email", HtmlString.Entities(newEmail) } }, callback));
        }

        private void OnChangeEmailCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnChangeEmailCompleted notified");
            Database.Completed -= OnChangeEmailCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), null));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private void ChangeRecovery(string name, string newQuestion, string newAnswer, Action<int, object> callback)
        {
            Database.Completed += OnChangeRecoveryCompleted;
            Database.Update(new DatabaseUpdateOperation("change recovery", "update_3.php", new Dictionary<string, string>() { { "name", HtmlString.Entities(name) }, { "question", HtmlString.Entities(newQuestion) }, { "answer", HtmlString.Entities(newAnswer) } }, callback));
        }

        private void OnChangeRecoveryCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnChangeRecoveryCompleted notified");
            Database.Completed -= OnChangeRecoveryCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), null));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private void CheckIfEmailTaken(string email, Action<int, object> callback)
        {
            Database.Completed += OnIsEmailAvailableChecked;
            Database.Query(new DatabaseQueryOperation("check email", "query_3.php", new Dictionary<string, string>() { { "email", HtmlString.Entities(email) } }, callback));
        }

        private void OnIsEmailAvailableChecked(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnIsEmailAvailableChecked notified");
            Database.Completed -= OnIsEmailAvailableChecked;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "id", "password", "email", "question" })) != null)
            {
                string Id = Result["id"];
                string EncryptedPassword = Result["password"];
                string Email = Result["email"];
                string RecoveryQuestion = Result["question"];
                Debug.WriteLine($"Account {Id}: password={EncryptedPassword}, email={Email}, question={RecoveryQuestion}");

                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.Success, Result));
            }
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.ErrorNotFound, null));
        }

        private void RegisterAndSendEmail(string name, string encryptedPassword, string email, string question, string answer, Action<int, object> callback)
        {
            Database.Completed += OnRegisterSendEmailCompleted;
            Database.Update(new DatabaseUpdateOperation("start register", "insert_1.php", new Dictionary<string, string>() { { "name", HtmlString.Entities(name) }, { "password", HtmlString.Entities(encryptedPassword) }, { "email", HtmlString.Entities(email) }, { "question", HtmlString.Entities(question) }, { "answer", HtmlString.Entities(answer) }, { "language", ((int)GetLanguage.LanguageState).ToString() } }, callback));
        }

        private void OnRegisterSendEmailCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnRegisterSendEmailCompleted notified");
            Database.Completed -= OnRegisterSendEmailCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), null));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private void BeginRecoveryAndSendEmail(string email, Action<int, object> callback)
        {
            Database.Completed += OnRecoverySendEmailCompleted;
            Database.Update(new DatabaseUpdateOperation("start recovery", "update_4.php", new Dictionary<string, string>() { { "email", HtmlString.Entities(email) }, { "language", ((int)GetLanguage.LanguageState).ToString() } }, callback));
        }

        private void OnRecoverySendEmailCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnRecoverySendEmailCompleted notified");
            Database.Completed -= OnRecoverySendEmailCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), null));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private void ActivateAccountAndSendEmail(string name, string email, string encryptedPassword, string encryptedAnswer, Action<int, object> callback)
        {
            Database.Completed += OnActivateAccountCompleted;
            Database.Update(new DatabaseUpdateOperation("activate account", "update_5.php", new Dictionary<string, string>() { { "name", HtmlString.Entities(name) }, { "email", HtmlString.Entities(email) }, { "password", HtmlString.Entities(encryptedPassword) }, { "answer", string.IsNullOrEmpty(encryptedAnswer) ? "" : HtmlString.Entities(encryptedAnswer) }, { "language", ((int)GetLanguage.LanguageState).ToString() } }, callback));
        }

        private void OnActivateAccountCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnActivateAccountCompleted notified");
            Database.Completed -= OnActivateAccountCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), null));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private void RecoverAccount(string name, string encryptedAnswer, string encryptedPassword, Action<int, object> callback)
        {
            Database.Completed += OnAccountRecoveryCompleted;
            Database.Update(new DatabaseUpdateOperation("recover account", "update_6.php", new Dictionary<string, string>() { { "name", HtmlString.Entities(name) }, { "answer", HtmlString.Entities(encryptedAnswer) }, { "password", HtmlString.Entities(encryptedPassword) } }, callback));
        }

        private void OnAccountRecoveryCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnAccountRecoveryCompleted notified");
            Database.Completed -= OnAccountRecoveryCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), null));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private Database Database = Database.Current;
        #endregion

        #region Simulation
        private void InitSimulation()
        {
            if (!string.IsNullOrEmpty(NetTools.UrlTools.GetBaseUrl()))
                return;

            OperationHandler.Add(new OperationHandler("/request/encrypt.php", OnEncrypt));
            OperationHandler.Add(new OperationHandler("/request/query_1.php", OnSignInMatchRequest));
            OperationHandler.Add(new OperationHandler("/request/update_1.php", OnChangePasswordRequest));
            OperationHandler.Add(new OperationHandler("/request/update_2.php", OnChangeEmailRequest));
            OperationHandler.Add(new OperationHandler("/request/update_3.php", OnChangeRecoveryRequest));
            OperationHandler.Add(new OperationHandler("/request/query_3.php", OnCheckEmailMatchRequest));
            OperationHandler.Add(new OperationHandler("/request/insert_1.php", OnSignUpRequest));
            OperationHandler.Add(new OperationHandler("/request/update_4.php", OnRecoveryRequest));
            OperationHandler.Add(new OperationHandler("/request/update_5.php", OnCompleteRegistrationRequest));
            OperationHandler.Add(new OperationHandler("/request/update_6.php", OnCompleteRecoveryRequest));
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
                if (Line.ContainsKey("id") && Line["id"] == Username && Line.ContainsKey("is active") && Line["is active"] == "1")
                    Result.Add(new Dictionary<string, string>()
                    {
                        { "id", Line["id"]},
                        { "password", Line["password"] },
                        { "email", Line["email"] },
                        { "question", Line["question"] },
                        { "name", Line["name"] },
                        { "login_url", Line["login_url"] },
                        { "meeting_url", Line["meeting_url"] },
                        { "validation_url", Line["validation_url"] },
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
                if (Line.ContainsKey("id") && Line["id"] == Username && Line.ContainsKey("is active") && Line["is active"] == "1" && Line.ContainsKey("password"))
                {
                    Line["password"] = Password;

                    Result.Add(new Dictionary<string, string>()
                    {
                        { "result", ((int)ErrorCodes.Success).ToString()},
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
                if (Line.ContainsKey("id") && Line["id"] == Username && Line.ContainsKey("is active") && Line["is active"] == "1" && Line.ContainsKey("email"))
                {
                    Line["email"] = Email;

                    Result.Add(new Dictionary<string, string>()
                    {
                        { "result", ((int)ErrorCodes.Success).ToString()},
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
                if (Line.ContainsKey("id") && Line["id"] == Username && Line.ContainsKey("is active") && Line["is active"] == "1")
                {
                    if (Line.ContainsKey("question"))
                        Line["question"] = NewQuestion;
                    else
                        Line.Add("question", NewQuestion);

                    if (Line.ContainsKey("answer"))
                        Line["answer"] = NewAnswer;
                    else
                        Line.Add("answer", NewAnswer);

                    Result.Add(new Dictionary<string, string>()
                    {
                        { "result", ((int)ErrorCodes.Success).ToString()},
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
                if (Line.ContainsKey("email") && Line["email"] == Email && Line.ContainsKey("is active") && Line["is active"] == "1")
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
            NewLine.Add("is active", "0");
            KnownUserTable.Add(NewLine);

            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)ErrorCodes.Success).ToString()},
            });

            DispatcherTimer SignUpConfirmedTimer = new DispatcherTimer();
            SignUpConfirmedTimer.Interval = TimeSpan.FromSeconds(3);
            SignUpConfirmedTimer.Tick += (object sender, object e) => OnSignUpConfirmed(sender, e, Username, Email, Question);
            SignUpConfirmedTimer.Start();

            return Result;
        }

        private void OnSignUpConfirmed(object sender, object e, string username, string email, string question)
        {
            DispatcherTimer SignUpConfirmedTimer = (DispatcherTimer)sender;
            SignUpConfirmedTimer.Stop();

            if (MessageBox.Show("Continue registration?", "Email sent", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Dictionary<string, string> QueryString = App.QueryString;
                QueryString.Clear();
                QueryString.Add("type", "register");
                QueryString.Add("username", username);
                QueryString.Add("email", email);
                QueryString.Add("question", Login.EncodedRecoveryQuestion(question));

                PageNames NextPageName;
                On_RegisterEnd(PageNames.homePage, null, null, out NextPageName);
                (App.Current as App).GoTo(NextPageName);
            }
        }

        private List<Dictionary<string, string>> OnRecoveryRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string Email;
            if (parameters.ContainsKey("email"))
                Email = parameters["email"];
            else
                Email = null;

            string Language;
            if (parameters.ContainsKey("language"))
                Language = parameters["language"];
            else
                Language = "0";

            if (Email == null)
                return Result;

            foreach (Dictionary<string, string> Line in KnownUserTable)
                if (Line.ContainsKey("email") && Line["email"] == Email && Line.ContainsKey("is active") && Line["is active"] == "1" && Line.ContainsKey("question") && Line["question"].Length > 0)
                {
                    Result.Add(new Dictionary<string, string>()
                    {
                        { "result", ((int)ErrorCodes.Success).ToString()},
                    });

                    string Username = Line.ContainsKey("id") ? Line["id"] : "";

                    DispatcherTimer RecoveryConfirmedTimer = new DispatcherTimer();
                    RecoveryConfirmedTimer.Interval = TimeSpan.FromSeconds(3);
                    RecoveryConfirmedTimer.Tick += (object sender, object e) => OnRecoveryConfirmed(sender, e, Username, Email, Line["question"]);
                    RecoveryConfirmedTimer.Start();
                    break;
                }

            return Result;
        }

        private void OnRecoveryConfirmed(object sender, object e, string username, string email, string question)
        {
            DispatcherTimer SignUpConfirmedTimer = (DispatcherTimer)sender;
            SignUpConfirmedTimer.Stop();

            if (MessageBox.Show("Continue recovery?", "Email sent", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Dictionary<string, string> QueryString = App.QueryString;
                QueryString.Clear();
                QueryString.Add("type", "recovery");
                QueryString.Add("username", username);
                QueryString.Add("email", email);
                QueryString.Add("question", question);

                PageNames NextPageName;
                On_RecoveryEnd(PageNames.homePage, null, null, out NextPageName);
                (App.Current as App).GoTo(NextPageName);
            }
        }

        private List<Dictionary<string, string>> OnCompleteRegistrationRequest(Dictionary<string, string> parameters)
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

            string EncryptedPassword;
            if (parameters.ContainsKey("password"))
                EncryptedPassword = parameters["password"];
            else
                EncryptedPassword = null;

            string EncryptedAnswer;
            if (parameters.ContainsKey("answer"))
                EncryptedAnswer = parameters["answer"];
            else
                EncryptedAnswer = null;

            string Language;
            if (parameters.ContainsKey("language"))
                Language = parameters["language"];
            else
                Language = "0";

            if (Username == null || Email == null || EncryptedPassword == null || EncryptedAnswer == null)
                return Result;

            foreach (Dictionary<string, string> Line in KnownUserTable)
                if (Line.ContainsKey("id") && Line["id"] == Username)
                {
                    ErrorCodes Error;
                    if ((Line.ContainsKey("password") && Line["password"] == EncryptedPassword) && (Line.ContainsKey("answer") && Line["answer"] == EncryptedAnswer))
                    {
                        if (Line.ContainsKey("is active"))
                            Line["is active"] = "1";
                        else
                            Line.Add("is active", "1");

                        Error = ErrorCodes.Success;
                    }
                    else if (EncryptedAnswer.Length == 0)
                        Error = ErrorCodes.InvalidUsernameOrPassword;
                    else
                        Error = ErrorCodes.InvalidUsernamePasswordOrAnswer;

                    Result.Add(new Dictionary<string, string>()
                    {
                        { "result", ((int)Error).ToString()},
                    });

                    break;
                }

            return Result;
        }

        private List<Dictionary<string, string>> OnCompleteRecoveryRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string Username;
            if (parameters.ContainsKey("name"))
                Username = parameters["name"];
            else
                Username = null;

            string EncryptedAnswer;
            if (parameters.ContainsKey("answer"))
                EncryptedAnswer = parameters["answer"];
            else
                EncryptedAnswer = null;

            string EncryptedPassword;
            if (parameters.ContainsKey("password"))
                EncryptedPassword = parameters["password"];
            else
                EncryptedPassword = null;

            if (Username == null || EncryptedAnswer == null || string.IsNullOrEmpty(EncryptedPassword))
                return Result;

            foreach (Dictionary<string, string> Line in KnownUserTable)
                if (Line.ContainsKey("id") && Line["id"] == Username && Line.ContainsKey("is active") && Line["is active"] == "1")
                {
                    ErrorCodes Error;
                    if (Line.ContainsKey("answer") && Line["answer"] == EncryptedAnswer)
                    {
                        if (Line.ContainsKey("password"))
                            Line["password"] = EncryptedPassword;
                        else
                            Line.Add("password", EncryptedPassword);

                        Error = ErrorCodes.Success;
                    }
                    else
                        Error = ErrorCodes.InvalidUsernameOrAnswer;

                    Result.Add(new Dictionary<string, string>()
                    {
                        { "result", ((int)Error).ToString()},
                    });

                    break;
                }

            return Result;
        }

        public static string EncodedRecoveryQuestion(string text)
        {
            string Result = "";
            foreach (char c in text)
                Result += (char)(((int)c ^ 204 ) + 40);

            return Result;
        }

        public static string DecodedRecoveryQuestion(string text)
        {
            string Result = "";
            foreach (char c in text)
                Result += (char)(((int)c - 40) ^ 204);

            return Result;
        }

        public static int ParseResult(string result)
        {
            int IntError;
            if (int.TryParse(result, out IntError))
                return IntError;
            else
                return -1;
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
                { "is active", "1" },
                { "name", Eqmlp.KnownOrganizationTable[0]["name"] },
                { "login_url", Eqmlp.KnownOrganizationTable[0]["login_url"] },
                { "meeting_url", Eqmlp.KnownOrganizationTable[0]["meeting_url"] },
                { "validation_url", Eqmlp.KnownOrganizationTable[0]["validation_url"] },
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
