namespace URLEntryMVC.Entities
{
    public class Product
    {
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public double TotalRemaining { get; set; }
        public double DailyUsageRate { get; set; }
        public int RemainingDays { get; set; }
    }
}
