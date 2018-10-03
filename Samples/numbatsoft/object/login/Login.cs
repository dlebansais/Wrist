using NetTools;
using Presentation;
using SmallArgon2d;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            EmailAddressAlreadyUsed = 8,
        }

        private enum EncryptionUse
        {
            Password = 1,
            SecretAnswer,
        }

        private class Credential
        {
            public Credential(string username, string password, string password_settings, string email_address, string salt, bool active, string name, string login_url, string meeting_url, string validation_url)
            {
                this.username = username;
                this.password = password;
                this.password_settings = password_settings;
                this.email_address = email_address;
                this.salt = salt;
                question = null;
                answer = null;
                answer_settings = null;
                this.active = active;
                this.name = name;
                this.login_url = login_url;
                this.meeting_url = meeting_url;
                this.validation_url = validation_url;
                operation = null;
                end_date = null;
            }

            public Credential(string username, string password, string password_settings, string email_address, string salt, string question, string answer, string answer_settings, bool active, string name, string login_url, string meeting_url, string validation_url)
            {
                this.username = username;
                this.password = password;
                this.password_settings = password_settings;
                this.email_address = email_address;
                this.salt = salt;
                this.question = question;
                this.answer = answer;
                this.answer_settings = answer_settings;
                this.active = active;
                this.name = name;
                this.login_url = login_url;
                this.meeting_url = meeting_url;
                this.validation_url = validation_url;
                operation = null;
                end_date = null;
            }

            public string username { get; set; }
            public string password { private get; set; }
            public string password_settings { get; set; }
            public string email_address { get; set; }
            public string salt { get; set; }
            public string question { get; set; }
            public string answer { private get; set; }
            public string answer_settings { get; set; }
            public string operation { private get; set; }
            public DateTime? end_date { private get; set; }
            public bool active { get; set; }
            public string name { get; set; }
            public string login_url { get; set; }
            public string meeting_url { get; set; }
            public string validation_url { get; set; }

            public static bool insert_1_1(IList<Credential> credentials, string param_username, string param_password, string param_passwordSettings, string param_emailAddress, string param_salt, string param_question, string param_answer, string param_answerSettings, string param_operation, DateTime param_begin, DateTime param_endDate)
            {
                foreach (Credential Row in credentials)
                    if (Row.username == param_username || Row.email_address == param_emailAddress)
                        return false;

                Credential NewRow = new Credential(param_username, param_password, param_passwordSettings, param_emailAddress, param_salt, param_question, param_answer, param_answerSettings, false, "", "", "", "");
                credentials.Add(NewRow);
                return true;
            }

            public static bool insert_1_2(IList<Credential> credentials, string param_username, string param_password, string param_passwordSettings, string param_emailAddress, string param_salt, string param_operation, DateTime param_begin, DateTime param_endDate)
            {
                foreach (Credential Row in credentials)
                    if (Row.username == param_username || Row.email_address == param_emailAddress)
                        return false;

                Credential NewRow = new Credential(param_username, param_password, param_passwordSettings, param_emailAddress, param_salt, false, "", "", "", "");
                credentials.Add(NewRow);
                return true;
            }

            public static bool query_1(IEnumerable<Credential> credentials, bool param_active, string param_operation, out string username, out string emailAddress, out string salt, out string question)
            {
                foreach (Credential Row in credentials)
                    if (Row.active == param_active && Row.operation == param_operation && DateTime.UtcNow < Row.end_date)
                    {
                        username = Row.username;
                        emailAddress = Row.email_address;
                        salt = Row.salt;
                        question = Row.question;
                        return true;
                    }

                username = null;
                emailAddress = null;
                salt = null;
                question = null;
                return false;
            }

            public static bool query_3(IEnumerable<Credential> credentials, string param_emailAddress, out string question)
            {
                foreach (Credential Row in credentials)
                    if (Row.email_address == param_emailAddress && Row.active == true)
                    {
                        question = Row.question;
                        return true;
                    }

                question = null;
                return false;
            }

            public static bool query_7(IEnumerable<Credential> credentials, string param_username, string param_emailAddress, out string username, out string emailAddress)
            {
                foreach (Credential Row in credentials)
                    if (Row.username == param_username || Row.email_address == param_emailAddress)
                    {
                        username = Row.username;
                        emailAddress = Row.email_address;
                        return true;
                    }

                username = null;
                emailAddress = null;
                return false;
            }

            public static bool query_8(IEnumerable<Credential> credentials, string param_identifier, out string salt)
            {
                foreach (Credential Row in credentials)
                    if (Row.active == true && (Row.username == param_identifier || Row.email_address == param_identifier))
                    {
                        salt = Row.salt;
                        return true;
                    }

                salt = null;
                return false;
            }

            public static bool query_9(IEnumerable<Credential> credentials, string param_password, string param_passwordSettings, string param_identifier, out string username, out string emailAddress, out string question, out string name, out string loginUrl, out string meetingUrl, out string validationUrl)
            {
                foreach (Credential Row in credentials)
                    if (Row.active == true && Row.password == param_password && Row.password_settings == param_passwordSettings && (Row.username == param_identifier || Row.email_address == param_identifier))
                    {
                        username = Row.username;
                        emailAddress = Row.email_address;
                        question = Row.question;
                        name = Row.name;
                        loginUrl = Row.login_url;
                        meetingUrl = Row.meeting_url;
                        validationUrl = Row.validation_url;
                        return true;
                    }

                username = null;
                emailAddress = null;
                question = null;
                name = null;
                loginUrl = null;
                meetingUrl = null;
                validationUrl = null;
                return false;
            }

            public static bool update_1(IEnumerable<Credential> credentials, string param_username, string param_password, string param_passwordSettings, string param_newPassword, string param_newPasswordSettings)
            {
                foreach (Credential Row in credentials)
                    if (Row.username == param_username && Row.password == param_password && Row.password_settings == param_passwordSettings && Row.active == true)
                    {
                        Row.password = param_newPassword;
                        Row.password_settings = param_newPasswordSettings;
                        return true;
                    }

                return false;
            }

            public static bool update_2(IEnumerable<Credential> credentials, string param_username, string param_password, string param_passwordSettings, string param_newEmailAddress)
            {
                foreach (Credential Row in credentials)
                    if (Row.username == param_username && Row.password == param_password && Row.password_settings == param_passwordSettings && Row.active == true)
                    {
                        Row.email_address = param_newEmailAddress;
                        return true;
                    }

                return false;
            }

            public static bool update_3_1(IEnumerable<Credential> credentials, string param_username, string param_password, string param_passwordSettings, string param_newQuestion, string param_newAnswer, string param_newAnswerSettings)
            {
                foreach (Credential Row in credentials)
                    if (Row.username == param_username && Row.password == param_password && Row.password_settings == param_passwordSettings && Row.active == true)
                    {
                        Row.question = param_newQuestion;
                        Row.answer = param_newAnswer;
                        Row.answer_settings = param_newAnswerSettings;
                        return true;
                    }

                return false;
            }

            public static bool update_3_2(IEnumerable<Credential> credentials, string param_username, string param_password, string param_passwordSettings)
            {
                foreach (Credential Row in credentials)
                    if (Row.username == param_username && Row.password == param_password && Row.password_settings == param_passwordSettings && Row.active == true)
                    {
                        Row.question = null;
                        Row.answer = null;
                        Row.answer_settings = null;
                        return true;
                    }

                return false;
            }

            public static bool update_4(IEnumerable<Credential> credentials, string param_emailAddress, string param_operation, DateTime param_endDate)
            {
                foreach (Credential Row in credentials)
                    if (Row.email_address == param_emailAddress && Row.active == true && !string.IsNullOrEmpty(Row.question))
                    {
                        Row.operation = param_operation;
                        Row.end_date = param_endDate;
                        return true;
                    }

                return false;
            }

            public static bool update_5_1(IEnumerable<Credential> credentials, string param_username, string param_password, string param_passwordSettings, string param_answer, string param_answerSettings, string param_operation)
            {
                foreach (Credential Row in credentials)
                    if (Row.username == param_username && Row.password == param_password && Row.password_settings == param_passwordSettings && Row.answer == param_answer && Row.answer_settings == param_answerSettings && Row.operation == param_operation && DateTime.UtcNow < Row.end_date)
                    {
                        Row.active = true;
                        Row.operation = null;
                        Row.end_date = null;
                        return true;
                    }

                return false;
            }

            public static bool update_5_2(IEnumerable<Credential> credentials, string param_username, string param_password, string param_passwordSettings, string param_operation)
            {
                foreach (Credential Row in credentials)
                    if (Row.username == param_username && Row.password == param_password && Row.password_settings == param_passwordSettings && Row.operation == param_operation && DateTime.UtcNow < Row.end_date)
                    {
                        Row.active = true;
                        Row.operation = null;
                        Row.end_date = null;
                        return true;
                    }

                return false;
            }

            public static bool update_6(IEnumerable<Credential> credentials, string param_username, string param_answer, string param_answerSettings, string param_newPassword, string param_newPasswordSettings, string param_operation)
            {
                foreach (Credential Row in credentials)
                    if (Row.username == param_username && Row.answer == param_answer && Row.answer_settings == param_answerSettings && Row.active == true && !string.IsNullOrEmpty(Row.question) && Row.operation == param_operation && DateTime.UtcNow < Row.end_date)
                    {
                        Row.password = param_newPassword;
                        Row.password_settings = param_newPasswordSettings;
                        Row.operation = null;
                        Row.end_date = null;
                        return true;
                    }

                return false;
            }
        }

        public Login()
        {
            Username = Persistent.GetValue("username", null);
            EmailAddress = Persistent.GetValue("email_address", null);
            Question = Persistent.GetValue("question", null);
            Remember = (Persistent.GetValue("remember", null) != null);
            LoginState = (Username != null ? LoginStates.SignedIn : LoginStates.LoggedOff);

            Database.DebugLog = true;
            Database.DebugLogFullResponse = true;

            InitSimulation();
        }

        public LoginStates LoginState { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string EmailAddress { get; set; }
        public string NewEmailAddress { get; set; }
        public string Question { get; set; }
        public string NewQuestion { get; set; }
        public string Answer { get; set; }
        public string ConfirmAnswer { get; set; }
        public bool Remember { get; set; }
        public string Operation { get; set; }
        public bool HasQuestion { get { return !string.IsNullOrEmpty(Question); } }
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
        private void Encrypt(string value, byte[] salt, EncryptionUse use, out string HashString, out string HashSettings)
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
                    HashString = HashTools.GetString(Hash);
                    HashSettings = Argon2.GetSettings();
                    //string EncodedHash = Argon2.GetEncoded();
                }
            }
            else
            {
                HashString = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
                HashSettings = "Base64";
            }
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
            string NameValue = Username.Trim();
            string EmailAddressValue = EmailAddress.Trim();
            string QuestionValue = Question.Trim();

            string PasswordValue;
            string AnswerValue;

            bool IsPasswordValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.Password)}", out PasswordValue);
            bool IsAnswerValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.Answer)}", out AnswerValue);

            if (string.IsNullOrEmpty(NameValue) || !IsPasswordValid || string.IsNullOrEmpty(EmailAddressValue) || !EmailAddressValue.Contains("@"))
                destinationPageName = PageNames.register_failed_1Page;

            else if (!string.IsNullOrEmpty(QuestionValue) && !IsAnswerValid)
                destinationPageName = PageNames.register_failed_2Page;

            else if (string.IsNullOrEmpty(QuestionValue) && IsAnswerValid)
                destinationPageName = PageNames.register_failed_3Page;

            else
            {
                if (string.IsNullOrEmpty(QuestionValue) || !IsAnswerValid)
                    StartRegister(NameValue, PasswordValue, EmailAddressValue, "", "");
                else
                    StartRegister(NameValue, PasswordValue, EmailAddressValue, QuestionValue, AnswerValue);

                destinationPageName = PageNames.CurrentPage;
            }
        }

        private void StartRegister(string username, string password, string email, string question, string answer)
        {
            QueryNewCredential(username, email, (int getError, object getResult) => Register_OnNewCredentialReceived(getError, getResult, username, password, email, question, answer));
        }

        private void Register_OnNewCredentialReceived(int error, object result, string username, string password, string email, string question, string answer)
        {
            if (error == (int)ErrorCodes.Success && result != null)
            {
                Dictionary<string, string> NewCredentialResult = (Dictionary<string, string>)result;
                string SaltString = NewCredentialResult["salt"];
                SaltString = MixedSalt(SaltString);

                byte[] NewSalt;
                if (HashTools.TryParse(SaltString, out NewSalt))
                {
                    string EncryptedPassword;
                    string EncryptedPasswordSettings;
                    Encrypt(password, NewSalt, EncryptionUse.Password, out EncryptedPassword, out EncryptedPasswordSettings);

                    string EncryptedAnswer;
                    string EncryptedAnswerSettings;
                    if (!string.IsNullOrEmpty(answer))
                        Encrypt(answer, NewSalt, EncryptionUse.SecretAnswer, out EncryptedAnswer, out EncryptedAnswerSettings);
                    else
                    {
                        EncryptedAnswer = "";
                        EncryptedAnswerSettings = "";
                    }

                    RegisterAndSendEmail(username, EncryptedPassword, EncryptedPasswordSettings, email, question, EncryptedAnswer, EncryptedAnswerSettings, SaltString.ToLower(), Register_OnEmailSent);
                }
                else
                    (App.Current as App).GoTo(PageNames.register_failed_4Page);
            }
            else if (error == (int)ErrorCodes.UsernameAlreadyUsed)
                (App.Current as App).GoTo(PageNames.register_failed_5Page);
            else if (error == (int)ErrorCodes.EmailAddressAlreadyUsed)
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
                string QueryEmailAddress = QueryString.ContainsKey("email_address") ? QueryString["email_address"] : null;
                string SaltString = QueryString.ContainsKey("salt") ? QueryString["salt"] : null;
                string QueryQuestion = QueryString.ContainsKey("question") ? QueryString["question"] : null;
                string QueryOperation = QueryString.ContainsKey("operation") ? QueryString["operation"] : null;

                byte[] QuerySalt;
                if (!string.IsNullOrEmpty(QueryName) && !string.IsNullOrEmpty(QueryEmailAddress) && HashTools.TryParse(SaltString, out QuerySalt))
                {
                    Username = QueryName;
                    EmailAddress = QueryEmailAddress;
                    Salt = QuerySalt;
                    Question = QueryQuestion;
                    Operation = QueryOperation;
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
                Persistent.SetValue("username", null);
                Persistent.SetValue("email_address", null);
                Persistent.SetValue("salt", null);
                Persistent.SetValue("question", null);
                Persistent.SetValue("remember", null);
            }

            string PasswordValue;
            string AnswerValue;

            bool IsPasswordValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.Password)}", out PasswordValue);
            bool IsAnswerValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.Answer)}", out AnswerValue);

            if (!IsPasswordValid)
                destinationPageName = PageNames.registration_end_failed_1Page;

            else if (HasQuestion && !IsAnswerValid)
                destinationPageName = PageNames.registration_end_failed_2Page;

            else
            {
                string EncryptedPassword;
                string EncryptedPasswordSettings;
                Encrypt(PasswordValue, Salt, EncryptionUse.Password, out EncryptedPassword, out EncryptedPasswordSettings);

                string EncryptedAnswer;
                string EncryptedAnswerSettings;
                if (HasQuestion)
                    Encrypt(AnswerValue, Salt, EncryptionUse.SecretAnswer, out EncryptedAnswer, out EncryptedAnswerSettings);
                else
                {
                    EncryptedAnswer = "";
                    EncryptedAnswerSettings = "";
                }

                ActivateAccountAndSendEmail(Username, EmailAddress, EncryptedPassword, EncryptedPasswordSettings, EncryptedAnswer, EncryptedAnswerSettings, Operation, (int checkError, object checkResult) => CompleteRegistration_AccountActivated(checkError, checkResult, Remember));

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
                    Persistent.SetValue("username", Username);
                    Persistent.SetValue("email_address", EmailAddress);
                    Persistent.SetValue("salt", HashTools.GetString(Salt));
                    Persistent.SetValue("question", Question);
                    Persistent.SetValue("remember", "1");
                }

                LoginState = LoginStates.SignedIn;

                NotifyPropertyChanged(nameof(EmailAddress));
                NotifyPropertyChanged(nameof(Question));
                NotifyPropertyChanged(nameof(LoginState));

                (App.Current as App).GoTo(PageNames.registration_completePage);
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
                Persistent.SetValue("username", null);
                Persistent.SetValue("email_address", null);
                Persistent.SetValue("salt", null);
                Persistent.SetValue("question", null);
                Persistent.SetValue("remember", null);
            }

            string NameValue = Username.Trim();

            string PasswordValue;
            if (!GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.Password)}", out PasswordValue))
                destinationPageName = PageNames.login_failedPage;

            else if (string.IsNullOrEmpty(NameValue))
                destinationPageName = PageNames.login_failedPage;

            else
            {
                StartLogin(NameValue, PasswordValue, Remember);
                destinationPageName = PageNames.CurrentPage;
            }
        }

        private void StartLogin(string identifier, string currentPassword, bool remember)
        {
            GetUserSalt(identifier, (int getError, object getResult) => Login_OnSaltReceived(getError, getResult, identifier, currentPassword, remember));
        }

        private void Login_OnSaltReceived(int error, object result, string identifier, string currentPassword, bool remember)
        {
            if (error == (int)ErrorCodes.Success && result != null)
            {
                Dictionary<string, string> GetCredentialResult = (Dictionary<string, string>)result;
                string SaltString = GetCredentialResult["salt"];

                byte[] TestSalt;
                if (HashTools.TryParse(SaltString, out TestSalt))
                {
                    string EncryptedCurrentPassword;
                    string EncryptedCurrentPasswordSettings;
                    Encrypt(currentPassword, TestSalt, EncryptionUse.Password, out EncryptedCurrentPassword, out EncryptedCurrentPasswordSettings);
                    SignIn(identifier, EncryptedCurrentPassword, EncryptedCurrentPasswordSettings, (int signInError, object signInResult) => Login_OnSignIn(signInError, signInResult, TestSalt, remember));
                }
                else
                    (App.Current as App).GoTo(PageNames.login_failedPage);
            }
            else
                (App.Current as App).GoTo(PageNames.login_failedPage);
        }

        private void Login_OnSignIn(int error, object result, byte[] TestSalt, bool remember)
        {
            if (error == (int)ErrorCodes.Success && result != null)
            {
                Dictionary<string, string> SignInResult = (Dictionary<string, string>)result;
                Username = SignInResult["username"];
                EmailAddress = SignInResult["email_address"];
                Salt = TestSalt;
                Question = SignInResult["question"];

                if (remember)
                {
                    Persistent.SetValue("username", Username);
                    Persistent.SetValue("email_address", EmailAddress);
                    Persistent.SetValue("salt", HashTools.GetString(Salt));
                    Persistent.SetValue("question", Question);
                    Persistent.SetValue("remember", "1");
                }

                LoginState = LoginStates.SignedIn;

                NotifyPropertyChanged(nameof(EmailAddress));
                NotifyPropertyChanged(nameof(Question));
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
            Username = null;
            EmailAddress = null;
            Salt = null;
            Question = null;
            ((Eqmlp)GetEqmlp).Logout();

            Persistent.SetValue("username", null);
            Persistent.SetValue("email_address", null);
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

            else if (NewPasswordValue == PasswordValue)
                destinationPageName = PageNames.change_password_failed_6Page;

            else if (NewPasswordValue != ConfirmPasswordValue)
                destinationPageName = PageNames.change_password_failed_3Page;

            else
            {
                string EncryptedCurrentPassword;
                string EncryptedCurrentPasswordSettings;
                Encrypt(PasswordValue, Salt, EncryptionUse.Password, out EncryptedCurrentPassword, out EncryptedCurrentPasswordSettings);

                string EncryptedNewPassword;
                string EncryptedNewPasswordSettings;
                Encrypt(NewPasswordValue, Salt, EncryptionUse.Password, out EncryptedNewPassword, out EncryptedNewPasswordSettings);
                ChangePassword(Username, EncryptedCurrentPassword, EncryptedCurrentPasswordSettings, EncryptedNewPassword, EncryptedNewPasswordSettings, ChangePassword_OnPasswordChanged);

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

        #region Change Email Address
        public void On_ChangeEmailAddress(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            string EmailAddressValue = NewEmailAddress.Trim();

            string PasswordValue;
            if (!GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.Password)}", out PasswordValue))
                destinationPageName = PageNames.change_email_failed_1Page;

            else if (string.IsNullOrEmpty(EmailAddressValue))
                destinationPageName = PageNames.change_email_failed_2Page;

            else if (!EmailAddressValue.Contains("@"))
                destinationPageName = PageNames.change_email_failed_3Page;

            else
            {
                string EncryptedPassword;
                string EncryptedPasswordSettings;
                Encrypt(PasswordValue, Salt, EncryptionUse.Password, out EncryptedPassword, out EncryptedPasswordSettings);
                ChangeEmailAddress(Username, EncryptedPassword, EncryptedPasswordSettings, EmailAddressValue, (int changeError, object changeResult) => ChangeEmail_OnEmailAddressChanged(changeError, changeResult, EmailAddressValue));

                destinationPageName = PageNames.CurrentPage;
            }
        }

        private void ChangeEmail_OnEmailAddressChanged(int error, object result, string newEmailAddress)
        {
            if (error == (int)ErrorCodes.Success)
            {
                EmailAddress = newEmailAddress;
                NewEmailAddress = null;
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
            string QuestionValue = NewQuestion.Trim();

            string PasswordValue;
            string AnswerValue;
            string ConfirmAnswerValue;

            bool IsPasswordValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.Password)}", out PasswordValue);
            bool IsAnswerValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.Answer)}", out AnswerValue);
            bool IsConfirmAnswerValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.ConfirmAnswer)}", out ConfirmAnswerValue);

            if (!IsPasswordValid)
                destinationPageName = PageNames.change_recovery_failed_1Page;

            else if (string.IsNullOrEmpty(QuestionValue) && !IsAnswerValid && !IsConfirmAnswerValid)
            {
                string EncryptedPassword;
                string EncryptedPasswordSettings;
                Encrypt(PasswordValue, Salt, EncryptionUse.Password, out EncryptedPassword, out EncryptedPasswordSettings);
                ChangeRecovery(Username, EncryptedPassword, EncryptedPasswordSettings, "", "", "", (int changeError, object changeResult) => ChangeRecovery_OnRecoveryChanged(changeError, changeResult, QuestionValue));
                destinationPageName = PageNames.CurrentPage;
            }

            else if (string.IsNullOrEmpty(QuestionValue) || !IsAnswerValid || !IsConfirmAnswerValid)
                destinationPageName = PageNames.change_recovery_failed_2Page;

            else if (AnswerValue != ConfirmAnswerValue)
                destinationPageName = PageNames.change_recovery_failed_3Page;

            else
            {
                string EncryptedPassword;
                string EncryptedPasswordSettings;
                Encrypt(PasswordValue, Salt, EncryptionUse.Password, out EncryptedPassword, out EncryptedPasswordSettings);

                string EncryptedNewAnswer;
                string EncryptedNewAnswerSettings;
                Encrypt(AnswerValue, Salt, EncryptionUse.SecretAnswer, out EncryptedNewAnswer, out EncryptedNewAnswerSettings);
                ChangeRecovery(Username, EncryptedPassword, EncryptedPasswordSettings, QuestionValue, EncryptedNewAnswer, EncryptedNewAnswerSettings, (int changeError, object changeResult) => ChangeRecovery_OnRecoveryChanged(changeError, changeResult, QuestionValue));
                destinationPageName = PageNames.CurrentPage;
            }
        }

        private void ChangeRecovery_OnRecoveryChanged(int error, object result, string newQuestion)
        {
            if (error == (int)ErrorCodes.Success)
            {
                Question = newQuestion;
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
            string EmailAddressValue = EmailAddress.Trim();

            if (string.IsNullOrEmpty(EmailAddressValue))
                destinationPageName = PageNames.recovery_failed_1Page;

            else
            {
                StartRecovery(EmailAddressValue);
                destinationPageName = PageNames.CurrentPage;
            }
        }

        private void StartRecovery(string email)
        {
            CheckIfEmailAddressValid(email, (int checkError, object checkResult) => Recovery_OnEmailAddressChecked(checkError, checkResult, email));
        }

        private void Recovery_OnEmailAddressChecked(int error, object result, string email)
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
                string QueryEmailAddress = QueryString.ContainsKey("email_address") ? QueryString["email_address"] : null;
                string SaltString = QueryString.ContainsKey("salt") ? QueryString["salt"] : null;
                string QueryQuestion = QueryString.ContainsKey("question") ? QueryString["question"] : null;
                string QueryOperation = QueryString.ContainsKey("operation") ? QueryString["operation"] : null;

                byte[] QuerySalt;
                if (!string.IsNullOrEmpty(QueryName) && !string.IsNullOrEmpty(QueryEmailAddress) && !string.IsNullOrEmpty(QueryQuestion) && HashTools.TryParse(SaltString, out QuerySalt))
                {
                    Username = QueryName;
                    EmailAddress = QueryEmailAddress;
                    Question = QueryQuestion;
                    Salt = QuerySalt;
                    Operation = QueryOperation;
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

            bool IsAnswerValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.Answer)}", out AnswerValue);
            bool IsNewPasswordValid = GetApp.GetPasswordValue($"{nameof(Login)}.{nameof(Login.NewPassword)}", out NewPasswordValue);

            if (!IsAnswerValid)
                destinationPageName = PageNames.recovery_end_failed_1Page;

            else if (!IsNewPasswordValid)
                destinationPageName = PageNames.recovery_end_failed_2Page;

            else
            {
                string EncryptedAnswer;
                string EncryptedAnswerSettings;
                Encrypt(AnswerValue, Salt, EncryptionUse.SecretAnswer, out EncryptedAnswer, out EncryptedAnswerSettings);

                string EncryptedNewPassword;
                string EncryptedNewPasswordSettings;
                Encrypt(NewPasswordValue, Salt, EncryptionUse.Password, out EncryptedNewPassword, out EncryptedNewPasswordSettings);

                RecoverAccount(Username, EncryptedAnswer, EncryptedAnswerSettings, EncryptedNewPassword, EncryptedNewPasswordSettings, Operation, CompleteRecovery_OnGetUserInfo);
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

                (App.Current as App).GoTo(PageNames.recovery_completePage);
            }
            else if (error == (int)ErrorCodes.InvalidUsernameOrAnswer)
                (App.Current as App).GoTo(PageNames.recovery_end_failed_4Page);
            else
                (App.Current as App).GoTo(PageNames.recovery_end_failed_3Page);
        }
        #endregion

        #region Operations
        private void QueryNewCredential(string username, string email, Action<int, object> callback)
        {
            Database.Completed += OnQueryNewCredentialCompleted;
            Database.Query(new DatabaseQueryOperation("new credential", "query_7.php", new Dictionary<string, string>() { { "username", HtmlString.PercentEncoded(username) }, { "email_address", HtmlString.PercentEncoded(email) } }, callback));
        }

        private void OnQueryNewCredentialCompleted(object sender, CompletionEventArgs e)
        {
            Database.Completed -= OnQueryNewCredentialCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result", "salt" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.OperationFailed, null));
        }

        private void ChangePassword(string username, string encryptedCurrentPassword, string encryptedCurrentPasswordSettings, string encryptedNewPassword, string encryptedNewPasswordSettings, Action<int, object> callback)
        {
            Database.Completed += OnChangePasswordCompleted;
            Database.Update(new DatabaseUpdateOperation("change password", "update_1.php", new Dictionary<string, string>() { { "username", HtmlString.PercentEncoded(username) }, { "password", encryptedCurrentPassword }, { "password_settings", HtmlString.PercentEncoded(encryptedCurrentPasswordSettings) }, { "new_password", encryptedNewPassword }, { "new_password_settings", HtmlString.PercentEncoded(encryptedNewPasswordSettings) } }, callback));
        }

        private void OnChangePasswordCompleted(object sender, CompletionEventArgs e)
        {
            Database.Completed -= OnChangePasswordCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(-1, null));
        }

        private void ChangeEmailAddress(string username, string encryptedPassword, string encryptedPasswordSettings, string newEmailAddress, Action<int, object> callback)
        {
            Database.Completed += OnChangeEmailAddressCompleted;
            Database.Update(new DatabaseUpdateOperation("change email", "update_2.php", new Dictionary<string, string>() { { "username", HtmlString.PercentEncoded(username) }, { "password", encryptedPassword }, { "password_settings", HtmlString.PercentEncoded(encryptedPasswordSettings) }, { "new_email_address", HtmlString.PercentEncoded(newEmailAddress) } }, callback));
        }

        private void OnChangeEmailAddressCompleted(object sender, CompletionEventArgs e)
        {
            Database.Completed -= OnChangeEmailAddressCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private void ChangeRecovery(string username, string encryptedPassword, string encryptedPasswordSettings, string newQuestion, string encryptedNewAnswer, string encryptedNewAnswerSettings, Action<int, object> callback)
        {
            Database.Completed += OnChangeRecoveryCompleted;
            Database.Update(new DatabaseUpdateOperation("change recovery", "update_3.php", new Dictionary<string, string>() { { "username", HtmlString.PercentEncoded(username) }, { "password", encryptedPassword }, { "password_settings", HtmlString.PercentEncoded(encryptedPasswordSettings) }, { "new_question", HtmlString.PercentEncoded(newQuestion) }, { "new_answer", encryptedNewAnswer }, { "new_answer_settings", HtmlString.PercentEncoded(encryptedNewAnswerSettings) } }, callback));
        }

        private void OnChangeRecoveryCompleted(object sender, CompletionEventArgs e)
        {
            Database.Completed -= OnChangeRecoveryCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private void CheckIfEmailAddressValid(string email, Action<int, object> callback)
        {
            Database.Completed += OnIsEmailAddressValidityChecked;
            Database.Query(new DatabaseQueryOperation("check email", "query_3.php", new Dictionary<string, string>() { { "email_address", HtmlString.PercentEncoded(email) } }, callback));
        }

        private void OnIsEmailAddressValidityChecked(object sender, CompletionEventArgs e)
        {
            Database.Completed -= OnIsEmailAddressValidityChecked;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null && Result.ContainsKey("result"))
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private void RegisterAndSendEmail(string username, string encryptedPassword, string encryptedPasswordSettings, string email, string question, string encryptedAnswer, string encryptedAnswerSettings, string salt, Action<int, object> callback)
        {
            Database.Completed += OnRegisterSendEmailCompleted;
            Database.Update(new DatabaseUpdateOperation("start register", "insert_1.php", new Dictionary<string, string>() { { "username", HtmlString.PercentEncoded(username) }, { "password", encryptedPassword }, { "password_settings", HtmlString.PercentEncoded(encryptedPasswordSettings) }, { "email_address", HtmlString.PercentEncoded(email) }, { "question", HtmlString.PercentEncoded(question) }, { "answer", encryptedAnswer }, { "answer_settings", HtmlString.PercentEncoded(encryptedAnswerSettings) }, { "salt", salt }, { "language", ((int)GetLanguage.LanguageState).ToString() } }, callback));
        }

        private void OnRegisterSendEmailCompleted(object sender, CompletionEventArgs e)
        {
            Database.Completed -= OnRegisterSendEmailCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private void BeginRecoveryAndSendEmail(string email, Action<int, object> callback)
        {
            Database.Completed += OnRecoverySendEmailCompleted;
            Database.Update(new DatabaseUpdateOperation("start recovery", "update_4.php", new Dictionary<string, string>() { { "email_address", HtmlString.PercentEncoded(email) }, { "language", ((int)GetLanguage.LanguageState).ToString() } }, callback));
        }

        private void OnRecoverySendEmailCompleted(object sender, CompletionEventArgs e)
        {
            Database.Completed -= OnRecoverySendEmailCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private void ActivateAccountAndSendEmail(string username, string email, string encryptedPassword, string encryptedPasswordSettings, string encryptedAnswer, string encryptedAnswerSettings, string operation, Action<int, object> callback)
        {
            Database.Completed += OnActivateAccountCompleted;
            Database.Update(new DatabaseUpdateOperation("activate account", "update_5.php", new Dictionary<string, string>() { { "username", HtmlString.PercentEncoded(username) }, { "email_address", HtmlString.PercentEncoded(email) }, { "password", encryptedPassword }, { "password_settings", HtmlString.PercentEncoded(encryptedPasswordSettings) }, { "answer", encryptedAnswer }, { "answer_settings", HtmlString.PercentEncoded(encryptedAnswerSettings) }, { "operation", operation }, { "language", ((int)GetLanguage.LanguageState).ToString() } }, callback));
        }

        private void OnActivateAccountCompleted(object sender, CompletionEventArgs e)
        {
            Database.Completed -= OnActivateAccountCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private void RecoverAccount(string username, string encryptedAnswer, string encryptedAnswerSettings, string encryptedNewPassword, string encryptedNewPasswordSettings, string operation, Action<int, object> callback)
        {
            Database.Completed += OnAccountRecoveryCompleted;
            Database.Update(new DatabaseUpdateOperation("recover account", "update_6.php", new Dictionary<string, string>() { { "username", HtmlString.PercentEncoded(username) }, { "answer", encryptedAnswer }, { "answer_settings", HtmlString.PercentEncoded(encryptedAnswerSettings) }, { "new_password", encryptedNewPassword }, { "new_password_settings", HtmlString.PercentEncoded(encryptedNewPasswordSettings) }, { "operation", operation } }, callback));
        }

        private void OnAccountRecoveryCompleted(object sender, CompletionEventArgs e)
        {
            Database.Completed -= OnAccountRecoveryCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private void GetUserSalt(string identifier, Action<int, object> callback)
        {
            Database.Completed += OnGetUserSaltCompleted;
            Database.Query(new DatabaseQueryOperation("get user salt", "query_8.php", new Dictionary<string, string>() { { "identifier", HtmlString.PercentEncoded(identifier) } }, callback));
        }

        private void OnGetUserSaltCompleted(object sender, CompletionEventArgs e)
        {
            Database.Completed -= OnGetUserSaltCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "result", "salt" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.OperationFailed, null));
        }

        private void SignIn(string identifier, string encryptedPassword, string encryptedPasswordSettings, Action<int, object> callback)
        {
            Database.Completed += OnSignInCompleted;
            Database.Query(new DatabaseQueryOperation("sign in", "query_9.php", new Dictionary<string, string>() { { "identifier", HtmlString.PercentEncoded(identifier) }, { "password", encryptedPassword }, { "password_settings", HtmlString.PercentEncoded(encryptedPasswordSettings) } }, callback));
        }

        private void OnSignInCompleted(object sender, CompletionEventArgs e)
        {
            Database.Completed -= OnSignInCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            Dictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "username", "email_address", "question", "name", "login_url", "meeting_url", "validation_url", "result" })) != null && Result.ContainsKey("result"))
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
            OperationHandler.Add(new OperationHandler("/request/update_2.php", OnChangeEmailAddressRequest));
            OperationHandler.Add(new OperationHandler("/request/update_3.php", OnChangeRecoveryRequest));
            OperationHandler.Add(new OperationHandler("/request/query_3.php", OnCheckEmailAddressValidityRequest));
            OperationHandler.Add(new OperationHandler("/request/insert_1.php", OnSignUpRequest));
            OperationHandler.Add(new OperationHandler("/request/update_5.php", OnCompleteSignUpRequest));
            OperationHandler.Add(new OperationHandler("/request/update_4.php", OnRecoveryRequest));
            OperationHandler.Add(new OperationHandler("/request/update_6.php", OnCompleteRecoveryRequest));
            OperationHandler.Add(new OperationHandler("/request/query_7.php", OnQueryNewCredentialRequest));
            OperationHandler.Add(new OperationHandler("/request/query_8.php", OnQuerySaltRequest));
            OperationHandler.Add(new OperationHandler("/request/query_9.php", OnSignInRequest));
        }

        private List<Dictionary<string, string>> OnChangePasswordRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string QueryUsername;
            if (parameters.ContainsKey("username"))
                QueryUsername = parameters["username"];
            else
                QueryUsername = null;

            string EncryptedPassword;
            if (parameters.ContainsKey("password"))
                EncryptedPassword = parameters["password"];
            else
                EncryptedPassword = null;

            string PasswordSettings;
            if (parameters.ContainsKey("password_settings"))
                PasswordSettings = parameters["password_settings"];
            else
                PasswordSettings = null;

            string EncryptedNewPassword;
            if (parameters.ContainsKey("new_password"))
                EncryptedNewPassword = parameters["new_password"];
            else
                EncryptedNewPassword = null;

            string NewPasswordSettings;
            if (parameters.ContainsKey("new_password_settings"))
                NewPasswordSettings = parameters["new_password_settings"];
            else
                NewPasswordSettings = null;

            if (string.IsNullOrEmpty(QueryUsername) || string.IsNullOrEmpty(EncryptedPassword) || PasswordSettings == null || string.IsNullOrEmpty(EncryptedNewPassword) || NewPasswordSettings == null)
                return Result;

            ErrorCodes ErrorCode;
            if (Credential.update_1(KnownUserTable, QueryUsername, EncryptedPassword, PasswordSettings, EncryptedNewPassword, NewPasswordSettings))
                ErrorCode = ErrorCodes.Success;
            else
                ErrorCode = ErrorCodes.ErrorNotFound;

            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)ErrorCode).ToString()},
            });

            return Result;
        }

        private List<Dictionary<string, string>> OnChangeEmailAddressRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string QueryUsername;
            if (parameters.ContainsKey("username"))
                QueryUsername = parameters["username"];
            else
                QueryUsername = null;

            string EncryptedPassword;
            if (parameters.ContainsKey("password"))
                EncryptedPassword = parameters["password"];
            else
                EncryptedPassword = null;

            string PasswordSettings;
            if (parameters.ContainsKey("password_settings"))
                PasswordSettings = parameters["password_settings"];
            else
                PasswordSettings = null;

            string QueryNewEmailAddress;
            if (parameters.ContainsKey("new_email_address"))
                QueryNewEmailAddress = parameters["new_email_address"];
            else
                QueryNewEmailAddress = null;

            if (string.IsNullOrEmpty(QueryUsername) || string.IsNullOrEmpty(EncryptedPassword) || PasswordSettings == null || string.IsNullOrEmpty(QueryNewEmailAddress))
                return Result;

            ErrorCodes ErrorCode;
            if (Credential.update_2(KnownUserTable, QueryUsername, EncryptedPassword, PasswordSettings, QueryNewEmailAddress))
                ErrorCode = ErrorCodes.Success;
            else
                ErrorCode = ErrorCodes.ErrorNotFound;

            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)ErrorCode).ToString()},
            });
            return Result;
        }

        private List<Dictionary<string, string>> OnChangeRecoveryRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string QueryUsername;
            if (parameters.ContainsKey("username"))
                QueryUsername = parameters["username"];
            else
                QueryUsername = null;

            string EncryptedPassword;
            if (parameters.ContainsKey("password"))
                EncryptedPassword = parameters["password"];
            else
                EncryptedPassword = null;

            string PasswordSettings;
            if (parameters.ContainsKey("password_settings"))
                PasswordSettings = parameters["password_settings"];
            else
                PasswordSettings = null;

            string QueryNewQuestion;
            if (parameters.ContainsKey("new_question"))
                QueryNewQuestion = parameters["new_question"];
            else
                QueryNewQuestion = null;

            string EncryptedNewAnswer;
            if (parameters.ContainsKey("new_answer"))
                EncryptedNewAnswer = parameters["new_answer"];
            else
                EncryptedNewAnswer = null;

            string NewAnswerSettings;
            if (parameters.ContainsKey("new_answer_settings"))
                NewAnswerSettings = parameters["new_answer_settings"];
            else
                NewAnswerSettings = null;

            if (string.IsNullOrEmpty(QueryUsername) || string.IsNullOrEmpty(EncryptedPassword) || PasswordSettings == null || QueryNewQuestion == null || EncryptedNewAnswer == null || NewAnswerSettings == null)
                return Result;

            if ((QueryNewQuestion.Length == 0 && EncryptedNewAnswer.Length > 0) || (QueryNewQuestion.Length > 0 && EncryptedNewAnswer.Length == 0))
                return Result;

            ErrorCodes ErrorCode;
            if (QueryNewQuestion.Length > 0 && EncryptedNewAnswer.Length > 0)
            {
                if (Credential.update_3_1(KnownUserTable, QueryUsername, EncryptedPassword, PasswordSettings, QueryNewQuestion, EncryptedNewAnswer, NewAnswerSettings))
                    ErrorCode = ErrorCodes.Success;
                else
                    ErrorCode = ErrorCodes.ErrorNotFound;
            }
            else
            {
                if (Credential.update_3_2(KnownUserTable, QueryUsername, EncryptedPassword, PasswordSettings))
                    ErrorCode = ErrorCodes.Success;
                else
                    ErrorCode = ErrorCodes.ErrorNotFound;
            }

            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)ErrorCode).ToString()},
            });
            return Result;
        }

        private List<Dictionary<string, string>> OnCheckEmailAddressValidityRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string QueryEmailAddress;
            if (parameters.ContainsKey("email_address"))
                QueryEmailAddress = parameters["email_address"];
            else
                QueryEmailAddress = null;

            if (string.IsNullOrEmpty(QueryEmailAddress))
                return Result;

            string ResultQuestion;
            ErrorCodes ErrorCode;
            if (Credential.query_3(KnownUserTable, QueryEmailAddress, out ResultQuestion) && !string.IsNullOrEmpty(ResultQuestion))
                ErrorCode = ErrorCodes.Success;
            else
                ErrorCode = ErrorCodes.ErrorNoQuestion;

            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)ErrorCode).ToString() },
            });
            return Result;
        }

        private List<Dictionary<string, string>> OnSignUpRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string QueryUsername;
            if (parameters.ContainsKey("username"))
                QueryUsername = parameters["username"];
            else
                QueryUsername = null;

            string EncryptedPassword;
            if (parameters.ContainsKey("password"))
                EncryptedPassword = parameters["password"];
            else
                EncryptedPassword = null;

            string PasswordSettings;
            if (parameters.ContainsKey("password_settings"))
                PasswordSettings = parameters["password_settings"];
            else
                PasswordSettings = null;

            string QueryEmailAddress;
            if (parameters.ContainsKey("email_address"))
                QueryEmailAddress = parameters["email_address"];
            else
                QueryEmailAddress = null;

            string QueryQuestion;
            if (parameters.ContainsKey("question"))
                QueryQuestion = parameters["question"];
            else
                QueryQuestion = null;

            string EncryptedAnswer;
            if (parameters.ContainsKey("answer"))
                EncryptedAnswer = parameters["answer"];
            else
                EncryptedAnswer = null;

            string AnswerSettings;
            if (parameters.ContainsKey("answer_settings"))
                AnswerSettings = parameters["answer_settings"];
            else
                AnswerSettings = null;

            string QuerySalt;
            if (parameters.ContainsKey("salt"))
                QuerySalt = parameters["salt"];
            else
                QuerySalt = null;

            string QueryLanguage;
            if (parameters.ContainsKey("language"))
                QueryLanguage = parameters["language"];
            else
                QueryLanguage = "0";

            if (string.IsNullOrEmpty(QueryUsername) || string.IsNullOrEmpty(EncryptedPassword) || string.IsNullOrEmpty(QueryEmailAddress) || QueryQuestion == null || EncryptedAnswer == null || string.IsNullOrEmpty(QuerySalt))
                return Result;

            ErrorCodes ErrorCode;
            string RegisterOperation = CreateOperation();

            if (QueryQuestion.Length > 0 && EncryptedAnswer.Length > 0)
            {
                if (Credential.insert_1_1(KnownUserTable, QueryUsername, EncryptedPassword, PasswordSettings, QueryEmailAddress, QuerySalt, QueryQuestion, EncryptedAnswer, AnswerSettings, RegisterOperation, DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromDays(1)))
                    ErrorCode = ErrorCodes.Success;
                else
                    ErrorCode = ErrorCodes.OperationFailed;
            }
            else
            {
                if (Credential.insert_1_2(KnownUserTable, QueryUsername, EncryptedPassword, PasswordSettings, QueryEmailAddress, QuerySalt, RegisterOperation, DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromDays(1)))
                    ErrorCode = ErrorCodes.Success;
                else
                    ErrorCode = ErrorCodes.OperationFailed;
            }

            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)ErrorCode).ToString()},
            });

            if (ErrorCode == ErrorCodes.Success)
            {
                DispatcherTimer SignUpConfirmedTimer = new DispatcherTimer();
                SignUpConfirmedTimer.Interval = TimeSpan.FromSeconds(3);
                SignUpConfirmedTimer.Tick += (object sender, object e) => OnSignUpConfirmed(sender, e, RegisterOperation);
                SignUpConfirmedTimer.Start();
            }

            return Result;
        }

        private void OnSignUpConfirmed(object sender, object e, string operation)
        {
            DispatcherTimer SignUpConfirmedTimer = (DispatcherTimer)sender;
            SignUpConfirmedTimer.Stop();

            string ResultUsername;
            string ResultEmailAddress;
            string ResultSalt;
            string ResultQuestion;
            if (Credential.query_1(KnownUserTable, false, operation, out ResultUsername, out ResultEmailAddress, out ResultSalt, out ResultQuestion))
            {
                if (MessageBox.Show("Continue registration?", "Email sent", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Dictionary<string, string> QueryString = App.QueryString;
                    QueryString.Clear();
                    QueryString.Add("type", "register");
                    QueryString.Add("username", ResultUsername);
                    QueryString.Add("email_address", ResultEmailAddress);
                    QueryString.Add("question", ResultQuestion);
                    QueryString.Add("operation", operation);

                    PageNames NextPageName;
                    On_RegisterEnd(PageNames.homePage, null, null, out NextPageName);
                    (App.Current as App).GoTo(NextPageName);
                }
            }
        }

        private List<Dictionary<string, string>> OnCompleteSignUpRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string QueryUsername;
            if (parameters.ContainsKey("username"))
                QueryUsername = parameters["username"];
            else
                QueryUsername = null;

            string QueryEmailAddress;
            if (parameters.ContainsKey("email_address"))
                QueryEmailAddress = parameters["email_address"];
            else
                QueryEmailAddress = null;

            string EncryptedPassword;
            if (parameters.ContainsKey("password"))
                EncryptedPassword = parameters["password"];
            else
                EncryptedPassword = null;

            string PasswordSettings;
            if (parameters.ContainsKey("password_settings"))
                PasswordSettings = parameters["password_settings"];
            else
                PasswordSettings = null;

            string EncryptedAnswer;
            if (parameters.ContainsKey("answer"))
                EncryptedAnswer = parameters["answer"];
            else
                EncryptedAnswer = null;

            string AnswerSettings;
            if (parameters.ContainsKey("answer_settings"))
                AnswerSettings = parameters["answer_settings"];
            else
                AnswerSettings = null;

            string QueryOperation;
            if (parameters.ContainsKey("operation"))
                QueryOperation = parameters["operation"];
            else
                QueryOperation = null;

            string QueryLanguage;
            if (parameters.ContainsKey("language"))
                QueryLanguage = parameters["language"];
            else
                QueryLanguage = "0";

            if (string.IsNullOrEmpty(QueryUsername) || string.IsNullOrEmpty(QueryEmailAddress) || string.IsNullOrEmpty(EncryptedPassword) || EncryptedAnswer == null || string.IsNullOrEmpty(QueryOperation))
                return Result;

            ErrorCodes ErrorCode;
            if (EncryptedAnswer.Length > 0)
            {
                if (Credential.update_5_1(KnownUserTable, QueryUsername, EncryptedPassword, PasswordSettings, EncryptedAnswer, AnswerSettings, QueryOperation))
                    ErrorCode = ErrorCodes.Success;
                else
                    ErrorCode = ErrorCodes.InvalidUsernamePasswordOrAnswer;
            }
            else
            {
                if (Credential.update_5_2(KnownUserTable, QueryUsername, EncryptedPassword, PasswordSettings, QueryOperation))
                    ErrorCode = ErrorCodes.Success;
                else
                    ErrorCode = ErrorCodes.InvalidUsernameOrPassword;
            }

            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)ErrorCode).ToString()},
            });

            return Result;
        }

        private List<Dictionary<string, string>> OnRecoveryRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string QueryEmailAddress;
            if (parameters.ContainsKey("email_address"))
                QueryEmailAddress = parameters["email_address"];
            else
                QueryEmailAddress = null;

            string QueryLanguage;
            if (parameters.ContainsKey("language"))
                QueryLanguage = parameters["language"];
            else
                QueryLanguage = "0";

            if (string.IsNullOrEmpty(QueryEmailAddress))
                return Result;

            ErrorCodes ErrorCode;
            string RecoveryOperation = CreateOperation();

            if (Credential.update_4(KnownUserTable, QueryEmailAddress, RecoveryOperation, DateTime.UtcNow))
                ErrorCode = ErrorCodes.Success;
            else
                ErrorCode = ErrorCodes.ErrorNotFound;

            if (ErrorCode == ErrorCodes.Success)
            {
                DispatcherTimer RecoveryConfirmedTimer = new DispatcherTimer();
                RecoveryConfirmedTimer.Interval = TimeSpan.FromSeconds(3);
                RecoveryConfirmedTimer.Tick += (object sender, object e) => OnRecoveryConfirmed(sender, e, RecoveryOperation);
                RecoveryConfirmedTimer.Start();
            }

            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)ErrorCode).ToString()},
            });

            return Result;
        }

        private void OnRecoveryConfirmed(object sender, object e, string operation)
        {
            DispatcherTimer SignUpConfirmedTimer = (DispatcherTimer)sender;
            SignUpConfirmedTimer.Stop();

            string ResultUsername;
            string ResultEmailAddress;
            string ResultSalt;
            string ResultQuestion;
            if (Credential.query_1(KnownUserTable, false, operation, out ResultUsername, out ResultEmailAddress, out ResultSalt, out ResultQuestion))
            {
                if (MessageBox.Show("Continue recovery?", "Email sent", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Dictionary<string, string> QueryString = App.QueryString;
                    QueryString.Clear();
                    QueryString.Add("type", "recovery");
                    QueryString.Add("username", ResultUsername);
                    QueryString.Add("email_address", ResultEmailAddress);
                    QueryString.Add("question", ResultQuestion);
                    QueryString.Add("operation", operation);

                    PageNames NextPageName;
                    On_RecoveryEnd(PageNames.homePage, null, null, out NextPageName);
                    (App.Current as App).GoTo(NextPageName);
                }
            }
        }

        private List<Dictionary<string, string>> OnCompleteRecoveryRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string QueryUsername;
            if (parameters.ContainsKey("username"))
                QueryUsername = parameters["username"];
            else
                QueryUsername = null;

            string EncryptedAnswer;
            if (parameters.ContainsKey("answer"))
                EncryptedAnswer = parameters["answer"];
            else
                EncryptedAnswer = null;

            string AnswerSettings;
            if (parameters.ContainsKey("answer_settings"))
                AnswerSettings = parameters["answer_settings"];
            else
                AnswerSettings = null;

            string EncryptedNewPassword;
            if (parameters.ContainsKey("new_password"))
                EncryptedNewPassword = parameters["new_password"];
            else
                EncryptedNewPassword = null;

            string NewPasswordSettings;
            if (parameters.ContainsKey("new_password_settings"))
                NewPasswordSettings = parameters["new_password_settings"];
            else
                NewPasswordSettings = null;

            string QueryOperation;
            if (parameters.ContainsKey("operation"))
                QueryOperation = parameters["operation"];
            else
                QueryOperation = null;

            if (string.IsNullOrEmpty(QueryUsername) || string.IsNullOrEmpty(EncryptedAnswer) || string.IsNullOrEmpty(EncryptedNewPassword) || string.IsNullOrEmpty(QueryOperation))
                return Result;

            ErrorCodes ErrorCode;
            if (Credential.update_6(KnownUserTable, QueryUsername, EncryptedAnswer, AnswerSettings, EncryptedNewPassword, NewPasswordSettings, QueryOperation))
                ErrorCode = ErrorCodes.Success;
            else
                ErrorCode = ErrorCodes.InvalidUsernameOrAnswer;

            Result.Add(new Dictionary<string, string>()
            {
                { "result", ((int)ErrorCode).ToString()},
            });

            return Result;
        }

        private List<Dictionary<string, string>> OnQueryNewCredentialRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string QueryUsername;
            if (parameters.ContainsKey("username"))
                QueryUsername = parameters["username"];
            else
                QueryUsername = null;

            string QueryEmailAddress;
            if (parameters.ContainsKey("email_address"))
                QueryEmailAddress = parameters["email_address"];
            else
                QueryEmailAddress = null;

            if (string.IsNullOrEmpty(QueryUsername) || string.IsNullOrEmpty(QueryEmailAddress))
                return Result;

            string SaltString = "";

            string ResultUsername;
            string ResultEmailAddress;
            if (Credential.query_7(KnownUserTable, QueryUsername, QueryEmailAddress, out ResultUsername, out ResultEmailAddress))
            {
                ErrorCodes ErrorCode;

                if (ResultUsername == QueryUsername)
                    ErrorCode = ErrorCodes.UsernameAlreadyUsed;
                else if (ResultEmailAddress == QueryEmailAddress)
                    ErrorCode = ErrorCodes.EmailAddressAlreadyUsed;
                else
                    ErrorCode = ErrorCodes.OperationFailed;

                Result.Add(new Dictionary<string, string>()
                {
                    { "result", ((int)ErrorCode).ToString() },
                });
            }
            else
            {
                long Ticks = DateTime.Now.Ticks;
                byte[] TickBytes = BitConverter.GetBytes(Ticks);
                string TickString = HashTools.GetString(TickBytes);
                SaltString = TickString + TickString + TickString + TickString + TickString + TickString + TickString + TickString;

                Result.Add(new Dictionary<string, string>()
                {
                    { "result", ((int)ErrorCodes.Success).ToString() },
                    { "salt", SaltString },
                });
            }

            return Result;
        }

        private List<Dictionary<string, string>> OnQuerySaltRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string QueryIdentifier;
            if (parameters.ContainsKey("identifier"))
                QueryIdentifier = parameters["identifier"];
            else
                QueryIdentifier = null;

            if (string.IsNullOrEmpty(QueryIdentifier))
                return Result;

            string ResultSalt;
            if (Credential.query_8(KnownUserTable, QueryIdentifier, out ResultSalt))
            {
                Result.Add(new Dictionary<string, string>()
                {
                    { "result", ((int)ErrorCodes.Success).ToString() },
                    { "salt", ResultSalt },
                });
            }
            else
            {
                Result.Add(new Dictionary<string, string>()
                {
                    { "result", ((int)ErrorCodes.ErrorNotFound).ToString() },
                });
            }

            return Result;
        }

        private List<Dictionary<string, string>> OnSignInRequest(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            string QueryIdentifier;
            if (parameters.ContainsKey("identifier"))
                QueryIdentifier = parameters["identifier"];
            else
                QueryIdentifier = null;

            string EncryptedPassword;
            if (parameters.ContainsKey("password"))
                EncryptedPassword = parameters["password"];
            else
                EncryptedPassword = null;

            string PasswordSettings;
            if (parameters.ContainsKey("password_settings"))
                PasswordSettings = parameters["password_settings"];
            else
                PasswordSettings = null;

            if (string.IsNullOrEmpty(QueryIdentifier) || string.IsNullOrEmpty(EncryptedPassword) || PasswordSettings == null)
                return Result;

            string ResultUsername;
            string ResultEmailAddress;
            string ResultQuestion;
            string ResultName;
            string ResultLoginUrl;
            string ResultMeetingUrl;
            string ResultValidationUrl;
            if (Credential.query_9(KnownUserTable, EncryptedPassword, PasswordSettings, QueryIdentifier, out ResultUsername, out ResultEmailAddress, out ResultQuestion, out ResultName, out ResultLoginUrl, out ResultMeetingUrl, out ResultValidationUrl))
            {
                Result.Add(new Dictionary<string, string>()
                {
                    { "username", ResultUsername},
                    { "email_address", ResultEmailAddress },
                    { "question", DecodedQuestion(ResultQuestion) },
                    { "name", ResultName },
                    { "login_url", ResultLoginUrl },
                    { "meeting_url", ResultMeetingUrl },
                    { "validation_url", ResultValidationUrl },
                    { "result", ((int)ErrorCodes.Success).ToString() },
                });
            }
            else
            {
                Result.Add(new Dictionary<string, string>()
                {
                    { "result", ((int)ErrorCodes.ErrorNotFound).ToString() },
                });
            }

            return Result;
        }

        public static string EncodedQuestion(string text)
        {
            string Result = "";
            foreach (char c in text)
                Result += (char)(((int)c ^ 204 ) + 40);

            return Result;
        }

        public static string DecodedQuestion(string text)
        {
            string Result = "";
            foreach (char c in text)
                Result += (char)(((int)c - 40) ^ 204);

            return Result;
        }

        public string CreateOperation()
        {
            return "";
        }

        public static int ParseResult(string result)
        {
            int IntError;
            if (int.TryParse(result, out IntError))
                return IntError;
            else
                return -1;
        }

        private List<Credential> KnownUserTable = new List<Credential>
        {
            new Credential(
                "test",
                Convert.ToBase64String(Encoding.UTF8.GetBytes("toto")),
                "",
                "test@test.com",
                "c32e25dca5caa30aa442eb2e9190e08cc93868ea60fd91eff53f53b5bc420bd2e5079d3fb5197046f806936b5ae6d1eab72a49a5bd22a0522890423266b993350064ec298e1ad608",
                EncodedQuestion("foo"),
                Convert.ToBase64String(Encoding.UTF8.GetBytes("not foo")),
                "",
                true,
                Eqmlp.KnownOrganizationTable[0]["name"],
                Eqmlp.KnownOrganizationTable[0]["login_url"],
                Eqmlp.KnownOrganizationTable[0]["meeting_url"],
                Eqmlp.KnownOrganizationTable[0]["validation_url"]),
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
