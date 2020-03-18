using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class Submission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "الورقة البحثية")]
        public int ManuscriptId { get; set; }
        public  Manuscript Manuscript { get; set; }

        [Display(Name = "المحرر المكلف")]
        public int? AEId { get; set; }
        public  AreaEditor AE { get; set; }

        [Display(Name = "تاريخ التقديم")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfSubmission { get; set; }

        [Display(Name = "استجابة المؤلف")]
        public int? ResponsetoDecision { get; set; }

        [Display(Name = "قرار رئيس التحرير")] //(Accept,Minor rev, Major rev, Revise for lang editing, Reject & resubmit, Reject, Reject with option tp appeal, Reject Transfer another journal in ARID )
        public EICDecision EICDecision { get; set; }

        [Display(Name = "حالة التقديم")] 
        public SubmissionStatus SubmissionStatus { get; set; }

        [Display(Name = "تاريخ القرار")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? DecisionDate { get; set; }

        [Display(Name = "قرار رئيس التحرير")]
        [StringLength(1000)]
        public string EicDecisionText { get; set; }

        [Display(Name = "رسالة القرار")]
               public string DecisionLetter { get; set; }

        [Display(Name = "اقرار عدم الاستلال")]
        public bool PlagiarismConfirmation { get; set; }

        [Display(Name = "اقرار بعدم الارسال لمجلة ثانية")]
        public bool DuplicateSubmissionConfirmation { get; set; }

        [Display(Name = "رسالة التقديم لرئيس التحرير")]
        [StringLength(2000)]
        public string CoverLetter { get; set; }

        [Display(Name = "قرار مساعد رئيس التحرير")] //(Accept,Minor rev, Major rev, Revise for lang editing, Reject & resubmit, Reject, Reject with option tp appeal, Reject Transfer another journal in ARID )
        public EICDecision AEDecision { get; set; }

        [Display(Name = "تاريخ قرار مساعد رئيس التحرير")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? AEDecisionDate { get; set; }

        [Display(Name = "قرار مساعد رئيس التحرير")]
        [StringLength(1000)]
        public string AEDecisionText { get; set; }

    }
}
