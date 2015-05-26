using PowerCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerCalculator.Processor.Algorithms
{
	public class CalcPowerIndicators
	{
		public double ud, id, il;
	}

	public class CalcPowerAlgorithm
	{
		private const double I1MESS = 0.0024;
		private Line Line;
		public Dictionary<Device, CalcPowerIndicators> Result { get; set; }


		public CalcPowerAlgorithm(Line line)
		{
			Line = line;
			Result = new Dictionary<Device, CalcPowerIndicators>();

			foreach (Device device in Line.Devices)
				Result.Add(device, new CalcPowerIndicators() { id = Dr(device.DriverType).I, ud = Dr(device.DriverType).U, il = 0 });

		}

		public void Calculate()
		{
			produce();
		}

		#region wrappers
		private void produce()
		{
			int be, en;
			uint nu = (uint)Line.Devices.Sum(e => Dr(e.DriverType).Mult);

			be = 0;
			for (int i = 1; i < Line.Devices.Count; i++)
			{
				if (Dr(i).DeviceType == DeviceType.Supplier)
				{
					en = i;
					nu -= Dr(i).Mult;
					calc(be, en, nu);
					be = en;
				}
				else
				{
					nu -= Dr(i).Mult;
				}
			}
			if (be != Line.Devices.Count - 1)
				calc(be, Line.Devices.Count - 1, nu);

		}

		private void calc(int b, int e, uint n)
		{
			double il, rsum, rleft, iright, ileft;

			n -= Dr(e).Mult;
			rsum = 0;
			if (Dr(e).DeviceType == DeviceType.Supplier)
			{
				for (int i = e; i > b; i--)
				{
					n += Dr(i).Mult;
					Di(i).id += n * I1MESS;
					rsum += Dr(i).R + De(i).Cable.Resistance;
				}

				il = 1000 * (Dr(b).U - Dr(e).U) / rsum;
				rleft = 0;
				iright = 0;
				ileft = 0;
				for (int i = b + 1; i <= e; i++)
				{
					rleft += De(i).Cable.Resistance + Dr(i).R;
					ileft = Di(i).id * (rsum - rleft) / rsum;
					Di(i).il = il + iright;
					iright -= Di(i).id - ileft;
					for (int j = b + 1; j <= i; j++)
						Di(j).il += ileft;
				}
				for (int i = b + 1; i < e; i++)
					Di(i).ud = Di(i - 1).ud - Di(i).il * (Dr(i).R + De(i).Cable.Resistance) * 0.001;
			}
			else
			{
				il = 0;
				for (int i = e; i > b; i--)
				{
					n += Dr(i).Mult;
					Di(i).id += n * I1MESS;
					il += Di(i).id;
					Di(i).il = il;
				}
				for (int i = b + 1; i <= e; i++)
					Di(i).ud = Di(i - 1).ud - Di(i).il * (Dr(i).R + De(i).Cable.Resistance) * 0.001;
			}

		}

		private Driver Dr(DriverType driverType)
		{
			return DriversHelper.GetDriver(driverType);
		}

		private Driver Dr(int index, bool reverse = false)
		{
			return DriversHelper.GetDriver(De(index, reverse).DriverType);
		}

		private Device De(int index, bool reverse = false)
		{
			return Line.Devices[reverse ? Line.Devices.Count - 1 - index : index];
		}

		private CalcPowerIndicators Di(int index, bool reverse = false)
		{
			return Result[De(index, reverse)];
		}

		#endregion
	}
}