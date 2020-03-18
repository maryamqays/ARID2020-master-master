using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class Publisher
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 5)]
        [Display(Name = "الاسم بالعربي")]
        public string ArName { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 5)]
        [Display(Name = "الاسم بالانجليزي")]
        public string EnName { get; set; }

        [StringLength(5000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "الوصف")]
        public string Description { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(50, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "البريد الالكتروني")]
        public string Email { get; set; }

              [Display(Name = "مقدم الطلب")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name = "الحالة")]
        public bool Status { get; set; }

        [Display(Name = "تاريخ الانشاء")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime EstablishmentDate { get; set; }

        [Display(Name = "تاريخ الاضافة")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfRecord { get; set; }

        [Required]
        [Display(Name = "التخصص")]
        public int SpecialityId { get; set; }
        public  Speciality Speciality { get; set; }

        [Required]
        [Display(Name = "البلد")]
        public int CountryId { get; set; }
        public  Country Country { get; set; }

        [Display(Name = "العنوان")]
        [StringLength(500, ErrorMessage = "العنوان طويل")]
        public string Address { get; set; }
        
        [Display(Name = "الموقع الالكتروني")]
        [StringLength(500, ErrorMessage = "العنوان طويل")]
        public string Website { get; set; }

        [Display(Name = "Facebook")]
        [StringLength(500, ErrorMessage = "العنوان طويل")]
        public string Fb { get; set; }

        [Display(Name = "Twitter")]
        [StringLength(500, ErrorMessage = "العنوان طويل")]
        public string Twitter { get; set; }

        [Display(Name = "معلومات التواصل")]
        [StringLength(500, ErrorMessage = "العنوان طويل")]
        public string ContactUs { get; set; }

        [Display(Name = "مميز")]
        public bool IsFeatured { get; set; }

        [Display(Name = "نوع البلاغ")]
        public ReportType ReportType { get; set; }

        [StringLength(250, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "الشعار")]
        public string Logo { get; set; }

        [Display(Name = " حالة الظهور")]
        public bool IsVisible { get; set; }

              [Display(Name = "رقم التسجيل")]
        [StringLength(100, ErrorMessage = "العنوان طويل")]
        public string RegistrationNumber { get; set; }
        
                [Display(Name = "اللغات")]
        [StringLength(100, ErrorMessage = "العنوان طويل")]
        public string Languages { get; set; }
        
        [Display(Name = "الكلمات المفتاحية")]
        [StringLength(100, ErrorMessage = "الكلمات المدخلة طويلة")]
        public string Tags { get; set; }

                [Display(Name = "موافقة الادارة")]
        public bool IsAdminAccepted { get; set; }
    }
}
