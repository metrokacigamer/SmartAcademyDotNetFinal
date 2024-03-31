using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
	public class EmailConfirmationViewModel<T> where T : class
	{
        public string Key { get; set; }

        [Required]
        public string EnteredKey { get; set; }
		public T Model { get; set; }
	}
}
