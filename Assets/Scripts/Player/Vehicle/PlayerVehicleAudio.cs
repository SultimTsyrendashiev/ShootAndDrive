using UnityEngine;

namespace SD.PlayerLogic
{
    [RequireComponent(typeof(PlayerVehicle))]
    class PlayerVehicleAudio : MonoBehaviour
    {
        const float MinTimeBetweenSounds = 0.1f;

        [SerializeField]
        AudioClip[] crashSounds;

        /// <summary>
        /// Vehicle's audio sources.
        /// 0 - vehicle colliding
        /// 1 - vehicle steering
        /// 2 - vehicle engine
        /// </summary>
        AudioSource[] vehicleAudio;
        PlayerVehicle vehicle;

        float previousSoundStartTime;

        void Start()
        {
            vehicle = GetComponent<PlayerVehicle>();
            vehicleAudio = GetComponents<AudioSource>();

            vehicle.OnVehicleCollision += CollideVehicle;
            vehicle.OnSteering += UpdateSteeringAudio;
            vehicle.OnVehicleHealthChange += ProcessVehicleBreak;
            vehicle.OnVehicleStart += StartEngineAudio;
            GameController.OnPlayerDeath += ProcessPlayerDeath;

            StartEngineAudio();

            Debug.Assert(vehicleAudio.Length >= 3, "Must 3 audio sources", this);
        }

        void OnDestroy()
        {
            if (vehicle != null)
            {
                vehicle.OnVehicleCollision -= CollideVehicle;
                vehicle.OnSteering -= UpdateSteeringAudio;
                vehicle.OnVehicleHealthChange -= ProcessVehicleBreak;
                vehicle.OnVehicleStart -= StartEngineAudio;
                GameController.OnPlayerDeath -= ProcessPlayerDeath;
            }
        }

        void StartEngineAudio()
        {
            vehicleAudio[2].Play();

            Debug.Log("Vehicle engine sound started");
        }

        void UpdateSteeringAudio(float steering)
        {
            // steering in [-1..1]
            float volume = Mathf.Abs(steering);
            vehicleAudio[1].volume = volume;
        }

        void ProcessPlayerDeath(Player obj)
        {
            vehicleAudio[2].Stop();
        }

        void ProcessVehicleBreak(float health)
        {
            if (health == 0)
            {
                vehicleAudio[2].Stop();
            }
        }

        void CollideVehicle(IVehicle other, float damage)
        {
            // play sound if not overlapping
            if (Time.time - previousSoundStartTime >= MinTimeBetweenSounds)
            {
                vehicleAudio[0].PlayOneShot(crashSounds[Random.Range(0, crashSounds.Length)]);
                previousSoundStartTime = Time.time;
            }
        }
    }
}
