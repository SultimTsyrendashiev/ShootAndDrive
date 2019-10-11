using UnityEngine;

namespace SD.Enemies
{
    /// <summary>
    /// Script that activates pass by vehicle sound.
    /// Should be attached to audio listener
    /// </summary>
    [RequireComponent(typeof(Collider))]
    class PassbyTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            // if (other.tag == "Passby")
            {
                other.GetComponent<PassbyVehicleSound>().PlaySound();
            }
        }
    }
}
