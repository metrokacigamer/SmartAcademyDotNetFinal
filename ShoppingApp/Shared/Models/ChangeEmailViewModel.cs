using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
	public class ChangeEmailViewModel
	{
		public string Id { get; set; }
		public string Email { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		public string NewEmail { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		public string ConfirmNewEmail { get; set; }
	}
}
