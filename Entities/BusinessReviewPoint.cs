namespace URLEntryMVC.Entities
{
    public partial class BusinessReviewPoint
    {
        public int BusinessPointId { get; set; }

        public string? PointUrl { get; set; }

        public int? DelayTimeInMinuts { get; set; }

        public int? DelayTimeInHours { get; set; }

        public bool? IsCurrentlyActive { get; set; }

        public int? UrlIdFk { get; set; }

        public int? CustomerIdFk { get; set; }
    }
}
