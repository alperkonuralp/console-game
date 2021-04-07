using ConsoleGame.Events;
using GameAnalyticsSDK.Net;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using System.Threading.Tasks;

namespace ConsoleGame.GameAnalyticsSystem
{
	public class GameStartingHandler : IHandleMessages<GameStarting>
	{
		private readonly ILogger _logger;

		public GameStartingHandler(ILogger<GameStartingHandler> logger)
		{
			_logger = logger;
		}

		public Task Handle(GameStarting message)
		{
			_logger.LogInformation("{@ClassName}.{@MethodName} {@Status}.", GetType().FullName, nameof(Handle), "Starting");
			
			GameAnalytics.AddProgressionEvent(EGAProgressionStatus.Start, "ConsoleGame_" + message.Level);

			_logger.LogInformation("{@ClassName}.{@MethodName} {@Status}.", GetType().FullName, nameof(Handle), "Finished");
			
			return Task.CompletedTask;
		}
	}
}