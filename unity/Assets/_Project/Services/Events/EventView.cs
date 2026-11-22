namespace Steel.Services
{
    /// <summary>The events of PRD appendix E that wait for the player or colour the day, as the Buku usaha shows them.</summary>
    public struct EventView
    {
        public string Note;
        /// <summary>Today AI collectors are on strike and bring this share of their usual load (1 when they are not).</summary>
        public bool Strike;
        public double Supply;
        /// <summary>IBN opened today's prices more than 8% up, by this fraction.</summary>
        public bool Spike;
        public double SpikeRise;
        /// <summary>A dirty load on the scale: grade, gross kg, share of dirt, what it costs with the cut, game hours left to answer.</summary>
        public bool Dirty;
        public string DirtyGrade;
        public double DirtyKg, Dirt, DirtyPrice, DirtyHoursLeft;
        /// <summary>A flash tender: the product, kg ordered and already sent, price per kg, game hours left.</summary>
        public bool Flash, FlashTaken;
        public string FlashProduct;
        public double FlashKg, FlashSent, FlashPrice, FlashHoursLeft, FlashReady;
    }

    public partial interface IGameService
    {
        EventView Events { get; }
        /// <summary>Takes the dirty load with the cut, or turns it away. Returns what happened.</summary>
        string AnswerDeposit(bool accept);
        string TakeFlashTender();
        /// <summary>Sends the mill's stock of the ordered product. Returns the payment.</summary>
        double DeliverFlashTender();
    }
}
