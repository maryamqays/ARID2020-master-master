using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class JournalIssue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "اسم العدد")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "تاريخ الاصدار")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime Releasedate { get; set; }

        [StringLength(100)]
        [Display(Name = "الغلاف")]
        public string Cover { get; set; }

        
        [StringLength(100)]
        [Display(Name = "تحميل PDF")]
        public string Pdf { get; set; }

        [Required]
        [Display(Name = "التسلسل")]
        public int Number { get; set; }

        [Required]
        [Display(Name = "المجلة")]
        public int JournalId { get; set; }
        public  Journal Journal { get; set; }

        [Required]
        [Display(Name = "المجلد")]
        public int VolumeId { get; set; }
        public Volume Volume { get; set; }
    
        [Display(Name = "منشور")]
        public bool IsPublished { get; set; }

        [Display(Name = "يستقبل بحوث")]
        public bool IsOpen { get; set; }

        [Display(Name = "عدد الزوار")]
        public int Visitors { get; set; }

        [Display(Name = "عدد مرات التحميل")]
        public int PdfDownloadCounter { get; set; }


        [StringLength(2500)]
        [Display(Name = "الوصف")]
        public string Description { get; set; }
    }
}
