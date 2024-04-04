using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
	public class ItemViewModel
	{
		public string Id { get; set; }
        public string ProductId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public double Price { get; set; }
        public IEnumerable<string> ImagePaths { get; set; }
        public int Quantity { get; set; }
        public Category Category { get; set; }
    }
}
