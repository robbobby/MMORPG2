using Mirror;
using Server;
using UnityEngine;

namespace Login
{
    public class CharSelectManager : MonoBehaviour
    {
        public GameObject maleWarrior;
        public GameObject maleWizard;
        public GameObject maleTaoist;
        public GameObject maleAssassin;
        public GameObject maleArcher;
        public GameObject femaleWarrior;
        public GameObject femaleWizard;
        public GameObject femaleTaoist;
        public GameObject femaleAssassin;
        public GameObject femaleArcher;

        public bool m_isMale = true;
        public CharacterClass m_characterClass = CharacterClass.Warrior;
        public void FemaleClicked() {
            m_isMale = false;
            SetActivePrefab();
        }
        public void MaleClicked() {
            m_isMale = true;
            SetActivePrefab();
        }
        public void WarriorClicked() {
            m_characterClass = CharacterClass.Warrior;
            SetActivePrefab();
        }
        public void WizardClicked() {
            m_characterClass = CharacterClass.Wizard;
            SetActivePrefab();
        }
        public void TaoistClicked() {
            m_characterClass = CharacterClass.Taoist;
            SetActivePrefab();
        }
        public void AssassinClicked() {
            m_characterClass = CharacterClass.Assassin;
            SetActivePrefab();
        }
        public void ArcherClicked() {
            print("Hello");
            m_characterClass = CharacterClass.Archer;
            SetActivePrefab();
        }
        private void SetActivePrefab() {
            print("Hello");
            SetAllInvisable();
            switch (m_characterClass) {
                case CharacterClass.Warrior:
                    if (m_isMale) maleWarrior.SetActive(true);
                    else femaleWarrior .SetActive(true);
                    break;
                case CharacterClass.Wizard:
                    if (m_isMale) maleWizard.SetActive(true);
                    else femaleWizard.SetActive(true);
                    break;
                case CharacterClass.Taoist:
                    if (m_isMale) maleTaoist.SetActive(true);
                    else femaleTaoist.SetActive(true);
                    break;
                case CharacterClass.Assassin:
                    if (m_isMale) maleAssassin.SetActive(true);
                    else femaleAssassin.SetActive(true);
                    break;
                case CharacterClass.Archer:
                    if (m_isMale) maleArcher.SetActive(true);
                    else femaleArcher.SetActive(true);
                    break;
            }
        }
        private void SetAllInvisable() {
            maleWarrior.SetActive(false);
            maleWizard.SetActive(false);
            maleTaoist.SetActive(false);
            maleAssassin.SetActive(false);
            maleArcher.SetActive(false);
            femaleWarrior.SetActive(false);
            femaleWizard.SetActive(false);
            femaleTaoist.SetActive(false);
            femaleAssassin.SetActive(false);
            femaleArcher.SetActive(false);
        }
    }
}
