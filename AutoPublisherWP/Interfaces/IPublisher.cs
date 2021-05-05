using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPublisherWP.Interfaces
{
    interface IPublisher
    {

        Task Publish();

        Task TestConnection();

        Task CheckImage();
    }
}
