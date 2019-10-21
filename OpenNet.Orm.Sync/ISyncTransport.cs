using OpenNet.Orm.Sync.Agents;

namespace OpenNet.Orm.Sync
{
    public interface ISyncTransport : IObservableProgession
    {
        /// <summary>
        /// Should indicate if communication with pair can be establish
        /// </summary>
        bool IsPairConnected { get; }

        /// <summary>
        /// Compute lenght of obj when serialized it
        /// </summary>
        /// <typeparam name="T">Type of object to mesure</typeparam>
        /// <param name="obj">Object instance to mesure</param>
        /// <returns>Lenght in byte</returns>
        long GetLenght<T>(T obj);

        /// <summary>
        /// Send object to connected pair
        /// </summary>
        /// <typeparam name="T">Type of object to send</typeparam>
        /// <param name="obj">Object to send</param>
        void Send<T>(T obj);

        /// <summary>
        /// Receive object from connected pair
        /// </summary>
        /// <typeparam name="T">Type of object to receive</typeparam>
        /// <returns>Object received</returns>
        T Receive<T>();
        
        /// <summary>
        /// Initialize before start sharing data
        /// </summary>
        void Initialize();

        /// <summary>
        /// Abort current operation
        /// </summary>
        void Abort();
    }
}