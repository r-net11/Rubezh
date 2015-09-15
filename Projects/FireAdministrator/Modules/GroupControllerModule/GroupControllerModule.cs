using System;
using System.Collections.Generic;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using GKModule.Plans;
using GKModule.Validation;
using GKModule.ViewModels;
using GKProcessor;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using Infrastructure.Common.Services;
using System.IO;
using Ionic.Zip;
using System.Text;
using FiresecAPI.GK;

namespace GKModule
{
    public class GroupControllerModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
    {
        DevicesViewModel DevicesViewModel;
        ParameterTemplatesViewModel ParameterTemplatesViewModel;
        ZonesViewModel ZonesViewModel;
        DirectionsViewModel DirectionsViewModel;
        PumpStationsViewModel PumpStationsViewModel;
        MPTsViewModel MPTsViewModel;
        DelaysViewModel DelaysViewModel;
        CodesViewModel CodesViewModel;
        GuardZonesViewModel GuardZonesViewModel;
        DoorsViewModel DoorsViewModel;
        SKDZonesViewModel SKDZonesViewModel;
        LibraryViewModel DeviceLidraryViewModel;
		OPCsViewModel OPCViewModel;
        DescriptorsViewModel DescriptorsViewModel;
        DiagnosticsViewModel DiagnosticsViewModel;
        GKPlanExtension _planExtension;

        public override void CreateViewModels()
        {
            ServiceFactory.Events.GetEvent<CreateGKZoneEvent>().Subscribe(OnCreateGKZone);
            ServiceFactory.Events.GetEvent<EditGKZoneEvent>().Subscribe(OnEditGKZone);
            ServiceFactory.Events.GetEvent<CreateGKGuardZoneEvent>().Subscribe(OnCreateGKGuardZone);
            ServiceFactory.Events.GetEvent<EditGKGuardZoneEvent>().Subscribe(OnEditGKGuardZone);
            ServiceFactory.Events.GetEvent<CreateGKSKDZoneEvent>().Subscribe(OnCreateSKDZone);
            ServiceFactory.Events.GetEvent<EditGKSKDZoneEvent>().Subscribe(OnEditSKDZone);
            ServiceFactory.Events.GetEvent<CreateGKDelayEvent>().Subscribe(OnCreateGKDelay);
            ServiceFactory.Events.GetEvent<EditGKDelayEvent>().Subscribe(OnEditGKDelay);
            ServiceFactory.Events.GetEvent<CreateGKDirectionEvent>().Subscribe(OnCreateGKDirection);
            ServiceFactory.Events.GetEvent<EditGKDirectionEvent>().Subscribe(OnEditGKDirection);
            ServiceFactory.Events.GetEvent<CreateGKMPTEvent>().Subscribe(OnCreateGKMPT);
            ServiceFactory.Events.GetEvent<EditGKMPTEvent>().Subscribe(OnEditGKMPT);
            ServiceFactory.Events.GetEvent<CreateGKDoorEvent>().Subscribe(OnCreateGKDoor);
            ServiceFactory.Events.GetEvent<EditGKDoorEvent>().Subscribe(OnEditGKDoor);
            ServiceFactory.Events.GetEvent<CreateGKCodeEvent>().Subscribe(OnCreateGKCode);

            DevicesViewModel = new DevicesViewModel();
            ParameterTemplatesViewModel = new ParameterTemplatesViewModel();
            ZonesViewModel = new ZonesViewModel();
            DirectionsViewModel = new DirectionsViewModel();
            DelaysViewModel = new DelaysViewModel();
            PumpStationsViewModel = new PumpStationsViewModel();
            MPTsViewModel = new MPTsViewModel();
            DelaysViewModel = new DelaysViewModel();
            CodesViewModel = new CodesViewModel();
            GuardZonesViewModel = new GuardZonesViewModel();
            DoorsViewModel = new DoorsViewModel();
            SKDZonesViewModel = new SKDZonesViewModel();
            DeviceLidraryViewModel = new LibraryViewModel();
			OPCViewModel = new OPCsViewModel();
            DescriptorsViewModel = new DescriptorsViewModel();
            DiagnosticsViewModel = new DiagnosticsViewModel();
            _planExtension = new GKPlanExtension(DevicesViewModel, ZonesViewModel, GuardZonesViewModel, SKDZonesViewModel, DelaysViewModel, DirectionsViewModel, MPTsViewModel, DoorsViewModel);
        }

        public override void Initialize()
        {
            DevicesViewModel.Initialize();
            ParameterTemplatesViewModel.Initialize();
            ZonesViewModel.Initialize();
            DirectionsViewModel.Initialize();
            PumpStationsViewModel.Initialize();
            MPTsViewModel.Initialize();
            DelaysViewModel.Initialize();
            CodesViewModel.Initialize();
            GuardZonesViewModel.Initialize();
            DoorsViewModel.Initialize();
            SKDZonesViewModel.Initialize();
			OPCViewModel.Initialize();

            _planExtension.Initialize();
            ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
            _planExtension.Cache.BuildAllSafe();
        }
        public override IEnumerable<NavigationItem> CreateNavigation()
        {
            return new List<NavigationItem>()
			{
				new NavigationItem(ModuleType.ToDescription(), "Tree", new List<NavigationItem>()
				{
					new NavigationItem<ShowGKDeviceEvent, Guid>(DevicesViewModel, "Устройства", "Tree", null, null, Guid.Empty),
					new NavigationItem<ShowGKParameterTemplatesEvent, Guid>(ParameterTemplatesViewModel, "Шаблоны","Briefcase", null, null, Guid.Empty),
					new NavigationItemEx<ShowGKZoneEvent, Guid>(ZonesViewModel, "Пожарные зоны", "Zones", null, null, Guid.Empty),
                    new NavigationItem<ShowGKDirectionEvent, Guid>(DirectionsViewModel, "Направления", "Direction", null, null, Guid.Empty),
					new NavigationItem<ShowGKPumpStationEvent, Guid>(PumpStationsViewModel, "НС", "PumpStation", null, null, Guid.Empty),
					new NavigationItem<ShowGKMPTEvent, Guid>(MPTsViewModel, "МПТ", "MPT", null, null, Guid.Empty),
					new NavigationItem<ShowXDelayEvent, Guid>(DelaysViewModel, "Задержки", "Watch", null, null, Guid.Empty),

					new NavigationItem("Охрана", "tree",
						new List<NavigationItem>()
						{
							new NavigationItem<ShowGKCodeEvent, Guid>(CodesViewModel, "Коды", "User", null, null, Guid.Empty),
							new NavigationItemEx<ShowGKGuardZoneEvent, Guid>(GuardZonesViewModel, "Зоны", "Zones", null, null, Guid.Empty),
						}),
					new NavigationItem("СКД", "tree",
						new List<NavigationItem>()
						{
							new NavigationItemEx<ShowGKDoorEvent, Guid>(DoorsViewModel, "Точки доступа", "DoorW", null, null, Guid.Empty),
							new NavigationItemEx<ShowGKSKDZoneEvent, Guid>(SKDZonesViewModel, "Зоны", "Zones", null, null, Guid.Empty),
						}),

					new NavigationItem<ShowGKDescriptorsEvent, object>(DescriptorsViewModel, "Дескрипторы", "Descriptors"),
#if DEBUG
					new NavigationItem<ShowGKOPCDevicesEvent, Guid>(OPCViewModel,"OPC Сервер", "tree", null, null, Guid.Empty),
					new NavigationItem<ShowGKDeviceLidraryEvent, object>(DeviceLidraryViewModel, "Библиотека", "Book"),
#endif
					new NavigationItem<ShowGKDiagnosticsEvent, object>(DiagnosticsViewModel, "Диагностика", "Bug"),
				}) {IsExpanded = true},
			};
        }
        public override ModuleType ModuleType
        {
            get { return ModuleType.GK; }
        }
        public override void RegisterResource()
        {
            base.RegisterResource();
            var resourceService = new ResourceService();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Delays/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Descriptors/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DeviceLibrary/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Devices/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Diagnostics/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Directions/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Guard/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Journal/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "MPTs/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "OPC/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Parameters/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "PumpStation/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Selectation/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "SKD/DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Zones/DataTemplates/Dictionary.xaml"));
        }

        #region IValidationModule Members
        public IEnumerable<IValidationError> Validate()
        {
            var validator = new Validator();
            return validator.Validate();
        }
        #endregion

        private void OnCreateGKZone(CreateGKZoneEventArg createZoneEventArg)
        {
            ZonesViewModel.CreateZone(createZoneEventArg);
        }
        private void OnEditGKZone(Guid zoneUID)
        {
            ZonesViewModel.EditZone(zoneUID);
        }

        private void OnCreateGKGuardZone(CreateGKGuardZoneEventArg createZoneEventArg)
        {
            GuardZonesViewModel.CreateZone(createZoneEventArg);
        }
        private void OnEditGKGuardZone(Guid zoneUID)
        {
            GuardZonesViewModel.EditZone(zoneUID);
        }

        public void OnCreateGKDelay(CreateGKDelayEventArgs createDelayEventArg)
        {
			DelaysViewModel.CreateDelay(createDelayEventArg);
        }

        public void OnEditGKDelay(Guid delayUID)
        {
			DelaysViewModel.EditDelay(delayUID);
        }

        private void OnCreateGKDirection(CreateGKDirectionEventArg createDirectionEventArg)
        {
            DirectionsViewModel.CreateDirection(createDirectionEventArg);
        }
        private void OnEditGKDirection(Guid directionUID)
        {
            DirectionsViewModel.EditDirection(directionUID);
        }

        private void OnCreateGKMPT(CreateGKMPTEventArg createMPTEventArg)
        {
            MPTsViewModel.CreateMPT(createMPTEventArg);
        }
        private void OnEditGKMPT(Guid mptUID)
        {
            MPTsViewModel.EditMPT(mptUID);
        }

        private void OnCreateGKDoor(CreateGKDoorEventArg createGKDoorEventArg)
        {
            DoorsViewModel.CreateDoor(createGKDoorEventArg);
        }
        private void OnEditGKDoor(Guid doorUID)
        {
            DoorsViewModel.EditDoor(doorUID);
        }
        private void OnCreateSKDZone(CreateGKSKDZoneEventArg createZoneEventArg)
        {
            SKDZonesViewModel.CreateZone(createZoneEventArg);
        }
        private void OnEditSKDZone(Guid zoneUID)
        {
            SKDZonesViewModel.EditZone(zoneUID);
        }
        private void OnCreateGKCode(CreateGKCodeEventArg createGKCodeEventArg)
        {
            CodesViewModel.CreateCode(createGKCodeEventArg);
        }

        public override void AfterInitialize()
        {
            SafeFiresecService.CallbackOperationResultEvent -= new Action<CallbackOperationResult>(OnCallbackOperationResult);
            SafeFiresecService.CallbackOperationResultEvent += new Action<CallbackOperationResult>(OnCallbackOperationResult);
        }

        void OnCallbackOperationResult(CallbackOperationResult callbackOperationResult)
        {
            if (callbackOperationResult != null)
            {
                ApplicationService.Invoke(() =>
                {
                    LoadingService.Close();

                    if (callbackOperationResult.CallbackOperationResultType == CallbackOperationResultType.GetAllUsers)
                    {
                        if (!callbackOperationResult.HasError)
                        {
                            GKUsersViewModel gkUsersViewModel = null;
                            WaitHelper.Execute(() =>
                            {
                                gkUsersViewModel = new GKUsersViewModel(callbackOperationResult.Users);
                            });
                            //LoadingService.Close();
                            DialogService.ShowModalWindow(gkUsersViewModel);
                        }
                        else
                        {
                            //LoadingService.Close();
                            MessageBoxService.ShowWarning(callbackOperationResult.Error, "Ошибка при перезаписи пользователей");
                        }
                    }
                    if (callbackOperationResult.CallbackOperationResultType == CallbackOperationResultType.RewriteUsers)
                    {
                        if (!callbackOperationResult.HasError)
                        {
                            MessageBoxService.Show("Операция перезаписи пользователей завершилась успешно");
                        }
                        else
                        {
                            MessageBoxService.ShowWarning(callbackOperationResult.Error, "Ошибка при получении пользователей");
                        }
                    }
                    if (callbackOperationResult.CallbackOperationResultType == CallbackOperationResultType.WriteConfiguration)
                    {
                        if (!callbackOperationResult.HasError)
                        {
                            MessageBoxService.Show("Операция записи конфигурации в прибор завершилась успешно");
                        }
                        else
                        {
                            MessageBoxService.ShowWarning(callbackOperationResult.Error, "Ошибка при записи конфигурации в прибор");
                        }
                    }
                    if (callbackOperationResult.CallbackOperationResultType == CallbackOperationResultType.ReadConfigurationFromGKFile)
                    {
                        if (!callbackOperationResult.HasError)
                        {
                            var stream = FiresecManager.FiresecService.GetServerFile(callbackOperationResult.FileName);

                            if (stream != Stream.Null)
                            {
                                var folderName = AppDataFolderHelper.GetLocalFolder("GKFile");
                                var configFileName = Path.Combine(folderName, "Config.fscp");
                                if (Directory.Exists(folderName))
                                    Directory.Delete(folderName, true);
                                Directory.CreateDirectory(folderName);

                                var configFileStream = File.Create(configFileName);
                                FiresecManager.CopyStream(stream, configFileStream);
                                configFileStream.Close();

                                if (new FileInfo(configFileName).Length == 0)
                                {
                                    MessageBoxService.ShowError("Ошибка при чтении конфигурации");
                                    return;
                                }

                                var zipFile = ZipFile.Read(configFileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
                                var fileInfo = new FileInfo(configFileName);
                                var unzipFolderPath = fileInfo.Directory.FullName;
                                zipFile.ExtractAll(unzipFolderPath);
                                zipFile.Dispose();
                                var configurationFileName = Path.Combine(unzipFolderPath, "GKDeviceConfiguration.xml");
                                if (!File.Exists(configurationFileName))
                                {
                                    MessageBoxService.ShowError("Ошибка при распаковке файла");
                                    return;
                                }
                                var deviceConfiguration = ZipSerializeHelper.DeSerialize<GKDeviceConfiguration>(configurationFileName, true);

                                ConfigurationCompareViewModel configurationCompareViewModel = null;
                                WaitHelper.Execute(() =>
                                {
                                    DescriptorsManager.Create();
                                    deviceConfiguration.UpdateConfiguration();
                                    deviceConfiguration.PrepareDescriptors();
                                    configurationCompareViewModel = new ConfigurationCompareViewModel(GKManager.DeviceConfiguration, deviceConfiguration, DevicesViewModel.SelectedDevice.Device, true, configFileName);
                                });
                                //LoadingService.Close();
                                if (configurationCompareViewModel.Error != null)
                                {
                                    MessageBoxService.ShowError(configurationCompareViewModel.Error, "Ошибка при чтении конфигурации");
                                    return;
                                }
                                if (DialogService.ShowModalWindow(configurationCompareViewModel))
                                    ServiceFactoryBase.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
                            }
                            else
                            {
                                //LoadingService.Close();
                                MessageBoxService.ShowWarning("Ошибка при чтении конфигурационного файла");
                            }
                        }
                        else
                        {
                            MessageBoxService.ShowWarning(callbackOperationResult.Error, "Ошибка при чтении конфигурации из прибора");
                        }
                    }
                });
            }
        }

        #region ILayoutDeclarationModule Members

        public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
        {
            yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Indicator, 110, "Индикаторы", "Панель индикаторов состояния", "BAlarm.png", false, new LayoutPartSize() { PreferedSize = new Size(1000, 100) });
            yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.ConnectionIndicator, 111, "Индикатор связи", "Панель индикаторов связи", "BConnectionIndicator.png", true, new LayoutPartSize() { PreferedSize = new Size(50, 30) });
            yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Alarms, 112, "Состояния", "Панель состояний", "BAlarm.png");
            yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.GDevices, 113, "Устройства", "Панель с устройствами", "BTree.png");
            yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Zones, 114, "Зоны", "Панель зон", "BZones.png");
            yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.GuardZones, 115, "Охранные зоны", "Панель охранных зон", "BZones.png");
            yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Directions, 116, "Направления", "Панель направления", "BDirection.png");
            yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.PumpStations, 117, "НС", "Панель НС", "BPumpStation.png");
            yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.MPTs, 118, "МПТ", "Панель МПТ", "BMPT.png");
            yield return new LayoutPartDescription(LayoutPartDescriptionGroup.GK, LayoutPartIdentities.Doors, 119, "Точки доступа", "Панель точек досткпа", "BMPT.png");
        }

        #endregion
    }
}