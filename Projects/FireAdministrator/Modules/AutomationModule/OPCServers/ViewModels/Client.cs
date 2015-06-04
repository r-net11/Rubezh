using Softing.Opc.Ua.Toolkit;
using Softing.Opc.Ua.Toolkit.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationModule.ViewModels
{
	public class OPCUAClient
	{
		public Session m_session = null;
		private const string m_monitoredItemNodeId = "ns=3;i=10846";
		public MonitoredItem m_monitoredItem = null;

		internal List<string> GetEndpoints(string serverUrl)
		{
			var result = new List<string>();

			IList<EndpointDescription> endpoints = Softing.Opc.Ua.Toolkit.Application.GetEndpoints(serverUrl);

			string previousApplicationName = string.Empty;
			foreach (EndpointDescription endpoint in endpoints)
			{
				if (previousApplicationName != endpoint.EndpointUrl)
				{
					result.Add(endpoint.EndpointUrl);
					previousApplicationName = endpoint.EndpointUrl;
				}

			}

			return result;
		}

		internal Session CreateSession(string sessionName, string serverUrl, MessageSecurityMode securityMode, SecurityPolicy securityPolicy, MessageEncoding messageEncoding, UserIdentity userId)
		{
			Session session = new Session(serverUrl, securityMode, securityPolicy.ToString(), messageEncoding, userId, null);
			session.SessionName = sessionName;
			session.Connect(false, true);
			return session;
		}

		internal void CreateOpcTcpSessionWithNoSecurity(string ServerUrl)
		{
			if (m_session != null)
			{
				return;
			}

			m_session = CreateSession(
				"UaBinaryNoSecuritySession",
				ServerUrl,
				MessageSecurityMode.None,
				SecurityPolicy.None,
				MessageEncoding.Binary,
				new AnonymousUserIdentity());
		}

		internal void DisconnectSession()
		{
			if (m_session == null)
			{
				return;
			}

			m_session.Disconnect(false);
			m_session.Dispose();
			m_session = null;
		}

		internal IList<ReferenceDescription> Browse(NodeId nodeId, object sender)
		{
			if (m_session == null)
			{
				return null;
			}
			IList<ReferenceDescription> results = null;

			results = m_session.Browse(nodeId, sender);

			return results;
		}

		internal List<string> BrowseAll(ReferenceDescription rootReferenceDescription, string spaces)
		{
			var result = new List<string>();
			if (rootReferenceDescription != null)
				result.Add(spaces + "+" + rootReferenceDescription.DisplayName);
			IList<ReferenceDescription> list = Browse(rootReferenceDescription == null ? null : new NodeId(rootReferenceDescription.NodeId), null);
			if (list != null)
			{
				foreach (ReferenceDescription l in list)
				{
					var ls = BrowseAll(l, spaces + "   ");
					for (int i = 0; i <= ls.Count - 1; i++)
					{
						result.Add(ls[i]);
					}
				}
			}
			return result;
		}

		internal List<string> BrowseMain()
		{
			var result = new List<string>();

			IList<ReferenceDescription> rootReferenceDescriptions = Browse(null, null);
			if (rootReferenceDescriptions != null)
			{
				foreach (ReferenceDescription rootReferenceDescription in rootReferenceDescriptions)
				{
					result.Add("  -" + rootReferenceDescription.DisplayName);
					if (rootReferenceDescription.BrowseName.Name == "Objects")
					{
						IList<ReferenceDescription> objectReferenceDescriptions = new List<ReferenceDescription>();
						objectReferenceDescriptions = Browse(new NodeId(rootReferenceDescription.NodeId), null);
						foreach (ReferenceDescription objectReferenceDescription in objectReferenceDescriptions)
						{
							result.Add("     -" + objectReferenceDescription.DisplayName);
							if (objectReferenceDescription.BrowseName.Name == "Server")
							{
								IList<ReferenceDescription> serverReferenceDescriptions = new List<ReferenceDescription>();
								serverReferenceDescriptions = Browse(new NodeId(objectReferenceDescription.NodeId), null);
								foreach (ReferenceDescription serverReferenceDescription in serverReferenceDescriptions)
								{
									result.Add("        -" + serverReferenceDescription.DisplayName);
								}
							}
						}
					}
				}
			}

			return result;
		}

		internal void CreateSubscription(string subscriptionname)
		{
			if (m_session != null)
			{
				Subscription subscription = new Subscription(m_session, subscriptionname);
			}
			else
			{
				throw new Exception("Session is not created");
			}
		}

		internal void DeleteSubscription(string subscriptionname)
		{
			if (m_session != null)
			{
				var l = m_session.Subscriptions.Where(x => x.DisplayName == subscriptionname).ToList<Subscription>();
				if (l.Count > 0)
				{
					foreach (Subscription kkk in l)
					{
						kkk.Dispose();
					}
				}
				else
				{
					throw new Exception("SubscriptionCount = 0");
				}
			}
			else
			{
				throw new Exception("Session is not created");
			}
		}

		internal List<string> GetSubscriptionList()
		{
			var result = new List<string>();
			if (m_session != null)
			{
				var l = m_session.Subscriptions.ToList<Subscription>();
				foreach (Subscription subsrc in l)
				{
					result.Add(subsrc.DisplayName);
				}
			}
			else
			{
				throw new Exception("Session is not created");
			}
			return result;
		}
	}
}