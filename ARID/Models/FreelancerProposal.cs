using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class FreelancerProposal
    {
        [Key]
        public int Id { get; set; }

        [StringLength(450)]
        [Display(Name = "مقدم الطلب")]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "هل الإسم مخفي")]
        public bool IsNameVisible { get; set; }

        [Display(Name = "هل يكون العرض غير مرئي الا من قبل صاحب المشروع")]
        public bool IsPrivate { get; set; }

        [Display(Name = "هل تم اختياره من قبل صاحب المشروع")]
        public bool IsAssigned { get; set; }

        [Display(Name = "مبلغ عنه")]
        public bool IsReported { get; set; }

        [Display(Name = "مرئي")]
        public bool Isvisible { get; set; }

        [StringLength(450)]
        [Display(Name = "الشخص الذي قام بالتبليغ")]
        public string ReportedByUserId { get; set; }
        public  ApplicationUser ReportedByUser { get; set; }

        [Display(Name = "تفاصيل العرض")]
        [Required(ErrorMessage = "هذاالحقل ضروري")]
        [StringLength(5000, ErrorMessage = "هذا الحقل يجب ان يحتوي 30 الى 5000 حرف", MinimumLength = 30)]
        public string Description { get; set; }

        [Required(ErrorMessage = "هذاالحقل ضروري")]
        [Display(Name = "مدة التسليم (يوم)")]
        public int DaysRequired { get; set; }


        [Display(Name = "المشروع")]
        public int FreelancerProjectId { get; set; }
        [Display(Name = "المشروع")]
        public FreelancerProject FreelancerProject { get; set; }

        [Display(Name = "تاريخ الاضافة")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateofRecord { get; set; }

        [Display(Name = "تاريخ التعيين")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfAssignment { get; set; }

        [Display(Name = "تاريخ التسليم")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DeliveredDate { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "قيمة العرض($)")]
        public int BidAmount { get; set; }

        [Display(Name = "حالة العرض")]
        public ProposalStatus ProposalStatus { get; set; }
    }
}
