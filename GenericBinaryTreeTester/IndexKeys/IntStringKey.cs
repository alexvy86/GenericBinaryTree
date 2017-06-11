using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericBinaryTreeTester.IndexKeys
{
	public class IntStringKey : IndexKey
	{
		public int IntValue { get; set; }
		public string StringValue { get; set; }

		public IntStringKey(int intValue, string dnis)
		{
			IntValue = intValue;
			StringValue = dnis;
		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			IntStringKey typedObj = obj as IntStringKey;

			int clientIdComparison = this.IntValue - typedObj.IntValue;
			if (clientIdComparison != 0)
				return clientIdComparison;

			return this.StringValue.CompareTo(typedObj.StringValue);
		}

		#endregion
	}
}
