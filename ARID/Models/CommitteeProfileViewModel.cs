using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class CommitteeProfileViewModel
    {
        public CommitteeProfile CommitteeProfile { get; set; }
        public IEnumerable<CommitteeProfile> CommitteeProfiles { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
