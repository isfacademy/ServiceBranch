using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBranch.Models.ViewModel
{
    public class SubstitueVM
    {
        public int StartYear { get; set; }
        public int StartMonth { get; set; }
        public int EndYear { get; set; }
        public int EndMonth { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<Substitute> Substitutes { get; set; }
    }
}
