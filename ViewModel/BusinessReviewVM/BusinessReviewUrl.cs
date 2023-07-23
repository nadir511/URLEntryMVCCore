using System.ComponentModel.DataAnnotations;

namespace URLEntryMVC.ViewModel.BusinessReviewVM
{
    public class BusinessReviewUrl
    {
        public string? UrlName { get; set; }
        public int? BusinessPointId { get; set; }
        [Display(Name = "Point URL")]
        [DataType(DataType.Url)]
        public string? PointUrl { get; set; }
    }
    public class BusinessReviewPoints
    {
        public int BusinessPointId { get; set; }

        public string? PointUrl { get; set; }

        public int? DelayTimeInMinuts { get; set; }

        public int? DelayTimeInHours { get; set; }

        public bool? IsCurrentlyActive { get; set; }

        public int? UrlIdFk { get; set; }

        public int? CustomerIdFk { get; set; }
        public int? selectedBRpointId { get; set; }

    }
}
