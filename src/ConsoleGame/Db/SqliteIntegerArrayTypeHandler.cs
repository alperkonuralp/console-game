using Dapper;
using System;
using System.Data;
using System.Linq;

namespace ConsoleGame.Db
{
	public class SqliteIntegerArrayTypeHandler : SqlMapper.TypeHandler<int[]>
	{
		public override void SetValue(IDbDataParameter parameter, int[] value)
		{
			parameter.Value = string.Join(',', value);
		}

		public override int[] Parse(object value)
		{
			if (value == null || value == DBNull.Value)
				return Array.Empty<int>();
			if (value is int[] ia)
				return ia;

			if (value is string s)
			{
				return s.Split(',', StringSplitOptions.None)
					.Select(x => int.Parse(x))
					.ToArray();
			}

			return Array.Empty<int>();
		}
	}
}