using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class VipDeclarationViewModel
    {
        public PagingInfo PagingInfo { get; set; }
        public VipDeclaration VipDeclaration { get; set; }
        public IEnumerable<VipDeclaration> vipDeclarations { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}
