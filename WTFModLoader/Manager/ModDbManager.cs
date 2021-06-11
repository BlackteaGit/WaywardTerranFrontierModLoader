using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Data;
using System.IO;
using CoOpSpRpG;

namespace WTFModLoader.Manager
{
    class ModDbManager
    {
		public enum GlobalFlag : uint
		{
		}

		public enum GlobalInt : uint
		{
		}

		public enum GlobalDouble : uint
		{
			DbVersion
		}
		public enum GlobalString : uint
		{
		}

		public static SQLiteConnection modCfgCon;

		public static Dictionary<GlobalFlag, bool> globalflags = new Dictionary<GlobalFlag, bool>();
		public static Dictionary<GlobalInt, int> globalints = new Dictionary<GlobalInt, int>();
		public static Dictionary<GlobalDouble, double> globaldoubles = new Dictionary<GlobalDouble, double>();
		public static Dictionary<GlobalString, string> globalstrings = new Dictionary<GlobalString, string>();
		public static void Init()
        {
			if (!File.Exists(WTFModLoader.ModsDirectory + "\\" + "modCfgDb.sqlite"))
			{
				SQLiteConnection.CreateFile(WTFModLoader.ModsDirectory + "\\" + "modCfgDb.sqlite");
				modCfgCon = new SQLiteConnection("Data Source=" + WTFModLoader.ModsDirectory + "\\" + "modCfgDb.sqlite" + ";Version=3;");
				modCfgCon.Open();
				createAllTables();
			}
			else
			{

				modCfgCon = new SQLiteConnection("Data Source=" + WTFModLoader.ModsDirectory + "\\" + "modCfgDb.sqlite" + ";Version=3;");
				modCfgCon.Open();
				createAllTables();
			}

			globalflags = new Dictionary<GlobalFlag, bool>();

			globalints = new Dictionary<GlobalInt, int>();

			globaldoubles = new Dictionary<GlobalDouble, double>();
			globaldoubles.Add(GlobalDouble.DbVersion, 0.4);

			globalstrings = new Dictionary<GlobalString, string>();

		}
		public static void createAllTables()
		{
			try
			{
				var commandText = "create table IF NOT EXISTS globalflags (name varchar(20), value BOOL, PRIMARY KEY (name))";
				var sqliteCommand = new SQLiteCommand(commandText, modCfgCon);
				sqliteCommand.ExecuteNonQuery();
				commandText = "create table IF NOT EXISTS globalints (name varchar(20), value INT, PRIMARY KEY (name))";
				sqliteCommand = new SQLiteCommand(commandText, modCfgCon);
				sqliteCommand.ExecuteNonQuery();
				commandText = "create table IF NOT EXISTS globaldoubles (name varchar(20), value FLOAT, PRIMARY KEY (name))";
				sqliteCommand = new SQLiteCommand(commandText, modCfgCon);
				sqliteCommand.ExecuteNonQuery();
				commandText = "create table IF NOT EXISTS globalstrings (name varchar(20), value TEXT, PRIMARY KEY (name))";
				sqliteCommand = new SQLiteCommand(commandText, modCfgCon);
				sqliteCommand.ExecuteNonQuery();
				commandText = "create table IF NOT EXISTS inactivemods (location varchar(20), name varchar(20), version varchar(20), PRIMARY KEY (location))";
				sqliteCommand = new SQLiteCommand(commandText, modCfgCon);
				sqliteCommand.ExecuteNonQuery();
				
			}
			catch (Exception e)
			{
				SCREEN_MANAGER.alerts.Enqueue("Failed to build WTFML config datasave."); //error message for debug
				SCREEN_MANAGER.alerts.Enqueue("Please contact the WTFML author."); //error message for debug
				Logger.Log($"Error while building {modCfgCon.FileName}");
				Logger.Log($"{e}");
			}
		}

		public static void writeModCfgData()
		{
			try
			{
				if (WTFModLoader._modManager.InactiveMods.IsValueCreated && modCfgCon != null)
				{
					string commandText = "delete from inactivemods";
					SQLiteCommand sqliteCommand = new SQLiteCommand(commandText, modCfgCon);
					sqliteCommand.ExecuteNonQuery();

					foreach (var modentry in WTFModLoader._modManager.InactiveMods.Value)
					{
						string commandText2 = "insert or replace into inactivemods (location, name, version) values (@location, @name, @version)";
						SQLiteCommand sqliteCommand2 = new SQLiteCommand(commandText2, modCfgCon);
						sqliteCommand2.Parameters.Add("@location", DbType.String).Value = modentry.Source;
						sqliteCommand2.Parameters.Add("@name", DbType.String).Value = modentry.ModMetadata.Name;
						sqliteCommand2.Parameters.Add("@version", DbType.String).Value = modentry.ModMetadata.Version;
						sqliteCommand2.ExecuteNonQuery();
					}
				}
			}
			catch (Exception e)
			{
				Logger.Log($"Error while saving mod settings into {modCfgCon.FileName}");
				Logger.Log($"{e}");
			}
		}

		public static bool isInactive(ModEntry entry)
		{
			try
			{
				SQLiteCommand sqliteCommand = new SQLiteCommand("select count(name) from inactivemods where location = '" + entry.Source + "' and name = @name and version = @version", modCfgCon);
				sqliteCommand.Parameters.Add("@name", DbType.String).Value = entry.ModMetadata.Name;
				sqliteCommand.Parameters.Add("@version", DbType.String).Value = entry.ModMetadata.Version;
				return Convert.ToInt32(sqliteCommand.ExecuteScalar()) > 0;
			}
			catch (Exception e)
			{
				Logger.Log($"Error while loading mod `{entry.ModMetadata.Name} (v{entry.ModMetadata.Version})` settings from {modCfgCon.FileName}");
				Logger.Log($"{e}");
				return false;
			}
		}
		public static void writeCfgData()
		{
			try
			{
				if (modCfgCon != null) // saving flags
				{
					string commandText1 = "delete from globalflags";
					SQLiteCommand sqliteCommand1 = new SQLiteCommand(commandText1, modCfgCon);
					sqliteCommand1.ExecuteNonQuery();
					foreach (var flag in globalflags)
					{
						string commandText2 = "insert or replace into globalflags (name, value) values (@name, @value)";
						SQLiteCommand sqliteCommand2 = new SQLiteCommand(commandText2, modCfgCon);
						sqliteCommand2.Parameters.Add("@name", DbType.String).Value = flag.Key;
						sqliteCommand2.Parameters.Add("@value", DbType.Boolean).Value = flag.Value;
						sqliteCommand2.ExecuteNonQuery();
					}
				}
				if (modCfgCon != null) // saving ints
				{
					string commandText1 = "delete from globalints";
					SQLiteCommand sqliteCommand1 = new SQLiteCommand(commandText1, modCfgCon);
					sqliteCommand1.ExecuteNonQuery();
					foreach (var integer in globalints)
					{
						string commandText2 = "insert or replace into globalints (name, value) values (@name, @value)";
						SQLiteCommand sqliteCommand2 = new SQLiteCommand(commandText2, modCfgCon);
						sqliteCommand2.Parameters.Add("@name", DbType.String).Value = integer.Key;
						sqliteCommand2.Parameters.Add("@value", DbType.Int32).Value = integer.Value;
						sqliteCommand2.ExecuteNonQuery();
					}
				}
				if (modCfgCon != null) // saving floats
				{
					string commandText1 = "delete from globaldoubles";
					SQLiteCommand sqliteCommand1 = new SQLiteCommand(commandText1, modCfgCon);
					sqliteCommand1.ExecuteNonQuery();
					foreach (var doubl in globaldoubles)
					{
						string commandText2 = "insert or replace into globaldoubles (name, value) values (@name, @value)";
						SQLiteCommand sqliteCommand2 = new SQLiteCommand(commandText2, modCfgCon);
						sqliteCommand2.Parameters.Add("@name", DbType.String).Value = doubl.Key;
						sqliteCommand2.Parameters.Add("@value", DbType.Double).Value = doubl.Value;
						sqliteCommand2.ExecuteNonQuery();
					}
				}
				if (modCfgCon != null) // saving strings
				{
					string commandText1 = "delete from globalstrings";
					SQLiteCommand sqliteCommand1 = new SQLiteCommand(commandText1, modCfgCon);
					sqliteCommand1.ExecuteNonQuery();
					foreach (var str in globalstrings)
					{
						string commandText2 = "insert or replace into globalstrings (name, value) values (@name, @value)";
						SQLiteCommand sqliteCommand2 = new SQLiteCommand(commandText2, modCfgCon);
						sqliteCommand2.Parameters.Add("@name", DbType.String).Value = str.Key;
						sqliteCommand2.Parameters.Add("@value", DbType.String).Value = str.Value;
						sqliteCommand2.ExecuteNonQuery();
					}
				}
			}
			catch (Exception e)
			{
				Logger.Log($"Error while writing Loader settings to {modCfgCon.FileName}");
				Logger.Log($"{e}");
				SCREEN_MANAGER.alerts.Enqueue("Failed to write loader config datasave."); //error message for debug
				SCREEN_MANAGER.alerts.Enqueue("Please contact the WTFML author."); //error message for debug
			}
		}

		public static void updateCfgDb()
		{
			double modversion = 0;
			string connectionString = modCfgCon.ConnectionString;
			string file = modCfgCon.DataSource;
			string commandText = "select * from globaldoubles";
			SQLiteCommand sqliteCommand = new SQLiteCommand(commandText, modCfgCon);
			SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
			while (sqliteDataReader.Read())
			{
				string template = "unknown";
				try
				{
					template = sqliteDataReader["name"].ToString();
				}
				catch (Exception e)
				{
					Logger.Log($"Error while upgrading Loader config database {modCfgCon.FileName}");
					Logger.Log($"{e}");
					SCREEN_MANAGER.alerts.Enqueue("Failed to load loader config datasave."); //error message for debug
					SCREEN_MANAGER.alerts.Enqueue("Please contact the WTFML author."); //error message for debug
					return;
				}
				if (!Enum.TryParse(template, true, out GlobalDouble type))
				{
					SCREEN_MANAGER.alerts.Enqueue("Failed to load " + template + " data for WTFModLoader."); //error message for debug
					SCREEN_MANAGER.alerts.Enqueue("Please contact the WTFML author."); //error message for debug
					return;
				}
				if (type == GlobalDouble.DbVersion)
					modversion = (double)sqliteDataReader["value"];
			}
			if (modversion > 0 && modversion < globaldoubles[GlobalDouble.DbVersion])
			{
				SCREEN_MANAGER.alerts.Enqueue($"Updating config database {modCfgCon.FileName} to the current v {modversion} of the WTFModLoader."); //error message for debug
				Logger.Log($"Updating config database {modCfgCon.FileName} to the current v {modversion} of the WTFModLoader.");
				string commandText2 = "insert or replace into globaldoubles (name, value) values (@name, @value)";
				SQLiteCommand sqliteCommand2 = new SQLiteCommand(commandText2, modCfgCon);
				sqliteCommand2.Parameters.Add("@name", DbType.String).Value = GlobalDouble.DbVersion;
				sqliteCommand2.Parameters.Add("@value", DbType.Double).Value = globaldoubles[GlobalDouble.DbVersion];
				sqliteCommand2.ExecuteNonQuery();
			}
			if (modversion > globaldoubles[GlobalDouble.DbVersion])
			{
				SCREEN_MANAGER.alerts.Enqueue("WTFModLoader config database v" + modversion.ToString() + " is not compatible to current config database v" + globaldoubles[GlobalDouble.DbVersion].ToString());
				SCREEN_MANAGER.alerts.Enqueue("All mod configuration data will be reset.");
				Logger.Log($"WTFModLoader config database v {modversion} is not compatible to current config database v {globaldoubles[GlobalDouble.DbVersion]}");
				Logger.Log($"All mod configuration data will be reset.");
				string savedir = WTFModLoader.ModsDirectory;
				String ModSaveFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(savedir, System.IO.Path.Combine(@file + ".sqlite")));

				if (System.IO.File.Exists(ModSaveFile))
				{
					String BackupFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(savedir, System.IO.Path.Combine(@file + ".sqlite" + ".old")));
					File.Copy(ModSaveFile, BackupFile, true);
				}
				string commandText1 = "PRAGMA writable_schema = 1;";
				SQLiteCommand sqliteCommand1 = new SQLiteCommand(commandText1, modCfgCon);
				sqliteCommand1.ExecuteNonQuery();
				string commandText2 = "delete from sqlite_master where type in ('table', 'index', 'trigger');";
				SQLiteCommand sqliteCommand2 = new SQLiteCommand(commandText2, modCfgCon);
				sqliteCommand2.ExecuteNonQuery();
				string commandText3 = "PRAGMA writable_schema = 0;";
				SQLiteCommand sqliteCommand3 = new SQLiteCommand(commandText3, modCfgCon);
				sqliteCommand3.ExecuteNonQuery();
				string commandText4 = "VACUUM;";
				SQLiteCommand sqliteCommand4 = new SQLiteCommand(commandText4, modCfgCon);
				sqliteCommand4.ExecuteNonQuery();
				string commandText5 = "PRAGMA INTEGRITY_CHECK;";
				SQLiteCommand sqliteCommand5 = new SQLiteCommand(commandText5, modCfgCon);
				sqliteCommand5.ExecuteNonQuery();
				createAllTables();
				SCREEN_MANAGER.alerts.Enqueue("Old datasave renamed to '" + file + ".old" + "'");
				Logger.Log("Old datasave renamed to '" + file + ".old" + "'");
			}
		}

		public static void loadCfgData()
		{
			getGlobalFlagsData();
			getGlobalIntsData();
			getGlobalFloatsData();
			getGlobalStringsData();
		}

		private static void getGlobalFlagsData() //loading global event flags
		{
			string commandText = "select * from globalflags";
			SQLiteCommand sqliteCommand = new SQLiteCommand(commandText, modCfgCon);
			SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
			while (sqliteDataReader.Read())
			{
				string template = "unknown";
				try
				{
					template = sqliteDataReader["name"].ToString();
				}
				catch (Exception e)
				{
					Logger.Log($"Error while reading Loader settings from {modCfgCon.FileName}");
					Logger.Log($"{e}");
					SCREEN_MANAGER.alerts.Enqueue("Failed to load WTFModLoader datasave."); //error message for debug
					SCREEN_MANAGER.alerts.Enqueue("Please contact the WTFML author."); //error message for debug
					return;
				}
				if (!Enum.TryParse(template, true, out GlobalFlag type))
				{
					SCREEN_MANAGER.alerts.Enqueue("Failed to load " + template + " data for WTFModLoader."); //error message for debug
					SCREEN_MANAGER.alerts.Enqueue("Please contact the WTFML author."); //error message for debug
					return;
				}
				globalflags[type] = (bool)sqliteDataReader["value"];
			}
		}



		private static void getGlobalIntsData() //loading global ints
		{
			string commandText = "select * from globalints";
			SQLiteCommand sqliteCommand = new SQLiteCommand(commandText, modCfgCon);
			SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
			while (sqliteDataReader.Read())
			{
				string template = "unknown";
				try
				{
					template = sqliteDataReader["name"].ToString();
				}
				catch (Exception e)
				{
					Logger.Log($"Error while reading Loader settings from {modCfgCon.FileName}");
					Logger.Log($"{e}");
					SCREEN_MANAGER.alerts.Enqueue("Failed to load WTFModLoader datasave."); //error message for debug
					SCREEN_MANAGER.alerts.Enqueue("Please contact the WTFML author."); //error message for debug
					return;
				}
				if (!Enum.TryParse(template, true, out GlobalInt type))
				{
					SCREEN_MANAGER.alerts.Enqueue("Failed to load " + template + " data for WTFModLoader."); //error message for debug
					SCREEN_MANAGER.alerts.Enqueue("Please contact the WTFML author."); //error message for debug
					return;
				}
				globalints[type] = (int)sqliteDataReader["value"];
			}
		}

		private static void getGlobalFloatsData() //loading global floats
		{
			string commandText = "select * from globaldoubles";
			SQLiteCommand sqliteCommand = new SQLiteCommand(commandText, modCfgCon);
			SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
			while (sqliteDataReader.Read())
			{
				string template = "unknown";
				try
				{
					template = sqliteDataReader["name"].ToString();
				}
				catch (Exception e)
				{
					Logger.Log($"Error while reading Loader settings from {modCfgCon.FileName}");
					Logger.Log($"{e}");
					SCREEN_MANAGER.alerts.Enqueue("Failed to load WTFModLoader datasave."); //error message for debug
					SCREEN_MANAGER.alerts.Enqueue("Please contact the WTFML author."); //error message for debug
					return;
				}
				if (!Enum.TryParse(template, true, out GlobalDouble type))
				{
					SCREEN_MANAGER.alerts.Enqueue("Failed to load " + template + " data for WTFModLoader."); //error message for debug
					SCREEN_MANAGER.alerts.Enqueue("Please contact the WTFML author."); //error message for debug
					return;
				}
				globaldoubles[type] = (double)sqliteDataReader["value"];
			}
		}

		private static void getGlobalStringsData() //loading global strings
		{
			string commandText = "select * from globalstrings";
			SQLiteCommand sqliteCommand = new SQLiteCommand(commandText, modCfgCon);
			SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
			while (sqliteDataReader.Read())
			{
				string template = "unknown";
				try
				{
					template = sqliteDataReader["name"].ToString();
				}
				catch (Exception e)
				{
					Logger.Log($"Error while reading Loader settings from {modCfgCon.FileName}");
					Logger.Log($"{e}");
					SCREEN_MANAGER.alerts.Enqueue("Failed to load WTFModLoader datasave."); //error message for debug
					SCREEN_MANAGER.alerts.Enqueue("Please contact the WTFML author."); //error message for debug
					return;
				}
				if (!Enum.TryParse(template, true, out GlobalString type))
				{
					SCREEN_MANAGER.alerts.Enqueue("Failed to load " + template + " data for WTFModLoader."); //error message for debug
					SCREEN_MANAGER.alerts.Enqueue("Please contact the WTFML author."); //error message for debug
					return;
				}
				globalstrings[type] = sqliteDataReader["value"].ToString();
			}
		}
	}
}

