using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class Manuscript
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="هذا الحقل ضروري")]
        [Display(Name = "العنوان باللغة العربية")]
        [StringLength(500)]
        public string ArTitle { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "العنوان باللغة الانجليزية")]
        [StringLength(500)]
        public string EnTitle { get; set; }

        [Display(Name = "العنوان المختصر")]
        [StringLength(500)]
        public string RunningTitle { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "الملخص باللغة الانجليزية")]
        [StringLength(2000, ErrorMessage = "الكلمات يجب أن تتراوح بين 10 -250 كلمة ",MinimumLength =50)]
        public string EnAbstract { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "الملخص باللغة العربية")]
        [StringLength(2000, ErrorMessage = "الكلمات يجب أن تتراوح بين 10 -250 كلمة ", MinimumLength = 50)]
        public string ArAbstract { get; set; }

        [Display(Name = "المؤلف المختص بالتواصل")]
        [StringLength(450)]
        public string CorrespondingAuthorId { get; set; }
        [Display(Name = "المؤلف المختص بالتواصل")]
        public  ApplicationUser CorrespondingAuthor { get; set; }
        
        [Display(Name = "تاريخ الاضافة")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        [Display(Name = "تاريخ الصلاحية")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DueDateforAuthorResponse { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "الكلمات المفتاحية")]
        [StringLength(100)]
        public string Keywords { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "مجال البحث الدقيق")]
        [StringLength(100)]
        public string Areas { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "نوع المقالة")]
        public int JournalArticleTypeId { get; set; }
        public JournalArticleType JournalArticleType { get; set; }

        [Display(Name = "العدد")]
        public int SubmittedforIssueId { get; set; }
        public  JournalIssue SubmittedforIssue { get; set; }

        [Display(Name = "التخصص")]
        public int SpecialityId { get; set; }
        public  Speciality Speciality { get; set; }

        [Display(Name = "المجلة")]
        public int JournalId { get; set; }
        public  Journal Journal { get; set; }

        [Display(Name = "حالة البحث")]
        public ManuscriptCurrentStatus CurrentStatus { get; set; }
        
   
        // IsDeleted True/False
        [Display(Name = "تم الحذف")]
        public bool IsDeleted { get; set; }

        // IsDeleted True/False
        [Display(Name = "مفتوحة الولوج")]
        public bool OpenAccess { get; set; }

        [Display(Name = "تفاصيل تمويل البحث")]
        [StringLength(500, ErrorMessage = "عدد الكلمات المدخلة تفوق الحد الأقصى", MinimumLength = 0)]
        public string FundingInfo { get; set; }

        [Display(Name = "الصورة التعبيرية")]
        [StringLength(100)]
        public string GraphicalAbstract { get; set; }
    }
}
