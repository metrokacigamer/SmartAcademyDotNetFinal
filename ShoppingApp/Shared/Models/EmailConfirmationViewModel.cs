
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace Shared.Models
{
	public class EmailConfirmationViewModel<T> where T : class
	{
        public string Key { get; set; }

        [Required]
        public string EnteredKey { get; set; }
		[FromQuery]
		public T Model { get; set; }
	}
}
