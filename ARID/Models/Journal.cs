using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class Journal
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 5)]
        [Display(Name = " إسم المجلة باللغة العربية")]
        public string ArName { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 5)]
        [Display(Name = "إسم المجلة باللغة الإنجليزية")]
        public string EnName { get; set; }

        [StringLength(5000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "الوصف باللغة العربية")]
        public string ArDescription { get; set; }

        [StringLength(5000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "الوصف باللغة الانجليزية")]
        public string EnDescription { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(10, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "المختصر اللاتيني")]
        public string ShortName { get; set; }


        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "الشعار")]
        public string Logo { get; set; }


        [StringLength(10, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "P-ISSN")]
        public string PISSN { get; set; }


        [StringLength(10, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "ISSN")]
        public string EISSN { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(50, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "بريد المجلة")]
        public string Email { get; set; }

        [Display(Name = "تاريخ التفعيل")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime ActivationDate { get; set; }

        [Display(Name = "مقدم الطلب")]
        [StringLength(450)]
        public string CreatedByUserId { get; set; }
        public  ApplicationUser CreatedByUser { get; set; }

        [Required]
        [Display(Name = "رئيس التحرير")]
        [StringLength(450)]
        public string EiCId { get; set; }
        [Display(Name = "رئيس التحرير")]
        public  ApplicationUser EiC { get; set; }

        [Required]
        [Display(Name = "الحالة")]
        public JournalStatus JournalStatus { get; set; }

        [Display(Name = "عدد الزوار")]
        public int Visitors { get; set; }

        [Display(Name = "شهادة المحكم")]
        [StringLength(100)]
        public string ReviewerCertificateBackgroundFile { get; set; }

        [Display(Name = "ترويسة المجلة")]
        [StringLength(100)]
        public string JournalHeader { get; set; }

        [Display(Name = "شهادة المؤلف")]
        [StringLength(100)]
        public string AuthorCertificateBackgroundFile { get; set; }

        [Display(Name = "الناشر")]
        [Required]
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }
    }
}
