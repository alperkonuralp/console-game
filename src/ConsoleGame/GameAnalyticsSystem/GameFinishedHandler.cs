using AutoMapper;
using ConsoleGame.Events;
using GameAnalyticsSDK.Net;
using Rebus.Bus;
using Rebus.Handlers;
using System.Threading.Tasks;

namespace ConsoleGame.GameAnalyticsSystem
{
	public class GameFinishedHandler : IHandleMessages<GameFinished>
	{

		public Task Handle(GameFinished message)
		{
			GameAnalytics.AddProgressionEvent(
				EGAProgressionStatus.Complete
				, "ConsoleGame_" + message.Level
				, message.GamePoint ?? 0);

			return Task.CompletedTask;
		}
	}
}