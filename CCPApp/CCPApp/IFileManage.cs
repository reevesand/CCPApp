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
		/// <summary>
		/// Get all the files in the top level directory for the app that correspond to CCP checklists.  Presently, that means zip files.
		/// Returns their absolute paths.
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> GetAllValidFiles();
		/// <summary>
		/// Get the name of the xml file in this directory.  Won't work if there are more than one.
		/// Expects an absolute path.
		/// </summary>
		/// <param name="directory">The directory to search.</param>
		/// <returns></returns>
		string GetXmlFile(string directory);
		XmlReader LoadXml(string filename);
		//One or more of these uses absolute paths.  I think it's GetXmlFile and DeleteFile.
		//Originally comes from GetAllValidFiles, which returns the absolute paths of said files.
		void MoveDirectoryToPrivate(string sourceDirectory, string destinationDirectory);
		void DeleteFile(string fileName);
		void CopyFileFromTempToPublic(string SourceName, string DestinationName);
		string GetLibraryFolder();
		void DeleteTempFile(string fileName);
	}
}
