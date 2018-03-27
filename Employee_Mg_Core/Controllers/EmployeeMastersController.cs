using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Employee_Mg_Core.Models;
using System.IO;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Employee_Mg_Core.Controllers
{
    public class EmployeeMastersController : Controller
    {
        private readonly EmployeeMgContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        public EmployeeMastersController(EmployeeMgContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: EmployeeMasters
        public async Task<IActionResult> Index(string searchText, string sortOrder, string currentFilter, int? page)
        {
            //var EmployeeList = from Emp in _context.EmployeeMaster join Dep in _context.DepartmentMaster on Emp.Department_Id equals Dep.Department_Id select new { Emp.Employee_Id, Emp.Employee_Name,  Emp.Email, Emp.MobileNo ,Dep.Department_Name};
            // var aa = EmployeeList.ToList();

            //return View(await EmployeeList.ToListAsync());
            if (HttpContext.Session.GetString("LoginID") != null && HttpContext.Session.GetString("AssignRole") != null && HttpContext.Session.GetString("LoginID") == "1")
            {
                ViewData["CurrentSort"] = sortOrder;
                ViewBag.idSortParm = String.IsNullOrEmpty(sortOrder) ? "Id" : "";
                ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
                ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";

                if (searchText != null)
                {
                    page = 1;
                }
                else
                {
                    searchText = currentFilter;
                }

                ViewBag.currentFilter = searchText;

                var employeeMasters = _context.EmployeeMaster.Where(e => e.IsDelete == 0 && e.DepartmentMaster.IsDelete == 0);

                if (!String.IsNullOrEmpty(searchText))
                {
                    employeeMasters = employeeMasters.Where(s => s.Employee_Name.Contains(searchText) || s.Email.Contains(searchText));
                }
                switch (sortOrder)
                {
                    case "Id":
                        employeeMasters = employeeMasters.OrderBy(s => s.Employee_Id);
                        break;
                    case "name_desc":
                        employeeMasters = employeeMasters.OrderByDescending(s => s.Employee_Name);
                        break;
                    case "Email":
                        employeeMasters = employeeMasters.OrderBy(s => s.Email);
                        break;
                    case "email_desc":
                        employeeMasters = employeeMasters.OrderByDescending(s => s.Email);
                        break;
                    default:
                        employeeMasters = employeeMasters.OrderBy(s => s.Employee_Name);
                        break;
                }
                int pageSize = 3;
                return View(await PaginatedList<EmployeeMaster>.CreateAsync(employeeMasters.AsNoTracking(), page ?? 1, pageSize));
            }
            else
            {
                return RedirectToAction("LogIn", "Registrations");
            }
        }

        // GET: EmployeeMasters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("LoginID") != null && HttpContext.Session.GetString("AssignRole") != null && HttpContext.Session.GetString("LoginID") == "1")
            {
                if (id == null)
                {
                    return NotFound();
                }

                var employeeMaster = await _context.EmployeeMaster
                    .SingleOrDefaultAsync(m => m.Employee_Id == id);
                if (employeeMaster == null)
                {
                    return NotFound();
                }
                var department = await _context.DepartmentMaster
                    .SingleOrDefaultAsync(m => m.Department_Id == employeeMaster.Department_Id);
                ViewBag.image = employeeMaster.Image;
                ViewBag.depName = department.Department_Name;
                return View(employeeMaster);
            }
            else
            {
                return RedirectToAction("LogIn", "Registrations");
            }
        }

        // GET: EmployeeMasters/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("LoginID") != null && HttpContext.Session.GetString("AssignRole") != null && HttpContext.Session.GetString("LoginID") == "1")
            {
                if (HttpContext.Session.GetString("AssignRole").ToString().Equals("GuestUser"))
                {
                    return RedirectToAction("Index");
                }
                var DepartmentLists = _context.DepartmentMaster.Where(s => s.IsDelete == 0).ToList();
                ViewBag.Department = new SelectList(DepartmentLists, "Department_Id", "Department_Name");
                return View();
            }
            else
            {
                return RedirectToAction("LogIn", "Registrations");
            }
        }


        // POST: EmployeeMasters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Employee_Id,Employee_Name,Department_Id,JoiningDate,Address,IsDelete,Email,MobileNo,Image")] EmployeeMaster employeeMaster)
        {
            if (ModelState.IsValid)
            {
                if (_context.EmployeeMaster.Any(name => name.Email.Equals(employeeMaster.Email) && name.IsDelete == 0))
                {
                    ModelState.AddModelError(string.Empty, "Employee is already exists");
                    var DepartmentLists = _context.DepartmentMaster.Where(s => s.IsDelete == 0).ToList();
                    ViewBag.Department = new SelectList(DepartmentLists, "Department_Id", "Department_Name");

                    return View(employeeMaster);
                }
                else
                {
                    var files = HttpContext.Request.Form.Files;
                    foreach (var Image in files)
                    {
                        if (Image != null && Image.Length > 0)
                        {
                            var supportedTypes = new[] { "jpg", "jpeg", "png", "gif", "bmp" };
                            var fileExt = System.IO.Path.GetExtension(Image.FileName).Substring(1);
                            if (!supportedTypes.Contains(fileExt))
                            {
                                string ErrorMessage = "File Extension Is InValid - Only Upload Image File";
                                ModelState.AddModelError(string.Empty, ErrorMessage);
                                var DepartmentLists = _context.DepartmentMaster.Where(s => s.IsDelete == 0).ToList();
                                ViewBag.Department = new SelectList(DepartmentLists, "Department_Id", "Department_Name");
                                return View(employeeMaster);
                            }
                            var file = Image;
                            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "Emp_IMg");

                            if (file.Length > 0)
                            {
                                var fileName = ContentDispositionHeaderValue.Parse
                                    (file.ContentDisposition).FileName.ToString().Trim('"');

                                System.Console.WriteLine(fileName);
                                using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                    employeeMaster.Image = file.FileName;
                                }


                            }
                        }
                    }
                    //employeeMaster.Department_Id = 2;
                    employeeMaster.IsDelete = 0;
                    _context.Add(employeeMaster);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(employeeMaster);
        }

        // GET: EmployeeMasters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("LoginID") != null && HttpContext.Session.GetString("AssignRole") != null && HttpContext.Session.GetString("LoginID") == "1")
            {
                if (id == null)
                {
                    return NotFound();
                }
                var employeeMaster = await _context.EmployeeMaster.SingleOrDefaultAsync(m => m.Employee_Id == id);
                if (HttpContext.Session.GetString("AssignRole").ToString().Equals("GuestUser") && !HttpContext.Session.GetString("UserName").ToString().Equals(employeeMaster.Email))
                {
                    return RedirectToAction("Index");
                }

                var DepartmentLists = _context.DepartmentMaster.Where(s => s.IsDelete == 0).ToList();
                ViewBag.Department = new SelectList(DepartmentLists, "Department_Id", "Department_Name");

                if (employeeMaster == null)
                {
                    return NotFound();
                }
                return View(employeeMaster);
            }
            else
            {
                return RedirectToAction("LogIn", "Registrations");
            }
        }

        // POST: EmployeeMasters/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Employee_Id,Employee_Name,Department_Id,JoiningDate,Address,IsDelete,Email,MobileNo,Image")] EmployeeMaster employeeMaster)
        {
            if (id != employeeMaster.Employee_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.EmployeeMaster.Any(name => name.Email.Equals(employeeMaster.Email) && name.Employee_Id != employeeMaster.Employee_Id && name.IsDelete == 0))
                    {
                        ModelState.AddModelError(string.Empty, "Employee is already exists");
                        var DepartmentLists = _context.DepartmentMaster.Where(s => s.IsDelete == 0).ToList();
                        ViewBag.Department = new SelectList(DepartmentLists, "Department_Id", "Department_Name");
                        return View(employeeMaster);
                    }
                    else
                    {
                        var files = HttpContext.Request.Form.Files;
                        foreach (var Image in files)
                        {
                            if (Image != null && Image.Length > 0)
                            {
                                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif", "bmp" };
                                var fileExt = System.IO.Path.GetExtension(Image.FileName).Substring(1);
                                if (!supportedTypes.Contains(fileExt))
                                {
                                    string ErrorMessage = "File Extension Is InValid - Only Upload Image File";
                                    ModelState.AddModelError(string.Empty, ErrorMessage);
                                    var DepartmentLists = _context.DepartmentMaster.Where(s => s.IsDelete == 0).ToList();
                                    ViewBag.Department = new SelectList(DepartmentLists, "Department_Id", "Department_Name");
                                    return View(employeeMaster);
                                }

                                var file = Image;
                                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "Emp_IMg");

                                if (file.Length > 0)
                                {
                                    var fileName = ContentDispositionHeaderValue.Parse
                                        (file.ContentDisposition).FileName.ToString().Trim('"');

                                    System.Console.WriteLine(fileName);
                                    using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                                    {
                                        await file.CopyToAsync(fileStream);
                                        employeeMaster.Image = file.FileName;
                                    }


                                }
                            }
                        }
                        //  employeeMaster.Department_Id = 2;
                        employeeMaster.IsDelete = 0;
                        _context.Update(employeeMaster);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeMasterExists(employeeMaster.Employee_Id))
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
            return View(employeeMaster);
        }

        // GET: EmployeeMasters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("LoginID") != null && HttpContext.Session.GetString("AssignRole") != null && HttpContext.Session.GetString("LoginID") == "1")
            {
                if (id == null)
                {
                    return NotFound();
                }
                var employeeMaster = await _context.EmployeeMaster.SingleOrDefaultAsync(m => m.Employee_Id == id);
                if (HttpContext.Session.GetString("AssignRole").ToString().Equals("GuestUser") && !HttpContext.Session.GetString("UserName").ToString().Equals(employeeMaster.Email))
                {
                    return RedirectToAction("Index");
                }

                var department = await _context.DepartmentMaster
                   .SingleOrDefaultAsync(m => m.Department_Id == employeeMaster.Department_Id);

                ViewBag.depName = department.Department_Name;
                ViewBag.image = employeeMaster.Image;
                if (employeeMaster == null)
                {
                    return NotFound();
                }

                return View(employeeMaster);

            }
            else
            {
                return RedirectToAction("LogIn", "Registrations");
            }
        }

        // POST: EmployeeMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeMaster = await _context.EmployeeMaster.SingleOrDefaultAsync(m => m.Employee_Id == id);
            try
            {
                employeeMaster.IsDelete = 1;
                _context.Update(employeeMaster);
                //_context.EmployeeMaster.Remove(employeeMaster);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                message += string.Format("<b>StackTrace:</b> {0}<br /><br />", ex.StackTrace.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>Source:</b> {0}<br /><br />", ex.Source.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>TargetSite:</b> {0}", ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                ModelState.AddModelError(string.Empty, message);
            }
            return View(employeeMaster);
        }

        private bool EmployeeMasterExists(int id)
        {
            return _context.EmployeeMaster.Any(e => e.Employee_Id == id);
        }
    }
}
