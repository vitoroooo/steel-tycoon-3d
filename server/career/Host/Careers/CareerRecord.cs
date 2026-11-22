using Steel.Services.Remote;

namespace Steel.Career.Careers
{
    /// <summary>A career as it is written to the database after every batch.</summary>
    public sealed class CareerRecord
    {
        public string Save;
        public long SeenMs, Seq;
        /// <summary>The answer to batch Seq, kept so a batch sent twice is answered twice and run once.</summary>
        public RemoteReply Reply;
    }
}
