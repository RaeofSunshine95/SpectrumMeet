using System.Web.Mvc;

namespace SpectrumMeetMVC.Areas.PrivateMessage
{
    public class PrivateMessageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PrivateMessage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PrivateMessage_default",
                "PrivateMessage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}