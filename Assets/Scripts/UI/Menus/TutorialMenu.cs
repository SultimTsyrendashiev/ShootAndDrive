using UnityEngine;
using SD.Game;
using System;

namespace SD.UI.Menus
{
    [RequireComponent(typeof(MenuController))]
    class TutorialMenu : MonoBehaviour, IMenu
    {
        [SerializeField]
        GameObject weaponSwitchBlock;

        [SerializeField]
        string emptyTutorial;

        [SerializeField]
        string moveTutorial;
        [SerializeField]
        string weaponSwitchTutorial;
        [SerializeField]
        string shootTutorial;
        [SerializeField]
        string jamTutorial;
        [SerializeField]
        string breakTutorial;

        MenuController tutorialMenuController;
        Action current;

        public void Init(MenuController menuController)
        {
            tutorialMenuController = GetComponent<MenuController>();

            TutorialManager.OnTutorialStart += Activate;
            TutorialManager.OnTutorialEnd += Deactivate;

            TutorialManager.OnTutorial_Move += TutorialManager_OnTutorial_Move;
            TutorialManager.OnTutorial_WeaponSwitch += TutorialManager_OnTutorial_WeaponSwitch;
            TutorialManager.OnTutorial_Shoot += TutorialManager_OnTutorial_Shoot;
            TutorialManager.OnTutorial_WeaponJam += TutorialManager_OnTutorial_WeaponJam;
            TutorialManager.OnTutorial_WeaponBreak += TutorialManager_OnTutorial_WeaponBreak;
        }

        private void TutorialManager_OnTutorial_WeaponBreak(TutorialManager obj)
        {
            tutorialMenuController.EnableMenu(breakTutorial);

            current = () => obj.SetWaitForWeaponBreak(false);
        }

        private void TutorialManager_OnTutorial_WeaponJam(TutorialManager obj)
        {
            tutorialMenuController.EnableMenu(jamTutorial);

            current = () => obj.SetWaitForWeaponJam(false);
        }

        private void TutorialManager_OnTutorial_WeaponSwitch(TutorialManager obj)
        {
            weaponSwitchBlock.SetActive(false);
            tutorialMenuController.EnableMenu(weaponSwitchTutorial);

            current = () => obj.SetWaitForWeaponSwitch(false);
        }

        private void TutorialManager_OnTutorial_Shoot(TutorialManager obj)
        {
            tutorialMenuController.EnableMenu(shootTutorial);

            current = () => obj.SetWaitForShoot(false);
        }

        private void TutorialManager_OnTutorial_Move(TutorialManager obj)
        {
            tutorialMenuController.EnableMenu(moveTutorial);

            current = () => obj.SetWaitForMove(false);
        }

        public void ApplyCurrentTutorial()
        {
            current?.Invoke();
            tutorialMenuController.EnableMenu(emptyTutorial);
        }

        void OnDestroy()
        {
            TutorialManager.OnTutorialStart -= Activate;
            TutorialManager.OnTutorialEnd -= Deactivate;

            TutorialManager.OnTutorial_Move -= TutorialManager_OnTutorial_Move;
            TutorialManager.OnTutorial_WeaponSwitch -= TutorialManager_OnTutorial_WeaponSwitch;
            TutorialManager.OnTutorial_Shoot -= TutorialManager_OnTutorial_Shoot;
            TutorialManager.OnTutorial_WeaponJam -= TutorialManager_OnTutorial_WeaponJam;
            TutorialManager.OnTutorial_WeaponBreak -= TutorialManager_OnTutorial_WeaponBreak;
        }

        public void Activate()
        {
            weaponSwitchBlock.SetActive(true);
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
