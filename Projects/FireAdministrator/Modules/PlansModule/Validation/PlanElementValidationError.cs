﻿using System;
using StrazhAPI.Enums;
using FiresecClient;
using Infrastructure.Common.Validation;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using StrazhAPI.Plans.Elements;

namespace PlansModule.Validation
{
	public class PlanElementValidationError : PlanObjectValidationError<ElementBase, ShowPlanElementEvent, Guid>
	{
		private ElementError _elementError;
		public PlanElementValidationError(ElementError elementError)
			: base(elementError.Element, elementError.Error, elementError.IsCritical ? ValidationErrorLevel.CannotSave : ValidationErrorLevel.Warning, true, elementError.PlanUID)
		{
			_elementError = elementError;
		}

		public override ModuleType Module
		{
			get { return ModuleType.Plans; }
		}

		protected override Guid KeyValue
		{
			get { return _elementError.Element.UID; }
		}
		public override string Source
		{
			get { return _elementError.Element.PresentationName; }
		}
		public override string ImageSource
		{
			get { return _elementError.ImageSource; }
		}
		public override string Address
		{
			get
			{
				var plan = PlanUID.HasValue ? FiresecManager.PlansConfiguration[PlanUID.Value] : null;
				return plan == null ? null : plan.Caption;
			}
		}

		public override void Navigate()
		{
			if (_elementError.Navigate != null)
				_elementError.Navigate();
			ApplicationService.Shell.RightPanelVisible = true;

			base.Navigate();
		}
	}
}
