using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class ScoreLog
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "القيمة")]
        public int ScoreValueId { get; set; }
        [Display(Name = "القيمة")]
        public ScoreValue ScoreValue { get; set; }

        [Display(Name = "المشاركة")]
        public Guid PostId { get; set; }
        [Display(Name = "المشاركة")]
        public Post Post { get; set; }

        [Display(Name = "التاريخ")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
    }
}
