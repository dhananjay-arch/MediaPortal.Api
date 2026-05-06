namespace MediaPortal.Api.Models;

public class UploadResponse
{
    public int Inserted { get; set; }
    public int Updated { get; set; }
    public List<string> Errors { get; set; } = [];
}
