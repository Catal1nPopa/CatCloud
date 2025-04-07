
namespace Domain.Entities.Statistics
{
    public class StatisticsEntity
    {
        public double availableMemory { get; set; }
        public double totalMemory { get; set; }
        public double usedMemory { get; set; }
        public int documents { get; set; }
        public double documentsSize { get; set; }
        public int videos { get; set; }
        public double videosSize { get; set; }
        public int images { get; set; }
        public double imagesSize { get; set; }
        public int other { get; set; }
        public double otherSize { get; set; }
    }
}
