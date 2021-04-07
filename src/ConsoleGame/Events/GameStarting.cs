using ConsoleGame.Entities;
using System;

namespace ConsoleGame.Events
{
	public class GameStarting : IEvent
	{
		public Guid RowGuid { get; set; }

		public Guid UserId { get; set; }

		public GameLevel Level { get; set; }

		public DateTime StartTime { get; set; }

		public int PcNumber { get; set; }
	}
}