using System.Collections.Generic;

namespace AppCSHtml5
{
    public interface ILogin
    {
        LoginStates State { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string NewPassword { get; set; }
        string ConfirmPassword { get; set; }
        string RecoveryQuestion { get; set; }
        string RecoveryAnswer { get; set; }
        bool Remember { get; set; }
        void On_Login(string pageName, string sourceName, string sourceContent, out string destinationPageName);
        void On_ChangePassword(string pageName, string sourceName, string sourceContent, out string destinationPageName);
        void On_Logout(string pageName, string sourceName, string sourceContent);
    }
}
