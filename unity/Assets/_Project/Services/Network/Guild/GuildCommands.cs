using System;

namespace Steel.Services
{
    [Serializable]
    internal sealed class PlainCommand
    {
        public string key = Guid.NewGuid().ToString("N");
        public string action;
    }

    [Serializable]
    internal sealed class NameCommand
    {
        public string key = Guid.NewGuid().ToString("N");
        public string action, name;
    }

    [Serializable]
    internal sealed class TextCommand
    {
        public string key = Guid.NewGuid().ToString("N");
        public string action = "say", text;
    }

    [Serializable]
    internal sealed class ReportCommand
    {
        public string key = Guid.NewGuid().ToString("N");
        public string action = "report";
        public long message;
    }
}
