using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Helper;
using Pharmacy.Models;

namespace Pharmacy.Controllers
{
    [Authorize]
    public class DrugsController : BaseController
    {
        private readonly IHostingEnvironment _IHostingEnvironment;
        public DrugsController(IHostingEnvironment IHostingEnvironment, UserManager<IdentityUser> userManager, ApplicationDbContext context) : base(userManager, context)
        {
            _IHostingEnvironment = IHostingEnvironment;
        }

        // GET: Drugs
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetDataAjax([FromBody]dynamic data)
        {
            DataTableHelper d = new DataTableHelper(data);

            var query = _context.Drug.Include(p => p.Category).Where(x =>
            (d.SearchKey == null || x.Name.Contains(d.SearchKey)));

            int totalCount = query.Count();

            var items = query.Select(x => new
            {
                x.Id,
                x.Name,
                catName = x.Category.Name,
                x.Price,
                x.Description,
                x.ImageName,
                x.CreatedAt,
                createdBy = _context.Users.SingleOrDefault(o => o.Id.Equals(x.CreatedBy)).Email,
            }).Skip(d.Start).Take(d.Length).ToList();

            var result =
               new
               {
                   draw = d.Draw,
                   recordsTotal = totalCount,
                   recordsFiltered = totalCount,
                   data = items
               };
            return Json(result);
        }

        // GET: Drugs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drug = await _context.Drug
                .Include(d => d.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drug == null)
            {
                return NotFound();
            }

            return View(drug);
        }

        // GET: Drugs/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name");
            return View();
        }

        // POST: Drugs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,CategoryId,Description,Id,ImageName")] Drug drug ,IFormFile ImageName)
        {
            if (ModelState.IsValid)
            {
                String fileName = null;
                if (ImageName != null && ImageName.Length > 0)
                {
                    var uploads = Path.Combine(_IHostingEnvironment.WebRootPath, "Images");
                    if (ImageName.Length > 0)
                    {
                        fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(ImageName.FileName);
                        using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                        {
                            await ImageName.CopyToAsync(fileStream);
                        }
                    }
                }
                if (fileName != null)
                {
                    drug.ImageName = fileName;
                }
                else
                {
                    drug.ImageName = "Drug_default.png";
                }
                drug.CreatedAt = DateTime.Now;
                drug.CreatedBy = UserId;
                _context.Add(drug);
                await _context.SaveChangesAsync();
                return Json(new
                {
                    status = 1,
                    msg = "s: تمت عملية الاضافة بنجاح",
                    close = 1
                });
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", drug.CategoryId);
            return View(drug);
        }

        // GET: Drugs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drug = await _context.Drug.FindAsync(id);
            if (drug == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", drug.CategoryId);
            return View(drug);
        }

        // POST: Drugs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Price,CategoryId,Description,Id,CreatedAt,CreatedBy,ImageName")] Drug drug)
        {
            if (id != drug.Id)
            {
                return NotFound();
            }
            ModelState.Remove("CreatedAt");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("ImageName");
            if (ModelState.IsValid)
            {
                try
                {
                    drug.UpdatedAt = DateTime.Now;
                    drug.UpdatedBy = UserId;
                    _context.Update(drug);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DrugExists(drug.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Json(new
                {
                    status = 1,
                    msg = "s: تمت عملية التعديل بنجاح",
                    close = 1
                });
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", drug.CategoryId);
            return View(drug);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drug = await _context.Drug
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drug == null)
            {
                return NotFound();
            }

            _context.Drug.Remove(drug);
            await _context.SaveChangesAsync();
            return Json(new
            {
                status = 1,
                msg = "s: تمت عملية الحذف بنجاح"
            });
        }
        private bool DrugExists(int id)
        {
            return _context.Drug.Any(e => e.Id == id);
        }
    }
}
