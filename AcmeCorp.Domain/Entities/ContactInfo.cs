using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcmeCorp.Domain.Entities
{
    public class ContactInfo
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
