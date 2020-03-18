using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class SideAd
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(450)]
        public string Description { get; set; }

        [StringLength(100)]
        public string Image { get; set; }

        [StringLength(200)]
        public string ExternalLink { get; set; }

        [StringLength(50)]
        public string Controller { get; set; }

        [StringLength(50)]
        public string Action { get; set; }

        public int Indx { get; set; }
        public int Hits { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        public bool IsVisible { get; set; }

        [StringLength(450)]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public AdsPositionType AdsPositionType { get; set; }
        public TargetType TargetType { get; set; }
    }
}
