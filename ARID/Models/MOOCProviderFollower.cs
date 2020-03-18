using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class MOOCProviderFollower
    {
        [Key]
        public Guid Id { get; set; }

        public int MOOCProviderId { get; set; }
        public MOOCProvider MOOCProvider { get; set; }

        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

    }
}
