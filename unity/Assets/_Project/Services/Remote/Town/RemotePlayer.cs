using System;

namespace Steel.Services.Remote
{
    /// <summary>Another player as the server last saw him in the shared city.</summary>
    [Serializable] public sealed class RemotePlayer
    {
        public string name;
        public float x, z;
        /// <summary>The vehicle he drives ("cart", "triseda", ...), or empty on foot.</summary>
        public string vehicle;
        public int caste;
    }
}
