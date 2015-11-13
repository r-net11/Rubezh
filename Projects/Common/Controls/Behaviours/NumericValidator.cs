using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Controls.Behaviours
{
	public class NumericValidator : TextBoxBehavior
	{
		private readonly Func<string, bool> _validator;

		public NumericValidator()
		{
			_validator = text => Regex.IsMatch(text, @"^\d+\.{0,1}\d*$");
		}

		protected override Func<string, bool> Validator
		{
			get { return _validator; }
		}
	}
}
