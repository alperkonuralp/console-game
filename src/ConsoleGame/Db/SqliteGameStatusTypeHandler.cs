using ConsoleGame.Entities;
using Dapper;
using System;
using System.Data;

namespace ConsoleGame.Db
{
	public class SqliteGameStatusTypeHandler : SqlMapper.TypeHandler<GameStatus>
	{
		public override void SetValue(IDbDataParameter parameter, GameStatus value)
		{
			parameter.Value = value.ToString();
		}

		public override GameStatus Parse(object value)
		{
			// Dapper may pass a Enum instead of a string
			return value is GameStatus g ? g : Enum.Parse<GameStatus>(value.ToString());
		}
	}
}