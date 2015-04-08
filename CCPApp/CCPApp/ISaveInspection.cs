using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPApp
{
	public interface ISaveInspection
	{
		void ExportInspection(Inspection inspection, string filename);
	}
}
