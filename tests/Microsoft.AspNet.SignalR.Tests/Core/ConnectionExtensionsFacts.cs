using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests.Core
{
    public class ConnectionExtensionsFacts
    {
        [Fact]
        public void SendThrowsNullExceptionWhenConnectionIdIsNull()
        {
            Object obj = new Object();
            //IConnection conn = new RawConnection();
            try
            {
                ConnectionExtensions.Send(null, null, new string[0]);
            }
            catch (Exception ex)
            {
                Assert.IsType(typeof(ArgumentNullException), ex);
            }
        }
    }
}
