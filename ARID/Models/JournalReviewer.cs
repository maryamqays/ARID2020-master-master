using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace ARID.Models
{
    public class JournalReviewer
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "رقم العضوية")]
        [StringLength(450)]
        public string ReviewerUserId { get; set; }
        public ApplicationUser ReviewerUser { get; set; }
        
        [Display(Name = "وصف")]
        [StringLength(500, ErrorMessage = "MinMaxTextFieldError", MinimumLength = 0)]
        public string Description { get; set; }
     
        [Display(Name = "قعال؟")]
        public bool IsActive { get; set; }

        [Display(Name = "المجلة")]
        public int JournalId { get; set; }
        public Journal Journal { get; set; }
    }
}
