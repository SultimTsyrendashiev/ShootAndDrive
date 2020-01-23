using System;
using System.Collections;
using UnityEngine;
using SD.PlayerLogic;

namespace SD.Game
{
    class TutorialManager : MonoBehaviour
    {
        public static event Action OnTutorialStart;
        public static event Action OnTutorialEnd;

        public static event Action OnTutorialPanelActivate;
        public static event Action OnTutorialPanelDeactivate;

        public static event Action<TutorialManager> OnTutorial_Move;
        public static event Action<TutorialManager> OnTutorial_WeaponSwitch;
        public static event Action<TutorialManager> OnTutorial_Shoot;
        public static event Action<TutorialManager> OnTutorial_WeaponJam;
        public static event Action<TutorialManager> OnTutorial_WeaponBreak;

        private bool waitForMove;
        private bool waitForWeaponSwitch;
        private bool waitForShoot;
        private bool waitForWeaponJam;
        private bool waitForWeaponBreak;

        private bool weaponSelected;
        private bool weaponJammed;
        private bool weaponBroke;

        bool isStarted;

        float nextTutorialTime;

        Action startEnemySpawn;

        public void StartTutorial(Player player, Action startEnemySpawn)
        {
            if (isStarted)
            {
                return;
            }

            this.startEnemySpawn = startEnemySpawn;

            isStarted = true;

            waitForMove = true;
            waitForShoot = true;
            waitForWeaponBreak = true;
            waitForWeaponJam = true;
            waitForWeaponSwitch = true;

            weaponSelected = false;
            weaponJammed = false;
            weaponBroke = false;

            GameController.OnPlayerDeath += ProcessPlayerDeath;
            GameController.OnMainMenuActivate += ForceStop;
            GameController.OnWeaponSelectionDisable += ProcessWeaponSelection;
            Weapons.Weapon.OnStateChange += Weapon_OnStateChange;

            OnTutorialStart?.Invoke();

            player.Vehicle.Accelerate();
            StartCoroutine(WaitForTutorial());

            Debug.Log("Tutorial invoked", this);
        }

        void OnDestroy()
        {
            GameController.OnPlayerDeath -= ProcessPlayerDeath;
            GameController.OnMainMenuActivate -= ForceStop;
            GameController.OnWeaponSelectionDisable -= ProcessWeaponSelection;
            Weapons.Weapon.OnStateChange -= Weapon_OnStateChange;
        }

        private void Weapon_OnStateChange(Weapons.WeaponState prev, Weapons.WeaponState cur)
        {
            if (cur == Weapons.WeaponState.ReadyForUnjam)
            {
                ProcessWeaponJam();
            }
            else if (prev == Weapons.WeaponState.Breaking && cur == Weapons.WeaponState.Disabling)
            {
                ProcessWeaponBreak();
            }
        }

        private void ProcessWeaponSelection()
        {
            weaponSelected = true;
        }

        void ProcessPlayerDeath(Player obj)
        {
            ForceStop();
        }

        void ForceStop()
        {
            if (isStarted)
            {
                StopAllCoroutines();
                Stop();
            }
        }

        const float WaitBeforeMove = 1.0f;
        const float WaitBeforeSwitch = 3.0f;
        const float WaitBeforeShoot = 1.0f;

        IEnumerator WaitForTutorial()
        {
            startEnemySpawn();

            yield return new WaitForSeconds(WaitBeforeMove);
            OnTutorialPanelActivate?.Invoke();
            OnTutorial_Move?.Invoke(this);

            // wait for menu to disable
            while (waitForMove)
            {
                yield return null;
            }
            OnTutorialPanelDeactivate?.Invoke();



            yield return new WaitForSeconds(WaitBeforeSwitch);
            OnTutorialPanelActivate?.Invoke();
            OnTutorial_WeaponSwitch?.Invoke(this);

            // wait for menu to disable
            while (waitForWeaponSwitch)
            {
                yield return null;
            }
            OnTutorialPanelDeactivate?.Invoke();

            // wait weapon to select
            while (!weaponSelected)
            {
                yield return null;
            }


            yield return new WaitForSeconds(WaitBeforeShoot);
            OnTutorialPanelActivate?.Invoke();
            OnTutorial_Shoot?.Invoke(this);

            // wait for menu to disable
            while (waitForShoot)
            {
                yield return null;
            }
            OnTutorialPanelDeactivate?.Invoke();



            // wait weapon for jam
            while (!weaponJammed)
            {
                yield return null;
            }
            OnTutorialPanelActivate?.Invoke();
            OnTutorial_WeaponJam?.Invoke(this);

            // wait for menu to disable
            while (waitForWeaponJam)
            {
                yield return null;
            }
            OnTutorialPanelDeactivate?.Invoke();



            // wait weapon to break
            while (!weaponBroke)
            {
                yield return null;
            }
            OnTutorialPanelActivate?.Invoke();
            OnTutorial_WeaponBreak?.Invoke(this);
          
            // wait for menu to disable
            while (waitForWeaponBreak)
            {
                yield return null;
            }
            OnTutorialPanelDeactivate?.Invoke();

            Stop();
        }

        private void ProcessWeaponJam()
        {
            weaponJammed = true;
        }

        private void ProcessWeaponBreak()
        {
            weaponBroke = true;
        }

        void Stop()
        {
            if (!isStarted)
            {
                return;
            }

            isStarted = false;
            OnTutorialEnd?.Invoke();

            Debug.Log("Tutorial ended", this);
        }
       
        public void SetWaitForMove(bool value)
        {
            waitForMove = value;
        }

        public void SetWaitForWeaponSwitch(bool value)
        {
            waitForWeaponSwitch = value;
        }

        public void SetWaitForShoot(bool value)
        {
            waitForShoot = value;
        }

        public void SetWaitForWeaponJam(bool value)
        {
            waitForWeaponJam = value;
        }

        public void SetWaitForWeaponBreak(bool value)
        {
            waitForWeaponBreak = value;
        }
    }
}
