using System;

namespace ConsoleGame.Entities
{
	public class PlayHistory
	{
		public int Id { get; set; }

		public Guid UserId { get; set; }

		public GameLevel Level { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime FinishTime { get; set; }

		public bool IsTheWinner { get; set; }

		public int PcNumber { get; set; }

		public int IterationNumber { get; set; }

		public int[] Shoots { get; set; }
	}
}