using System.Collections.Generic;
using System.Linq;
using PowerCalculator.Models;

namespace PowerCalculator.Processor.Algorithms
{
	public class CalcPowerIndicators
	{
		public double ud, id, il;
	}

	public class CalcPowerAlgorithm
	{
		const double I1MESS = 0.0024;
		Line Line;
		public Dictionary<Device, CalcPowerIndicators> Result { get; set; }

		public CalcPowerAlgorithm(Line line)
		{
			Line = line;
		}

        void InitializeResult()
        {
            Result = new Dictionary<Device, CalcPowerIndicators>();
            Result.Add(Line.KAU, new CalcPowerIndicators() { id = Line.KAU.Driver.I, ud = Line.KAU.Driver.U, il = 0 });
            foreach (Device device in Line.Devices)
                Result.Add(device, new CalcPowerIndicators() { id = Dr(device.DriverType).I, ud = Dr(device.DriverType).U, il = 0 });
        }

		public void Calculate()
		{
            InitializeResult();
			produce();
		}

		#region wrappers
		void produce(bool reverse = false)
		{
			int be, en;
			uint nu = (uint)Line.Devices.Sum(e => Dr(e.DriverType).Mult);

			be = 0;
			for (int i = 1; i < Dc(); i++)
			{
				if (Dr(i, reverse).DeviceType == DeviceType.Supplier)
				{
					en = i;
					nu -= Dr(i).Mult;
					calc(be, en, nu, reverse);
					be = en;
				}
				else
				{
					nu -= Dr(i, reverse).Mult;
				}
			}
			if (be != Dc() - 1)
                calc(be, Dc() - 1, nu, reverse);

		}

		void calc(int b, int e, uint n, bool reverse = false)
		{
			double il, rsum, rleft, iright, ileft;

            n -= Dr(e, reverse).Mult;
			rsum = 0;
            if (Dr(e, reverse).DeviceType == DeviceType.Supplier)
			{
				for (int i = e; i > b; i--)
				{
                    n += Dr(i, reverse).Mult;
                    Di(i, reverse).id += n * I1MESS;
                    rsum += Dr(i, reverse).R + De(i, reverse).Cable.Resistance;
				}

                il = 1000 * (Dr(b, reverse).U - Dr(e, reverse).U) / rsum;
				rleft = 0;
				iright = 0;
				ileft = 0;
				for (int i = b + 1; i <= e; i++)
				{
                    rleft += De(i, reverse).Cable.Resistance + Dr(i, reverse).R;
                    ileft = Di(i, reverse).id * (rsum - rleft) / rsum;
                    Di(i, reverse).il += il + iright;
                    iright -= Di(i, reverse).id - ileft;
					for (int j = b + 1; j <= i; j++)
                        Di(j, reverse).il += ileft;
				}
				for (int i = b + 1; i < e; i++)
                    Di(i, reverse).ud = Di(i - 1, reverse).ud - Di(i, reverse).il * (Dr(i, reverse).R + De(i, reverse).Cable.Resistance) * 0.001;
			}
			else
			{
				il = 0;
				for (int i = e; i > b; i--)
				{
                    n += Dr(i, reverse).Mult;
                    Di(i, reverse).id += n * I1MESS;
                    il += Di(i, reverse).id;
                    Di(i, reverse).il = il;
				}
				for (int i = b + 1; i <= e; i++)
                    Di(i, reverse).ud = Di(i - 1, reverse).ud - Di(i, reverse).il * (Dr(i, reverse).R + De(i, reverse).Cable.Resistance) * 0.001;
			}

		}

		Driver Dr(DriverType driverType)
		{
			return DriversHelper.GetDriver(driverType);
		}

		Driver Dr(int index, bool reverse = false)
		{
			return DriversHelper.GetDriver(De(index, reverse).DriverType);
		}

		Device De(int index, bool reverse = false)
		{
			return
                index == 0 || index == Line.Devices.Count + 1 && Line.IsCircular ?
                Line.KAU : 
                Line.Devices[reverse ? Line.Devices.Count - index : index - 1];
		}

		CalcPowerIndicators Di(int index, bool reverse = false)
		{
			return Result[De(index, reverse)];
		}

        int Dc()
        {
            return Line.Devices.Count + (Line.IsCircular ? 2 : 1);
        }

		#endregion
	}
}