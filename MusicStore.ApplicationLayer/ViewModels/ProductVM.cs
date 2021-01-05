using Microsoft.AspNetCore.Mvc.Rendering;
using MusicStore.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicStore.ApplicationLayer.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }

        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SelectListItem> CoverTypeList { get; set; }
    }
}
