using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class Volume
    {
        [Key]
        public int Id { get; set; }

        [StringLength(5)]
        [Display(Name = "السنة")]
        public string Year { get; set; }

        [StringLength(20)]
        [Display(Name = "الإسم")]
        public string Name { get; set; }

        [StringLength(2000)]
        [Display(Name = "الوصف")]
        public string Description { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "المجلة")]
        public int JournalId { get; set; }
        public Journal Journal { get; set; }

        [Display(Name = "منشور")]
        public bool IsPublished { get; set; }
    }
}
