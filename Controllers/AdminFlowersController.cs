﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom19_WebBanHoa.Models;

namespace Nhom19_WebBanHoa.Controllers
{
    public class AdminFlowersController : Controller
    {
        private readonly FlowerContext _context;

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "admin";
        }

        public AdminFlowersController(FlowerContext context)
        {
            _context = context;
        }

        // GET: AdminFlowers
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View(await _context.Flowers.ToListAsync());
        }

        // GET: AdminFlowers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();

            var flower = await _context.Flowers.FirstOrDefaultAsync(m => m.Id == id);
            if (flower == null) return NotFound();

            return View(flower);
        }

        // GET: AdminFlowers/Create
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: AdminFlowers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ten,MoTa,Gia,HinhAnh")] Flower flower)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                _context.Add(flower);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(flower);
        }

        // GET: AdminFlowers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();

            var flower = await _context.Flowers.FindAsync(id);
            if (flower == null) return NotFound();

            return View(flower);
        }

        // POST: AdminFlowers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ten,MoTa,Gia,HinhAnh")] Flower flower)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id != flower.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flower);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlowerExists(flower.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(flower);
        }

        // GET: AdminFlowers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();

            var flower = await _context.Flowers.FirstOrDefaultAsync(m => m.Id == id);
            if (flower == null) return NotFound();

            return View(flower);
        }

        // POST: AdminFlowers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var flower = await _context.Flowers.FindAsync(id);
            if (flower != null)
            {
                _context.Flowers.Remove(flower);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlowerExists(int id)
        {
            return _context.Flowers.Any(e => e.Id == id);
        }
    }
}
