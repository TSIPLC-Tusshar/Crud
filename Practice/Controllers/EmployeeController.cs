using Microsoft.AspNetCore.Mvc;

using Practice.Models;
using Practice.Services;
using Practice.Services.Interfaces;

namespace Practice.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService service;
        public EmployeeController(IEmployeeService employeeService)
        {
            service = employeeService;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            var data = await service.GetEmployees();
            return View(data);
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeDataModel = await service.GetEmployeeById(id.Value);
            if (employeeDataModel == null)
            {
                return NotFound();
            }

            return View(employeeDataModel);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmpNo,Name,Phone,Email")] EmployeeDataModel employeeDataModel)
        {
            if (ModelState.IsValid)
            {
                await service.Create(employeeDataModel);
                return RedirectToAction(nameof(Index));
            }
            return View(employeeDataModel);
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeDataModel = await service.GetEmployeeById(id.Value);
            if (employeeDataModel == null)
            {
                return NotFound();
            }
            return View(employeeDataModel);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmpNo,Name,Phone,Email")] EmployeeDataModel employeeDataModel)
        {
            if (id != employeeDataModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await service.UpdateEmployee(id, employeeDataModel);
                }
                catch (Exception)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employeeDataModel);
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeDataModel = await service.GetEmployeeById(id.Value);
            if (employeeDataModel == null)
            {
                return NotFound();
            }

            return View(employeeDataModel);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await service.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
