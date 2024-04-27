namespace App.PL.Helpers
{
    public enum AllowedFiles
    {
        Picture,
        Document,
        Audio,
        Video
    }

    public class FilesManager
    {
        public static async Task<(bool, string)> UploadFileAsync(IWebHostEnvironment environment, IFormFile file, string mainFolder, string fileName, AllowedFiles allowed, int maxFileSizeMB = 5)
        {
            var Response = "Empty File";
            try
            {
                if (file == null || file.Length == 0)
                    return (false, Response);

                if (!IsFileValid(file, allowed, maxFileSizeMB, out string errorMessage))
                {
                    Response = errorMessage;
                    return (false, Response);
                }

                fileName = $"{fileName}{Path.GetExtension(file.FileName)}";

                // Delete existing file if any
                DeleteFile(environment, mainFolder, fileName, allowed);

                // Save the file to the specified path
                var storagePath = GetStoragePath(environment, mainFolder, fileName, allowed);
                using (var fs = new FileStream(storagePath, FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }

                Response = GetFileUrl(mainFolder, fileName, allowed);
                return (true, Response);
            }
            catch (Exception)
            {
                Response = "Error uploading the file";
                return (false, Response);
            }
        }

        public static bool DeleteFile(IWebHostEnvironment environment, string mainFolder, string fileName, AllowedFiles allowed)
        {
            try
            {
                var filePath = Path.Combine(environment.WebRootPath, "files", GetMainRoot(allowed), mainFolder, fileName);
                if (File.Exists(filePath))
                    File.Delete(filePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool IsFileValid(IFormFile file, AllowedFiles allowed, int maxFileSizeMB, out string errorMessage)
        {
            errorMessage = string.Empty;
            var allowedExtensions = GetAllowedExtensions(allowed);
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                errorMessage = $"Only the following file extensions are allowed: {string.Join(", ", allowedExtensions)}.";
                return false;
            }

            var maxFileSize = maxFileSizeMB * 1024 * 1024; // Convert MB to bytes
            if (file.Length > maxFileSize)
            {
                errorMessage = $"The file size exceeds the allowed limit {maxFileSize}.";
                return false;
            }

            return true;
        }

        private static List<string> GetAllowedExtensions(AllowedFiles allowed)
        {
            return allowed switch
            {
                AllowedFiles.Picture => [".jpg", ".jpeg", ".png", ".gif", ".webp", ".heic"],
                AllowedFiles.Document => [".doc", ".docx", ".pdf", ".txt"],
                AllowedFiles.Audio => [".mp3", ".wav", ".ogg", ".flac"],
                AllowedFiles.Video => [".mp4", ".avi", ".mkv", ".mov"],
                _ => [],
            };
        }

        private static string GetStoragePath(IWebHostEnvironment environment, string mainFolder, string fileName, AllowedFiles allowed)
        {
            var folder = Path.Combine(environment.WebRootPath, "files", GetMainRoot(allowed), mainFolder);
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, fileName);
        }

        private static string GetFileUrl(string mainFolder, string fileName, AllowedFiles allowed)
        {
            mainFolder = mainFolder.Replace('\\', '/');
            return $"/files/{GetMainRoot(allowed)}/{mainFolder}/{fileName}";
        }

        private static string GetMainRoot(AllowedFiles allowed)
        {
            return allowed switch
            {
                AllowedFiles.Picture => "Pictures",
                AllowedFiles.Document => "Documents",
                AllowedFiles.Audio => "Audios",
                AllowedFiles.Video => "Videos",
                _ => "Files",
            };
        }
    }
}
