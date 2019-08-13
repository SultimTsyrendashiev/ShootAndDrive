using UnityEngine;

namespace SD.Background
{
    class Skybox3D : MonoBehaviour
    {
        /// <summary>
        /// This scale will be applied to objects
        /// </summary>
        public float Scale = 0.1f;

        /// <summary>
        /// All objects for 3D skybox must be children of this transform
        /// </summary>
        [SerializeField]
        Transform container;

        /// <summary>
        /// If camera is not set, main camera will be used
        /// </summary>
        public Camera MainCamera;

        // special camera
        Camera skyboxCamera;

        void Start()
        {
            if (container == null || Scale <= 0)
            {            
                // disable if not set
                Debug.Log("3D skybox disabled", this);
                enabled = false;
            }
        }

        void OnDestroy()
        {
            if (skyboxCamera != null)
            {
                Destroy(skyboxCamera.gameObject);
            }
        }
        
        void Update()
        {
            if (container == null || Scale <= 0)
            {
                return;
            }

            if (MainCamera != null && skyboxCamera == null)
            {
                var camObj = new GameObject("SkyboxCamera");
                skyboxCamera = camObj.AddComponent<Camera>();
            }

            // scaled position for main camera
            skyboxCamera.transform.position = MainCamera.transform.position * Scale;
        }
    }
}
