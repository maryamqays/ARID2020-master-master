using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class PublicationTitle
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 5)]
        [Display(Name = "اسم المجلة باللغة العربية")]
        public string ArName { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 5)]
        [Display(Name = "اسم المجلة باللغة الانكليزية")]
        public string EnName { get; set; }

        [StringLength(5000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "الوصف")]
        public string ArDescription { get; set; }



        [StringLength(250, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "الغلاف")]
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

        [Display(Name = "تاريخ انشاء المجلة")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime EstablishmentDate { get; set; }

        [Display(Name = "تاريخ الانشاء")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfRecord { get; set; }

        [Display(Name = "مقدم الطلب")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name = "التخصص")]
        public int SpecialityId { get; set; }
        public  Speciality Speciality { get; set; }

        [Required]
        [Display(Name = "البلد")]
        public int CountryId { get; set; }
        public  Country Country { get; set; }

        [Required]
        [Display(Name = "رئيس التحرير")]
        [StringLength(400)]
        public string EditorinChief { get; set; }

        [Required]
        [Display(Name = "الناشر")]
        public int PublisherId{ get; set; }
        public Publisher Publisher { get; set; }

        [Display(Name = "تمت الموافقة")]
        public bool IsAdminAccepted { get; set; }

        [Display(Name = " غير مرئي")]
        public bool IsVisible { get; set; }

        [Display(Name = "مميز")]
        public bool IsFeatured { get; set; }

        [Display(Name = "ISI Indexed")]
        public bool IsIIndexed{ get; set; }

        [Display(Name = "'Scopus'هل المجلة ضمن تصنيف ")]
        public bool IsScopusIndexed { get; set; }

        [Display(Name = "هل للمجلة معامل تأثير")]
        public bool IsImpactFactor{ get; set; }

        [Display(Name = "الرابط")]
        [StringLength(500, ErrorMessage = "العنوان طويل")]
        public string Website { get; set; }

        [Display(Name = "رابط فيسبوك")]
        [StringLength(500, ErrorMessage = "العنوان طويل")]
        public string Fb { get; set; }

        [Display(Name = "رابط تويتر")]
        [StringLength(500, ErrorMessage = "العنوان طويل")]
        public string Twitter { get; set; }

        [Display(Name = "معلومات التواصل")]
        [StringLength(500, ErrorMessage = "العنوان طويل")]
        public string ContactUs { get; set; }

        [Display(Name = "اللغات")]
        [StringLength(100, ErrorMessage = "العنوان طويل")]
        public string Languages{ get; set; }

        [Display(Name = "الكلمات المفتاحية")]
        [StringLength(100, ErrorMessage = "العنوان طويل")]
        public string Tags { get; set; }


        [Required]
        [Display(Name = "الحالة")]
        public bool Status { get; set; }

        [Display(Name = "نوع البلاغ")]
        public ReportType ReportType { get; set; }

        [Display(Name = "نوع النشر")]
        public PublicationTypes PublicationTypes { get; set; }


    }
}

