using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GkWeb.Hubs;
using GkWeb.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GkWeb.Controllers
{
	[Authorize]
	public class TaskController : Controller
    {
		private readonly IHubContext _context;
		private readonly ILogger<TaskController> _logger;
		private readonly string _channel = Constants.TaskChannel;

		public TaskController(IHubContext<EventHub> hubContext, ILogger<TaskController> logger) {
			_logger = logger;
			_context = hubContext;
		}

		[Route("long")]
		[HttpGet]
		public IActionResult GetLongTask() {
			_logger.LogInformation("Starting long task");

			double steps = 10;
			var eventName = "longTask.status";

			ExecuteTask(eventName, steps);

			return Ok("Long task complete");
		}

		[Route("short")]
		[HttpGet]
		public IActionResult GetShortTask() {
			_logger.LogInformation("Starting short task");

			double steps = 5;
			var eventName = "shortTask.status";

			ExecuteTask(eventName, steps);

			return Ok("Short task complete");
		}

		private void ExecuteTask(string eventName, double steps) {
			var status = new Status {
				State = "starting",
				PercentComplete = 0.0
			};

			PublishEvent(eventName, status);

			for (double i = 0; i < steps; i++) {
				// Update the status and publish a new event
				//
				status.State = "working";
				status.PercentComplete = (i / steps) * 100;
				PublishEvent(eventName, status);

				Thread.Sleep(500);
			}

			status.State = "complete";
			status.PercentComplete = 100;
			PublishEvent(eventName, status);
		}

		private void PublishEvent(string eventName, Status status) {
			// From .NET code like this we can't invoke the methods that
			//  exist on our actual Hub class...because we only have a proxy
			//  to it. So to publish the event we need to call the method that
			//  the clients will be listening on.
			//
			_context.Clients.Group(_channel).OnEvent(Constants.TaskChannel, new ChannelEvent {
				ChannelName = Constants.TaskChannel,
				Name = eventName,
				Data = status
			});
		}       
    }

	public class Status
	{
		public string State { get; set; }

		public double PercentComplete { get; set; }
	}
}
