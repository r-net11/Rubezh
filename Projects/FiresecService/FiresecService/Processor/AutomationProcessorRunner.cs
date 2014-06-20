using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Automation;
using GKProcessor;

namespace FiresecService.Processor
{
	public static class AutomationProcessorRunner
	{
		public static bool Run(Procedure procedure, List<Variable> arguments)
		{
			try
			{
				foreach (var step in procedure.Steps)
				{
					RunStep(step);
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		static void RunStep(ProcedureStep procedureStep)
		{
			switch(procedureStep.ProcedureStepType)
			{
				case ProcedureStepType.If:
					if (true)
					{
						foreach(var childStep in procedureStep.Children[0].Children)
						{
							RunStep(childStep);
						}
					}
					else
					{
						foreach (var childStep in procedureStep.Children[1].Children)
						{
							RunStep(childStep);
						}
					}
					break;

				case ProcedureStepType.Foreach:
					while (true)
					{
						foreach (var childStep in procedureStep.Children[0].Children)
						{
							RunStep(childStep);
						}
					}
					break;

				case ProcedureStepType.PlaySound:
					break;

				case ProcedureStepType.SendMessage:
					GKProcessorManager.AddGKMessage(EventNameEnum.Команда_оператора, "Запуск процедуры", null, null);
					break;
			}
		}
	}
}