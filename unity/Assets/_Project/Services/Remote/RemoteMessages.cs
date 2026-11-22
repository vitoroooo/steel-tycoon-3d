using System;

namespace Steel.Services.Remote
{
    /// <summary>One IGameService method the phone ran, and the phone's game clock (real seconds) when it did.</summary>
    [Serializable] public sealed class RemoteCall
    {
        public string name;
        public string[] args;
        public double at;
    }

    /// <summary>A batch of calls. seq is one more than the last batch the server answered; digest is the phone's own result.</summary>
    [Serializable] public sealed class RemoteBatch
    {
        public long seq;
        public RemoteCall[] calls;
        public string digest;
        /// <summary>The phone's game clock when it took the digest; the server runs its career up to here before comparing.</summary>
        public double at;
    }

    [Serializable] public sealed class RemoteResult
    {
        public bool ok;
        /// <summary>Why the server refused, in the game's own words.</summary>
        public string error;
    }

    /// <summary>The server's answer. save is the whole career, sent when the phone's digest is not the server's: the phone then
    /// drops its own guess and shows the server's career.</summary>
    [Serializable] public sealed class RemoteReply
    {
        public long seq;
        public RemoteResult[] results;
        public string digest, save;
        /// <summary>The server's clock (Unix seconds) the save was written at.</summary>
        public long savedUtc;
        /// <summary>Unity reads a missing text as empty, the server as null: both mean no career was sent.</summary>
        public bool HasSave => !string.IsNullOrEmpty(save);
        /// <summary>Why the first refused call was refused; null when every call ran.</summary>
        public string Refusal()
        {
            foreach (RemoteResult result in results ?? new RemoteResult[0])
                if (!result.ok) return string.IsNullOrEmpty(result.error) ? "Ditolak server." : result.error;
            return null;
        }
    }
}
