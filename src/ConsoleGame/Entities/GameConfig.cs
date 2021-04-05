using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGame.Entities
{
	public class GameConfig
	{
		public int Id { get; set; }

		public Guid UserId { get; set; }

		public string UserName { get; set; }

		public int NumberOfPlays { get; set; }

		public int NumberOfWins { get; set; }
	}
}
