using System;
using Login;
using SQLite4Unity3d;

namespace ServerSQL.Model {
    public class Character {
        [PrimaryKey, AutoIncrement] 
        public int Id { get; set; }
        public string AccountId {get; set;}
        public string Name {get; set;}
        public int Level {get; set;}
        public int CharClass {get; set;}
        public bool IsMale {get; set;}
        public int CurrentHp {get; set;}
        public int CurrentMp {get; set;}
        
        public Character MakeNewCharacter(string name, CharacterClass charClass, bool isMale, string accountId) {
            this.AccountId = accountId;
            this.Name = name;
            this.IsMale = isMale;
            this.Level = 1;
            this.CharClass = (int) charClass;
            switch (charClass) {
                case CharacterClass.Warrior:
                    WarriorBaseStats();
                    break;
                case CharacterClass.Wizard:
                    WizardBaseStats();
                    break;
                case CharacterClass.Taoist:
                    TaoistBaseStats();
                    break;
                case CharacterClass.Assassin:
                    AssassinBaseStats();
                    break;
                case CharacterClass.Archer:
                    ArcherBaseStats();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(charClass), charClass, null);
            }
            return this;
        }
        private void ArcherBaseStats() {
            CurrentHp = 20;
            CurrentMp = 20;
        }
        private void AssassinBaseStats() {
            CurrentHp = 30;
            CurrentMp = 30;
        }

        private void TaoistBaseStats() {
            CurrentHp = 40;
            CurrentMp = 50;
        }

        private void WizardBaseStats() {
            CurrentHp = 50;
            CurrentMp = 50;
        }

        private void WarriorBaseStats() {
            CurrentHp = 60;
            CurrentMp = 60;
        }
    }
}