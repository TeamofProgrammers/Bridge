using System;

namespace ToP.Bridge.Model.Events.Irc
{
    public class IrcMessageEventArgs : EventArgs
    {
        public IrcMessageEventArgs(IrcMessageEvent message)
        {
            Message = message;
        }

        public IrcMessageEvent Message { get; }
    }
}
