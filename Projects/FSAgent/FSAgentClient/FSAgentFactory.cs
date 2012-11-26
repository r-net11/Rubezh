using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSAgentAPI;
using System.ServiceModel;
using Common;
using System.Threading;
using System.ServiceModel.Description;

namespace FSAgentClient
{
    public class FSAgentFactory
    {
        public static Guid UID = Guid.NewGuid();
        ChannelFactory<IFSAgentContract> ChannelFactory;

        public IFSAgentContract Create(string serverAddress)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    return DoCreate(serverAddress);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "FSAgentClient.Create");
                    if (serverAddress.StartsWith("net.pipe:"))
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            }
            return null;
        }

        IFSAgentContract DoCreate(string serverAddress)
        {
            serverAddress = "net.pipe://127.0.0.1/FSAgent/";
            var binding = BindingHelper.CreateNetNamedPipeBinding();

            var endpointAddress = new EndpointAddress(new Uri(serverAddress));
            ChannelFactory = new ChannelFactory<IFSAgentContract>(binding, endpointAddress);

            foreach (OperationDescription operationDescription in ChannelFactory.Endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
                if (dataContractSerializerOperationBehavior != null)
                    dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = Int32.MaxValue;
            }

            ChannelFactory.Open();

            IFSAgentContract firesecService = ChannelFactory.CreateChannel();
            (firesecService as IContextChannel).OperationTimeout = TimeSpan.FromMinutes(1);
            return firesecService;
        }

        public void Dispose()
        {
            try
            {
                if (ChannelFactory != null)
                {
                    try
                    {
                        ChannelFactory.Close();
                    }
                    catch { }
                    try
                    {
                        ChannelFactory.Abort();
                    }
                    catch { }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "FSAgentClient.Dispose");
            }
        }
    }
}