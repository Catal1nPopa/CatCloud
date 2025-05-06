namespace Application.DTOs.Storage;

public class StorageInfoDTO
{
    public string RootDirectory { get; set; }
    public bool DirectoryStatus { get; set; }
    public long DirectoryTotalSpace { get; set; }
    public long DirectoryAvailableSpace { get; set; }

    public StorageInfoDTO(string rootDirectory, bool directoryStatus, long directoryTotalSpace, long directoryAvailableSpace)
    {
        RootDirectory = rootDirectory;
        DirectoryStatus = directoryStatus;
        DirectoryTotalSpace = directoryTotalSpace;
        DirectoryAvailableSpace = directoryAvailableSpace;
    }
}