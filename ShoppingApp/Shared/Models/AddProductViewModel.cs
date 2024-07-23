using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
	public class AddProductViewModel
	{
		public string Name { get; set; }
		public string Description { get; set; }
        public int QuantityInStock { get; set; }
        public Category Category { get; set; }
        public IEnumerable<IFormFile>? ProductImages { get; set; }
        public double Price { get; set; }
    }
}
