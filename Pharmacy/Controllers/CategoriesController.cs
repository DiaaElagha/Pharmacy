using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class CategoriesController : BaseController
    {
        public CategoriesController(UserManager<IdentityUser> userManager, ApplicationDbContext context) : base(userManager, context)
        {
        }

        // GET: Categories
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetDataAjax([FromBody]dynamic data)
        {
            DataTableHelper d = new DataTableHelper(data);

            var query = _context.Category.Where(x =>
            (d.SearchKey == null || x.Name.Contains(d.SearchKey)));

            int totalCount = query.Count();

            var items = query.Select(x => new
            {
                x.Id,
                x.Name,
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

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Id")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.CreatedAt = DateTime.Now;
                category.CreatedBy = UserId;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return Json(new
                {
                    status = 1,
                    msg = "s: تمت عملية الاضافة بنجاح",
                    close = 1
                });
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id,CreatedAt,CreatedBy")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            ModelState.Remove("CreatedAt");
            ModelState.Remove("CreatedBy");
            if (ModelState.IsValid)
            {
                try
                {
                    category.UpdatedAt = DateTime.Now;
                    category.UpdatedBy = UserId;
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Category == null)
            {
                return NotFound();
            }

            _context.Category.Remove(Category);
            await _context.SaveChangesAsync();
            return Json(new
            {
                status = 1,
                msg = "s: تمت عملية الحذف بنجاح"
            });
        }
        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
    }
}
