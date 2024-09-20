using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sharesafely.Services;

namespace Sharesafely.Pages
{
    public class IndexModel : PageModel
    {
        private readonly BlobService _blobService;

        public IndexModel(BlobService blobService)
        {
            _blobService = blobService;
        }

        [BindProperty]
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();

        public string UploadResult { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Files.Count > 0)
            {
                var containerName = "sharesafelystoragecontainer"; // Replace with your container name
                var fileStreams = new Dictionary<string, Stream>();

                foreach (var file in Files)
                {
                    if (file.Length > 0)
                    {
                        var blobName = Path.GetFileName(file.FileName);
                        var stream = file.OpenReadStream();
                        fileStreams[blobName] = stream; // Add stream to the dictionary
                    }
                }

                // Upload the files using the BlobService
                try
                {
                    await _blobService.UploadBlobsAsync(containerName, fileStreams);
                    UploadResult = "Files uploaded successfully!";
                }
                catch (InvalidOperationException ex)
                {
                    UploadResult = ex.Message;
                }
                finally
                {
                    // Dispose of streams to free resources
                    foreach (var stream in fileStreams.Values)
                    {
                        stream.Dispose();
                    }
                }
            }
            else
            {
                UploadResult = "Please select at least one file to upload.";
            }

            return Page();
        }
    }
}
