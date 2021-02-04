using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBranch.Models
{
    public class Substitute
    {
        public int Id { get; set; }
        [DisplayName("اليوم")]
        public string Day { get; set; }
        [DisplayName("التاريخ")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SubstituteDate { get; set; }
        [DisplayName("الرتبة والاسم والشهرة")]
        public string FullName { get; set; }
        [DisplayName("رقم الكنية")]
        public string MilitaryNumber { get; set; }
        public bool IsVacation { get; set; }
        
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
