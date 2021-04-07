using ConsoleGame.Events;
using GameAnalyticsSDK.Net;
using Rebus.Handlers;
using System.Threading.Tasks;

namespace ConsoleGame.GameAnalyticsSystem
{
	public class GameStartingHandler : IHandleMessages<GameStarting>
	{
		public Task Handle(GameStarting message)
		{
			GameAnalytics.AddProgressionEvent(EGAProgressionStatus.Start, "ConsoleGame_" + message.Level);

			return Task.CompletedTask;
		}
	}
}