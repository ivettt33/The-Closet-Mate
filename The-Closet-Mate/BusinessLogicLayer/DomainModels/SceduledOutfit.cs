namespace BusinessLogicLayer.DomainModels
{
    public class ScheduledOutfit
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int OutfitId { get; set; }
        public DateTime ScheduledDate { get; set; }
    }
}
