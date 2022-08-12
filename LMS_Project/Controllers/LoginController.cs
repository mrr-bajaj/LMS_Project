using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS_Project.Models;
using Microsoft.AspNetCore.Http;

namespace LMS_Project.Controllers
{
    public class LoginController : Controller
    {
        private readonly LMS_ProjectContext _context;
        private readonly ILoginRepo _user;

        public LoginController(LMS_ProjectContext context, ILoginRepo user)
        {
            _context = context;
            _user = user;
        }

        // GET: Accounts
        public IActionResult Index(string username, string password)
        {/*
            HttpContext.Session.Clear();*/
            if (username != null && password != null)
            {
                var user = _user.getUserByName(username);
                if (username.Equals(user.UserName) && password.Equals(user.Password))
                {
                    if (user.Role.Equals("admin"))
                    {
                        HttpContext.Session.SetString("username", username);
                        ViewBag.Message = "Admin";
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (user.Role.Equals("user"))
                    {
                        HttpContext.Session.SetString("username", username);
                        ViewBag.Message = "user";
                        return RedirectToAction("Index", "User");
                    }
                    else
                    {
                        ViewBag.Message = "Fail";
                    }
                }
            }
            return View(_context.Accounts.ToList());
        }

    }
}
