using System.Drawing;

namespace HomeAccountant.Core.Model
{
    public class ChartValue
    {
        //Commented because of no references
        //public ChartValue()
        //{
            
        //}

        public ChartValue(string label, double value, Color color)
        {
            Label = label;
            Value = value;
            Color = color;
        }

        public string Label { get; set; }
        
        public double Value { get; set; }

        public Color Color { get; set; }
    }
}
