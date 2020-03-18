using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class EventViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public ScientificEvent ScientificEvent { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public IEnumerable<ScientificEvent> ScientificEvents { get; set; }
        public UserBadge UserBadge { get; set; }

    }
}
