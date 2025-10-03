namespace SimpleWpfToMvcApp.Web.Helpers;

public static class ImagePathHelper
{
    /// <summary>
    /// Converts legacy WPF image paths to web-compatible paths
    /// </summary>
    /// <param name="imagePath">The original image path from the database</param>
    /// <returns>Web-compatible image path</returns>
    public static string ConvertToWebPath(string? imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
            return "/images/placeholder.png";
            
        // Handle old ProductImages/ paths by converting them to images/products/
        if (imagePath.StartsWith("ProductImages/"))
        {
            return "/images/products/" + imagePath.Substring("ProductImages/".Length);
        }
        
        // Handle old uploads/ paths (from incorrect previous uploads) by converting them to images/products/
        if (imagePath.StartsWith("uploads/"))
        {
            return "/images/products/" + imagePath.Substring("uploads/".Length);
        }
        
        // Handle paths that already start with images/products/ - these are correct
        if (imagePath.StartsWith("images/products/"))
        {
            return "/" + imagePath;
        }
        
        // Handle absolute paths starting with / - assume they are correct
        if (imagePath.StartsWith("/"))
            return imagePath;
            
        // Default to images/products/ for any other case
        return "/images/products/" + imagePath;
    }
}
