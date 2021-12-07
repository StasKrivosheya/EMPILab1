using System.Collections.Generic;
using Prism.Events;

namespace EMPILab1.Events
{
    public class InitialDatasetChanged : PubSubEvent<List<double>>
    {
    }
}
