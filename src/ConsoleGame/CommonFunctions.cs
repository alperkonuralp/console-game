using System.Security.Cryptography;

namespace ConsoleGame
{
	public class CommonFunctions : ICommonFunctions
	{

		public int Next(int min, int max)
		{
			return RandomNumberGenerator.GetInt32(min, max);
		}

	}
}