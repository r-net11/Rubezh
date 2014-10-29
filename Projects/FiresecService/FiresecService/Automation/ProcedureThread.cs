using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using FiresecAPI.Automation;
using FiresecAPI.Journal;

namespace FiresecService
{
	public partial class ProcedureThread
	{
		public DateTime StartTime { get; private set; }
		public bool IsAlive { get; set; }
		public int TimeOut { get; private set; }
		public TimeType TimeType { get; private set; }
		AutoResetEvent AutoResetEvent { get; set; }
		Thread Thread { get; set; }
		List<Variable> AllVariables { get; set; } 
		List<ProcedureStep> Steps { get; set; }
		bool IsSync { get; set; }
		ProcedureThread ChildProcedureThread { get; set; }
		JournalItem JournalItem { get; set; }

		public ProcedureThread(Procedure procedure, List<Argument> arguments, List<Variable> callingProcedureVariables, JournalItem journalItem = null)
		{
			IsAlive = true;
			JournalItem = journalItem;
			TimeOut = procedure.TimeOut;
			TimeType = procedure.TimeType;
			AutoResetEvent = new AutoResetEvent(false);
			Steps = procedure.Steps;
			AllVariables = new List<Variable>(Utils.Clone(procedure.Variables));
			var procedureArguments = Utils.Clone(procedure.Arguments);
			InitializeArguments(procedureArguments, arguments, callingProcedureVariables);
			AllVariables.AddRange(procedureArguments);
			AllVariables.AddRange(ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables);
			Thread = new Thread(() => RunInThread(arguments));
			IsSync = procedure.IsSync;
		}

		public void Start()
		{
			StartTime = DateTime.Now;
			Thread.Start();
			if (IsSync)
				Thread.Join();
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
				AutoResetEvent.Set();
			}
		}

		public bool RunInThread(List<Argument> arguments)
		{
			try
			{
				if (Steps.Any(step => RunStep(step) == Result.Exit))
				{
					return true;
				}
			}
			catch
			{
				return false;
			}
			finally
			{
				IsAlive = false;
			}
			return true;
		}

		Result RunStep(ProcedureStep procedureStep)
		{
			if (IsTimeOut)
				return Result.Normal; 
			switch (procedureStep.ProcedureStepType)
			{
				case ProcedureStepType.If:
					if (Compare(procedureStep))
					{
						foreach (var childStep in procedureStep.Children[0].Children)
						{
							var result = RunStep(childStep);
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
							var result = RunStep(childStep);
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
							var result = RunStep(childStep);
							if (result == Result.Break)
								return Result.Normal;
							if (result == Result.Continue)
								break;
							if (result == Result.Exit)
								return Result.Exit;
						}
					}
					break;

				case ProcedureStepType.Foreach:
					var foreachArguments = procedureStep.ForeachArguments;
					var listVariable = AllVariables.FirstOrDefault(x => x.Uid == foreachArguments.ListArgument.VariableUid);
					var itemVariable = AllVariables.FirstOrDefault(x => x.Uid == foreachArguments.ItemArgument.VariableUid);
					if (listVariable != null)
						foreach (var explicitValue in listVariable.ExplicitValues)
						{
							if (itemVariable != null)
								SetValue(itemVariable, GetValue<object>(explicitValue, itemVariable.ExplicitType, itemVariable.EnumType));
							foreach (var childStep in procedureStep.Children[0].Children)
							{
								var result = RunStep(childStep);
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
					var indexerVariable = AllVariables.FirstOrDefault(x => x.Uid == forArguments.IndexerArgument.VariableUid);
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
								var result = RunStep(childStep);
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


				case ProcedureStepType.ProcedureSelection:
					{
						var childProcedure = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures.
								FirstOrDefault(x => x.Uid == procedureStep.ProcedureSelectionArguments.ScheduleProcedure.ProcedureUid);
						if (childProcedure != null)
						{
							ChildProcedureThread = new ProcedureThread(childProcedure, procedureStep.ProcedureSelectionArguments.ScheduleProcedure.Arguments, AllVariables);
							ProcedureRunner.ProceduresThreads.Add(ChildProcedureThread);
							ChildProcedureThread.Start();
						}
					}
					break;

				case ProcedureStepType.GetObjectProperty:
					GetObjectProperty(procedureStep);
					break;

				case ProcedureStepType.Arithmetics:
					Calculate(procedureStep);
					break;

				case ProcedureStepType.PlaySound:
					PlaySound(procedureStep);
					break;

				case ProcedureStepType.Pause:
					Pause(procedureStep);
					break;

				case ProcedureStepType.AddJournalItem:
					AddJournalItem(procedureStep);
					break;

				case ProcedureStepType.ShowMessage:
					ShowMessage(procedureStep);
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

				case ProcedureStepType.ControlSKDZone:
					ControlSKDZone(procedureStep);
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

				case ProcedureStepType.CheckPermission:
					CheckPermission(procedureStep);
					break;

				case ProcedureStepType.GetListCount:
					GetListCount(procedureStep);
					break;

				case ProcedureStepType.GetListItem:
					GetListItem(procedureStep);
					break;

				case ProcedureStepType.GetJournalItem:
					GetJournalItem(procedureStep);
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

		public void InitializeArguments(List<Variable> variables, List<Argument> arguments, List<Variable> callingProcedureVariables)
		{
			int i = 0;
			foreach (var variable in variables)
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
					var argumentVariable = callingProcedureVariables.FirstOrDefault(x => x.Uid == argument.VariableUid);
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
