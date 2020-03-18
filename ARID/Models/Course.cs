using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    //we need to add periodic(time) section announcement after 7 days and also email content for each section >>>that will be send to register number
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 5)]
        [Display(Name = "إسم الدورة  باللغة العربية")]
        public string ArName { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 5)]
        [Display(Name = "إسم الدورة  باللغة الانكليزية")]
        public string EnName { get; set; }

        [Display(Name = "مقدم الطلب")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "تاريخ البدء ")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime StartingDate { get; set; }

        [Display(Name = "تاريخ الإنتهاء ")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime EndingDate { get; set; }

        [Display(Name = "تاريخ الاضافة")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfRecord { get; set; }

        [Display(Name = "تاريخ اخر تحديث")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime LastUpdate { get; set; }

        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "صورة")]
        public string Image { get; set; }

        [StringLength(50, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "رابط فيديو تعريفي")]
        public string IntroductoryVideo { get; set; }

        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "نشرة إعلانية")]
        public string Flyer { get; set; }

        [StringLength(250, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "ملف Pdf")]
        public string FilePdf { get; set; }

        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        [Display(Name = "الجهد المطلوب")]
        [StringLength(100, ErrorMessage = "العنوان طويل")]
        public string Effort { get; set; }

        [Display(Name = "مدفوعة الثمن")]
        public bool IsPaid { get; set; }

        [Display(Name = "رسوم الدورة")]
        //[DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal CourseFees { get; set; }


        [Display(Name = "نوع الشهادة")]
        public Certificate Certificate { get; set; }

        [Display(Name = "رسوم الشهادة")]
        //[DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal CertificateCost { get; set; }

        [Display(Name = "تمت الموافقة")]
        public bool IsAdminApproved { get; set; }

        [Display(Name = "هل الدورة متاحة للتسجيل")]
        public bool IsActive { get; set; }

        [Display(Name = "مميزة")]
        public bool IsFeatured { get; set; }

        [Required(ErrorMessage = "تفاصيل الدورة مطلوبة")]
        [StringLength(5000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "تفاصيل الدورة")]
        public string Overview { get; set; }

        [Required(ErrorMessage = "المقدمة مطلوبة")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "مقدمة")]
        public string Introduction { get; set; }

        [Required(ErrorMessage = "مخرجات الدورة مطلوبة")]
        [StringLength(5000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "مخرجات الدورة")]
        public string LearningOutcomes { get; set; }

        [Required(ErrorMessage = "متطلبات دخول الدورة مطلوبة")]
        [StringLength(5000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "متطلبات دخول الدورة")]
        public string Requirements { get; set; }

        [Required(ErrorMessage = "الكلمات المفتاحية مطلوبة")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "الكلمات المفتاحية")]
        public string Tags { get; set; }

        [Required]
        [Display(Name = "التخصص")]
        public int SpecialityId { get; set; }
        public Speciality Speciality { get; set; }

        [Required(ErrorMessage = "الفئة المستهدفة مطلوبة")]
        [StringLength(5000, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = " الفئة المستهدفة")]
        public string TargetStudents { get; set; }

        [Required(ErrorMessage = "التواريخ المهمة مطلوبة")]
        [StringLength(100, ErrorMessage = "عدد الحروف يجب أن يكون أقل من 100 حرف", MinimumLength = 0)]
        [Display(Name = " التواريخ المهمة")]
        public string ImportantDates { get; set; }

        [Required]
        [Display(Name = "اللغة")]
        public Language Language { get; set; }

        [Display(Name = "مسجلة مسبقا")]
        public bool IsPrerecorded { get; set; }

        [Display(Name = "محذوفة")]
        public bool IsDeleted { get; set; }

        [Display(Name = "مخفية")]
        public bool Ishidden { get; set; }

        [Display(Name = "مبلغ عنه")]
        public bool IsReported { get; set; }

        [Display(Name = "خاص")]
        public bool IsPrivate { get; set; }

        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        [Display(Name = "كلمة المرور")]
        public string AccessPassword { get; set; }

        [Display(Name = "درجة النجاح")]
        public int PassingMark { get; set; }
    }
}
