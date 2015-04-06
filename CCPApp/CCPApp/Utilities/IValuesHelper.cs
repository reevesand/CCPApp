using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPApp.Utilities
{
	public interface IValuesHelper
	{
		string exportInstructions();
		string deleteChecklistWarning(string title);
		string outbriefingInstructions();
	}
}
