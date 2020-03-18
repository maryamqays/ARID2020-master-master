using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class RegistrationForm
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "اسم الاستمارة")]
        [StringLength(500)]
        public string Name { get; set; }
        
        [Display(Name = "الوصف")]
               public string Description { get; set; }

        [Display(Name = "صيغة الدعوة")]
        [StringLength(2000)]
        public string InvitationContext { get; set; }

        [Display(Name = "ترويسة الدعوة")]
        [StringLength(50)]
        public string InvitationHeader { get; set; }

        [Display(Name = "ذيل الدعوة")]
        [StringLength(50)]
        public string InvitationFooter { get; set; }

        [Display(Name = "تضمين رابط تحميل الدعوة")]
        public bool IsPdfInvitation { get; set; }

        [Display(Name = "الحالة")]
        public bool IsActive { get; set; }

        [Display(Name = "مطلوب وسام ؟")]
        public bool IsEntBadgeRequired { get; set; }

        [Display(Name = "الباحث")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        [Display(Name = "الباحث")]
        public  ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name = "تاريخ الانشاء")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        [Required]
        [Display(Name = "تاريخ الحدث")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Display(Name = "EmailContentId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public int EmailContentId { get; set; }
        public  EmailContent EmailContent { get; set; }

        [Required]
        [Display(Name = "تاريخ النفاذ")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [Display(Name = "عدد ايام التذكير")]
        public int ReminderDays { get; set; }

        [Display(Name = "EmailContentReminderId")] //send reminder 48 hours prior to ExpiryDate
        [Required(ErrorMessage = "RequiredFieldError")]
        public int EmailContentReminderId { get; set; }
        public  EmailContent EmailContentReminder { get; set; }
    }
}
