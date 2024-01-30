namespace Entities.DTOs.Request
{
    public class TestDetailInfoDTO
    {
        public long Id { get; set; }

        public string Abbreviation { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public double Price { get; set; }

        public byte Duration { get; set; }
    }
}
