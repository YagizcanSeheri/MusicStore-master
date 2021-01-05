using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.ApplicationLayer;
using MusicStore.DomainLayer.Entities;
using MusicStore.DomainLayer.UnitOfWork.Abstraction;

namespace MusicStore.PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ProjectConstant.Role_Admin+","+ ProjectConstant.Role_Emp)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region APICALLS
        public IActionResult GetAll()
        {
            var allCompanies = _unitOfWork.Company.GetAll();
            return Json(new { data = allCompanies });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var deleteCompanies = _unitOfWork.Company.Get(id);
            if (deleteCompanies == null)
            {
                return Json(new { success = false, message = "Company Not Found" });
            }
            else
            {
                _unitOfWork.Company.Remove(deleteCompanies);
                _unitOfWork.Commit();
                return Json(new { success = true, message = "Company Deleted" });
            }

        }
        #endregion

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id == null)
            {
                return View(company);
            }
            company = _unitOfWork.Company.Get((int)id);
            if (company != null)
            {
                return View(company);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                }
                _unitOfWork.Commit();
                return RedirectToAction("Index");
            }
            return View(company);
        }
    }
}
