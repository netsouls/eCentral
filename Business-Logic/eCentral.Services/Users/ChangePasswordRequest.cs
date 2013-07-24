
namespace eCentral.Services.Users
{
    public class ChangePasswordRequest
    {
        public string UserName { get; set; }
        public bool ValidateRequest { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }

        public ChangePasswordRequest(string userName, bool validateRequest, 
            string newPassword, string oldPassword = "")
        {
            this.UserName = userName;
            this.ValidateRequest = validateRequest;
            this.NewPassword = newPassword;
            this.OldPassword = oldPassword;
        }
    }
}
