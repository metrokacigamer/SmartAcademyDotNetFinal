using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
	public class ProductViewModel
	{
		public string Id { get; set; }

		[Required]
		public string Name { get; set; }
		public string Description { get; set; }

		[Required]
		public double Price { get; set; }
		public int QuantityInStock { get; set; }
		public IEnumerable<ImageViewModel> Images { get; set; }

		[Required]
        public Category Category { get; set; }
    }
}
