namespace DemoSite.Components.Contact;

using DemoSite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

[ViewComponent(Name = "Contact")]
    public class ContactViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ContactViewModel model)
        {
            return View(model);
        }
    }
