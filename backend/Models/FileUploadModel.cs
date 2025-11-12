namespace backend.Models
{
    // Model used specifically for file uploads via API
    public class FileUploadModel
    {
        public IFormFile? File { get; set; }
    }
}