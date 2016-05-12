using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GkWeb.Models;
using Microsoft.AspNet.SignalR;

namespace GkWeb.Hubs
{
	/// <summary>
	/// A signalR hub that provides channel-based event broadcasting
	/// that clients can subscribe to
	/// </summary>
	public class EventHub : Hub
	{
		public async Task Subscribe(string channel) {
			await Groups.Add(Context.ConnectionId, channel);

			var ev = new ChannelEvent {
				ChannelName = Constants.AdminChannel,
				Name = "user.subscribed",
				Data = new {
					Context.ConnectionId,
					ChannelName = channel
				}
			};

			await Publish(ev);
		}

		public async Task Unsubscribe(string channel) {
			await Groups.Remove(Context.ConnectionId, channel);

			var ev = new ChannelEvent {
				ChannelName = Constants.AdminChannel,
				Name = "user.unsubscribed",
				Data = new {
					Context.ConnectionId,
					ChannelName = channel
				}
			};

			await Publish(ev);
		}


		public Task Publish(ChannelEvent channelEvent) {
			Clients.Group(channelEvent.ChannelName).OnEvent(channelEvent.ChannelName, channelEvent);

			if (channelEvent.ChannelName != Constants.AdminChannel) {
				// Push this out on the admin channel
				//
				Clients.Group(Constants.AdminChannel).OnEvent(Constants.AdminChannel, channelEvent);
			}

			return Task.FromResult(0);
		}


		public override Task OnConnected() {
			var ev = new ChannelEvent {
				ChannelName = Constants.AdminChannel,
				Name = "user.connected",
				Data = new {
					Context.ConnectionId,
				}
			};

			Publish(ev);

			return base.OnConnected();
		}


		public override Task OnDisconnected(bool stopCalled) {
			var ev = new ChannelEvent {
				ChannelName = Constants.AdminChannel,
				Name = "user.disconnected",
				Data = new {
					Context.ConnectionId,
				}
			};

			Publish(ev);

			return base.OnDisconnected(stopCalled);
		}
	}
}