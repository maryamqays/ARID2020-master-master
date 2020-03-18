using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class JournalArticleType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "المجلة")]
        public int JournalId { get; set; }
        public  Journal Journal { get; set; }
        
        [Required]
        [Display(Name = "نوع الورقة البحثية")]
        public int ArticleTypeId { get; set; }
        public  ArticleType ArticleType { get; set; }

        [Display(Name = "محذوف")]
        public bool IsDeleted { get; set; }

    }
}
