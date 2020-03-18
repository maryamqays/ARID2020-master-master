using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CommunityWallViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public Community Community { get; set; }
        public IEnumerable<Community> Communities { get; set; }
        public Post Post { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<SideAd> SideAds { get; set; }
        public IEnumerable<ScientificEvent> ScientificEvents { get; set; }
        public PostMetric PostMetric { get; set; }
        public IEnumerable<PostMetric> PostMetrics { get; set; }
        public List<Project> Projects { get; set; }
        public List<Publication> Publications { get; set; }
        public List<Publication> Publications2 { get; set; }
            }
}
