using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
	public class ChangePasswordViewModel
	{
		public string Id { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string NewPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string ConfirmNewPassword { get; set; }
	}
}
