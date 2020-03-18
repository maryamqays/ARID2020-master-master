using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARID.Models
{
    public class AddressViewModel
    {
        public Address Address { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public IEnumerable<Address> UserAddresses { get; set; }
    }
}
