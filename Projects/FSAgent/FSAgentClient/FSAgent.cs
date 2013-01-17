using FSAgentAPI;

namespace FSAgentClient
{
    public partial class FSAgent : IFSAgentContract
    {
        FSAgentFactory FSAgentFactory;
        public IFSAgentContract FSAgentContract { get; set; }
        string _serverAddress;

        public FSAgent(string serverAddress)
        {
            _serverAddress = serverAddress;
            FSAgentFactory = new FSAgentFactory();
            FSAgentContract = FSAgentFactory.Create(serverAddress);
        }
    }
}