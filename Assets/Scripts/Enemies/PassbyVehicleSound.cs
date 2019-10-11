using UnityEngine;
using Random = UnityEngine.Random;

namespace SD.Enemies
{
    class PassbyVehicleSound : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip[] passbySounds;

        Rigidbody vehicle;

        void Start()
        {
            vehicle = GetComponentInParent<Rigidbody>();
        }

        public void PlaySound()
        {
            if (vehicle == null || audioSource == null)
            {
                return;
            }

            if (vehicle.isKinematic)
            {
                audioSource.PlayOneShot(passbySounds[Random.Range(0, passbySounds.Length)]);
            }
        }
    }
}
