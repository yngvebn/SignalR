using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
    public interface IContractsGenerator
    {
        object GenerateContracts();
    }
}
