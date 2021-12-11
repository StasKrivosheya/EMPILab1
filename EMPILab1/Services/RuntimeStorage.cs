using System.Collections.Generic;
using EMPILab1.Models;
using OxyPlot;

namespace EMPILab1.Services
{
    public class RuntimeStorage : IRuntimeStorage
    {
        public PlotModel HistogramModel { get; set; }

        public double ClassWidth { get; set; }

        public IList<ClassViewModel> Classes { get; set; }
    }
}
