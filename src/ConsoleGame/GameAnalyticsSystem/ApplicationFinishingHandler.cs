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
			_logger.LogInformation("{@ClassName}.{@MethodName} {@Status}.", GetType().FullName, nameof(Handle), "Starting");
			
			GameAnalytics.EndSession();
			GameAnalytics.OnQuit();

			_logger.LogInformation("{@ClassName}.{@MethodName} {@Status}.", GetType().FullName, nameof(Handle), "Finished");
			return Task.CompletedTask;
		}
	}
}