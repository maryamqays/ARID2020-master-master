using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class PublicationTitleViewModel
    {
        public PagingInfo PagingInfo { get; set; }
        public PublicationTitle PublicationTitle { get; set; }
        public IEnumerable<PublicationTitle>  PublicationTitles  { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}
