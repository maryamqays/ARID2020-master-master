using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class IssueResearch
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "عنوان البحث بالعربي")]
        [StringLength(500)]
        public string ArTitle { get; set; }

        [Display(Name = "عنوان البحث بالانجليزي")]
        [StringLength(500)]
        public string EnTitle { get; set; }

        [Display(Name = "الملخص العربي")]
        public string ArAbstract { get; set; }

        [Display(Name = "الملخص الانجليزي")]
        public string EnAbstract { get; set; }

        [Display(Name = "تحميل PDF")]
        [StringLength(500)]
        public string Pdf { get; set; }

        [Display(Name = "التسلسل")]
        public int Rank { get; set; }

        [Display(Name = "المؤلف المعني بالتواصل")]
        [StringLength(450)]
        public string CorrespondingAuthorId { get; set; }
        [Display(Name = "المؤلف المعني بالتواصل")]
        public  ApplicationUser CorrespondingAuthor { get; set; }

        [Display(Name = "المؤلفون الاخرون")]
        [StringLength(2000)]
        public string Authors { get; set; }

        [Required]
        [Display(Name = "العدد")]
        public int JournalIssueId { get; set; }
        public  JournalIssue JournalIssue { get; set; }

      
    }
}
