namespace eCentral.Web.Framework.Mvc
{
    public class ConfirmationModel : BaseEntityModel
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }

        public string ActionScript { get; set; }
    }
}