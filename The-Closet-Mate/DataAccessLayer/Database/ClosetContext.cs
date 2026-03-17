using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BusinessLogicLayer.DomainModels; // ✅ Use domain models from BLL

namespace DataAccessLayer.Database
{
    public class ClosetContext : IdentityDbContext<IdentityUser>
    {
        public ClosetContext(DbContextOptions<ClosetContext> options) : base(options) { }

        public DbSet<ClothingItem> ClothingItems { get; set; }
        public DbSet<Outfit> Outfits { get; set; }
        public DbSet<OutfitItem> OutfitItems { get; set; }
        public DbSet<ScheduledOutfit> ScheduledOutfits { get; set; } // Optional, if needed
    }
}
