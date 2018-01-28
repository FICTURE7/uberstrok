using System.Collections.Generic;

namespace UberStrok.Realtime.Server
{
    /// <summary>
    /// Represents a room/group of <see cref="BasePeer"/>.
    /// </summary>
    /// <typeparam name="TPeer">Type of <see cref="BasePeer"/>.</typeparam>
    public interface IRoom<TPeer> where TPeer : BasePeer
    {
        /// <summary>
        /// Gets the read-only list of <see cref="TPeer"/>.
        /// </summary>
        IReadOnlyList<TPeer> Peers { get; }

        /// <summary>
        /// Notifies the <see cref="IRoom{TPeer}"/> that the specified <see cref="TPeer"/> joined.
        /// </summary>
        /// <param name="peer"><see cref="TPeer"/> that joined.</param>
        void Join(TPeer peer);

        /// <summary>
        /// Notifies the <see cref="IRoom{TPeer}"/> that the specified <see cref="TPeer"/> left.
        /// </summary>
        /// <param name="peer"><see cref="TPeer"/> that left.</param>
        void Leave(TPeer peer);
    }
}
