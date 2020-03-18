using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class MOOCProviderViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public MOOCProvider MOOCProvider { get; set; }
        public MOOCProviderFollower MOOCProviderFollower { get; set; }
        public IEnumerable<MOOCProviderFollower> MOOCProviderFollowers { get; set; }
        public IEnumerable<MOOCProvider> MOOCProviders { get; set; }
        public IEnumerable<MOOCList> MOOCLists { get; set; }
    }
}
