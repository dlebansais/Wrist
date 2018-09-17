using NetTools;
using Presentation;
using SmallArgon2d;
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
            UsernameAlreadyUsed = 7,
            EmailAlreadyUsed = 8,
        }

        public enum EncryptionUse
        {
            Password = 1,
            SecretAnswer,
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
        private byte[] Salt;

        public void On_CheckLoggedIn(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            switch (pageName)
            {
                case PageNames.accountPage:
                case PageNames.change_emailPage:
                case PageNames.change_email_failed_1Page:
                case PageNames.change_email_failed_2Page:
                case PageNames.change_email_failed_3Page:
                case PageNames.change_email_failed_4Page:
                case PageNames.change_email_failed_5Page:
                case PageNames.change_email_successPage:
                case PageNames.change_passwordPage:
                case PageNames.change_password_failed_1Page:
                case PageNames.change_password_failed_2Page:
                case PageNames.change_password_failed_3Page:
                case PageNames.change_password_failed_4Page:
                case PageNames.change_password_failed_5Page:
                case PageNames.change_password_successPage:
                case PageNames.change_recoveryPage:
                case PageNames.change_recovery_failed_1Page:
                case PageNames.change_recovery_failed_2Page:
                case PageNames.change_recovery_failed_3Page:
                case PageNames.change_recovery_failed_4Page:
                case PageNames.change_recovery_failed_5Page:
                case PageNames.change_recovery_successPage:
                    if (LoginState != LoginStates.SignedIn)
                        destinationPageName = PageNames.loginPage;
                    else
                        destinationPageName = pageName;
                    break;

                default:
                    destinationPageName = pageName;
                    break;
            }
        }

        #region Encryption
        public string EncryptedValue(string value, byte[] salt, EncryptionUse use)
        {
            if (NetTools.UrlTools.IsUsingRestrictedFeatures)
            {
                using (Argon2d Argon2 = new Argon2d(Encoding.UTF8.GetBytes(value)))
                {
                    Argon2.Salt = salt;
                    Argon2.KnownSecret = SecretUuid.GuidBytes;
                    Argon2.Iterations = 1;
                    Argon2.MemorySize = 1024;
                    Argon2.AssociatedUse = (int)use;
                    byte[] Hash = Argon2.GetBytes(128);
                    string EncodedHash = Argon2.GetEncoded(HashTools.GetString(Hash));

                    return EncodedHash;
                }
            }
            else
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        public string MixedSalt(string salt)
        {
            long Ticks = DateTime.Now.Ticks;
            byte[] TickBytes = BitConverter.GetBytes(Ticks);
            string TickString = HashTools.GetString(TickBytes);

            return salt + TickString;
        }

        #endregion

        #region Register
        public void On_Register(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            string PasswordValue;
            string AnswerValue;

            bool IsPasswordValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.Password)}", out PasswordValue);
            bool IsAnswerValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.RecoveryAnswer)}", out AnswerValue);

            if (string.IsNullOrEmpty(Name) || !IsPasswordValid || string.IsNullOrEmpty(Email) || !Email.Contains("@"))
                destinationPageName = PageNames.register_failed_1Page;

            else if (!string.IsNullOrEmpty(RecoveryQuestion) && !IsAnswerValid)
                destinationPageName = PageNames.register_failed_2Page;

            else if (string.IsNullOrEmpty(RecoveryQuestion) && IsAnswerValid)
                destinationPageName = PageNames.register_failed_3Page;

            else
            {
                if (string.IsNullOrEmpty(RecoveryQuestion) || !IsAnswerValid)
                    StartRegister(Name, PasswordValue, Email, "", "");
                else
                    StartRegister(Name, PasswordValue, Email, RecoveryQuestion, AnswerValue);

                destinationPageName = PageNames.CurrentPage;
            }
        }

        private void StartRegister(string name, string password, string email, string question, string answer)
        {
            QueryNewCredential(name, email, (int getError, object getResult) => Register_OnNewCredentialReceived(getError, getResult, name, password, email, question, answer));
        }

        private void Register_OnNewCredentialReceived(int error, object result, string name, string password, string email, string question, string answer)
        {
            if (error == (int)ErrorCodes.Success && result != null)
            {
                Dictionary<string, string> NewCredentialResult = (Dictionary<string, string>)result;
                string SaltString = NewCredentialResult["salt"];
                SaltString = MixedSalt(SaltString);

                byte[] NewSalt;
                if (HashTools.TryParse(SaltString, out NewSalt))
                {
                    string EncryptedPassword = EncryptedValue(password, NewSalt, EncryptionUse.Password);

                    string EncryptedAnswer;
                    if (!string.IsNullOrEmpty(answer))
                        EncryptedAnswer = EncryptedValue(answer, NewSalt, EncryptionUse.SecretAnswer);
                    else
                        EncryptedAnswer = "";

                    RegisterAndSendEmail(name, EncryptedPassword, email, question, EncryptedAnswer, SaltString.ToLower(), (int checkError, object checkResult) => Register_OnEmailSent(checkError, checkResult));
                }
                else
                {
                    Debug.WriteLine("Failed to parse salt");
                    (App.Current as App).GoTo(PageNames.register_failed_4Page);
                }
            }
            else if (error == (int)ErrorCodes.UsernameAlreadyUsed)
                (App.Current as App).GoTo(PageNames.register_failed_5Page);
            else if (error == (int)ErrorCodes.EmailAlreadyUsed)
                (App.Current as App).GoTo(PageNames.register_failed_6Page);
            else
                (App.Current as App).GoTo(PageNames.register_failed_4Page);
        }

        private void Register_OnEmailSent(int error, object result)
        {
            if (error == (int)ErrorCodes.Success)
                (App.Current as App).GoTo(PageNames.registration_startedPage);
            else
                (App.Current as App).GoTo(PageNames.register_failed_4Page);
        }

        public void On_RegisterEnd(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            Dictionary<string, string> QueryString = App.QueryString;
            if (QueryString != null && QueryString.ContainsKey("type") && QueryString["type"] == "register")
            {
                string QueryName = QueryString.ContainsKey("username") ? QueryString["username"] : null;
                string QueryEmail = QueryString.ContainsKey("email") ? QueryString["email"] : null;
                string SaltString = QueryString.ContainsKey("salt") ? QueryString["salt"] : null;
                string QueryQuestion = QueryString.ContainsKey("question") ? QueryString["question"] : null;

                byte[] QuerySalt;
                if (!string.IsNullOrEmpty(QueryName) && !string.IsNullOrEmpty(QueryEmail) && HashTools.TryParse(SaltString, out QuerySalt))
                {
                    Name = QueryName;
                    Email = QueryEmail;
                    Salt = QuerySalt;
                    RecoveryQuestion = QueryQuestion;
                    destinationPageName = PageNames.registration_endPage;
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
                Persistent.SetValue("salt", null);
                Persistent.SetValue("question", null);
                Persistent.SetValue("remember", null);
            }

            string PasswordValue;
            string AnswerValue;

            bool IsPasswordValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.Password)}", out PasswordValue);
            bool IsAnswerValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.RecoveryAnswer)}", out AnswerValue);

            if (!IsPasswordValid)
                destinationPageName = PageNames.registration_end_failed_1Page;

            else if (HasQuestion && !IsAnswerValid)
                destinationPageName = PageNames.registration_end_failed_2Page;

            else
            {
                string EncryptedPassword = EncryptedValue(PasswordValue, Salt, EncryptionUse.Password);
                string EncryptedAnswer = HasQuestion ? EncryptedValue(AnswerValue, Salt, EncryptionUse.SecretAnswer) : "";

                ActivateAccountAndSendEmail(Name, Email, EncryptedPassword, EncryptedAnswer, (int checkError, object checkResult) => CompleteRegistration_AccountActivated(checkError, checkResult, Remember));

                destinationPageName = PageNames.CurrentPage;
            }
        }

        private void CompleteRegistration_AccountActivated(int error, object result, bool remember)
        {
            if (error == (int)ErrorCodes.Success && result != null)
            {
                Dictionary<string, string> CheckPasswordResult = (Dictionary<string, string>)result;
                if (remember)
                {
                    Persistent.SetValue("name", Name);
                    Persistent.SetValue("email", Email);
                    Persistent.SetValue("salt", HashTools.GetString(Salt));
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
                Persistent.SetValue("salt", null);
                Persistent.SetValue("question", null);
                Persistent.SetValue("remember", null);
            }

            string PasswordValue;
            if (!GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.Password)}", out PasswordValue))
                destinationPageName = PageNames.login_failedPage;

            else if (string.IsNullOrEmpty(Name))
                destinationPageName = PageNames.login_failedPage;

            else
            {
                StartLogin(Name, PasswordValue, Remember);
                destinationPageName = PageNames.CurrentPage;
            }
        }

        private void StartLogin(string name, string currentPassword, bool remember)
        {
            GetUserSalt(name, (int getError, object getResult) => Login_OnSaltReceived(getError, getResult, name, currentPassword, remember));
        }

        private void Login_OnSaltReceived(int error, object result, string name, string currentPassword, bool remember)
        {
            if (error == (int)ErrorCodes.Success && result != null)
            {
                Dictionary<string, string> GetCredentialResult = (Dictionary<string, string>)result;
                string SaltString = GetCredentialResult["salt"];

                byte[] TestSalt;
                if (HashTools.TryParse(SaltString, out TestSalt))
                {
                    string EncryptedCurrentPassword = EncryptedValue(currentPassword, TestSalt, EncryptionUse.Password);
                    SignIn(name, EncryptedCurrentPassword, (int signInError, object signInResult) => Login_OnSignIn(signInError, signInResult, TestSalt, remember));
                }
                else
                {
                    Debug.WriteLine("Failed to parse salt");
                    (App.Current as App).GoTo(PageNames.login_failedPage);
                }
            }
            else
                (App.Current as App).GoTo(PageNames.login_failedPage);
        }

        private void Login_OnSignIn(int error, object result, byte[] TestSalt, bool remember)
        {
            if (error == (int)ErrorCodes.Success && result != null)
            {
                Dictionary<string, string> SignInResult = (Dictionary<string, string>)result;
                Name = SignInResult["id"];
                Email = SignInResult["email"];
                Salt = TestSalt;
                RecoveryQuestion = SignInResult["question"];

                if (remember)
                {
                    Persistent.SetValue("name", Name);
                    Persistent.SetValue("email", Email);
                    Persistent.SetValue("salt", HashTools.GetString(Salt));
                    Persistent.SetValue("question", RecoveryQuestion);
                    Persistent.SetValue("remember", "1");
                }

                LoginState = LoginStates.SignedIn;

                NotifyPropertyChanged(nameof(Email));
                NotifyPropertyChanged(nameof(RecoveryQuestion));
                NotifyPropertyChanged(nameof(LoginState));

                string OrganizationName = SignInResult["name"];
                ((Eqmlp)GetEqmlp).Login(OrganizationName);

                (App.Current as App).GoTo(PageNames.accountPage);
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
            Salt = null;
            RecoveryQuestion = null;
            ((Eqmlp)GetEqmlp).Logout();

            Persistent.SetValue("name", null);
            Persistent.SetValue("email", null);
            Persistent.SetValue("salt", null);
            Persistent.SetValue("question", null);
        }
        #endregion

        #region Change Password
        public void On_ChangePassword(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            string PasswordValue;
            string NewPasswordValue;
            string ConfirmPasswordValue;

            bool IsCurrentPasswordValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.Password)}", out PasswordValue);
            bool IsNewPasswordValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.NewPassword)}", out NewPasswordValue);
            bool IsConfirmPasswordValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.ConfirmPassword)}", out ConfirmPasswordValue);

            if (!IsCurrentPasswordValid)
                destinationPageName = PageNames.change_password_failed_1Page;

            else if (!IsNewPasswordValid || !IsConfirmPasswordValid)
                destinationPageName = PageNames.change_password_failed_2Page;

            else if (NewPasswordValue != ConfirmPasswordValue)
                destinationPageName = PageNames.change_password_failed_3Page;

            else
            {
                string EncryptedCurrentPassword = EncryptedValue(PasswordValue, Salt, EncryptionUse.Password);
                string EncryptedNewPassword = EncryptedValue(NewPasswordValue, Salt, EncryptionUse.Password);
                ChangePassword(Name, EncryptedCurrentPassword, EncryptedNewPassword, ChangePassword_OnPasswordChanged);

                destinationPageName = PageNames.CurrentPage;
            }
        }

        private void ChangePassword_OnPasswordChanged(int error, object result)
        {
            if (error == (int)ErrorCodes.Success)
                (App.Current as App).GoTo(PageNames.change_password_successPage);
            else if (error == (int)ErrorCodes.ErrorNotFound)
                (App.Current as App).GoTo(PageNames.change_password_failed_5Page);
            else
                (App.Current as App).GoTo(PageNames.change_password_failed_4Page);
        }
        #endregion

        #region Change Email
        public void On_ChangeEmail(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            string PasswordValue;
            if (!GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.Password)}", out PasswordValue))
                destinationPageName = PageNames.change_email_failed_1Page;

            else if (string.IsNullOrEmpty(NewEmail))
                destinationPageName = PageNames.change_email_failed_2Page;

            else if (!NewEmail.Contains("@"))
                destinationPageName = PageNames.change_email_failed_3Page;

            else
            {
                string EncryptedPassword = EncryptedValue(PasswordValue, Salt, EncryptionUse.Password);
                ChangeEmail(Name, EncryptedPassword, NewEmail, (int changeError, object changeResult) => ChangeEmail_OnEmailChanged(changeError, changeResult, NewEmail));

                destinationPageName = PageNames.CurrentPage;
            }
        }

        private void ChangeEmail_OnEmailChanged(int error, object result, string newEmail)
        {
            if (error == (int)ErrorCodes.Success)
            {
                Email = newEmail;
                NewEmail = null;
                (App.Current as App).GoTo(PageNames.change_email_successPage);
            }
            else if (error == (int)ErrorCodes.ErrorNotFound)
                (App.Current as App).GoTo(PageNames.change_email_failed_5Page);
            else
                (App.Current as App).GoTo(PageNames.change_email_failed_4Page);
        }
        #endregion

        #region Change Recovery
        public void On_ChangeRecovery(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            string PasswordValue;
            string AnswerValue;
            string ConfirmAnswerValue;

            bool IsPasswordValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.Password)}", out PasswordValue);
            bool IsAnswerValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.RecoveryAnswer)}", out AnswerValue);
            bool IsConfirmAnswerValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.ConfirmAnswer)}", out ConfirmAnswerValue);

            if (!IsPasswordValid)
                destinationPageName = PageNames.change_recovery_failed_1Page;

            else if (string.IsNullOrEmpty(NewQuestion) && !IsAnswerValid && !IsConfirmAnswerValid)
            {
                string EncryptedPassword = EncryptedValue(PasswordValue, Salt, EncryptionUse.Password);
                ChangeRecovery(Name, EncryptedPassword, "", "", (int changeError, object changeResult) => ChangeRecovery_OnRecoveryChanged(changeError, changeResult, NewQuestion));
                destinationPageName = PageNames.CurrentPage;
            }

            else if (string.IsNullOrEmpty(NewQuestion) || !IsAnswerValid || !IsConfirmAnswerValid)
                destinationPageName = PageNames.change_recovery_failed_2Page;

            else if (AnswerValue != ConfirmAnswerValue)
                destinationPageName = PageNames.change_recovery_failed_3Page;

            else
            {
                string EncryptedPassword = EncryptedValue(PasswordValue, Salt, EncryptionUse.Password);
                string EncryptedNewAnswer = EncryptedValue(AnswerValue, Salt, EncryptionUse.SecretAnswer);
                ChangeRecovery(Name, EncryptedPassword, NewQuestion, EncryptedNewAnswer, (int changeError, object changeResult) => ChangeRecovery_OnRecoveryChanged(changeError, changeResult, NewQuestion));
                destinationPageName = PageNames.CurrentPage;
            }
        }

        private void ChangeRecovery_OnRecoveryChanged(int error, object result, string newQuestion)
        {
            if (error == (int)ErrorCodes.Success)
            {
                RecoveryQuestion = newQuestion;
                NewQuestion = null;
                (App.Current as App).GoTo(PageNames.change_recovery_successPage);
            }
            else if (error == (int)ErrorCodes.ErrorNotFound)
                (App.Current as App).GoTo(PageNames.change_recovery_failed_5Page);
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
            CheckIfEmailValid(email, (int checkError, object checkResult) => Recovery_OnEmailChecked(checkError, checkResult, email));
        }

        private void Recovery_OnEmailChecked(int error, object result, string email)
        {
            if (error == (int)ErrorCodes.Success)
                BeginRecoveryAndSendEmail(email, Recovery_OnEmailSent);
            else if (error == (int)ErrorCodes.ErrorNoQuestion)
                (App.Current as App).GoTo(PageNames.recovery_failed_2Page);
            else if (error == (int)ErrorCodes.ErrorNotFound)
                (App.Current as App).GoTo(PageNames.recovery_failed_2Page);
            else
                (App.Current as App).GoTo(PageNames.recovery_failed_3Page);
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
            if (QueryString != null && QueryString.ContainsKey("type") && QueryString["type"] == "recovery")
            {
                string QueryName = QueryString.ContainsKey("username") ? QueryString["username"] : null;
                string QueryEmail = QueryString.ContainsKey("email") ? QueryString["email"] : null;
                string SaltString = QueryString.ContainsKey("salt") ? QueryString["salt"] : null;
                string QueryQuestion = QueryString.ContainsKey("question") ? QueryString["question"] : null;

                byte[] QuerySalt;
                if (!string.IsNullOrEmpty(QueryName) && !string.IsNullOrEmpty(QueryEmail) && !string.IsNullOrEmpty(QueryQuestion) && HashTools.TryParse(SaltString, out QuerySalt))
                {
                    Name = QueryName;
                    Email = QueryEmail;
                    RecoveryQuestion = QueryQuestion;
                    Salt = QuerySalt;
                    destinationPageName = PageNames.recovery_endPage;
                }
                else
                    destinationPageName = PageNames.invalid_operationPage;
            }
            else
                destinationPageName = pageName;
        }

        public void On_CompleteRecovery(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            string AnswerValue;
            string NewPasswordValue;

            bool IsAnswerValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.RecoveryAnswer)}", out AnswerValue);
            bool IsNewPasswordValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.NewPassword)}", out NewPasswordValue);

            if (!IsAnswerValid)
                destinationPageName = PageNames.recovery_end_failed_1Page;

            else if (!IsNewPasswordValid)
                destinationPageName = PageNames.recovery_end_failed_2Page;

            else
            {
                string EncryptedAnswer = EncryptedValue(AnswerValue, Salt, EncryptionUse.SecretAnswer);
                string EncryptedNewPassword = EncryptedValue(NewPasswordValue, Salt, EncryptionUse.Password);

                RecoverAccount(Name, EncryptedAnswer, EncryptedNewPassword, CompleteRecovery_OnGetUserInfo);
                destinationPageName = PageNames.CurrentPage;
            }
        }

        private void CompleteRecovery_OnGetUserInfo(int error, object result)
        {
            if (error == (int)ErrorCodes.Success && result != null)
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
        private void QueryNewCredential(string name, string email, Action<int, object> callback)
        {
            Database.Completed += OnQueryNewCredentialCompleted;
            Database.Query(new DatabaseQueryOperation("new credential", "query_7.php", new Dictionary<string, string>() { { "name", HtmlString.PercentEncoded(name) }, { "email", HtmlString.PercentEncoded(email) } }, callback));
        }

        private void OnQueryNewCredentialCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnQueryNewCredentialCompleted notified");
            Database.Completed -= OnQueryNewCredentialCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result", "salt" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.OperationFailed, null));
        }

        private void ChangePassword(string name, string encryptedCurrentPassword, string encryptedNewPassword, Action<int, object> callback)
        {
            Database.Completed += OnChangePasswordCompleted;
            Database.Update(new DatabaseUpdateOperation("change password", "update_1.php", new Dictionary<string, string>() { { "name", HtmlString.PercentEncoded(name) }, { "password", encryptedCurrentPassword }, { "newpassword", encryptedNewPassword } }, callback));
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

        private void ChangeEmail(string name, string encryptedPassword, string newEmail, Action<int, object> callback)
        {
            Database.Completed += OnChangeEmailCompleted;
            Database.Update(new DatabaseUpdateOperation("change email", "update_2.php", new Dictionary<string, string>() { { "name", HtmlString.PercentEncoded(name) }, { "password", encryptedPassword }, { "email", HtmlString.PercentEncoded(newEmail) } }, callback));
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

        private void ChangeRecovery(string name, string encryptedPassword, string newQuestion, string encryptedNewAnswer, Action<int, object> callback)
        {
            Database.Completed += OnChangeRecoveryCompleted;
            Database.Update(new DatabaseUpdateOperation("change recovery", "update_3.php", new Dictionary<string, string>() { { "name", HtmlString.PercentEncoded(name) }, { "password", encryptedPassword }, { "question", HtmlString.PercentEncoded(newQuestion) }, { "answer", encryptedNewAnswer } }, callback));
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

        private void CheckIfEmailValid(string email, Action<int, object> callback)
        {
            Database.Completed += OnIsEmailValidityChecked;
            Database.Query(new DatabaseQueryOperation("check email", "query_3.php", new Dictionary<string, string>() { { "email", HtmlString.PercentEncoded(email) } }, callback));
        }

        private void OnIsEmailValidityChecked(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnIsEmailValidityChecked notified");
            Database.Completed -= OnIsEmailValidityChecked;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null && Result.ContainsKey("result"))
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private void RegisterAndSendEmail(string name, string encryptedPassword, string email, string question, string encryptedAnswer, string salt, Action<int, object> callback)
        {
            Database.Completed += OnRegisterSendEmailCompleted;
            Database.Update(new DatabaseUpdateOperation("start register", "insert_1.php", new Dictionary<string, string>() { { "name", HtmlString.PercentEncoded(name) }, { "password", encryptedPassword }, { "email", HtmlString.PercentEncoded(email) }, { "question", HtmlString.PercentEncoded(question) }, { "answer", encryptedAnswer }, { "salt", salt }, { "language", ((int)GetLanguage.LanguageState).ToString() } }, callback));
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
            Database.Update(new DatabaseUpdateOperation("start recovery", "update_4.php", new Dictionary<string, string>() { { "email", HtmlString.PercentEncoded(email) }, { "language", ((int)GetLanguage.LanguageState).ToString() } }, callback));
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
            Database.Update(new DatabaseUpdateOperation("activate account", "update_5.php", new Dictionary<string, string>() { { "name", HtmlString.PercentEncoded(name) }, { "email", HtmlString.PercentEncoded(email) }, { "password", encryptedPassword }, { "answer", encryptedAnswer }, { "language", ((int)GetLanguage.LanguageState).ToString() } }, callback));
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

        private void RecoverAccount(string name, string encryptedAnswer, string encryptedNewPassword, Action<int, object> callback)
        {
            Database.Completed += OnAccountRecoveryCompleted;
            Database.Update(new DatabaseUpdateOperation("recover account", "update_6.php", new Dictionary<string, string>() { { "name", HtmlString.PercentEncoded(name) }, { "answer", encryptedAnswer }, { "password", encryptedNewPassword } }, callback));
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

        private void GetUserSalt(string name, Action<int, object> callback)
        {
            Database.Completed += OnGetUserSaltCompleted;
            Database.Query(new DatabaseQueryOperation("get user salt", "query_8.php", new Dictionary<string, string>() { { "name", HtmlString.PercentEncoded(name) } }, callback));
        }

        private void OnGetUserSaltCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnGetUserSaltCompleted notified");
            Database.Completed -= OnGetUserSaltCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result", "salt" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.OperationFailed, null));
        }

        private void SignIn(string name, string encryptedPassword, Action<int, object> callback)
        {
            Database.Completed += OnSignInCompleted;
            Database.Query(new DatabaseQueryOperation("sign in", "query_9.php", new Dictionary<string, string>() { { "name", HtmlString.PercentEncoded(name) }, { "password", encryptedPassword } }, callback));
        }

        private void OnSignInCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnSignInCompleted notified");
            Database.Completed -= OnSignInCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "id", "email", "question", "name", "login_url", "meeting_url", "validation_url", "result" })) != null && Result.ContainsKey("result"))
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.OperationFailed, null));
        }

        private Database Database = Database.Current;
        #endregion

        #region Simulation
        private void InitSimulation()
        {
            if (NetTools.UrlTools.IsUsingRestrictedFeatures)
                return;

            OperationHandler.Add(new OperationHandler("/request/update_1.php", OnChangePasswordRequest));
            OperationHandler.Add(new OperationHandler("/request/update_2.php", OnChangeEmailRequest));
            OperationHandler.Add(new OperationHandler("/request/update_3.php", OnChangeRecoveryRequest));
            OperationHandler.Add(new OperationHandler("/request/query_3.php", OnCheckEmailValidityRequest));
            OperationHandler.Add(new OperationHandler("/request/insert_1.php", OnSignUpRequest));
            OperationHandler.Add(new OperationHandler("/request/update_4.php", OnRecoveryRequest));
            OperationHandler.Add(new OperationHandler("/request/update_5.php", OnCompleteRegistrationRequest));
            OperationHandler.Add(new OperationHandler("/request/update_6.php", OnCompleteRecoveryRequest));
            OperationHandler.Add(new OperationHandler("/request/query_7.php", OnQueryNewCredentialRequest));
            OperationHandler.Add(new OperationHandler("/request/query_8.php", OnQuerySaltRequest));
            OperationHandler.Add(new OperationHandler("/request/query_9.php", OnSignInRequest));
        }

        private List<Dictionary<string, string>> OnChangePasswordRequest(Dictionary<string, string> parameters)
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

            string EncryptedNewPassword;
            if (parameters.ContainsKey("newpassword"))
                EncryptedNewPassword = parameters["newpassword"];
            else
                EncryptedNewPassword = null;

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(EncryptedPassword) || string.IsNullOrEmpty(EncryptedNewPassword))
                return Result;

            ErrorCodes ErrorCode = ErrorCodes.ErrorNotFound;
            foreach (Dictionary<string, string> Line in KnownUserTable)
                if (Line.ContainsKey("id") && Line["id"] == Username && Line.ContainsKey("active") && Line["active"] == "1" && Line.ContainsKey("password") && Line["password"] == EncryptedPassword)
                {
                    Line["password"] = EncryptedNewPassword;
                    ErrorCode = ErrorCodes.Success;
                    break;
                }

            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)ErrorCode).ToString()},
            });

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

            string EncryptedPassword;
            if (parameters.ContainsKey("password"))
                EncryptedPassword = parameters["password"];
            else
                EncryptedPassword = null;

            string NewEmail;
            if (parameters.ContainsKey("email"))
                NewEmail = parameters["email"];
            else
                NewEmail = null;

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(EncryptedPassword) || string.IsNullOrEmpty(NewEmail))
                return Result;

            ErrorCodes ErrorCode = ErrorCodes.ErrorNotFound;
            foreach (Dictionary<string, string> Line in KnownUserTable)
                if (Line.ContainsKey("id") && Line["id"] == Username && Line.ContainsKey("active") && Line["active"] == "1" && Line.ContainsKey("password") && Line["password"] == EncryptedPassword && Line.ContainsKey("email"))
                {
                    Line["email"] = NewEmail;
                    ErrorCode = ErrorCodes.Success;
                    break;
                }


            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)ErrorCode).ToString()},
            });
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

            string EncryptedPassword;
            if (parameters.ContainsKey("password"))
                EncryptedPassword = parameters["password"];
            else
                EncryptedPassword = null;

            string NewQuestion;
            if (parameters.ContainsKey("question"))
                NewQuestion = parameters["question"];
            else
                NewQuestion = null;

            string EncryptedNewAnswer;
            if (parameters.ContainsKey("answer"))
                EncryptedNewAnswer = parameters["answer"];
            else
                EncryptedNewAnswer = null;

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(EncryptedPassword) || NewQuestion == null || EncryptedNewAnswer == null)
                return Result;

            ErrorCodes ErrorCode = ErrorCodes.ErrorNotFound;
            foreach (Dictionary<string, string> Line in KnownUserTable)
                if (Line.ContainsKey("id") && Line["id"] == Username && Line.ContainsKey("password") && Line["password"] == EncryptedPassword && Line.ContainsKey("active") && Line["active"] == "1")
                {
                    if (NewQuestion.Length > 0 && EncryptedNewAnswer.Length > 0)
                    {
                        Line["question"] = EncodedRecoveryQuestion(NewQuestion);
                        Line["answer"] = EncryptedNewAnswer;
                    }
                    else
                    {
                        Line["question"] = null;
                        Line["answer"] = null;
                    }

                    ErrorCode = ErrorCodes.Success;
                    break;
                }

            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)ErrorCode).ToString()},
            });
            return Result;
        }

        private List<Dictionary<string, string>> OnCheckEmailValidityRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string Email;
            if (parameters.ContainsKey("email"))
                Email = parameters["email"];
            else
                Email = null;

            if (string.IsNullOrEmpty(Email))
                return Result;

            ErrorCodes ErrorCode = ErrorCodes.ErrorNotFound;
            foreach (Dictionary<string, string> Line in KnownUserTable)
                if (Line.ContainsKey("email") && Line["email"] == Email && Line.ContainsKey("active") && Line["active"] == "1")
                {
                    if (Line.ContainsKey("question") && !string.IsNullOrEmpty(Line["question"]))
                        ErrorCode = ErrorCodes.Success;
                    else
                        ErrorCode = ErrorCodes.ErrorNoQuestion;

                    break;
                }

            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)ErrorCode).ToString() },
            });
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

            string Salt;
            if (parameters.ContainsKey("salt"))
                Salt = parameters["salt"];
            else
                Salt = null;

            string Language;
            if (parameters.ContainsKey("language"))
                Language = parameters["language"];
            else
                Language = "0";

            if (Username == null || EncryptedPassword == null || Email == null || Question == null || Answer == null || Salt == null)
                return Result;

            foreach (Dictionary<string, string> Line in KnownUserTable)
                if ((Line.ContainsKey("id") && Line["id"] == Username) || (Line.ContainsKey("email") && Line["email"] == Email))
                    return Result;

            Dictionary<string, string> NewLine = new Dictionary<string, string>();
            NewLine.Add("id", Username);
            NewLine.Add("password", EncryptedPassword);
            NewLine.Add("email", Email);
            NewLine.Add("salt", Salt);
            NewLine.Add("question", EncodedRecoveryQuestion(Question));
            NewLine.Add("answer", Answer);
            NewLine.Add("active", "0");
            NewLine.Add("name", "");
            NewLine.Add("login_url", "");
            NewLine.Add("meeting_url", "");
            NewLine.Add("validation_url", "");

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
                QueryString.Add("question", question);

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
                if (Line.ContainsKey("email") && Line["email"] == Email && Line.ContainsKey("active") && Line["active"] == "1" && Line.ContainsKey("question") && Line["question"].Length > 0)
                {
                    Result.Add(new Dictionary<string, string>()
                    {
                        { "result", ((int)ErrorCodes.Success).ToString()},
                    });

                    string Username = Line.ContainsKey("id") ? Line["id"] : "";

                    DispatcherTimer RecoveryConfirmedTimer = new DispatcherTimer();
                    RecoveryConfirmedTimer.Interval = TimeSpan.FromSeconds(3);
                    RecoveryConfirmedTimer.Tick += (object sender, object e) => OnRecoveryConfirmed(sender, e, Username, Email, DecodedRecoveryQuestion(Line["question"]));
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
                        if (Line.ContainsKey("active"))
                            Line["active"] = "1";
                        else
                            Line.Add("active", "1");

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

            string EncryptedNewPassword;
            if (parameters.ContainsKey("password"))
                EncryptedNewPassword = parameters["password"];
            else
                EncryptedNewPassword = null;

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(EncryptedAnswer) || string.IsNullOrEmpty(EncryptedNewPassword))
                return Result;

            ErrorCodes Error = ErrorCodes.InvalidUsernameOrAnswer;
            foreach (Dictionary<string, string> Line in KnownUserTable)
                if (Line.ContainsKey("id") && Line["id"] == Username && Line.ContainsKey("answer") && Line["answer"] == EncryptedAnswer && Line.ContainsKey("active") && Line["active"] == "1" && Line.ContainsKey("question") && !string.IsNullOrEmpty(Line["question"]))
                {
                    Line["password"] = EncryptedNewPassword;
                    Error = ErrorCodes.Success;

                    break;
                }

            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)Error).ToString()},
            });
            return Result;
        }

        private List<Dictionary<string, string>> OnQueryNewCredentialRequest(Dictionary<string, string> parameters)
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

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Email))
                return Result;

            foreach (Dictionary<string, string> Line in KnownUserTable)
            {
                if (Line.ContainsKey("id") && Line["id"] == Username)
                {
                    Result.Add(new Dictionary<string, string>()
                    {
                        { "result", ((int)ErrorCodes.UsernameAlreadyUsed).ToString() },
                    });
                    return Result;
                }

                else if (Line.ContainsKey("email") && Line["email"] == Email)
                {
                    Result.Add(new Dictionary<string, string>()
                    {
                        { "result", ((int)ErrorCodes.EmailAlreadyUsed).ToString() },
                    });
                    return Result;
                }
            }

            long Ticks = DateTime.Now.Ticks;
            byte[] TickBytes = BitConverter.GetBytes(Ticks);
            string TickString = HashTools.GetString(TickBytes);
            string SaltString = TickString + TickString + TickString + TickString + TickString + TickString + TickString + TickString;

            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)ErrorCodes.Success).ToString() },
                { "salt", SaltString },
            });

            return Result;
        }

        private List<Dictionary<string, string>> OnQuerySaltRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string Username;
            if (parameters.ContainsKey("name"))
                Username = parameters["name"];
            else
                Username = null;

            if (string.IsNullOrEmpty(Username))
                return Result;

            foreach (Dictionary<string, string> Line in KnownUserTable)
            {
                if (Line.ContainsKey("active") && Line["active"] == "1" && ((Line.ContainsKey("id") && Line["id"] == Username) || (Line.ContainsKey("email") && Line["email"] == Username)))
                {
                    Result.Add(new Dictionary<string, string>()
                    {
                        { "result", ((int)ErrorCodes.Success).ToString() },
                        { "salt", Line["salt"] },
                    });
                    return Result;
                }
            }

            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)ErrorCodes.ErrorNotFound).ToString() },
            });

            return Result;
        }

        private List<Dictionary<string, string>> OnSignInRequest(Dictionary<string, string> parameters)
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

            if (Username == null || EncryptedPassword == null)
                return Result;

            foreach (Dictionary<string, string> Line in KnownUserTable)
            {
                if (Line.ContainsKey("active") && Line["active"] == "1" && ((Line.ContainsKey("id") && Line["id"] == Username) || (Line.ContainsKey("email") && Line["email"] == Username)) && Line.ContainsKey("password") && Line["password"] == EncryptedPassword)
                    Result.Add(new Dictionary<string, string>()
                    {
                        { "id", Line["id"]},
                        { "email", Line["email"] },
                        { "question", DecodedRecoveryQuestion(Line["question"]) },
                        { "name", Line["name"] },
                        { "login_url", Line["login_url"] },
                        { "meeting_url", Line["meeting_url"] },
                        { "validation_url", Line["validation_url"] },
                        { "result", ((int)ErrorCodes.Success).ToString() },
                    });
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
                { "salt", "c32e25dca5caa30aa442eb2e9190e08cc93868ea60fd91eff53f53b5bc420bd2e5079d3fb5197046f806936b5ae6d1eab72a49a5bd22a0522890423266b993350064ec298e1ad608" },
                { "question", EncodedRecoveryQuestion("foo") },
                { "answer", Convert.ToBase64String(Encoding.UTF8.GetBytes("not foo")) },
                { "active", "1" },
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
