namespace SD
{
    interface ISteeringWheel
    {
        /// <summary>
        /// Steering in [-1..1]
        /// </summary>
        float Steering { get; }
        
        /// <summary>
        /// Steering in [0..1]
        /// </summary>
        float SteeringNormalized { get; }

        /// <summary>
        /// Process input
        /// </summary>
        void Steer(float steeringInput);

        /// <summary>
        /// Set init values
        /// </summary>
        void Restart();
        void Stop();
    }
}
