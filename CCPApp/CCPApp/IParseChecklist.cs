using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPApp
{
	public interface IParseChecklist
	{
		string GetChecklistId(string filename);
		void Parse(ChecklistModel model, string filename);
	}
}
