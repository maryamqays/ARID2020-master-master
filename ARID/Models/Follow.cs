using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public partial class Follow
    {
        public long Id { get; set; }

        [Display(Name = "UserId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        [StringLength(450)]
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }

        [Display(Name = "FollowedUserId")]
        [Required(ErrorMessage = "RequiredFieldError")]
        public string FollowedUserId { get; set; }
        public  ApplicationUser FollowedUser { get; set; }
    }
}
