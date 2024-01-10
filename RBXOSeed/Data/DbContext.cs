using LiteDB;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace RBXOSeed.Data
{
    internal static class DbContext
    {
        private static string MainFolder =  "RBX";
        public static LiteDatabase DB { set; get; }// stores blocks

        //Database names
        public const string RSRV_DB_NAME = @"rbxseeddata.db";

        //Database tables
        public const string RSRV_ADJUDICATOR = "rsrv_adjudicator";
        public const string RSRV_BEACON = "rsrv_beacon";
        public const string RSRV_NODES = "rsrv_nodes";
        public const string RSRV_EMAIL = "rsrv_email";

        internal static void Initialize()
        {
            string path = GetDatabasePath();

            //Only use if you have issues with DateTime/DateTimeOffset
            var mapper = new BsonMapper();
            mapper.RegisterType<DateTime>(
                value => value.ToString("o", CultureInfo.InvariantCulture),
                bson => DateTime.ParseExact(bson, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));
            mapper.RegisterType<DateTimeOffset>(
                value => value.ToString("o", CultureInfo.InvariantCulture),
                bson => DateTimeOffset.ParseExact(bson, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));

            DB = new LiteDatabase(new ConnectionString { Filename = path + RSRV_DB_NAME, Connection = ConnectionType.Direct, ReadOnly = false });

            //Assumes UTC Time for Dates
            DB.Pragma("UTC_DATE", true);
        }

        public static void CloseDB()
        {
            DB.Dispose();
        }

        public static async Task CheckPoint()
        {
            try
            {
                DB.Checkpoint();
            }
            catch { }
        }

        public static string GetDatabasePath()
        {
            string path = "";

            var databaseLocation = "Databases";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                path = homeDirectory + Path.DirectorySeparatorChar + MainFolder.ToLower() + Path.DirectorySeparatorChar + databaseLocation + Path.DirectorySeparatorChar;
            }
            else
            {
                if (Debugger.IsAttached)
                {
                    path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "DBs" + Path.DirectorySeparatorChar + databaseLocation + Path.DirectorySeparatorChar;
                }
                else
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + MainFolder + Path.DirectorySeparatorChar + databaseLocation + Path.DirectorySeparatorChar;
                }
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}