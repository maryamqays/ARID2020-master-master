using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class SubmissionReviewer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "الارسال")]
        public int SubmissionId { get; set; }
        public Submission Submission { get; set; }

        [Required]
        [Display(Name = "المحكم")]
        [StringLength(450)]
        public string ReviewerUserId { get; set; }
        public ApplicationUser ReviewerUser { get; set; }

        [Display(Name = "تاريخ التسليم المفترض")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Display(Name = "حالة المحكم")] //يتم التحكم بها من قبل رئيس التحرير
        public ReviewerStatus ReviewerStatus { get; set; }

        //IsCertificateofReviewingGranted
        [Display(Name = "منح شهادة اكمال التحكيم ؟")]
        public bool IsCertificateOfReviewingGranted { get; set; }

        //CertificateofReviewingGrantedDate
        [Display(Name = "تاريخ اصدار شهادة اكمال المراجعة")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateRevisionCertificae { get; set; }

        [StringLength(2000)]
        [Display(Name = "تعليمات رئيس التحرير/مدير القسم للمحكم")]
        public string EICInstructions { get; set; }

        [StringLength(2000)]
        [Display(Name = "تعليق المحكم الى ادارة المجلة")]
        public string ReviewerInstructionsToEIC { get; set; }

        [StringLength(5000)]
        [Display(Name = "تعليق المحكم الى الباحث")]
        public string ReviewerInstructionsToAuthor { get; set; }

        [Display(Name = "تاريخ الاسناد")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DateOfRecord { get; set; }

        [Display(Name = "تاريخ التحكيم")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime ReviewDate { get; set; }

        [Display(Name = "مسند المحكم")]
        [StringLength(450)]
        public string CieAeUserId { get; set; }
        public  ApplicationUser CieAeUser { get; set; }


        [Display(Name = "تاريخ القرار بالموافق/ الرفض لتحكيم البحث ")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime DecisionDate { get; set; }

        [Display(Name = "قرار المحكم لتحكيم البحث؟")]
        public ReviewerDecisionToReview ReviewerDecisionToReview { get; set; }

        [Display(Name = "حالة ولوج المحكم")]
        public bool IsAccess { get; set; }

        [Display(Name = "هل المعلومات التي تضمنها البحث جديدة؟")]
        public bool IsNewInfo { get; set; }

        [Display(Name = "هل موضوع الورقة متوافق مع نطاق المجلة؟")]
        public bool IsWithinJournalScope { get; set; }

        //Does the abstract bring out the main scope of the paper?
        [Display(Name = "هل يبرز الملخص النطاق الرئيسي للورقة؟")]
        public bool AbstractCompatibility { get; set; }

        //Is the form(text, formulae, tables, figures, nomenclature) acceptable?
        [Display(Name = "هل الاشكال (الجداول, النصوص , الصيغ, الرسومات, التسميات) مقبولة ؟")]
        public bool IsFormsAccepted { get; set; }
        
        [Display(Name = "	هل سبق نشر البحث في مكان اخر؟ ")]
        public bool IsPublishedPreviously { get; set; }

        [Display(Name = "هل المعلومات التي تضمنها البحث ذات قيمة؟")]
        public bool IsValuable { get; set; }

        [Display(Name = "هل المعلومات التي تضمنها البحث تحتوي تكرار لنتائج سابقة؟")]
        public bool IsRepeatedInfo { get; set; }

        [Display(Name = "طول البحث")]
        public ResearchLength ResearchLength { get; set; }

        [Display(Name = "النتائج النظرية او العملية")]
        public TheoreticalResults TheoreticalResults { get; set; }

        [Display(Name = "طريقة العمل")]
        public Method Method { get; set; }

        [Display(Name = "هل التزم الباحث بأساليب وقواعد البحث العلمي؟")]
        public IsCommitted IsCommitted { get; set; }

        [Display(Name = "هل تطابق العنوان مع المحتوى؟")]
        public IsMatchTitleContent IsMatchTitleContent { get; set; }

        [Display(Name = "هل تم اعتماد المصادر والمراجع الحديثة؟")]
        public IsModernSourcesAdopted IsModernSourcesAdopted { get; set; }

        [Display(Name = "هل المناقشة موثقة بطريقة صحيحة ومسندة بتجارب كافية وحديثة؟")]
        public bool IsDiscussionDocumentedJustified { get; set; }

        [Display(Name = "هل فسرت النتائج بشكل صحيح؟")]
        public bool IsInterpretedResult { get; set; }

        [Display(Name = "هل تتطابق جودة البحث بالنسبة لما منشور في المجلات العلمية الرصينة؟")]
        public Evaluation Evaluation { get; set; }

        [Display(Name = "ما هي توصيتك لنشر البحث؟")]
        public RecommendationToPublish RecommendationToPublish { get; set; }

        [StringLength(1000)]
        [Display(Name = "سبب عدم الصلاحية للنشر")]
        public string NotForPublicationReason { get; set; }

        [StringLength(5000)]
        [Display(Name = "التعديلات المطلوبة")]
        public string Adjustments { get; set; }

        [StringLength(5000)]
        [Display(Name = "مكان النشر السابق")]
        public string PublishLocation { get; set; }
        
        [Display(Name = "الملف بعد التعديل")]
        [StringLength(500)]
        public string ReviewReportFile { get; set; }

    }
}

