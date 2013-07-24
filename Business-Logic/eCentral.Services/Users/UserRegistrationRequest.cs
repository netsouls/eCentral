using eCentral.Core;

namespace eCentral.Services.Users
{
    public class UserRegistrationRequest
    {
        
        public string Username { get; set; }
        public string Password { get; set; }
        
        /// <summary>
        /// Get or set whether the user publishing status
        /// </summary>
        public PublishingStatus CurrentPublishingStatus { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or set whether the user is an administrator
        /// </summary>
        public bool IsAdministrator { get; set; }

        public UserRegistrationRequest(string username,
            string password, PublishingStatus currentPublishingStatus  = PublishingStatus.Active)
        {
            this.Username = username;
            this.Password = password;
            this.CurrentPublishingStatus = currentPublishingStatus;
        }
    }
}
