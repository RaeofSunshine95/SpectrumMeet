using System.Web.Mvc;

namespace SpectrumMeetMVC.Areas.MessageBoard
{
    public class MessageBoardAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MessageBoard";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MessageBoard_default",
                "MessageBoard/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}