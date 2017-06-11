using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericBinaryTreeTester
{
	public class Call
	{
		public long CallId { get; set; }
		public int ClientId { get; set; }
		public string DNIS { get; set; }
		public string ANI { get; set; }
		public string RingTo { get; set; }
		public decimal Duration { get; set; }
		public DateTime StartDateTime { get; set; }
	}
}
