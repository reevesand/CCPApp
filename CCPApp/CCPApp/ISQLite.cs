using SQLite.Net;
using SQLite.Net.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPApp
{
	public interface ISQLite
	{
		SQLiteConnection GetConnection();
		SQLiteAsyncConnection GetAsyncConnection(SQLiteConnection connection);
	}
}
