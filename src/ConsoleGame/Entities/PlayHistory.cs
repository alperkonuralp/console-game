using System;

namespace ConsoleGame.Entities
{
	public class PlayHistory
	{
		public int Id { get; set; }

		public Guid UserId { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime  FinishTime { get; set; }

		public bool IsTheWinner { get; set; }
	}
}
