using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
	public class FilterViewModel
	{
		public string SearchString { get; set; }
		public Category Category { get; set; }
		public double PriceLowerBound { get; set; }
		public double PriceUpperBound { get; set; }
		public string SortBy { get; set; } = "Price";
        public bool Ascending { get; set; } = false;
    }
}
