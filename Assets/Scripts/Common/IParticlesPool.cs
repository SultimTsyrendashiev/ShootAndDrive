using UnityEngine;

namespace SD
{
    interface IParticlesPool
    {
        /// <summary>
        /// Get particle system
        /// (f.e. for modifying simulation space of all particles in this pool)
        /// </summary>
        ParticleSystem GetParticleSystem(string name);
        
        /// <summary>
        /// Play particle system
        /// </summary>
        void Play(string name, Vector3 position, Quaternion rotation);
        
        /// <summary>
        /// Emit specified amount of particles
        /// </summary>
        /// <param name="count">how many particles to emit</param>
        void Emit(string name, Vector3 position, Quaternion rotation, int count);
    }
}
