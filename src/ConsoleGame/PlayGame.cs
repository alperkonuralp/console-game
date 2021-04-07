using AutoMapper;
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
		private readonly IMapper _mapper;

		public PlayGame(
			GameConfig gameConfig
			, ICommonFunctions commonFunctions
			, IReadOnlyDictionary<GameLevel, int> levelStandartPoints
			, IBus bus
			, IMapper mapper)
		{
			_gameConfig = gameConfig;
			_commonFunctions = commonFunctions;
			_levelStandartPoints = levelStandartPoints;
			_bus = bus;
			_mapper = mapper;
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

			_bus.Send(_mapper.Map<GameStarting>(history)).GetAwaiter().GetResult();

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
			if (!history.IsTheWinner.HasValue) history.IsTheWinner = false;
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
			if (!history.IsTheWinner.Value)
			{
				Console.WriteLine("You Lost!...");
			}
			else
			{
				_gameConfig.NumberOfWins++;
				AddUserPoints(gameLevel, history);
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

			var gameFinished = _mapper.Map<GameFinished>(history);
			gameFinished.Game = _mapper.Map<GameFinishedGame>(_gameConfig);

			_bus.Advanced.SyncBus.Send(gameFinished);

			Console.WriteLine("Please enter to the continue.");
			Console.ReadLine();
		}

		private void AddUserPoints(GameLevel gameLevel, PlayHistory history)
		{
			int p = (int)(_levelStandartPoints[gameLevel] * _gameConfig.PointMultiplier);
			history.GamePoint = p;
			_gameConfig.UserPoints += (uint)p;
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