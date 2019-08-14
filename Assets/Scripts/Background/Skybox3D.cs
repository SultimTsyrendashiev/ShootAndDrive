using UnityEngine;

namespace SD.Background
{
    class Skybox3D : MonoBehaviour
    {
        /// <summary>
        /// How big must be objects.
        /// Actually, depends only on skybox camera position
        /// </summary>
        public float Scale = 30.0f;

        /// <summary>
        /// If camera is not set, main camera will be used
        /// </summary>
        public Camera MainCamera;

        // special camera
        [SerializeField]
        Camera skyboxCamera;

        void Start()
        {
            if (skyboxCamera == null)
            {            
                // disable if not set
                Debug.Log("3D skybox disabled: container or camera is not set", this);
                enabled = false;
            }
        }
        
        void Update()
        {
            if (MainCamera == null || Scale <= 0)
            {
                return;
            }

            // scaled position for main camera
            skyboxCamera.transform.position = MainCamera.transform.position / Scale;
            skyboxCamera.transform.rotation = MainCamera.transform.rotation;
        }
    }
}
