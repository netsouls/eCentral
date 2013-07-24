
namespace eCentral.Services.Users
{
    public class ResetPasswordRequest
    {
        public PasswordResetSteps CurrentStep { get; set; }

        public string UserName { get; set; }

        public string SecurityAnswerOne { get; set; }
        public string SecurityAnswerTwo { get; set; }
        public string SecurityAnswerThree { get; set; }

        public string NewPassword { get; set; }

        public ResetPasswordRequest(string userName, PasswordResetSteps currentStep)
        {
            this.UserName = userName;
            this.CurrentStep = currentStep;
        }
    }

    public enum PasswordResetSteps
    {
        /// <summary>
        /// First step is to validate the email address of the user
        /// </summary>
        ValidateEmailAddress,

        /// <summary>
        /// Second step is to validate the security questions answers
        /// </summary>
        ValidateSecurityQuestions,

        /// <summary>
        /// Third step to reset the password
        /// </summary>
        ResetPassword
    }
}
