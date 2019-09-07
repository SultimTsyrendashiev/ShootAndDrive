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
        BackgroundController background;

        void OnEnable()
        {
            if (background == null)
            {
                background = FindObjectOfType<BackgroundController>();
            }

            background?.SetTarget(transform);
        }

        void OnDisable()
        {
            background?.SetTarget(null);
        }
    }
}
