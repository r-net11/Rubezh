using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using FiresecAPI;
using FiresecAPI.Automation;
using Infrastructure.Common.Video.RVI_VSS;

namespace FiresecService.Processor
{
	public partial class ProcedureThread
	{
		public DateTime StartTime { get; private set; }
		public bool IsAlive { get; set; }
		public int TimeOut { get; private set; }
		static Func<Procedure, List<Argument>, Procedure, List<Variable>, ProcedureThread> Run { get; set; }
		AutoResetEvent AutoResetEvent { get; set; }
		BackgroundWorker Thread { get; set; }
		ProcedureThread ChildProcedureThread { get; set; }

		public ProcedureThread(Procedure procedure, List<Argument> arguments, Procedure callingProcedure, Func<Procedure, List<Argument>, Procedure, List<Variable>, ProcedureThread> run)
		{
			IsAlive = true;
			TimeOut = procedure.TimeOut;
			Run = run;
			WinFormsPlayers = new List<WinFormsPlayer>();
			AutoResetEvent = new AutoResetEvent(false);
			Procedure = new Procedure();
			Procedure = ObjectCopier.Clone(procedure);
			InitializeArguments(Procedure, arguments, callingProcedure);
			Thread = new BackgroundWorker();
			Thread.DoWork += (sender, args) => RunInThread(Procedure, arguments);
			Thread.RunWorkerCompleted += (sender, args) => Stop();
		}

		public void Start()
		{
			StartTime = DateTime.Now;
			Thread.RunWorkerAsync();
		}

		void Stop()
		{
			IsAlive = ChildProcedureThread != null && ChildProcedureThread.IsAlive;
		}

		bool _isTimeOut;
		public bool IsTimeOut
		{
			get { return _isTimeOut; }
			set
			{
				_isTimeOut = value;
				if (ChildProcedureThread != null)
					ChildProcedureThread.IsTimeOut = value;
			}
		}
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
			if (IsTimeOut)
				return Result.Normal; 
			var allVariables = GetAllVariables(procedure);
			switch (procedureStep.ProcedureStepType)
			{
				case ProcedureStepType.If:
					if (Compare(procedureStep))
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
					while (Compare(procedureStep))
					{
						if (IsTimeOut)
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
					GetObjectProperty(procedureStep);
					break;

				case ProcedureStepType.Arithmetics:
					Calculate(procedureStep);
					break;

				case ProcedureStepType.Foreach:
					var foreachArguments = procedureStep.ForeachArguments;
					var listVariable = allVariables.FirstOrDefault(x => x.Uid == foreachArguments.ListArgument.VariableUid);
					var itemVariable = allVariables.FirstOrDefault(x => x.Uid == foreachArguments.ItemArgument.VariableUid);
					if (listVariable != null)
						foreach (var explicitValue in listVariable.ExplicitValues)
						{
							if (itemVariable != null)
								SetValue(itemVariable, GetValue<object>(explicitValue, itemVariable.ExplicitType, itemVariable.EnumType));
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
					var initialValue = GetValue<int>(forArguments.InitialValueArgument);
					var value = GetValue<int>(forArguments.ValueArgument);
					var iterator = GetValue<int>(forArguments.IteratorArgument);
					if (indexerVariable != null)
					{
						var condition = Compare(initialValue, value, forArguments.ConditionType);
						var currentIntValue = indexerVariable.ExplicitValue.IntValue;
						for (indexerVariable.ExplicitValue.IntValue = initialValue; condition != null && condition.Value; )
						{
							if (IsTimeOut)
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
							condition = Compare(indexerVariable.ExplicitValue.IntValue, value, forArguments.ConditionType);
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
					Pause(procedureStep);
					break;

				case ProcedureStepType.AddJournalItem:
					AddJournalItem(procedureStep);
					break;

				case ProcedureStepType.ShowMessage:
					automationCallbackResult = ShowMessage(procedureStep);
					automationCallbackResult.AutomationCallbackType = AutomationCallbackType.Message;
					automationCallbackResult.IsModalWindow = procedureStep.ShowMessageArguments.IsModalWindow;
					Service.FiresecService.NotifyAutomation(automationCallbackResult);
					break;

				case ProcedureStepType.FindObjects:
					FindObjects(procedureStep);
					break;

				case ProcedureStepType.ControlGKDevice:
					ControlGKDevice(procedureStep);
					break;

				case ProcedureStepType.ControlCamera:
					ControlCamera(procedureStep);
					break;

				case ProcedureStepType.ControlGKFireZone:
					ControlFireZone(procedureStep);
					break;

				case ProcedureStepType.ControlGKGuardZone:
					ControlGuardZone(procedureStep);
					break;

				case ProcedureStepType.ControlDirection:
					ControlDirection(procedureStep);
					break;

				case ProcedureStepType.ControlDoor:
					ControlDoor(procedureStep);
					break;

				case ProcedureStepType.ProcedureSelection:
					{
						var childProcedure = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures.
								FirstOrDefault(x => x.Uid == procedureStep.ProcedureSelectionArguments.ScheduleProcedure.ProcedureUid);
						ChildProcedureThread = Run(childProcedure, procedureStep.ProcedureSelectionArguments.ScheduleProcedure.Arguments, procedure, ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables);
					}
					break;

				case ProcedureStepType.IncrementValue:
					IncrementValue(procedureStep);
					break;

				case ProcedureStepType.SetValue:
					SetValue(procedureStep);
					break;

				case ProcedureStepType.Random:
					GetRandomValue(procedureStep);
					break;

				case ProcedureStepType.ChangeList:
					ChangeList(procedureStep);
					break;

				case ProcedureStepType.GetListCount:
					GetListCount(procedureStep);
					break;

				case ProcedureStepType.GetListItem:
					GetListItem(procedureStep);
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

		public void InitializeArguments(Procedure procedure, List<Argument> arguments, Procedure callingProcedure)
		{
			int i = 0;
			foreach (var variable in procedure.Arguments)
			{
				variable.ExplicitValues = new List<ExplicitValue>();
				if (arguments.Count <= i)
					break;
				var argument = arguments[i];
				if (argument == null)
					break;
				if (argument.VariableScope == VariableScope.ExplicitValue)
				{
					PropertyCopy.Copy(argument.ExplicitValue, variable.ExplicitValue);
					foreach (var explicitVal in argument.ExplicitValues)
					{
						var newExplicitValue = new ExplicitValue();
						PropertyCopy.Copy(explicitVal, newExplicitValue);
						variable.ExplicitValues.Add(newExplicitValue);
					}
				}
				else
				{
					var argumentVariable = GetAllVariables(callingProcedure).FirstOrDefault(x => x.Uid == argument.VariableUid);
					if (argumentVariable == null)
						continue;
					if (argumentVariable.IsReference)
					{
						variable.ExplicitValue = argumentVariable.ExplicitValue;
						variable.ExplicitValues = argumentVariable.ExplicitValues;
					}
					else
					{
						PropertyCopy.Copy(argumentVariable.ExplicitValue, variable.ExplicitValue);
						foreach (var explicitVal in argumentVariable.ExplicitValues)
						{
							var newExplicitValue = new ExplicitValue();
							PropertyCopy.Copy(explicitVal, newExplicitValue);
							variable.ExplicitValues.Add(newExplicitValue);
						}
					}
				}
				i++;
			}
		}
	}
}
