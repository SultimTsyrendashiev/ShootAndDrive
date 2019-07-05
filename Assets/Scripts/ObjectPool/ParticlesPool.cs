using System.Collections.Generic;
using UnityEngine;
using SD.ObjectPooling;

namespace SD
{
    class ParticlesPool : MonoBehaviour, IParticlesPool
    {
        /// <summary>
        /// Contains all prefabs
        /// </summary>
        [SerializeField]
        ParticlesPoolPrefabs prefabs;
        // Contains all particle systems in this pool
        Dictionary<string, ParticleSystem> systems;

        public static IParticlesPool Instance { get; private set; }

        void Awake()
        {
            Debug.Assert(Instance == null, "Several particle pools", this);
            Instance = this;

            systems = new Dictionary<string, ParticleSystem>();

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

            systems.Add(prefabName, system);
        }

        public ParticleSystem GetParticleSystem(string name)
        {
            Debug.Assert(systems.ContainsKey(name), "No particle system with name: " + name, this);
            return systems[name];
        }

        public void Play(string name, Vector3 position, Quaternion rotation)
        {
            ParticleSystem system = GetParticleSystem(name);

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
