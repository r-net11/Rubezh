using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecService.Processor;

namespace FiresecService.Automation
{
	public class ProcedureThread
	{
		BackgroundWorker Thread { get; set; }
		Procedure Procedure { get; set; }
		private DateTime StartTime { get; set; }
		static Func<Procedure, List<Argument>, Procedure, List<Variable>, bool> Run { get; set; }

		public ProcedureThread(Procedure procedure, List<Argument> arguments, Func<Procedure, List<Argument>, Procedure, List<Variable>, bool> run)
		{
			Run = run;
			Procedure = procedure;
			Thread = new BackgroundWorker();
			Thread.DoWork += (sender, args) => RunInThread(procedure, arguments);
			Thread.RunWorkerCompleted += (sender, args) => Complete();
		}

		public void Start()
		{
			StartTime = DateTime.Now;
			Thread.RunWorkerAsync();
			if (Procedure.TimeOut > 0)
				new Thread(CheckTimeOut).Start();
		}

		void CheckTimeOut()
		{
			var AutoResetEvent = new AutoResetEvent(false);
			while (true)
			{
				if (AutoResetEvent.WaitOne(TimeSpan.FromSeconds(1)))
				{
					return;
				}
				if ((int) ((DateTime.Now - StartTime).TotalSeconds) >= Procedure.TimeOut)
				{
					isTimeOut = true;
					break;
				}
			}
		}

		void Complete()
		{
			foreach (var argument in Procedure.Arguments)
			{
				argument.ExplicitValue = new ExplicitValue();
				argument.ExplicitValues = new List<ExplicitValue>();
			}
		}

		bool isTimeOut;
		public bool RunInThread(Procedure procedure, List<Argument> arguments)
		{
			try
			{
				if (procedure.Steps.Any(step => RunStep(step, procedure) == Result.Exit))
					return true;
			}
			catch
			{
				return false;
			}
			return true;
		}

		Result RunStep(ProcedureStep procedureStep, Procedure procedure)
		{
			if (isTimeOut)
				return Result.Normal; 
			ProcedureHelper.Procedure = procedure;
			var allVariables = ProcedureHelper.GetAllVariables(procedure);
			switch (procedureStep.ProcedureStepType)
			{
				case ProcedureStepType.If:
					if (ProcedureHelper.Compare(procedureStep))
					{
						foreach (var childStep in procedureStep.Children[0].Children)
						{
							var result = RunStep(childStep, procedure);
							if (result != Result.Normal)
							{
								return result;
							}
						}
					}
					else
					{
						foreach (var childStep in procedureStep.Children[1].Children)
						{
							var result = RunStep(childStep, procedure);
							if (result != Result.Normal)
							{
								return result;
							}
						}
					}
					break;
				case ProcedureStepType.While:
					while (ProcedureHelper.Compare(procedureStep))
					{
						if (isTimeOut)
							return Result.Normal; 
						foreach (var childStep in procedureStep.Children[0].Children)
						{
							var result = RunStep(childStep, procedure);
							if (result == Result.Break)
								return Result.Normal;
							if (result == Result.Continue)
								break;
							if (result == Result.Exit)
								return Result.Exit;
						}
					}
					break;
				case ProcedureStepType.GetObjectProperty:
					ProcedureHelper.GetObjectProperty(procedureStep);
					break;

				case ProcedureStepType.Arithmetics:
					ProcedureHelper.Calculate(procedureStep);
					break;

				case ProcedureStepType.Foreach:
					var foreachArguments = procedureStep.ForeachArguments;
					var listVariable = allVariables.FirstOrDefault(x => x.Uid == foreachArguments.ListArgument.VariableUid);
					var itemVariable = allVariables.FirstOrDefault(x => x.Uid == foreachArguments.ItemArgument.VariableUid);
					if (listVariable != null)
						foreach (var explicitValue in listVariable.ExplicitValues)
						{
							if (itemVariable != null)
								ProcedureHelper.SetValue(itemVariable, ProcedureHelper.GetValue<object>(explicitValue, itemVariable.ExplicitType, itemVariable.EnumType));
							foreach (var childStep in procedureStep.Children[0].Children)
							{
								var result = RunStep(childStep, procedure);
								if (result == Result.Break)
									return Result.Normal;
								if (result == Result.Continue)
									break;
								if (result == Result.Exit)
									return Result.Exit;
							}
						}
					break;

				case ProcedureStepType.For:
					var forArguments = procedureStep.ForArguments;
					var indexerVariable = allVariables.FirstOrDefault(x => x.Uid == forArguments.IndexerArgument.VariableUid);
					var initialValue = ProcedureHelper.GetValue<int>(forArguments.InitialValueArgument);
					var value = ProcedureHelper.GetValue<int>(forArguments.ValueArgument);
					var iterator = ProcedureHelper.GetValue<int>(forArguments.IteratorArgument);
					if (indexerVariable != null)
					{
						var condition = ProcedureHelper.Compare(initialValue, value, forArguments.ConditionType);
						var currentIntValue = indexerVariable.ExplicitValue.IntValue;
						for (indexerVariable.ExplicitValue.IntValue = initialValue; condition != null && condition.Value; )
						{
							if (isTimeOut)
								return Result.Normal;
							foreach (var childStep in procedureStep.Children[0].Children)
							{
								var result = RunStep(childStep, procedure);
								if (result == Result.Break)
								{
									indexerVariable.ExplicitValue.IntValue = currentIntValue;
									return Result.Normal;
								}
								if (result == Result.Continue)
									break;
								if (result == Result.Exit)
									return Result.Exit;
							}
							indexerVariable.ExplicitValue.IntValue = indexerVariable.ExplicitValue.IntValue + iterator;
							condition = ProcedureHelper.Compare(indexerVariable.ExplicitValue.IntValue, value, forArguments.ConditionType);
						}
						indexerVariable.ExplicitValue.IntValue = currentIntValue;
					}
					break;
				case ProcedureStepType.PlaySound:
					var automationCallbackResult = new AutomationCallbackResult();
					automationCallbackResult.SoundUID = procedureStep.SoundArguments.SoundUid;
					automationCallbackResult.AutomationCallbackType = AutomationCallbackType.Sound;
					Service.FiresecService.NotifyAutomation(automationCallbackResult);
					break;

				case ProcedureStepType.Pause:
					ProcedureHelper.Pause(procedureStep);
					break;

				case ProcedureStepType.AddJournalItem:
					ProcedureHelper.AddJournalItem(procedureStep);
					break;

				case ProcedureStepType.ShowMessage:
					automationCallbackResult = ProcedureHelper.ShowMessage(procedureStep);
					automationCallbackResult.AutomationCallbackType = AutomationCallbackType.Message;
					automationCallbackResult.IsModalWindow = procedureStep.ShowMessageArguments.IsModalWindow;
					Service.FiresecService.NotifyAutomation(automationCallbackResult);
					break;

				case ProcedureStepType.FindObjects:
					ProcedureHelper.FindObjects(procedureStep);
					break;

				case ProcedureStepType.ControlGKDevice:
					ProcedureHelper.ControlGKDevice(procedureStep);
					break;

				case ProcedureStepType.ControlCamera:
					ProcedureHelper.ControlCamera(procedureStep);
					break;

				case ProcedureStepType.ControlGKFireZone:
					ProcedureHelper.ControlFireZone(procedureStep);
					break;

				case ProcedureStepType.ControlGKGuardZone:
					ProcedureHelper.ControlGuardZone(procedureStep);
					break;

				case ProcedureStepType.ControlDirection:
					ProcedureHelper.ControlDirection(procedureStep);
					break;

				case ProcedureStepType.ControlDoor:
					ProcedureHelper.ControlDoor(procedureStep);
					break;

				case ProcedureStepType.ProcedureSelection:
					{
						var childProcedure = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures.
								FirstOrDefault(x => x.Uid == procedureStep.ProcedureSelectionArguments.ScheduleProcedure.ProcedureUid);
						Run(childProcedure, procedureStep.ProcedureSelectionArguments.ScheduleProcedure.Arguments, procedure, ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables);
					}
					break;

				case ProcedureStepType.IncrementValue:
					ProcedureHelper.IncrementValue(procedureStep);
					break;

				case ProcedureStepType.SetValue:
					ProcedureHelper.SetValue(procedureStep);
					break;

				case ProcedureStepType.Random:
					ProcedureHelper.GetRandomValue(procedureStep);
					break;

				case ProcedureStepType.ChangeList:
					ProcedureHelper.ChangeList(procedureStep);
					break;

				case ProcedureStepType.GetListCount:
					ProcedureHelper.GetListCount(procedureStep);
					break;

				case ProcedureStepType.GetListItem:
					ProcedureHelper.GetListItem(procedureStep);
					break;

				case ProcedureStepType.Exit:
					return Result.Exit;

				case ProcedureStepType.Break:
					return Result.Break;

				case ProcedureStepType.Continue:
					return Result.Continue;
			}
			return Result.Normal;
		}

		enum Result
		{
			Normal,
			Break,
			Continue,
			Exit
		}
	}
}
