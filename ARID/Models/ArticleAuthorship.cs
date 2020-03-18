using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class ArticleAuthorship
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "الورقة البحثية")]
        public int ManuscriptId { get; set; }
        public  Manuscript Manuscript { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "الدولة")]
        public int CountryId { get; set; }
        public  Country Country { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "المؤسسة")]
        public int UniversityId { get; set; }
        public  University University { get; set; }

        [Display(Name = "المؤلف")]
        [StringLength(450)]
        public string AuthorUserId { get; set; }
        public ApplicationUser AuthorUser { get; set; }

        [Display(Name = "تاريخ التقديم")]
        [DataType(DataType.Date, ErrorMessage = "DateError")]
        public DateTime DateOfRecord { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "التسلسل")]
        public int Indx { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [StringLength(50)]
        [Display(Name = "البريد الالكتروني")]
        public string Email { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [StringLength(1000)]
        [Display(Name = "تفاصيل المساهمة")]
        public string Contribution { get; set; }

    }
}
