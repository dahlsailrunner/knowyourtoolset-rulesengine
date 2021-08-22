namespace SampleApplication
{
    public class Discount
    {
        public decimal DiscountPct { get; set; }
        public short DaysValid { get; set; }

        public override string ToString()
        {
            return $"{DiscountPct * 100}% discount, valid for {DaysValid} days.";
        }
    }
}
