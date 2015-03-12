using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CCPApp
{
	public interface IFileManage
	{
		void SaveText(string filename, string text);
		string LoadText(string filename);
		IEnumerable<string> GetAllValidFiles();
		XmlReader LoadXml(string filename);
	}
}
