using System.Web.Mvc;


public class AuthLogAttribute : AuthorizeAttribute
{
    public AuthLogAttribute()
    {
        View = "AuthorizeFailed";
    }

    public string View { get; set; }


    public override void OnAuthorization(AuthorizationContext filterContext)
    {
        base.OnAuthorization(filterContext);
        IsUserAuthorized(filterContext);
    }

    private void IsUserAuthorized(AuthorizationContext filterContext)
    {
 
        if (filterContext.Result == null)
            return;

        if (filterContext.HttpContext.User.Identity.IsAuthenticated)
        {

            var vr = new ViewResult();
            vr.ViewName = View;

            ViewDataDictionary dict = new ViewDataDictionary();
            dict.Add("Message", "Nie masz uprawnień do wykonywania tej czynności!");

            vr.ViewData = dict;

            var result = vr;

            filterContext.Result = result;
        }
    }
}
