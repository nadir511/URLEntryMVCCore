using System.ComponentModel.DataAnnotations;

namespace URLEntryMVC.ViewModel.BusinessReviewVM
{
   
    public class BusinessReviewPoints
    {
        public string? UrlName { get; set; }
        public int BusinessPointId { get; set; }

        [Display(Name = "Point URL")]
        [DataType(DataType.Url)]
        public string? PointUrl { get; set; }
        [Display(Name = "Delay Time")]
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int? DelayTimeInMinuts { get; set; }
        [Display(Name = "Delay Time")]
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]

        public int? DelayTimeInHours { get; set; }

        public bool? IsCurrentlyActive { get; set; }

        public int? UrlIdFk { get; set; }
        public DateTime? DatePointer { get; set; }

        public int? CustomerIdFk { get; set; }
        public int? selectedBRpointId { get; set; }

    }
}
