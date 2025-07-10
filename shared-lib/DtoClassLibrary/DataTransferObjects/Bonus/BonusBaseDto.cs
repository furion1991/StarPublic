
namespace DtoClassLibrary.DataTransferObjects.Bonus
{
    public class BonusBaseDto : IBonusDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string? Name { get; set; }
        public string? Description { get; set; }
        public string? BonusImage { get; set; }
        public string? ImageForDepositView { get; set; }
        public BonusType BonusType { get; set; }
        public decimal DropChance { get; set; }
        public bool IsDeleted { get; set; }

    }
}
