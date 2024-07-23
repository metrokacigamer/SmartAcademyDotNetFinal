using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
	public class FilteredPageViewModel
	{
        public IEnumerable<ProductViewModel> ProductViewModels { get; set; }
        public FilterViewModel Filter { get; set; }
    }
}
