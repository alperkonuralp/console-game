using ConsoleGame.Entities;
using ConsoleGame.Events;
using Rebus.Bus;
using System;
using System.Collections.Generic;

namespace ConsoleGame
{
	public class PlayGame : IPlayGame
	{
		private readonly GameConfig _gameConfig;
		private readonly ICommonFunctions _commonFunctions;
		private readonly IReadOnlyDictionary<GameLevel, int> _levelStandartPoints;
		private readonly IBus _bus;

		public PlayGame(
			GameConfig gameConfig
			, ICommonFunctions commonFunctions
			, IReadOnlyDictionary<GameLevel, int> levelStandartPoints
			, IBus bus)
		{
			_gameConfig = gameConfig;
			_commonFunctions = commonFunctions;
			_levelStandartPoints = levelStandartPoints;
			_bus = bus;
		}

		public void Play(GameLevel gameLevel)
		{
			PlayHistory history = new PlayHistory()
			{
				UserId = _gameConfig.UserId,
				Level = gameLevel,
				StartTime = DateTimeOffset.UtcNow.DateTime,
				IsTheWinner = false,
				PcNumber = _commonFunctions.Next(1, 1001),
				Shoots = new int[GetShootCount(gameLevel)],
			};

			Console.Clear();
			Console.WriteLine("The Console Game");
			Console.WriteLine(new string('-', 30));

			for (history.IterationNumber = 1; history.IterationNumber <= history.Shoots.Length; history.IterationNumber++)
			{
				int shoot;
				do
				{
					Console.Write($"{history.IterationNumber}. Shoot : ");
					var a = Console.ReadLine();
					if (int.TryParse(a, out shoot))
					{
						break;
					}
					shoot = 0;
				} while (shoot == 0);

				history.Shoots[history.IterationNumber - 1] = shoot;
				if (shoot == history.PcNumber)
				{
					Console.WriteLine("You win");
					history.IsTheWinner = true;
					break;
				}
				else if (shoot < history.PcNumber)
				{
					Console.WriteLine("Make the number bigger.");
				}
				else
				{
					Console.WriteLine("Make the number smaller.");
				}
			}
			history.FinishTime = DateTimeOffset.UtcNow.DateTime;
			_gameConfig.NumberOfPlays++;
			switch (gameLevel)
			{
				case GameLevel.Easy:
					_gameConfig.NumberOfEasyPlays++;
					break;

				case GameLevel.Normal:
					_gameConfig.NumberOfNormalPlays++;
					break;

				case GameLevel.Hard:
					_gameConfig.NumberOfHardPlays++;
					break;

				default:
					break;
			}
			if (!history.IsTheWinner)
			{
				Console.WriteLine("You Lost!...");
			}
			else
			{
				_gameConfig.NumberOfWins++;
				AddUserPoints(gameLevel);
				switch (gameLevel)
				{
					case GameLevel.Easy:
						_gameConfig.NumberOfEasyWins++;
						break;

					case GameLevel.Normal:
						_gameConfig.NumberOfNormalWins++;
						break;

					case GameLevel.Hard:
						_gameConfig.NumberOfHardWins++;
						break;

					default:
						break;
				}
			}

			_gameConfig.LastPlayDateTime = DateTimeOffset.UtcNow.DateTime;

			_bus.Send(new SaveGameConfig { Config = _gameConfig }).GetAwaiter().GetResult();
			_bus.Send(new SavePlayHistory { History = history }).GetAwaiter().GetResult();

			Console.WriteLine("Please enter to the continue.");
			Console.ReadLine();
		}

		private void AddUserPoints(GameLevel gameLevel)
		{
			_gameConfig.UserPoints += (long)(_levelStandartPoints[gameLevel] * _gameConfig.PointMultiplier);
		}

		private int GetShootCount(GameLevel gameLevel)
		{
			return gameLevel switch
			{
				GameLevel.Easy => 13,
				GameLevel.Hard => 8,
				_ => 10,
			};
		}
	}
}