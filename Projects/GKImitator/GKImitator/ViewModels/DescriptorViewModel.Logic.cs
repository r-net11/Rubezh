using System;
using System.Linq;
using System.Windows.Documents;
using RubezhAPI.GK;
using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using RubezhDAL.DataClasses;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel
	{
		public BaseDescriptor LogicDescriptor { get; private set; }
		public List<FormulaOperation> FormulaOperations { get; protected set; }
		bool IsKauDecriptor;

		public void SetKauDescriptor(BaseDescriptor kauBaseDescriptor)
		{
			KauBaseDescriptor = kauBaseDescriptor;
			InitializeLogic();
		}

		public void InitializeLogic()
		{
			GKBaseDescriptor.Build();
			LogicDescriptor = GKBaseDescriptor;

			if (KauBaseDescriptor != null)
			{
				KauBaseDescriptor.Build();
				if (KauBaseDescriptor.Formula.FormulaOperations.Count > 1)
				{
					LogicDescriptor = KauBaseDescriptor;
					IsKauDecriptor = true;
				}
			}

			FormulaOperations = LogicDescriptor.Formula.FormulaOperations;
		}

		public void RecalculateOutputLogic()
		{
			foreach (var gkBase in LogicDescriptor.GKBase.OutputDescriptors)
			{
				var descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.GKDescriptorNo == gkBase.GKDescriptorNo);
				if (descriptorViewModel != null)
				{
					descriptorViewModel.RecalculateLogic();
				}
			}
		}

		public void RecalculateCurrentLogic()
		{
			var descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.GKDescriptorNo == GKBase.GKDescriptorNo);
			if (descriptorViewModel != null)
			{
				descriptorViewModel.RecalculateLogic();
			}
		}

		List<Tuple<int, int>> usersCurrentZones { get; set; }
		void RecalculateLogic()
		{
			Trace.WriteLine("\n" + GKBase.PresentationName);
			if (Regime == Regime.Ignore)
				return;
			var stack = new List<int>();
			var stateBitVales = new Dictionary<GKStateBit, bool>();

			for (int i = 0; i < FormulaOperations.Count; i++)
			{
				var formulaOperation = FormulaOperations[i];
				if (formulaOperation.FormulaOperationType == FormulaOperationType.END || formulaOperation.FormulaOperationType == FormulaOperationType.EXIT)
					break;

				var stateBit = (GKStateBit)formulaOperation.FirstOperand;
				var descriptorNo = formulaOperation.SecondOperand;
				DescriptorViewModel descriptorViewModel = null;
				if (IsKauDecriptor)
					descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.GKBase.KAUDescriptorNo == descriptorNo && x.GKBase == formulaOperation.GKBaseSecondOperand); // TODO проверить для больше чем одного КАУ
				else
					descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.GKDescriptorNo == descriptorNo && x.GKBase == formulaOperation.GKBaseSecondOperand);

				switch (formulaOperation.FormulaOperationType)
				{
					case FormulaOperationType.GETBIT:
						if (descriptorViewModel != null)
						{
							var bitValue = descriptorViewModel.GetStateBit(stateBit) ? 1 : 0;
							stack.Add(bitValue);
						}
						break;

					case FormulaOperationType.PUTBIT:
						if (stack.Any())
						{
							var currentStackValue = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);
							if (stateBitVales.ContainsKey(stateBit))
								stateBitVales[stateBit] = currentStackValue != 0;
							else
								stateBitVales.Add(stateBit, currentStackValue != 0);
						}
						break;

					case FormulaOperationType.CONST:
						stack.Add(formulaOperation.SecondOperand);
						break;

					case FormulaOperationType.GETBYTE:
						break;

					case FormulaOperationType.PUTBYTE:
						break;

					case FormulaOperationType.GETWORD:
						break;

					case FormulaOperationType.PUTWORD:
						break;

					case FormulaOperationType.ADD:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = currentStackValue1 + currentStackValue2;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.SUB:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = currentStackValue1 - currentStackValue2;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.MUL:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = currentStackValue1 * currentStackValue2;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.PUTP:
						var cardNo = stack.LastOrDefault();
						stack.RemoveAt(stack.Count - 1);
						var zoneNo = stack.LastOrDefault();
						stack.RemoveAt(stack.Count - 1);
						usersCurrentZones.Add(new Tuple<int, int> (cardNo, zoneNo));
						break;

					case FormulaOperationType.OR:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = (currentStackValue1 != 0 || currentStackValue2 != 0) ? 1 : 0;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.AND:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = (currentStackValue1 != 0 && currentStackValue2 != 0) ? 1 : 0;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.XOR:
						break;

					case FormulaOperationType.COM:
						if (stack.Any())
						{
							var currentStackValue = stack.LastOrDefault();
							currentStackValue = currentStackValue != 0 ? 0 : 1;
							stack.RemoveAt(stack.Count - 1);
							stack.Add(currentStackValue);
						}
						break;

					case FormulaOperationType.NEG:
						break;

					case FormulaOperationType.EQ:
						break;

					case FormulaOperationType.NE:
						break;

					case FormulaOperationType.GT:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = (currentStackValue2 > currentStackValue1) ? 1 : 0;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.GE:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = (currentStackValue2 >= currentStackValue1) ? 1 : 0;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.LT:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = (currentStackValue2 < currentStackValue1) ? 1 : 0;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.LE:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = (currentStackValue2 <= currentStackValue1) ? 1 : 0;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.TSTP:
						cardNo = stack.LastOrDefault();
						stack.RemoveAt(stack.Count - 1);
						zoneNo = formulaOperation.SecondOperand;
						var userCurrentZoneNo = usersCurrentZones.FirstOrDefault(x => x.Item1 == cardNo).Item2;
						if (userCurrentZoneNo != zoneNo)
							stack.AddRange(new List<int>{cardNo, 1});
						else
							stack.Add(0);
						break;

					case FormulaOperationType.CMPKOD:
						if (descriptorViewModel != null)
						{
							var code = descriptorViewModel.GKBase as GKCode;
							if (code != null)
							{
								var currentStackValue1 = stack.LastOrDefault();
								stack.RemoveAt(stack.Count - 1);

								var newStackValue = 0;
								if(formulaOperation.FirstOperand == 1)
								{
									newStackValue = (currentStackValue1 == code.Password) ? 1 : 0;
								}
								if (formulaOperation.FirstOperand == 2)
								{
									newStackValue = (currentStackValue1 != code.Password) ? 1 : 0;
								}
								stack.Add(newStackValue);
							}
						}
						break;

					case FormulaOperationType.KOD:
						if (descriptorViewModel != null)
						{
							stack.Add(descriptorViewModel.CurrentCardNo);
						}
						break;

					case FormulaOperationType.ACS:
					case FormulaOperationType.ACSP:
						var level = formulaOperation.FirstOperand;

						var isAccess = false;

						if (descriptorViewModel != null && descriptorViewModel.CurrentCardNo > 0)
						{
							var device = descriptorViewModel.GKBase as GKDevice;
							if (device != null && (device.Driver.IsCardReaderOrCodeReader))
							{
								ImitatorUser user = null;
								using (var dbService = new DbService())
								{
									user = dbService.ImitatorUserTraslator.GetByNo(descriptorViewModel.CurrentCardNo);
									if (user != null)
									{
										if (user.Level >= level)
										{
											if (IsInSchedule(dbService, user.ScheduleNo))
												isAccess = true;
										}

										foreach (var imitatorUserDevice in user.ImitatorUserDevices)
										{
											if (imitatorUserDevice.DescriptorNo == formulaOperation.SecondOperand)
											{
												if (IsInSchedule(dbService, imitatorUserDevice.ScheduleNo))
													isAccess = true;
											}
										}
									}
								}

								var journalItem = new ImitatorJournalItem(0, isAccess ? (byte)13 : (byte)15, 0, 0);
								journalItem.ObjectDeviceAddress = (short)device.GKDescriptorNo;
								if (user != null)
								{
									journalItem.ObjectFactoryNo = user.GKNo;
								}
								AddJournalItem(journalItem);
							}
						}

						if (formulaOperation.FormulaOperationType == FormulaOperationType.ACS)
							stack.Add(isAccess ? 1 : 0);
						else
							stack.AddRange(new List<int>{CurrentCardNo, isAccess ? 1 : 0});

						break;

					case FormulaOperationType.BR:
						var mustGoTo = false;
						switch (formulaOperation.FirstOperand)
						{
							case 0:
								mustGoTo = true;
								break;

							case 1:
								var currentStackValue = stack.LastOrDefault();
								mustGoTo = currentStackValue == 0;
								stack.RemoveAt(stack.Count - 1);
								break;

							case 2:
								currentStackValue = stack.LastOrDefault();
								mustGoTo = currentStackValue != 0;
								stack.RemoveAt(stack.Count - 1);
								break;
						}
						if (mustGoTo)
							i += formulaOperation.SecondOperand;
						break;

					case FormulaOperationType.DUP:
						if (stack.Any())
						{
							var currentStackValue = stack.LastOrDefault();
							stack.Add(currentStackValue);
						}
						break;

					case FormulaOperationType.PUTMEMB:
						var newZoneNo = stack.LastOrDefault();
						stack.RemoveAt(stack.Count - 1);
						cardNo = stack.LastOrDefault();
						stack.RemoveAt(stack.Count - 1);
						usersCurrentZones.RemoveAll(x => x.Item1 == cardNo);
						usersCurrentZones.Add(new Tuple<int, int>(cardNo, newZoneNo));
						break;

					case FormulaOperationType.GETMEMB:
						zoneNo = usersCurrentZones.FirstOrDefault(x => x.Item1 == CurrentCardNo).Item2;
						stack.AddRange(new List<int> { CurrentCardNo, zoneNo });
						break;
				}

				if (formulaOperation.FormulaOperationType == FormulaOperationType.PUTBIT)
					Trace.WriteLine(formulaOperation.FormulaOperationType + "\t" + string.Join(" ", (GKStateBit) formulaOperation.FirstOperand));
				else
					Trace.WriteLine(formulaOperation.FormulaOperationType + "\t" + string.Join(" ", stack));
			}

			var hasZoneBitsChanged = false;
			if (GKBase is GKZone)
			{
				if (stateBitVales.Any(x => x.Key == GKStateBit.Fire2 && x.Value))
				{
					stateBitVales[GKStateBit.Fire1] = false;
					stateBitVales[GKStateBit.Attention] = false;
					hasZoneBitsChanged = true;
				}
				else if (stateBitVales.Any(x => x.Key == GKStateBit.Fire1 && x.Value))
				{
					stateBitVales[GKStateBit.Attention] = false;
					hasZoneBitsChanged = true;
				}
				else if (stateBitVales.Any(x => x.Key == GKStateBit.Attention && x.Value))
				{
					hasZoneBitsChanged = true;
				}
				else
				{
					stateBitVales[GKStateBit.Attention] = false;
					stateBitVales[GKStateBit.Fire1] = false;
					stateBitVales[GKStateBit.Fire2] = false;
					hasZoneBitsChanged = true;
				}
			}

			var intState = StatesToInt();

			foreach (var stateBitVale in stateBitVales)
			{
				if (stateBitVale.Value)
				{
					if (stateBitVale.Key == GKStateBit.TurnOn_InAutomatic)
					{
						if (Regime == Regime.Automatic)
						{
							OnTurnOn();
						}
					}
					if (stateBitVale.Key == GKStateBit.TurnOnNow_InAutomatic)
					{
						if (Regime == Regime.Automatic)
						{
							OnTurnOnNow();
						}
					}
					if (stateBitVale.Key == GKStateBit.TurnOnNow_InManual)
					{
						if (Regime == Regime.Manual)
						{
							OnTurnOnNow();
						}
					}
					if (stateBitVale.Key == GKStateBit.TurnOff_InAutomatic)
					{
						if (Regime == Regime.Automatic)
						{
							OnTurnOff();
						}
					}
					if (stateBitVale.Key == GKStateBit.TurnOffNow_InAutomatic)
					{
						if (Regime == Regime.Automatic)
						{
							OnTurnOffNow();
						}
					}
					if (stateBitVale.Key == GKStateBit.TurnOffNow_InManual)
					{
						if (Regime == Regime.Manual)
						{
							OnTurnOffNow();
						}
					}
					if (stateBitVale.Key == GKStateBit.Fire1)
					{
						if (Regime == Regime.Automatic)
						{
							OnSetFire1();
						}
					}
					if (stateBitVale.Key == GKStateBit.Fire2)
					{
						if (Regime == Regime.Automatic)
						{
							OnSetFire2();
						}
					}
					if (stateBitVale.Key == GKStateBit.SetRegime_Manual)
					{
						if (Regime == Regime.Automatic)
						{
							OnSetManualRegime();
						}
					}
					if (stateBitVale.Key == GKStateBit.SetRegime_Automatic)
					{
						if (Regime == Regime.Manual)
						{
							OnSetAutomaticRegime();
						}
					}
					if (stateBitVale.Key == GKStateBit.TurnOn_InManual)
					{
						if (Regime == Regime.Manual)
						{
							OnTurnOn();
						}
					}
					if (stateBitVale.Key == GKStateBit.TurnOff_InManual)
					{
						if (Regime == Regime.Manual)
						{
							OnTurnOff();
						}
					}
					if (stateBitVale.Key == GKStateBit.Stop_InManual)
					{
						if (Regime == Regime.Automatic)
						{
							OnPauseTurnOn();
						}
					}
					if (stateBitVale.Key == GKStateBit.Reset)
					{
						if (Regime == Regime.Automatic)
						{
							OnResetFire();
						}
					}
					if (GKBase is GKGuardZone && stateBitVale.Key == GKStateBit.Attention)
					{
						var oNstateBit = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On);
						var attentionStateBit = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Attention);
						var fire1 = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Fire1);
						if (Regime == Regime.Automatic && oNstateBit != null && oNstateBit.IsActive
							&& attentionStateBit != null && !attentionStateBit.IsActive && fire1 != null && !fire1.IsActive)
						{
							SetGuardAlarm();
						}
					}
				}
			}

			if (GKBase is GKZone && hasZoneBitsChanged)
			{
				var hasChanged = false;
				foreach (var stateBitVale in stateBitVales)
				{
					if (SetStateBit(stateBitVale.Key, stateBitVale.Value))
						hasChanged = true;
				}

				if (hasChanged)
				{
					if (stateBitVales.ContainsKey(GKStateBit.Attention) && stateBitVales[GKStateBit.Attention])
					{
						var journalItem = new ImitatorJournalItem(2, 4, 0, 0);
						AddJournalItem(journalItem);
					}
					else if (stateBitVales.ContainsKey(GKStateBit.Fire1) && stateBitVales[GKStateBit.Fire1])
					{
						var journalItem = new ImitatorJournalItem(2, 2, 0, 0);
						AddJournalItem(journalItem);
					}
					else if (stateBitVales.ContainsKey(GKStateBit.Fire2) && stateBitVales[GKStateBit.Fire2])
					{
						var journalItem = new ImitatorJournalItem(2, 3, 0, 0);
						AddJournalItem(journalItem);
					}
					else
					{
						var journalItem = new ImitatorJournalItem(2, 14, 0, 0);
						AddJournalItem(journalItem);
					}
				}
			}

			var newIntState = StatesToInt();
			if (newIntState != intState)
			{
				RecalculateOutputLogic();
			}
		}

		private static bool IsInSchedule(DbService dbService, int scheduleNo)
		{
			var timeSpan = DateTime.Now - new DateTime(2000, 1, 1);
			var nowTotalSeconds = timeSpan.TotalSeconds;

			var schedule = dbService.ImitatorScheduleTranslator.GetByNo(scheduleNo);
			if (schedule != null)
			{
				foreach (var imitatorSheduleInterval in schedule.ImitatorSheduleIntervals)
				{
					var delta = 0;
					if (schedule.TotalSeconds > 0)
					{
						var periodsCount = (nowTotalSeconds - imitatorSheduleInterval.StartSeconds) / schedule.TotalSeconds;
						delta = (int)periodsCount * schedule.TotalSeconds;
					}

					if (nowTotalSeconds > imitatorSheduleInterval.StartSeconds + delta || nowTotalSeconds < imitatorSheduleInterval.EndSeconds + delta)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}