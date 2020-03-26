using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Report
{
    public class ReportUsersViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string LFP { get; set; }
        public decimal FullPrice { get; set; }
        public int CountOrderDishes { get; set; }
        public int CountOrder { get; set; }
    }
}
