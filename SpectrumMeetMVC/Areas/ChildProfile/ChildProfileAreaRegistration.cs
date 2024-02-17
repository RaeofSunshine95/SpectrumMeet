using System.Web.Mvc;

namespace SpectrumMeetMVC.Areas.ChildProfile
{
    public class ChildProfileAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ChildProfile";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ChildProfile_default",
                "ChildProfile/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}