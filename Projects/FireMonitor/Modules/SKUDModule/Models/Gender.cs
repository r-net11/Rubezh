using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SKUDModule.Models
{
	public enum Gender
	{
		[DescriptionAttribute("Мужской")]
		Male,
		[DescriptionAttribute("Женский")]
		Female
	}
}
