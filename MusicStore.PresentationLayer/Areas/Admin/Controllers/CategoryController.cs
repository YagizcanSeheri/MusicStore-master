using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.ApplicationLayer;
using MusicStore.DomainLayer.Entities;
using MusicStore.DomainLayer.UnitOfWork.Abstraction;

namespace MusicStore.PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =ProjectConstant.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
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
            var allCategories = _unitOfWork.Category.GetAll();
            return Json(new { data = allCategories });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var deleteCategory = _unitOfWork.Category.Get(id);
            if (deleteCategory == null) 
            { 
                return Json(new { success = false, message = "Category Not Found" }); 
            }
            else
            {
                _unitOfWork.Category.Remove(deleteCategory);
                _unitOfWork.Commit();
                return Json(new { success = true, message = "Category Deleted" });
            }
            
        }
        #endregion

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null)
            {
                return View(category);
            }
            category = _unitOfWork.Category.Get((int)id);
            if (category != null)
            {
                return View(category);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id==0)
                {
                    _unitOfWork.Category.Add(category);
                }
                else
                {
                    _unitOfWork.Category.Update(category);
                }
                _unitOfWork.Commit();
                return RedirectToAction("Index");
            }
            return View(category);
        }

    }
}
