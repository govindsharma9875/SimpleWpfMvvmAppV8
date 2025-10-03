using System.ComponentModel.DataAnnotations;

namespace SimpleWpfToMvcApp.Web.Models
{
    /// <summary>
    /// View model for error page display with request tracking
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Request ID for error tracking and debugging
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Indicates whether to show the request ID for debugging
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        /// <summary>
        /// Error title for display
        /// </summary>
        [Display(Name = "Error Title")]
        public string Title { get; set; } = "An error occurred";

        /// <summary>
        /// Error message for user display
        /// </summary>
        [Display(Name = "Error Message")]
        public string Message { get; set; } = "Sorry, an error occurred while processing your request.";

        /// <summary>
        /// Error code for categorization
        /// </summary>
        public int? ErrorCode { get; set; }

        /// <summary>
        /// Detailed error information for development environment
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Timestamp when the error occurred
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// URL where the error occurred
        /// </summary>
        public string? RequestPath { get; set; }

        /// <summary>
        /// HTTP method that caused the error
        /// </summary>
        public string? HttpMethod { get; set; }

        /// <summary>
        /// User identifier when error occurred (if authenticated)
        /// </summary>
        public string? UserId { get; set; }
    }
}
