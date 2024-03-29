using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
	public class RegisterViewModel
	{
        [Required]
		[DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
		[StringLength(20, MinimumLength = 5, ErrorMessage = "Username length should be anywhere between 5 and 20")]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
