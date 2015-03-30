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
		public IEnumerable<string> GetAllValidFiles()
		{
			var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			IEnumerable<string> filesInDirectory = Directory.EnumerateFiles(documentsPath);
			return filesInDirectory.Where(f => f.EndsWith(".zip"));
		}
		public XmlReader LoadXml(string filename)
		{
			string privatePath = GetLibraryFolder();
			string filePath = Path.Combine(privatePath, filename);
			return XmlReader.Create(filePath);
		}
		public string GetXmlFile(string directory)
		{
			IEnumerable<string> filesInDirectory = Directory.EnumerateFiles(directory);
			return filesInDirectory.Single(f => f.EndsWith(".xml"));
		}
		public void MoveDirectoryToPrivate(string sourceDirectory, string destinationDirectory)
		{
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string privatePath = GetLibraryFolder();
			string destinationPath = Path.Combine(privatePath, destinationDirectory);
			if (Directory.Exists(destinationPath)){
				Directory.Delete(destinationPath,true);
			}
			Directory.Move(sourceDirectory, destinationPath);
		}
		public void DeleteFile(string fileName)
		{
			File.Delete(fileName);
		}

		public void CopyFileFromTempToPublic(string SourceName, string DestinationName)
		{
			string tempPath = GetTempFolder();
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string sourcePath = Path.Combine(tempPath, SourceName);
			string destinationPath = Path.Combine(documentsPath, DestinationName);
			File.Copy(sourcePath, destinationPath, true);
		}

		public string GetLibraryFolder()
		{
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string libraryPath = Path.Combine(documentsPath, "..", "Library");
			return libraryPath;
		}
		public string GetTempFolder()
		{
			return Path.GetTempPath();
		}
		public void DeleteTempFile(string fileName)
		{
			string tempPath = GetTempFolder();
			string fullPath = Path.Combine(tempPath, fileName);
			File.Delete(fullPath);
		}
	}
}