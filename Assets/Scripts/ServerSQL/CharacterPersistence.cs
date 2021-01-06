using ServerSQL;
using ServerSQL.Model;
using SQL.Model;

namespace SQLite4Unity3d {
    public class CharacterPersistence {
        private static void CreateCharacterTable() {
            DataService dataService = new DataService();
            SQLiteConnection connection = dataService._connection;
            connection.CreateTable<Character>();
            connection.Close();
            
        }
        public static int InsertCharacter(Character character) {
            CreateCharacterTable();
            DataService dataService = new DataService();
            SQLiteConnection connection = dataService._connection;
            int success = connection.Insert(character);
            connection.Close();
            return success;
        }
    }
}