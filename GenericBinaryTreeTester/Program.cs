using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericBinaryTree;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Configuration;

namespace GenericBinaryTreeTester
{
	class Program
	{
		static Random _r = new Random();
		static Stopwatch sw = new Stopwatch();

		static void Main(string[] args)
		{
			var btree = new BTree<Tuple<int, string>>();
			int amountOfCalls = 100000;
			var lookups = new List<Tuple<int, string>>();
			int amountOfLookups = 10000;
			List<Call> rows = null;

			#region Create/retrieve calls
			ExecuteTimedComputation(() =>
			{
                //rows = CallsFromDbServer(amountOfCalls); // Use different methods to retrieve from memory, DB, etc
                rows = InMemoryList(amountOfCalls);
			}, $"Creating/retrieving {amountOfCalls} calls");
			#endregion

			#region Create index
			ExecuteTimedComputation(() =>
			{
				foreach (var row in rows)
                {
                    btree.Add(new Tuple<int, string>(row.ClientId, row.DNIS));
                }
			}, $"Creating index for {rows.Count} entries");
			#endregion

			#region Create lookup list
			ExecuteTimedComputation(() =>
			{
				for (int i = 0; i < amountOfLookups; i++)
                {
                    lookups.Add(new Tuple<int, string>(_r.Next(100), RandomDnis()));
                }
			}, $"Creating {amountOfLookups} lookup entries");
			#endregion

			#region Do linear lookups (COMMENT IF WORKING WITH BIG TABLES)
			ExecuteTimedComputation(() =>
            {
                foreach (var l in lookups)
                {
                    rows.FirstOrDefault(call => call.ClientId == l.Item1 && call.DNIS == l.Item2);
                }
			}, $"Doing {amountOfLookups} linear lookups");
			#endregion

			#region Do binary tree lookups
			ExecuteTimedComputation(() =>
			{
				foreach (var l in lookups)
                {
                    btree.Find(l);
                }
			}, $"Doing {amountOfLookups} lookups with index");
			#endregion

			Console.ReadLine();
		}

		private static List<Call> InMemoryList(int amountOfCallsToCreate)
		{
			var result = new List<Call>();
			for (int i = 1; i <= amountOfCallsToCreate; i++)
			{
				result.Add(new Call()
				{
					CallId = i,
					ClientId = 1 + _r.Next(100),
					ANI = RandomDigitString(10),
					DNIS = RandomDnis(),
					RingTo = RandomDigitString(10),
					Duration = 300 * (decimal)_r.NextDouble(),
					StartDateTime = DateTime.UtcNow.AddMinutes(-_r.Next(10000))
				});
			}
			return result;
		}

		private static List<Call> CallsFromDbServer(int amountOfCallsToRetrieve)
		{
			var result = new List<Call>();
			using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
			{
				conn.Open();
				using (var cmd = new SqlCommand($"SELECT TOP {amountOfCallsToRetrieve} CallId, ClientId, DNIS, CallerPhoneNumber, RingTo, Duration, StartDate FROM Calls", conn))
				{
					using (var rdr = cmd.ExecuteReader())
					{
						while (rdr.Read())
							result.Add(new Call()
							{
								CallId = rdr.GetInt64(0),
								ClientId = rdr.GetInt32(1),
								DNIS = rdr.GetString(2),
								ANI = rdr.GetString(3),
								RingTo = rdr.GetString(4),
								Duration = rdr.GetDecimal(5),
								StartDateTime = rdr.GetDateTime(6)
							});
					}
				}
			}
			return result.ToList();
		}

		private static string RandomDnis()
		{
			return $"800{RandomDigitString(7)}";
		}

		private static string RandomDigitString(int length)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < length; i++)
				sb.Append((char)(48 + _r.Next(10)));
			return sb.ToString();
		}

		static void ExecuteTimedComputation(Action method, string message)
		{
			sw.Start();
			method.Invoke();
			sw.Stop();
			Console.WriteLine($"{message} took {sw.ElapsedMilliseconds}ms");
			sw.Reset();
		}
	}
}
