using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CommitteeViewModel
    {
        public PagingInfo PagingInfo { get; set; }
        public Committee Committee { get; set; }
        public CommitteeProfile CommitteeProfile { get; set; }
        public CommitteeAchievement CommitteeAchievement { get; set; }
        public IEnumerable<Committee> Committees { get; set; }
        public IEnumerable<CommitteeAchievement> CommitteeAchievements { get; set; }
        public IEnumerable<CommitteeProfile> CommitteeProfiles { get; set; }
        public ApplicationUser ApplicationUser { get; set; }


    }
}
