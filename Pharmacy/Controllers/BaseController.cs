using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pharmacy.Data;

namespace Pharmacy.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly ApplicationDbContext _context;
        protected String UserId;
        protected String UserName;
        protected int UserTypeId;
        public BaseController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.Identity.IsAuthenticated)
            {
                base.OnActionExecuting(context);
                try
                {
                    UserId = _userManager.GetUserId(HttpContext.User);
                    IdentityUser user = _context.Users.Find(UserId);
                    ViewBag.UserEmail = user.Email;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }


    }
}