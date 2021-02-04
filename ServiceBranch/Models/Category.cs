using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBranch.Models
{
    public class Category
    {
        public int Id { get; set; }
        [DisplayName("ضباط/فروع/كليات مدارس/مشتركة")]
        public string Name { get; set; }
        public ICollection<Substitute> Substitutes { get; set; }
    }
}
