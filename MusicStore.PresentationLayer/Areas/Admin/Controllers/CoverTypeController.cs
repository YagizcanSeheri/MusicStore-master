using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.ApplicationLayer;
using MusicStore.DomainLayer.Entities;
using MusicStore.DomainLayer.UnitOfWork.Abstraction;

namespace MusicStore.PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ProjectConstant.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
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
            var allData = _unitOfWork.CoverTypeRepository.GetAll();
            return Json(new { data = allData });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var deleteData = _unitOfWork.CoverTypeRepository.Get(id);
            if (deleteData == null)
            {
                return Json(new { success = false, message = " Not Found" });
            }
            else
            {
                _unitOfWork.CoverTypeRepository.Remove(deleteData);
                _unitOfWork.Commit();
                return Json(new { success = true, message = " Deleted" });
            }

        }
        #endregion

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null)
            {
                return View(coverType);
            }
            coverType = _unitOfWork.CoverTypeRepository.Get((int)id);
            if (coverType != null)
            {
                return View(coverType);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                if (coverType.Id == 0)
                {
                    _unitOfWork.CoverTypeRepository.Add(coverType);
                }
                else
                {
                    _unitOfWork.CoverTypeRepository.Update(coverType);
                }
                _unitOfWork.Commit();
                return RedirectToAction("Index");
            }
            return View(coverType);
        }

    }
}
