using ConsoleGame.Events;
using GameAnalyticsSDK.Net;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using System.Threading.Tasks;

namespace ConsoleGame.GameAnalyticsSystem
{
	public class GameFinishedHandler : IHandleMessages<GameFinished>
	{
		private readonly ILogger _logger;

		public GameFinishedHandler(ILogger<GameFinishedHandler> logger)
		{
			_logger = logger;
		}

		public Task Handle(GameFinished message)
		{
			_logger.LogInformation("{@ClassName}.{@MethodName} {@Status}.", GetType().FullName, nameof(Handle), "Starting");

			GameAnalytics.AddProgressionEvent(
				EGAProgressionStatus.Complete
				, "ConsoleGame_" + message.Level
				, message.GamePoint ?? 0);

			_logger.LogInformation("{@ClassName}.{@MethodName} {@Status}.", GetType().FullName, nameof(Handle), "Finished");
			return Task.CompletedTask;
		}
	}
}