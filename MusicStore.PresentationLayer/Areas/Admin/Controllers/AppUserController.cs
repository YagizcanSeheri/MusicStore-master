using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.ApplicationLayer;
using MusicStore.DomainLayer.Entities;
using MusicStore.DomainLayer.UnitOfWork.Abstraction;
using MusicStore.InfrastructureLayer.Data;
using System;
using System.Linq;

namespace MusicStore.PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ProjectConstant.Role_Admin)]
    public class AppUserController : Controller
    {

        private readonly ApplicationDbContext _context;
        public AppUserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region APICALLS
        public IActionResult GetAll()
        {
            var userList = _context.AppUsers.Include(c => c.Company).ToList();
            var userRole = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();
            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(x => x.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(x => x.Id == roleId).Name;

                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = userList });

        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var data = _context.AppUsers.FirstOrDefault(x=> x.Id == id);
            if (data == null)
                 return Json(new { success = false, message = "Error while locking/unlocking" });
            if (data.LockoutEnd != null && data.LockoutEnd> DateTime.Now)
            {
                data.LockoutEnd = DateTime.Now;
            }
            else
            {
                data.LockoutEnd = DateTime.Now.AddYears(5);
            }

            _context.SaveChanges();
            return Json(new { success = true, message = "Operation is Successful" });
        }

        #endregion


    }
}
