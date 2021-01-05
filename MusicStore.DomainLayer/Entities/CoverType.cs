using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MusicStore.DomainLayer.Entities
{
    public class CoverType
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="Cover Type")]
        public string Name { get; set; }
    }
}
