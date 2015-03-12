using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Xamarin.Forms;
using CCPApp;
using CCPApp.iOS;
using System.Xml;

[assembly: Dependency(typeof(FileManage))]
namespace CCPApp.iOS
{
	class FileManage : IFileManage
	{
		public void SaveText(string filename, string text)
		{
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string filePath = Path.Combine(documentsPath, filename);
			File.WriteAllText(filePath, text);
		}
		public string LoadText(string filename)
		{
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string filePath = Path.Combine(documentsPath, filename);
			return File.ReadAllText(filePath);
		}
		public IEnumerable<string> GetAllValidFiles()
		{
			var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			IEnumerable<string> filesInDirectory = Directory.EnumerateFiles(documentsPath);
			return filesInDirectory.Where(f => f.EndsWith(".xml"));
		}
		public XmlReader LoadXml(string filename)
		{
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string filePath = Path.Combine(documentsPath, filename);
			return XmlReader.Create(filePath);
		}
	}
}