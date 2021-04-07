using ConsoleGame.Events;
using GameAnalyticsSDK.Net;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using System.Threading.Tasks;

namespace ConsoleGame.GameAnalyticsSystem
{
	public class ApplicationFinishingHandler : IHandleMessages<ApplicationFinishing>
	{
		private readonly ILogger _logger;

		public ApplicationFinishingHandler(ILogger<ApplicationFinishingHandler> logger)
		{
			_logger = logger;
		}

		public Task Handle(ApplicationFinishing message)
		{
			GameAnalytics.EndSession();
			GameAnalytics.OnQuit();

			_logger.LogInformation(ApplicationStartedHandler.LoggerName + "System is closed.");
			return Task.CompletedTask;
		}
	}
}