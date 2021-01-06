using Server;
using ServerSQL.Model;
using SQLite4Unity3d;
using UnityEngine;

namespace Login {
    public class CharacterCreation : MonoBehaviour {
        private CreateCharacter m_character;

        public void MakeNewCharacter() {
            Character newCharacter = GetNewCharacter();
            print(CharacterPersistence.InsertCharacter(newCharacter));
            print(newCharacter.CharClass);
        }

        private string GetID() {
            GameObject networkManager = GameObject.Find("NetworkManager");
            DatabaseId databaseId = networkManager.GetComponent<DatabaseId>();
            return databaseId.Id;
        }
        private Character GetNewCharacter() {
            GameObject charSelectHandler = GameObject.Find("CharSelectHandler");
            CharSelectManager charSelectManager = charSelectHandler.GetComponent<CharSelectManager>();
            CharacterClass characterClass = charSelectManager.m_characterClass;
            bool isMale = charSelectManager.m_isMale;
            Character newCharacter = new Character();
            newCharacter = newCharacter.MakeNewCharacter("Bob", characterClass, isMale, GetID());
            return newCharacter;
        }
    }
}
