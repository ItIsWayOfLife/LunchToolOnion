using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Report
{
    public class ReportProvidersViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal FullPrice { get; set; }
        public int CountOrderDishes { get; set; }
        public int CountOrder { get; set; }
    }
}
