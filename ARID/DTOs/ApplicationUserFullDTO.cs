using System;
using System.ComponentModel.DataAnnotations;

namespace ARID.DTOs
{
    public class ApplicationUserFullDTO
    {
        public string Id { get; set; }

        public string ARID { get; set; }

        [DataType(DataType.Date)]
        public DateTime RegDate { get; set; }

        public string ArName { get; set; }

        public string EnName { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string University { get; set; }

        public string Faculty { get; set; }

        public string Status { get; set; }

        public string EmailConfirmed { get; set; }

        public string PhoneNumberConfirmed { get; set; }

        public string TwoFactorEnabled { get; set; }

        public int Visitors { get; set; }

        public decimal Balance { get; set; }

        public string DAL { get; set; }

        public bool DALEnabled { get; set; }
    }
}
