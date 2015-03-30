using System;
using System.Collections.Generic;
using System.Text;

using CCPApp;
using SQLite.Net;
using SQLite.Net.Async;
using System.IO;
using Xamarin.Forms;
using CCPApp.iOS;
using SQLite.Net.Platform.XamarinIOS;

[assembly: Dependency (typeof(SQLite_iOS))]
namespace CCPApp.iOS
{
	public class SQLite_iOS : ISQLite
	{
		public SQLiteConnection GetConnection()
		{
			var sqliteFilename = "ChecklistDatabase.db3";
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
			string libraryPath = new FileManage().GetLibraryFolder();//Library folder.
			var path = Path.Combine(libraryPath, sqliteFilename);
			// Create the connection
			var plat = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();
			var conn = new SQLite.Net.SQLiteConnection(plat, path);
			// Return the database connection
			return conn;
		}
		public SQLiteAsyncConnection GetAsyncConnection(SQLiteConnection conn)
		{
			var connectionFactory = new Func<SQLiteConnectionWithLock>(() => new SQLiteConnectionWithLock(new SQLitePlatformIOS(), new SQLiteConnectionString(conn.DatabasePath, false)));
			return new SQLiteAsyncConnection(connectionFactory);
			//return new SQLiteAsyncConnection(conn);
		}
	}
}
