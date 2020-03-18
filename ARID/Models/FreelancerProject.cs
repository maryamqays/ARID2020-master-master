using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class FreelancerProject
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [StringLength(200, ErrorMessage = "هذا الحقل يجب ان يختوي من 10 الى 100 حرف", MinimumLength = 10)]
        [Display(Name = "إسم المشروع")]
        public string Name { get; set; }

        [Display(Name = "تفاصيل المشروع")]
        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [StringLength(4000, ErrorMessage = "هذا الحقل يجب ان يختوي من 30 الى 100 حرف", MinimumLength = 30)]
        public string Description { get; set; }

        [Display(Name = "الخدمة")]
        public Guid FreelancerReadyServiceId { get; set; }

        [Display(Name = "تاريخ الاضافة")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfRecord { get; set; }

        [Display(Name = "صاحب المشروع")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "إخفاء تفاصيل صاحب المشروع")]
        public bool HideProjectOwnerDetails { get; set; }

        [StringLength(100)]
        [Display(Name = "الملف المرفق - اختياري")]
        public string File { get; set; }

        [StringLength(1000)]
        [Display(Name = "رسالة إدارية")]
        public string AdminNotification { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "التخصص")]
        public int SpecialityId { get; set; }
        [Display(Name = "التخصص")]
        public Speciality Speciality { get; set; }


        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "المهارات")]
        public string Skills { get; set; }


        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "المبلغ التقريبي")]
        public BugetType BugetType { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "الغرض من هذا المشروع")]
        public PurposeType PurposeType { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "فئة مهارات المشروع")]
        public SkillCategoryType SkillCategoryType { get; set; }

        //[Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "تعيين وكيل للبحث عن العميل المناسب")]
        [DefaultValue(false)]
        public RecruiterProjectType RecruiterProjectType { get; set; }

        [Display(Name = "مدة التنفيذ (يوم)")]
        [Range(1, 90)]
        [Required(ErrorMessage = "هذا الحقل ضروري")]
        public int DaysRequired { get; set; }

        [Display(Name = "$ المبلغ التقريبي")]
        public int FixedPrice { get; set; }

        [Display(Name = "حالة المشروع")]
        public FreelancerProjectStatus FreelancerProjectStatus { get; set; }

        //[Display(Name = "الرسوم القطعية")]
        //public int FixedPrice { get; set; }

        public static string GetDate(DateTime yourDate)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.Now.Ticks - yourDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "الان" : "منذ " + ts.Seconds + " ث";

            if (delta < 2 * MINUTE)
                return "منذ دقيقة ";

            if (delta < 45 * MINUTE)
                return "منذ " + ts.Minutes + " د";

            if (delta < 90 * MINUTE)
                return "منذ ساعة";

            if (delta < 24 * HOUR)
                return "منذ " + ts.Hours + " ساعة";

            if (delta < 48 * HOUR)
                return "منذ الأمس";

            if (delta < 30 * DAY)
                return "منذ " + ts.Days + " يوم";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "منذ شهر" : "منذ " + months + " شهر";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "منذ سنة" : "منذ " + years + " سنة";
            }
        }
        public static int GetHours(DateTime yourDate)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;

            var ts = new TimeSpan(DateTime.Now.Ticks - yourDate.Ticks);
            int delta =(int)ts.TotalSeconds;



            return delta/HOUR;
           
        }
        public static string GetRemainingTime(DateTime yourDate)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(yourDate.Ticks - DateTime.Now.Ticks);
            double delta = ts.TotalSeconds;
            if (delta < 1)
                return "إنتهى الوقت";

            if (delta < 2 * MINUTE)
                return "دقيقة ";

            if (delta < 45 * MINUTE)
                return "" + ts.Minutes + " دقيقة";

            if (delta < 90 * MINUTE)
                return "ساعة";

            if (delta < 24 * HOUR)
                return "" + ts.Hours + " ساعة";

            if (delta < 48 * HOUR)
                return "يوم";

            if (delta < 30 * DAY)
                return "" + ts.Days + "يوم";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "شهر" : "" + months + " شهر";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "سنة" : "" + years + " سنة";
            }
        }
    }
}
