using UnityEngine;

namespace SD.PlayerLogic
{
    [RequireComponent(typeof(PlayerVehicle))]
    class PlayerVehicleAudio : MonoBehaviour
    {
        const float MinTimeBetweenSounds = 0.1f;

        [SerializeField]
        AudioClip[] crashSounds;

        AudioSource[] vehicleAudio;
        PlayerVehicle vehicle;

        float previousSoundStartTime;

        void Start()
        {
            vehicle = GetComponent<PlayerVehicle>();
            vehicleAudio = GetComponents<AudioSource>();

            vehicle.OnVehicleCollision += CollideVehicle;
            vehicle.OnSteering += UpdateSteeringAudio;

            Debug.Assert(vehicleAudio.Length >= 2, "Must 2 audio sources", this);
        }

        void OnDestroy()
        {
            vehicle.OnVehicleCollision -= CollideVehicle;
        }

        void UpdateSteeringAudio(float steering)
        {
            // steering in [-1..1]
            float volume = Mathf.Abs(steering);
            vehicleAudio[1].volume = volume;
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
