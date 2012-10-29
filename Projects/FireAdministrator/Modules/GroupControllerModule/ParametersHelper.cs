using System.Collections.Generic;
using System.Linq;
using Common.GK;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using System;
using Common;

namespace GKModule
{
    public static class ParametersHelper
    {
        public static void GetAllParameters()
        {
            DatabaseManager.Convert();
            foreach (var gkDatabase in DatabaseManager.GkDatabases)
            {
                LoadingService.Show("Запрос параметров", gkDatabase.BinaryObjects.Count);
                try
                {
                    foreach (var binaryObject in gkDatabase.BinaryObjects)
                    {
                        if (binaryObject.Device != null)
                        {
                            GetDeviceParameters(gkDatabase, binaryObject);
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
            foreach (var gkDatabase in DatabaseManager.GkDatabases)
            {
                LoadingService.Show("Запись параметров", gkDatabase.BinaryObjects.Count);
                try
                {
                    foreach (var binaryObject in gkDatabase.BinaryObjects)
                    {
                        if (binaryObject.Device != null)
                        {
                            SetDeviceParameters(gkDatabase, binaryObject);
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
                var gkDatabase = DatabaseManager.GkDatabases.FirstOrDefault(x => x.RootDevice == device.GkDatabaseParent);
                if (gkDatabase != null)
                {
                    var binaryObject = gkDatabase.BinaryObjects.FirstOrDefault(x => x.Device == device);
                    if (binaryObject != null)
                    {
                        SetDeviceParameters(gkDatabase, binaryObject);
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
                var gkDatabase = DatabaseManager.GkDatabases.FirstOrDefault(x => x.RootDevice == device.GkDatabaseParent);
                if (gkDatabase != null)
                {
                    var binaryObject = gkDatabase.BinaryObjects.FirstOrDefault(x => x.Device == device);
                    if (binaryObject != null)
                    {
                        GetDeviceParameters(gkDatabase, binaryObject);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "ParametersHelper.SetSingleParameter");
            }
            LoadingService.Close();
        }

        static void GetDeviceParameters(GkDatabase gkDatabase, BinaryObjectBase binaryObject)
        {
            var no = binaryObject.GetNo();
            LoadingService.DoStep("Запрос параметров объекта " + no);
            var sendResult = SendManager.Send(gkDatabase.RootDevice, 2, 9, ushort.MaxValue, BytesHelper.ShortToBytes(no));

            if (sendResult.HasError == false)
            {
                for (int i = 0; i < sendResult.Bytes.Count / 4; i++)
                {
                    byte paramNo = sendResult.Bytes[i * 4];
                    ushort paramValue = BytesHelper.SubstructShort(sendResult.Bytes, i * 4 + 1);

                    if (binaryObject.Device != null)
                    {
                        var driverProperty = binaryObject.Device.Driver.Properties.FirstOrDefault(x => x.No == paramNo);
                        if (driverProperty != null)
                        {
                            var property = binaryObject.Device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
                            if (property != null)
                            {
                                if (property.Value != paramValue)
                                    property.Value = paramValue;
                            }
                            else
                                MessageBoxService.Show("Не найдено свойство устройства");
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

        static void SetDeviceParameters(GkDatabase gkDatabase, BinaryObjectBase binaryObject)
        {
            if (binaryObject.Parameters.Count > 0)
            {
                var rootDevice = gkDatabase.RootDevice;
                var no = binaryObject.GetNo();
                var bytes = new List<byte>();
                bytes.AddRange(BytesHelper.ShortToBytes(no));
                bytes.AddRange(binaryObject.Parameters);
                LoadingService.DoStep("Запись параметров объекта " + no);
                SendManager.Send(rootDevice, (ushort)bytes.Count, 10, 0, bytes);
            }
        }
    }
}