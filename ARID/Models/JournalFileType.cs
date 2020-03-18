using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class JournalFileType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "المجلة")]
        public int JournalId { get; set; }
        public  Journal Journal { get; set; }


        [Required]
        [Display(Name = "نوع الملف")]
        public int FileTypeId { get; set; }
        public  FileType FileType { get; set; }

        [Required]
        [Display(Name = "إجباري؟")]
        public bool IsRequired { get; set; }

        [Display(Name = "محذوف")]
        public bool IsDeleted { get; set; }

    }
}
