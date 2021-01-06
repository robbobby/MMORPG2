using SQLite4Unity3d;

namespace SQL.Model {
    public class UserAccount {
        [PrimaryKey]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
    }
}