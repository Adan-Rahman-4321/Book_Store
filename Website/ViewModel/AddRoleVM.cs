using System.ComponentModel.DataAnnotations;

namespace Website.ViewModel
{
	public class AddRoleVM
	{
		[Required]
		[Display(Name ="Role")]
		public string RoleName { get; set; }

	}
}
