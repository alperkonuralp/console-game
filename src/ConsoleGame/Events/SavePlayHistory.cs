using ConsoleGame.Entities;
using System;

namespace ConsoleGame.Events
{
	public class SavePlayHistory : IEvent
	{
		public Guid RowGuid { get; set; }

		public Guid UserId { get; set; }

		public GameStatus Status { get; set; }

		public GameLevel Level { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime? FinishTime { get; set; }

		public bool? IsTheWinner { get; set; }

		public int PcNumber { get; set; }

		public int? GamePoint { get; set; }

		public int IterationNumber { get; set; }

		public int[] Shoots { get; set; }
	}
}