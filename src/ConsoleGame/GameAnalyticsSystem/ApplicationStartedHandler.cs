using ConsoleGame.Events;
using GameAnalyticsSDK.Net;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using System.Threading.Tasks;

namespace ConsoleGame.GameAnalyticsSystem
{
	public class ApplicationStartedHandler : IHandleMessages<ApplicationStarted>
	{
		public const string LoggerName = "GameAnalytics";
		private readonly ILogger _logger;

		public ApplicationStartedHandler(ILogger<ApplicationStartedHandler> logger)
		{
			_logger = logger;
		}

		public Task Handle(ApplicationStarted message)
		{
			SetupGameAnalytics(message);

			_logger.LogInformation(LoggerName + "System is started.");
			return Task.CompletedTask;
		}

		private void SetupGameAnalytics(ApplicationStarted message)
		{
			GameAnalytics.SetEnabledInfoLog(true);
			// GameAnalytics.SetEnabledVerboseLog(true)

			GameAnalytics.ConfigureBuild("0.10");
			GameAnalytics.ConfigureUserId($"ConsoleGame_{message.UserId}");

			GameAnalytics.ConfigureAvailableResourceCurrencies("gems", "gold");
			GameAnalytics.ConfigureAvailableResourceItemTypes("boost", "lives");

			GameAnalytics.ConfigureAvailableCustomDimensions01("ninja", "samurai");
			GameAnalytics.ConfigureAvailableCustomDimensions02("whale", "dolpin");
			GameAnalytics.ConfigureAvailableCustomDimensions03("horde", "alliance");

			GameAnalytics.Initialize("0561632006fdb5b79e1d0c05a271ce64", "49c2dd90ef213b9ce78dcb9311f97bd9b7c51bd5");

			GameAnalytics.OnMessageLogged += GameAnalyticsOnMessageLogged;

			GameAnalytics.StartSession();
		}

		private static void GameAnalyticsOnMessageLogged(string msg, EGALoggerMessageType level)
		{
			switch (level)
			{
				case EGALoggerMessageType.Error:
					Serilog.Log.Error("{@Logger} : " + msg, LoggerName);
					break;
				case EGALoggerMessageType.Warning:
					Serilog.Log.Warning("{@Logger} : " + msg, LoggerName);
					break;
				case EGALoggerMessageType.Info:
					Serilog.Log.Information("{@Logger} : " + msg, LoggerName);
					break;
				case EGALoggerMessageType.Debug:
					Serilog.Log.Debug("{@Logger} : " + msg, LoggerName);
					break;
				default:
					break;
			}
		}
	}
}