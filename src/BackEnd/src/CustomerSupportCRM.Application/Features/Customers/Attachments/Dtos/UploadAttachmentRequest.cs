using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CustomerSupportCRM.Application.Features.Customers.Attachments.Dtos
{
    public class UploadAttachmentRequest
    {
        public Guid CustomerId { get; set; }

        [Required]
        public IFormFile File { get; set; } 
    }
}
