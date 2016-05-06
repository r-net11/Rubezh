using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GkWeb.ViewModels
{
    public class SignInViewModel
    {
	    [Required]
		[Display(Name = "Имя пользователя")]
		public string User { get; set; }

		//[Required]
		[Display(Name = "Пароль")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
