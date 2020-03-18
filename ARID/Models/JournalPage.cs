using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class JournalPage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "المجلة")]
        public int JournalId { get; set; }
        public  Journal Journal { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "عنوان الصفحة")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "المحتوى")]
        public string Body { get; set; }

        [Required]
        [Display(Name = "الاتجاه")]
        public bool Direction { get; set; }

        [Required]
        [Display(Name = "تاريخ الاضافة")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime Releasedate { get; set; }
    }
}
