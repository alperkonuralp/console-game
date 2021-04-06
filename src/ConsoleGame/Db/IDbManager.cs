using ConsoleGame.Entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace ConsoleGame.Db
{
	public interface IDbManager
	{
		void CheckDbFile();

		SqliteConnection GetConnection();

		GameConfig GetGameConfig();

		IEnumerable<PlayHistory> ListOfLastNItem(Guid userId, int n);
	}
}