using System;
using System.Threading.Tasks;

namespace ToP.Bridge.Arguments
{
    public interface IArgument
    {
        Task Process(Main process, Action<string> logger, string input);
    }
}