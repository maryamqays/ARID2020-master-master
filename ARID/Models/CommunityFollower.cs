using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CommunityFollower
    {
        [Key]
        public Guid Id { get; set; }

        public int CommunityId { get; set; }
        public Community Community { get; set; }

        [Display(Name = "الباحث")]
        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        [Display(Name = "الباحث")]
        public  ApplicationUser ApplicationUser { get; set; }

        public bool NotifyMe { get; set; }

        [Display(Name = "مدير")]
        public bool IsAdmin { get; set; }

        [Display(Name = "مقبول")]
        public bool IsAccepted { get; set; }
    }
}
