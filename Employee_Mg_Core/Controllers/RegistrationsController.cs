using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Employee_Mg_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace Employee_Mg_Core.Controllers
{
    public class RegistrationsController : Controller
    {
        private readonly EmployeeMgContext _context;

        const string SessionEmpName = "UserName";
        const string SessionISRole = "AssignRole";
        const string SessionIsLogin = "LoginID";


        public RegistrationsController(EmployeeMgContext context)
        {

            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString(SessionIsLogin) == null || HttpContext.Session.GetString(SessionIsLogin) == "0")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Department");
            }

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Registration model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_context.Registration.Any(name => name.Email.Equals(model.Email)))
                    {
                        ModelState.AddModelError(string.Empty, "User is already exists");
                    }
                    else
                    {
                        _context.Add(model);
                        await _context.SaveChangesAsync();
                        HttpContext.Session.SetString(SessionEmpName, model.Email);
                        HttpContext.Session.SetString(SessionISRole, model.Role);
                        HttpContext.Session.SetString(SessionIsLogin, "1");
                        return RedirectToAction("Index", "Department");
                    }

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                message += string.Format("<b>StackTrace:</b> {0}<br /><br />", ex.StackTrace.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>Source:</b> {0}<br /><br />", ex.Source.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>TargetSite:</b> {0}", ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                ModelState.AddModelError(string.Empty, message);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LogIn()
        {
            if (HttpContext.Session.GetString(SessionIsLogin) == null || HttpContext.Session.GetString(SessionIsLogin) == "0")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Department");
            }

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(Registration userr)
        {
            if (userr.Email.Equals("") || userr.Password.Equals(""))
            {
                ModelState.AddModelError("", "Please Enter login details.");
            }
            else if (!userr.Email.Equals("") && !userr.Password.Equals(""))
            {
                var user = _context.Registration.FirstOrDefault(u => u.Email == userr.Email);
                if (user != null)
                {
                    if (user.Password == userr.Password)
                    {
                        HttpContext.Session.SetString(SessionEmpName, user.Email);
                        HttpContext.Session.SetString(SessionISRole, user.Role);
                        HttpContext.Session.SetString(SessionIsLogin, "1");
                        return RedirectToAction("Index", "Department");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Login details are wrong.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Login details are wrong.");
                }
            }
            return View(userr);
        }

        public ActionResult LogOut()
        {
            HttpContext.Session.SetString(SessionIsLogin, "0");
            HttpContext.Session.Clear();
            return RedirectToAction("LogIn", "Registrations");
        }


        public IActionResult ForgotPassword()
        {
            if (HttpContext.Session.GetString(SessionIsLogin) == null || HttpContext.Session.GetString(SessionIsLogin) == "0")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Department");
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(Registration userr)
        {
            var user = _context.Registration.FirstOrDefault(u => u.Email == userr.Email);
            if (user != null)
            {
                if (user.SecurityQuestion == userr.SecurityQuestion && user.SecurityAnswer == userr.SecurityAnswer)
                {
                    TempData["userid"] = user.UserId;
                    return RedirectToAction("CreateNewPassword");
                }
                else
                {
                    ModelState.AddModelError("", "Please Enter Valid Security Question Answer Details.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please Enter Valid Emailid.");
            }

            return View(userr);
        }

        public ActionResult CreateNewPassword()
        {
            if (HttpContext.Session.GetString(SessionIsLogin) == null || HttpContext.Session.GetString(SessionIsLogin) == "0")
            {
                int id = 0;
                TempData.Keep();
                if (TempData.ContainsKey("userid"))
                    int.TryParse(TempData["userid"].ToString(), out id);

             //   Registration reg = db.Registrations.Find(id);
                Registration reg =  _context.Registration.FirstOrDefault(m => m.UserId == id);
                if (reg == null)
                {
                    return RedirectToAction("ForgotPassword");
                }
                reg.Password = "";
                reg.ConfirmPassword = "";
                return View(reg);

            }
            else
            {
                return RedirectToAction("Index", "Department");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewPassword(Registration userr)
        {
            try
            {
                if (userr.UserId > 0)
                {
                    Registration reg = _context.Registration.FirstOrDefault(m => m.UserId == userr.UserId);
                    reg.Password = userr.Password;
                    reg.ConfirmPassword = userr.ConfirmPassword;

                    _context.Update(reg);
                     _context.SaveChangesAsync();
                    HttpContext.Session.SetString(SessionEmpName, reg.Email);
                    HttpContext.Session.SetString(SessionISRole, reg.Role);
                    HttpContext.Session.SetString(SessionIsLogin, "1");
                    return RedirectToAction("Index", "Department");
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                message += string.Format("<b>StackTrace:</b> {0}<br /><br />", ex.StackTrace.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>Source:</b> {0}<br /><br />", ex.Source.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>TargetSite:</b> {0}", ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                ModelState.AddModelError(string.Empty, message);
            }
            return View(userr);
        }
        private bool RegistrationExists(int id)
        {
            return _context.Registration.Any(e => e.UserId == id);
        }
    }
}
