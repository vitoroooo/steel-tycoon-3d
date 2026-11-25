using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steel.Services.Remote
{
    /// <summary>The phone's line to the career server: calls wait here, leave in numbered batches, and a batch the
    /// server did not answer is sent again under the same number, so it can never run twice.</summary>
    public sealed class RemoteLink
    {
        /// <summary>Seconds between reports of where the player is when nothing else is said, and before a failed batch is retried.</summary>
        public const double Beat = 3, Retry = 5;
        public const int MaxCalls = 48;
        private readonly Func<RemoteBatch, Task<RemoteReply>> send;
        private readonly List<RemoteCall> waiting = new List<RemoteCall>();
        private readonly Dictionary<string, RemoteCall> moved = new Dictionary<string, RemoteCall>();
        private Task<RemoteReply> flying;
        private RemoteBatch batch;
        private double quiet;
        public long Seq { get; private set; }
        /// <summary>Set while the server cannot be reached.</summary>
        public string Notice { get; private set; }
        /// <summary>Everything the server has not been sent yet, in the order it happened.</summary>
        public IEnumerable<RemoteCall> Unsent => waiting.Concat(moved.Values);

        public RemoteLink(Func<RemoteBatch, Task<RemoteReply>> send, long seq)
        {
            this.send = send ?? throw new ArgumentNullException(nameof(send)); Seq = seq;
        }

        /// <summary>A command: it travels with the positions reported before it, so the server sees it where it happened.</summary>
        public void Queue(RemoteCall call) { Settle(); waiting.Add(call); }
        /// <summary>A position: only the latest of each kind is kept.</summary>
        public void Move(string key, RemoteCall call) => moved[key] = call;

        /// <summary>Called every frame. Returns the server's answer once, when one has arrived.</summary>
        public RemoteReply Pump(double realSeconds, double clock, Func<string> digest)
        {
            quiet += realSeconds;
            if (flying != null)
            {
                if (!flying.IsCompleted) return null;
                Task<RemoteReply> done = flying; flying = null; quiet = 0;
                if (done.Status != TaskStatus.RanToCompletion || done.Result == null)
                {
                    Notice = "Server karier tidak terjangkau. Mencoba lagi."; return null;
                }
                batch = null; Notice = null; Seq = done.Result.seq;
                return done.Result;
            }
            if (batch != null) { if (quiet < Retry) return null; }
            else
            {
                if (waiting.Count == 0 && quiet < Beat) return null;
                Settle();
                // a silent phone still sends an empty batch, so the server knows the player is here
                int count = Math.Min(MaxCalls, waiting.Count);
                bool whole = count == waiting.Count;
                batch = new RemoteBatch { seq = Seq + 1, calls = waiting.Take(count).ToArray(),
                    at = whole ? clock : waiting[count - 1].at, digest = whole ? digest() : null };
                waiting.RemoveRange(0, count);
            }
            quiet = 0; flying = send(batch);
            return null;
        }

        /// <summary>After the server's career replaced the phone's: what is still unsent now happened at this time.</summary>
        public void Restamp(double clock) { foreach (RemoteCall call in Unsent) call.at = clock; }

        private void Settle() { waiting.AddRange(moved.Values.OrderBy(call => call.at)); moved.Clear(); }
    }
}
