using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class SubmissionFile
    {
        [Key]
        public int Id { get; set; }
      
        [Display(Name = "عملية التقديم")]
        public int SubmissionId { get; set; }
        public Submission Submission { get; set; }

        [Display(Name = "نوع الملف")]
        public int JournalFileTypeId { get; set; }
        public JournalFileType JournalFileType { get; set; }

        [Required(ErrorMessage = "هذا الحقل ضروري")]
        [Display(Name = "الملف")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string FileName { get; set; }

        [Display(Name = "وصف الملف")]
        [StringLength(100, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string Description { get; set; }

  
    }
}
