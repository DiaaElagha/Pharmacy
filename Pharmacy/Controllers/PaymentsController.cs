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
    public class PaymentsController : BaseController
    {
        public PaymentsController(UserManager<IdentityUser> userManager, ApplicationDbContext context) : base(userManager, context)
        {
        }

        // GET: Payments
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetDataAjax([FromBody]dynamic data)
        {
            DataTableHelper d = new DataTableHelper(data);

            var query = _context.Payment.Include(p => p.Drug).Include(p => p.User).Where(x =>
            (d.SearchKey == null || x.Drug.Name.Contains(d.SearchKey)));

            int totalCount = query.Count();

            var items = query.Select(x => new
            {
                x.Id,
                EmailUser = x.User.Email,
                DrugName = x.Drug.Name,
                x.Quntity,
                x.Total,
                x.CreatedAt,
                x.CreatedBy,
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

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment
                .Include(p => p.Drug)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create()
        {
            ViewData["DrugId"] = new SelectList(_context.Drug, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DrugId,UserId,Id,Total,Quntity")] Payment payment)
        {
            ModelState.Remove("Total");
            if (ModelState.IsValid)
            {
                payment.CreatedAt = DateTime.Now;
                payment.CreatedBy = UserId;
               
                payment.Total = payment.Quntity * _context.Drug.Single(d => d.Id == payment.DrugId).Price;
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return Json(new
                {
                    status = 1,
                    msg = "s: تمت عملية الاضافة بنجاح",
                    close = 1
                });
            }
            ViewData["DrugId"] = new SelectList(_context.Drug, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            ViewData["DrugId"] = new SelectList(_context.Drug, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DrugId,UserId,Id,Total,Quntity,CreatedAt,CreatedBy")] Payment payment)
        {
            if (id != payment.Id)
            {
                return NotFound();
            }
            ModelState.Remove("CreatedAt");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("Total");
            if (ModelState.IsValid)
            {
                try
                {
                    payment.UpdatedAt = DateTime.Now;
                    payment.UpdatedBy = UserId;
                    payment.Total = payment.Quntity * _context.Drug.Single(d => d.Id == payment.DrugId).Price;
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.Id))
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
            ViewData["DrugId"] = new SelectList(_context.Drug, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Category = await _context.Payment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Category == null)
            {
                return NotFound();
            }

            _context.Payment.Remove(Category);
            await _context.SaveChangesAsync();
            return Json(new
            {
                status = 1,
                msg = "s: تمت عملية الحذف بنجاح"
            });
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payment.FindAsync(id);
            _context.Payment.Remove(payment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payment.Any(e => e.Id == id);
        }
    }
}
