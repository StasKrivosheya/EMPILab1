namespace EMPILab1.Models
{
    public class TestModel
    {
        public TestModel(string xValue, double yValue)
        {
            Month = xValue;
            Target = yValue;
        }

        public string Month { get; set; }

        public double Target { get; set; }
    }
}
