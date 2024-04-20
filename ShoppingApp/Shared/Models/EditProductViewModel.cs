using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
	public class EditProductViewModel
	{
		public string Id { get; set; }

		[Required]
		public string Name { get; set; }
		public string Description { get; set; }

		[Required]
		public double Price { get; set; }
		public int QuantityInStock { get; set; }
		public IEnumerable<ImageViewModel>? Images { get; set; }
		public IEnumerable<string>? RemovedImageIds { get; set; }
        public IEnumerable<IFormFile>? NewImages { get; set; }
		[Required]
        public Category Category { get; set; }
	}
}
