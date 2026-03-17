
namespace BusinessLogicLayer.DomainModels
{
    public class OutfitItem
    {
        public int Id { get; set; }

        public int OutfitId { get; set; }
        public Outfit Outfit { get; set; }

        public int ClothingItemId { get; set; }
        public ClothingItem ClothingItem { get; set; }
    }
}