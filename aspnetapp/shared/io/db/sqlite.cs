using Microsoft.Data.Sqlite;

namespace Shared
{
    public class SQLite
    {


        public void CreateTable(string command, string path)
        {

            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            //Use DB in project directory.  If it does not exist, create it:
            connectionStringBuilder.DataSource = path;

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                //Create a table (drop if already exists first):

                var delTableCmd = connection.CreateCommand();
                delTableCmd.CommandText = "DROP TABLE IF EXISTS favorite_beers";
                delTableCmd.ExecuteNonQuery();

                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = "CREATE TABLE favorite_beers(name VARCHAR(50))";
                createTableCmd.ExecuteNonQuery();
            }
        }

        public void WriteTable(string command, string path)
        {

            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            //Use DB in project directory.  If it does not exist, create it:
            connectionStringBuilder.DataSource = path;

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                //Seed some data:
                using (var transaction = connection.BeginTransaction())
                {
                    var insertCmd = connection.CreateCommand();

                    /*insertCmd.CommandText = "INSERT INTO favorite_beers VALUES('LAGUNITAS IPA')";
                    insertCmd.ExecuteNonQuery();

                    insertCmd.CommandText = "INSERT INTO favorite_beers VALUES('JAI ALAI IPA')";
                    insertCmd.ExecuteNonQuery();

                    insertCmd.CommandText = "INSERT INTO favorite_beers VALUES('RANGER IPA')";
                    insertCmd.ExecuteNonQuery();*/

                    insertCmd.CommandText = "INSERT INTO favorite_beers VALUES('"+command+"')";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }



        public string ReadTable(string command, string path)
        {
            string ret = "";

            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            //Use DB in project directory.  If it does not exist, create it:
            connectionStringBuilder.DataSource = path;

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                //Read the newly inserted data:
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT name FROM favorite_beers";

                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret += reader.GetString(0);
                    }
                }
            }

             return ret;
        }
    }
}