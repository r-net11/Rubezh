using System.Collections.Generic;
using System.Linq;
using Common.GK;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using System;
using Common;
using System.Diagnostics;

namespace GKModule
{
    public static class ParametersHelper
    {
        public static void GetAllParameters()
        {
            DatabaseManager.Convert();
            foreach (var kauDatabase in DatabaseManager.KauDatabases)
            {
                LoadingService.Show("Запрос параметров", kauDatabase.BinaryObjects.Count);
                try
                {
                    foreach (var binaryObject in kauDatabase.BinaryObjects)
                    {
                        if (binaryObject.Device != null)
                        {
                            GetDeviceParameters(kauDatabase, binaryObject);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, "ParametersHelper.GetParametersFromDB");
                }
                LoadingService.Close();
            }
            ServiceFactory.SaveService.GKChanged = true;
        }

        public static void SetAllParameters()
        {
            DatabaseManager.Convert();
            foreach (var kauDatabase in DatabaseManager.KauDatabases)
            {
                LoadingService.Show("Запись параметров", kauDatabase.BinaryObjects.Count);
                try
                {
                    foreach (var binaryObject in kauDatabase.BinaryObjects)
                    {
                        if (binaryObject.Device != null)
                        {
                            SetDeviceParameters(kauDatabase, binaryObject);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, "ParametersHelper.SetParametersToDB");
                }
                LoadingService.Close();
            }
        }

        public static void SetSingleParameter(XDevice device)
        {
            DatabaseManager.Convert();
            LoadingService.Show("Запись параметров", 1);
            try
            {
                var kauDatabase = DatabaseManager.KauDatabases.FirstOrDefault(x => x.RootDevice == device.KauDatabaseParent);
                if (kauDatabase != null)
                {
                    var binaryObject = kauDatabase.BinaryObjects.FirstOrDefault(x => x.Device == device);
                    if (binaryObject != null)
                    {
                        SetDeviceParameters(kauDatabase, binaryObject);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "ParametersHelper.SetSingleParameter");
            }
            LoadingService.Close();
        }

        public static void GetSingleParameter(XDevice device)
        {
            DatabaseManager.Convert();
            LoadingService.Show("Запрос параметров", 1);
            try
            {
                var kauDatabase = DatabaseManager.KauDatabases.FirstOrDefault(x => x.RootDevice == device.KauDatabaseParent);
                if (kauDatabase != null)
                {
                    var binaryObject = kauDatabase.BinaryObjects.FirstOrDefault(x => x.Device == device);
                    if (binaryObject != null)
                    {
                        GetDeviceParameters(kauDatabase, binaryObject);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "ParametersHelper.SetSingleParameter");
            }
            LoadingService.Close();
        }

        static void GetDeviceParameters(KauDatabase kauDatabase, BinaryObjectBase binaryObject)
        {
            var no = binaryObject.GetNo();
            LoadingService.DoStep("Запрос параметров объекта " + no);
            var sendResult = SendManager.Send(kauDatabase.RootDevice, 2, 9, ushort.MaxValue, BytesHelper.ShortToBytes(no));

            if (sendResult.HasError == false)
            {
                var binProperties = new List<BinProperty>();
                for (int i = 0; i < sendResult.Bytes.Count / 4; i++)
                {
                    byte paramNo = sendResult.Bytes[i * 4];
                    ushort paramValue = BytesHelper.SubstructShort(sendResult.Bytes, i * 4 + 1);
                    var binProperty = new BinProperty()
                    {
                        ParamNo = paramNo,
                        ParamValue = paramValue
                    };
                    binProperties.Add(binProperty);
                }

                if (binaryObject.Device != null)
                {
                    foreach (var driverProperty in binaryObject.Device.Driver.Properties)
                    {
                        var binProperty = binProperties.FirstOrDefault(x => x.ParamNo == driverProperty.No);
                        if (binProperty != null)
                        {
                            var paramValue = (ushort)binProperty.ParamValue;
                            if (driverProperty.IsLowByte)
                            {
                                paramValue = (ushort)(paramValue << 8);
                                paramValue = (ushort)(paramValue >> 8);
                            }
                            if (driverProperty.IsHieghByte)
                            {
                                paramValue = (ushort)(paramValue >> 8);
                                paramValue = (ushort)(paramValue << 8);
                            }

                            paramValue = (ushort)(paramValue >> driverProperty.Offset);

                            var property = binaryObject.Device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
                            if (property != null)
                            {
                                if (property.Value != paramValue)
                                    property.Value = paramValue;

                                Trace.WriteLine("Property " + property.Name + " " + property.Value + " " + driverProperty.Offset.ToString() + " " + driverProperty.No.ToString());
                            }
                        }
                    }
                }
            }
            var deviceViewModel = DevicesViewModel.Current.Devices.FirstOrDefault(x => x.Device.UID == binaryObject.Device.UID);
            if (deviceViewModel != null)
            {
                deviceViewModel.UpdateProperties();
            }
        }

        static void SetDeviceParameters(KauDatabase kauDatabase, BinaryObjectBase binaryObject)
        {
            if (binaryObject.Parameters.Count > 0)
            {
                var rootDevice = kauDatabase.RootDevice;
                var no = binaryObject.GetNo();
                var bytes = new List<byte>();
                bytes.AddRange(BytesHelper.ShortToBytes(no));
                bytes.AddRange(binaryObject.Parameters);
                LoadingService.DoStep("Запись параметров объекта " + no);
                SendManager.Send(rootDevice, (ushort)bytes.Count, 10, 0, bytes);
            }
        }
    }

    class BinProperty
    {
        public byte ParamNo { get; set; }
        public ushort ParamValue { get; set; }
    }
}