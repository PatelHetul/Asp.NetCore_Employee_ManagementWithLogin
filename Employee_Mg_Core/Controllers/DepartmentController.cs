using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Employee_Mg_Core.Models;
using Microsoft.AspNetCore.Http;

namespace Employee_Mg_Core.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly EmployeeMgContext _context;

        public DepartmentController(EmployeeMgContext context)
        {
            _context = context;
        }

        // GET: Department
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("LoginID") != null && HttpContext.Session.GetString("AssignRole") != null && HttpContext.Session.GetString("LoginID") == "1")
            {
                //  var aa = _context.DepartmentMasters.ToList();
                var DepartmentLists = await _context.DepartmentMaster.Where(dep => dep.IsDelete == 0).ToListAsync();
                return View(DepartmentLists);
            }
            else
            {
                return RedirectToAction("LogIn", "Registrations");
            }
        }

        // GET: Department/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("LoginID") != null && HttpContext.Session.GetString("AssignRole") != null && HttpContext.Session.GetString("LoginID") == "1")
            {
                if (id == null)
                {
                    return NotFound();
                }

                var departmentMaster = await _context.DepartmentMaster
                    .SingleOrDefaultAsync(m => m.Department_Id == id);
                if (departmentMaster == null)
                {
                    return NotFound();
                }

                return View(departmentMaster);
            }
            else
            {
                return RedirectToAction("LogIn", "Registrations");
            }
        }

        // GET: Department/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("LoginID") != null && HttpContext.Session.GetString("AssignRole") != null && HttpContext.Session.GetString("LoginID") == "1")
            {
                if (HttpContext.Session.GetString("AssignRole").ToString().Equals("GuestUser"))
                {
                    return RedirectToAction("Index");
                }
                return View();
            }
            else
            {
                return RedirectToAction("LogIn", "Registrations");
            }
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Department_Id,Department_Name")] DepartmentMaster departmentMaster)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_context.DepartmentMaster.Any(name => name.Department_Name.Equals(departmentMaster.Department_Name) && name.IsDelete == 0))
                    {
                        ModelState.AddModelError(string.Empty, "Department is already exists");
                    }
                    else
                    {
                        departmentMaster.IsDelete = 0;
                        _context.Add(departmentMaster);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
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

            return View(departmentMaster);
        }

        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("LoginID") != null && HttpContext.Session.GetString("AssignRole") != null && HttpContext.Session.GetString("LoginID") == "1")
            {
                if (HttpContext.Session.GetString("AssignRole").ToString().Equals("GuestUser"))
                {
                    return RedirectToAction("Index");
                }
                if (id == null)
                {
                    return NotFound();
                }

                var departmentMaster = await _context.DepartmentMaster.SingleOrDefaultAsync(m => m.Department_Id == id);
                if (departmentMaster == null)
                {
                    return NotFound();
                }
                return View(departmentMaster);
            }
            else
            {
                return RedirectToAction("LogIn", "Registrations");
            }
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Department_Id,Department_Name")] DepartmentMaster departmentMaster)
        {
            if (id != departmentMaster.Department_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.DepartmentMaster.Any(name => name.Department_Name.Equals(departmentMaster.Department_Name) && name.Department_Id != departmentMaster.Department_Id && name.IsDelete == 0))
                    {
                        ModelState.AddModelError(string.Empty, "Department is already exists");
                        return View(departmentMaster);
                    }
                    else
                    {
                        departmentMaster.IsDelete = 0;
                        _context.Update(departmentMaster);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentMasterExists(departmentMaster.Department_Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(departmentMaster);
        }

        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("LoginID") != null && HttpContext.Session.GetString("AssignRole") != null && HttpContext.Session.GetString("LoginID") == "1")
            {
                if (HttpContext.Session.GetString("AssignRole").ToString().Equals("GuestUser"))
                {
                    return RedirectToAction("Index");
                }
                if (id == null)
                {
                    return NotFound();
                }

                var departmentMaster = await _context.DepartmentMaster
                    .SingleOrDefaultAsync(m => m.Department_Id == id);
                if (departmentMaster == null)
                {
                    return NotFound();
                }

                return View(departmentMaster);
            }
            else
            {
                return RedirectToAction("LogIn", "Registrations");
            }
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var departmentMaster = await _context.DepartmentMaster.SingleOrDefaultAsync(m => m.Department_Id == id);
            departmentMaster.IsDelete = 1;
            _context.Update(departmentMaster);
            // _context.DepartmentMaster.Remove(departmentMaster);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentMasterExists(int id)
        {
            return _context.DepartmentMaster.Any(e => e.Department_Id == id);
        }
    }
}
