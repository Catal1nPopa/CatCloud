namespace CatCloud.Models.Storage;

public class StorageInfoModel
{
    public string RootDirectory { get; set; }
    public bool DirectoryStatus { get; set; }
    public long DirectoryTotalSpace { get; set; }
    public long DirectoryAvailableSpace { get; set; }
}