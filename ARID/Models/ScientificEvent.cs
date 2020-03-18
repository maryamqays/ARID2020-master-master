using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class ScientificEvent
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "اسم الحدث باللغة العربية")]
        [Required(ErrorMessage = "حقل الاسم باللغة العربية مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم طويل")]
        public string EventNameAr { get; set; }

        [Display(Name = "اسم الحدث باللغة الانكليزية")]
        [StringLength(200, ErrorMessage = "الاسم طويل")]
        public string EventNameEn { get; set; }

        [Required(ErrorMessage = "يجب تحديد نوع الحدث")]
        [Display(Name = "نوع الحدث")]
        public EventType EventType { get; set; }

        [Required(ErrorMessage = "يجب تحديد بلد اقامة الحدث")]
        [Display(Name = "البلد")]
        public int CountryId { get; set; }
        [Display(Name = "البلد")]
        public Country Country { get; set; }

        [Required(ErrorMessage = "عنوان اقامة الحدث مطلوب")]
        [Display(Name = "عنوان انعقاد الحدث")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000, ErrorMessage = "اسم المكان طويل")]
        public string Venue { get; set; }

        [Display(Name = "نوع البلاغ")]
        public ReportType ReportType { get; set; }


        [StringLength(100)]
        [Display(Name = "صورة/شعار الحدث")]
        public string Image { get; set; }

        [Display(Name = "مميزات لاعضاء منصة اريد")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000, ErrorMessage = "النص طويل")]
        public string AridPrivileges { get; set; }

        [Display(Name = "تاريخ الاضافة الى الدليل")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ الحدث")]
        [Required(ErrorMessage = "تاريخ بدء الحدث مطلوب")]
        public DateTime EventDate { get; set; }

        [Display(Name = "المجال")]
        [Required(ErrorMessage = "مجال تخصص الحدث مطلوب")]
        public int SpecialityId { get; set; }
        [Display(Name = "المجال")]
        public Speciality Speciality { get; set; }

        [Display(Name = "الجهات المنظمة")]
        [StringLength(500, ErrorMessage = "اسم الجهات المنظمة طويل")]
        [Required(ErrorMessage = "يجب تحديد الجهة/الجهات المنظمة للحدث")]
        public string Organizers { get; set; }


        [Display(Name = "المستخدم")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        [Display(Name = "المستخدم")]
        public  ApplicationUser ApplicationUser { get; set; }

        [Required(ErrorMessage = "يجب تحديد تفاصيل التواصل والتسجيل")]
        [Display(Name = "تفاصيل التواصل والتسجيل")]
        [StringLength(500, ErrorMessage = "النص طويل ")]
        public string ContactDetails { get; set; }

        [Display(Name = "مميز")]
        public bool IsFeatured { get; set; }

        [Display(Name = "ظاهر")]
        public bool IsVisible { get; set; }

        [Display(Name = "تمت الموافقة")]
        public bool IsAccepted { get; set; }

        [Display(Name = "رابط الحدث")]
        [StringLength(500, ErrorMessage = "العنوان طويل")]
        public string Website { get; set; }

        [Display(Name = "هل انت صاحب قرار في هذا الحدث؟")]
        public bool IsDecisionMaker { get; set; }

        [Display(Name = "تفاصيل/المحاور/الاهداف")]
        [DataType(DataType.MultilineText)]
              public string Description { get; set; }
    }
}
