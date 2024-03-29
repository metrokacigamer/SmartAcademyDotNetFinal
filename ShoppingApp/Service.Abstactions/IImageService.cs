using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Abstactions
{
	public interface IImageService
	{
		Task AddImages(IEnumerable<IFormFile> productImages, Product product);
	}
}
