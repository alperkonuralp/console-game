using System;

namespace ConsoleGame.Events
{
	public class SaveGameConfig : IEvent
	{
		public int Id { get; set; }

		public Guid UserId { get; set; }

		public string UserName { get; set; }

		public int NumberOfPlays { get; set; }

		public int NumberOfWins { get; set; }

		public int NumberOfEasyPlays { get; set; }

		public int NumberOfEasyWins { get; set; }

		public int NumberOfNormalPlays { get; set; }

		public int NumberOfNormalWins { get; set; }

		public int NumberOfHardPlays { get; set; }

		public int NumberOfHardWins { get; set; }

		public DateTime LastPlayDateTime { get; set; }

		public long UserPoints { get; set; }

		public double PointMultiplier { get; set; }
	}
}