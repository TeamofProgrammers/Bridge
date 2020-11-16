using System;
using System.Threading.Tasks;

namespace ToP.Bridge.Arguments
{
    public class StatusArgument : IArgument
    {
        public async Task Process(Main process, Action<string> logger, string input)
        {
            logger(process?.UserStatus);
            logger(process?.ChannelStatus);
        }
    }
}