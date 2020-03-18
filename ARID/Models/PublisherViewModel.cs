using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class PublisherViewModel
    {
        public PagingInfo PagingInfo { get; set; }
        public Publisher Publisher { get; set; }
        public IEnumerable<Publisher> publishers { get; set; }

    }
}
