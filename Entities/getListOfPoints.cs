namespace URLEntryMVC.Entities
{
    public class getListOfPoints
    {
        public int? PointId { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? PointLink { get; set; }
        public string? DomainLink { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? PointName { get; set; }
        public string? PointManagementName { get; set; }
        public string? CustomerNotes { get; set; }
        public int? TotalCliks { get; set; }
        public string? Esubject { get; set; }
        public string? Body { get; set; }
        public string? Emails { get; set; }
        public bool? SaveInLibrary { get; set; }
    }
}
