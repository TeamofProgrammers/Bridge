using System;
using System.Threading.Tasks;

namespace ToP.Bridge.Arguments
{
    public class RehashArgument : IArgument
    {
        public async Task Process(Main process, Action<string> logger, string input)
        {
            logger($"Command currently unavailable: {input.Split(' ')[0]}");
            //TODO: Setup rehash of config
        }
    }
}