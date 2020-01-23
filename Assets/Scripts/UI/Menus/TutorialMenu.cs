using UnityEngine;
using SD.Game;
using System;

namespace SD.UI.Menus
{
    [RequireComponent(typeof(MenuController))]
    class TutorialMenu : MonoBehaviour
    {
        const float MinTimeForTutorial = 0.75f;

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
        float timeCurrentActionSet;

        public void Awake()
        {
            tutorialMenuController = GetComponent<MenuController>();

            TutorialManager.OnTutorialStart += StartTutorial;
            TutorialManager.OnTutorialEnd += StopTutorial;

            TutorialManager.OnTutorial_Move += TutorialManager_OnTutorial_Move;
            TutorialManager.OnTutorial_WeaponSwitch += TutorialManager_OnTutorial_WeaponSwitch;
            TutorialManager.OnTutorial_Shoot += TutorialManager_OnTutorial_Shoot;
            TutorialManager.OnTutorial_WeaponJam += TutorialManager_OnTutorial_WeaponJam;
            TutorialManager.OnTutorial_WeaponBreak += TutorialManager_OnTutorial_WeaponBreak;

            StopTutorial();
        }

        private void TutorialManager_OnTutorial_WeaponBreak(TutorialManager obj)
        {
            tutorialMenuController.EnableMenu(breakTutorial);

            SetCurrentAction(() => obj.SetWaitForWeaponBreak(false));
        }

        private void TutorialManager_OnTutorial_WeaponJam(TutorialManager obj)
        {
            tutorialMenuController.EnableMenu(jamTutorial);

            SetCurrentAction(() => obj.SetWaitForWeaponJam(false));
        }

        private void TutorialManager_OnTutorial_WeaponSwitch(TutorialManager obj)
        {
            weaponSwitchBlock.SetActive(false);
            tutorialMenuController.EnableMenu(weaponSwitchTutorial);

            SetCurrentAction(() => obj.SetWaitForWeaponSwitch(false));
        }

        private void TutorialManager_OnTutorial_Shoot(TutorialManager obj)
        {
            tutorialMenuController.EnableMenu(shootTutorial);

            SetCurrentAction(() => obj.SetWaitForShoot(false));
        }

        private void TutorialManager_OnTutorial_Move(TutorialManager obj)
        {
            tutorialMenuController.EnableMenu(moveTutorial);

            SetCurrentAction(() => obj.SetWaitForMove(false));
        }

        private void SetCurrentAction(Action action)
        {
            current = action;
            timeCurrentActionSet = Time.realtimeSinceStartup;
        }

        public void ApplyCurrentTutorial()
        {
            // don't apply if tutorial panel was skipped too fast
            if (Time.realtimeSinceStartup - timeCurrentActionSet < MinTimeForTutorial)
            {
                return;
            }
         
            current?.Invoke();
            current = null;

            tutorialMenuController.EnableMenu(emptyTutorial);
        }

        void OnDestroy()
        {
            TutorialManager.OnTutorialStart -= StartTutorial;
            TutorialManager.OnTutorialEnd -= StopTutorial;

            TutorialManager.OnTutorial_Move -= TutorialManager_OnTutorial_Move;
            TutorialManager.OnTutorial_WeaponSwitch -= TutorialManager_OnTutorial_WeaponSwitch;
            TutorialManager.OnTutorial_Shoot -= TutorialManager_OnTutorial_Shoot;
            TutorialManager.OnTutorial_WeaponJam -= TutorialManager_OnTutorial_WeaponJam;
            TutorialManager.OnTutorial_WeaponBreak -= TutorialManager_OnTutorial_WeaponBreak;
        }

        public void StartTutorial()
        {
            weaponSwitchBlock.SetActive(true);
            gameObject.SetActive(true);
        }

        void StopTutorial()
        {
            weaponSwitchBlock.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
