using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPApp.Models
{
	public class ReportOptionsModel
	{
		public bool Comments { get; set; }
		public bool Questions { get; set; }
		public bool Structure { get; set; }
		public bool Totals { get; set; }
		public bool ScoreSheet { get; set; }
		public bool GraphSheet { get; set; }

		public ReportOptionsModel()
		{
			Comments = true;
			Questions = true;
			Structure = true;
			Totals = true;
			ScoreSheet = true;
			GraphSheet = true;
		}
	}
}
