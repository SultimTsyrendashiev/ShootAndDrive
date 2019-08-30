using System.Collections.Generic;
using UnityEngine;
using SD.ObjectPooling;

namespace SD
{
    class ParticlesPool : MonoBehaviour, IParticlesPool
    {
        class PooledParticleSystem
        {
            public ParticleSystem Particles;
            public AudioSource Audio;

            public PooledParticleSystem(ParticleSystem particles, AudioSource audio)
            {
                Particles = particles;
                Audio = audio;

                if (Audio != null)
                {
                    Audio.playOnAwake = false;
                }
            }
        }

        /// <summary>
        /// Contains all prefabs
        /// </summary>
        [SerializeField]
        ParticlesPoolPrefabs prefabs;

        // Contains all particle systems in this pool
        Dictionary<string, PooledParticleSystem> systems;

        bool isInitialized = false;


        public static IParticlesPool Instance { get; private set; }

        void Awake()
        {
            if (isInitialized)
            {
                return;
            }

            if (Instance != null)
            {
                Debug.Log("Several object pools. Destroying: ", this);

                // deactivate
                Destroy(this);
            }

            Init();
        }

        public void Init()
        {
            isInitialized = true;

            Instance = this;
            systems = new Dictionary<string, PooledParticleSystem>();

            foreach (var p in prefabs.Prefabs)
            {
                Register(p);
            }
        }

        void Register(GameObject prefab)
        {
            GameObject allocated = GameObject.Instantiate(prefab, transform);
            string prefabName = prefab.name;

            ParticleSystem system = allocated.GetComponent<ParticleSystem>();
            Debug.Assert(system != null, "Can't find particle system on object " + prefabName, this);

            // force system to use world space
            var main = system.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            // force system not to play on awake
            main.playOnAwake = false;

            systems.Add(prefabName, new PooledParticleSystem(system, system.GetComponent<AudioSource>()));
        }

        PooledParticleSystem GetPooledParticleSystem(string name)
        {
            Debug.Assert(systems.ContainsKey(name), "No particle system with name: " + name, this);
            return systems[name];
        }

        public ParticleSystem GetParticleSystem(string name)
        {
            return GetPooledParticleSystem(name).Particles;
        }

        public void Play(string name, Vector3 position, Quaternion rotation)
        {
            var pooled = GetPooledParticleSystem(name);
            ParticleSystem system = pooled.Particles;

            if (pooled.Audio != null)
            {
                pooled.Audio.Play();
            }

            Transform systemTransform = system.transform;
            systemTransform.position = position;
            systemTransform.rotation = rotation;

            system.Play(true);
        }

        public void Emit(string name, Vector3 position, Quaternion rotation, int count)
        {
            ParticleSystem system = GetParticleSystem(name);

            Transform systemTransform = system.transform;
            systemTransform.position = position;
            systemTransform.rotation = rotation;

            system.Emit(count);
        }
    }
}
