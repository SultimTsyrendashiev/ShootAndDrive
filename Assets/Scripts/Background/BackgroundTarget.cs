using UnityEngine;

namespace SD.Background
{
    /// <summary>
    /// Attach this script to object that must be 
    /// a target of background controller.
    /// It means that background blocks will be
    /// spawned around it
    /// </summary>
    class BackgroundTarget : MonoBehaviour
    {
        void Awake()
        {
            var b = FindObjectOfType<BackgroundController>();
            b?.SetTarget(transform);
        }
    }
}
