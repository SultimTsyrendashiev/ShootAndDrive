using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SD.Player
{
    public class CameraShaker
    {
        [SerializeField]
        private Transform camera;
        private static CameraShaker instance;

        public static CameraShaker Instance { get { return instance; } }

        void Awake()
        {
            instance = this;

            if (camera == null)
            {
                camera = Camera.main.transform;
            }
        }

        void Shake(float power, float duration)
        {
            
        }
    }
}
