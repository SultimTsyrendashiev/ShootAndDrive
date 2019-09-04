using UnityEngine;

namespace SD.Weapons
{
    class Minigun : Gun
    {
        const float SpinUpTime = 0.3f;
        const float SpinDownTime = 0.6f;

        enum RotatorState
        {
            Nothing,
            Idle,
            Spinning,
            SpinningUp,
            SpinningDown
        }

        [SerializeField]
        Transform rotator;
        [SerializeField]
        float rotatorMaxAngleSpeed = 600;

        [SerializeField]
        AudioClip rotatorSpinUp;
        [SerializeField]
        AudioClip rotatorSpinDown;
        [SerializeField]
        AudioClip rotatorSpin;

        // Note: change it only through 'SetRotatorState'
        RotatorState rotatorState;

        AudioSource rotatorAudioSource;

        float spinTime;
        float angleSpeed;

        Vector3 rotatorEuler;

        protected override void InitWeapon()
        {
            Debug.Assert(rotator != null, "Rotator must be assigned", this);

            rotatorAudioSource = GetComponent<AudioSource>();
            Debug.Assert(rotatorAudioSource != null, "Audio source must be attached to minigun", this);
        }

        protected override void Activate()
        {
            spinTime = 0;
            SetRotatorState(RotatorState.Idle);
        }

        protected override void Deactivate()
        {
            spinTime = 0;
            SetRotatorState(RotatorState.Nothing);
        }

        protected override void PrimaryAttack()
        {
            // shoot only if spinned up
            if (rotatorState == RotatorState.Spinning)
            {
                base.PrimaryAttack();
            }
        }

        void Update()
        {
            if (State != WeaponState.Ready && State != WeaponState.Reloading)
            {
                return;
            }

            if (UI.InputController.FireButton)
            {
                spinTime += Time.deltaTime;
                spinTime = Mathf.Clamp(spinTime, 0, SpinUpTime);

                if (spinTime == SpinUpTime)
                {
                    if (rotatorState != RotatorState.Spinning)
                    {
                        SetRotatorState(RotatorState.Spinning);
                    }
                }
                else
                {
                    if (rotatorState != RotatorState.SpinningUp)
                    {
                        SetRotatorState(RotatorState.SpinningUp);
                    }
                }
            }
            else
            {
                // SpinDownTime deltaTime is scaled to SpinUpTime deltaTime
                spinTime -= Time.deltaTime * SpinUpTime / SpinDownTime;
                spinTime = Mathf.Clamp(spinTime, 0, SpinUpTime);

                if (spinTime == 0)
                {
                    if (rotatorState != RotatorState.Idle)
                    {
                        SetRotatorState(RotatorState.Idle);
                    }
                }
                else
                {
                    if (rotatorState != RotatorState.SpinningDown)
                    {
                        SetRotatorState(RotatorState.SpinningDown);
                    }
                }
            }

            // rotate
            float target = Mathf.Lerp(0, rotatorMaxAngleSpeed, spinTime / SpinUpTime);
            angleSpeed = Mathf.Lerp(angleSpeed, target, 20 * Time.deltaTime);

            rotatorEuler.y += angleSpeed * Time.deltaTime;
            rotator.localEulerAngles = rotatorEuler;
        }

        void SetRotatorState(RotatorState newState)
        {
            rotatorState = newState;

            switch (rotatorState)
            {
                case RotatorState.Spinning:
                    rotatorAudioSource.loop = true;
                    rotatorAudioSource.clip = rotatorSpin;
                    rotatorAudioSource?.Play();
                    break;
                case RotatorState.SpinningDown:
                    rotatorAudioSource?.PlayOneShot(rotatorSpinDown);
                    break;
                case RotatorState.SpinningUp:
                    rotatorAudioSource?.PlayOneShot(rotatorSpinUp);
                    break;
                case RotatorState.Nothing:
                    rotatorAudioSource?.Stop();
                    break;
            }
        }
    }
}
