using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleWpfToMvcApp.Web.Models;
using SimpleWpfToMvcApp.Web.Repositories;
using SimpleWpfToMvcApp.Web.Services;

namespace SimpleWpfToMvcApp.Web.Controllers;

[Authorize]
public class ProductsController(
    IProductRepository productRepository,
    IFileUploadService fileUploadService,
    ILogger<ProductsController> logger) : Controller
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IFileUploadService _fileUploadService = fileUploadService;
    private readonly ILogger<ProductsController> _logger = logger;

    // GET: Products
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Products index page accessed by user: {User}", User.Identity?.Name);
        var products = await _productRepository.GetAllProductsAsync();
        return View(products);
    }

    // GET: Products/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productRepository.GetProductByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Product not found with ID: {ProductId}", id);
            return NotFound();
        }

        return View(product);
    }

    // GET: Products/Create
    public IActionResult Create()
    {
        return View(new ProductCreateEditViewModel());
    }

    // POST: Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Check for duplicate SKU
        if (await _productRepository.SKUExistsAsync(model.SKU))
        {
            ModelState.AddModelError(nameof(model.SKU), "SKU already exists. Please enter a unique SKU.");
            return View(model);
        }

        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            SKU = model.SKU,
            IsActive = model.IsActive
        };

        // Handle image upload
        if (model.ImageFile != null && _fileUploadService.IsValidImageFile(model.ImageFile))
        {
            var imagePath = await _fileUploadService.UploadFileAsync(model.ImageFile, "images/products");
            if (imagePath != null)
            {
                product.ImagePath = imagePath;
            }
        }

        try
        {
            await _productRepository.AddProductAsync(product);
            _logger.LogInformation("Product created successfully: {ProductName} by user: {User}", 
                product.Name, User.Identity?.Name);
            
            TempData["SuccessMessage"] = "Product created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product: {ProductName}", model.Name);
            ModelState.AddModelError(string.Empty, "An error occurred while creating the product. Please try again.");
            return View(model);
        }
    }

    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productRepository.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var model = new ProductCreateEditViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            SKU = product.SKU,
            ImagePath = product.ImagePath,
            IsActive = product.IsActive
        };

        return View(model);
    }

    // POST: Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductCreateEditViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Check for duplicate SKU (excluding current product)
        if (await _productRepository.SKUExistsAsync(model.SKU, model.Id))
        {
            ModelState.AddModelError(nameof(model.SKU), "SKU already exists. Please enter a unique SKU.");
            return View(model);
        }

        var product = await _productRepository.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        // Store old image path for potential deletion
        var oldImagePath = product.ImagePath;

        // Update product properties
        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = model.Price;
        product.SKU = model.SKU;
        product.IsActive = model.IsActive;

        // Handle new image upload
        if (model.ImageFile != null && _fileUploadService.IsValidImageFile(model.ImageFile))
        {
            var newImagePath = await _fileUploadService.UploadFileAsync(model.ImageFile, "images/products");
            if (newImagePath != null)
            {
                product.ImagePath = newImagePath;
                
                // Delete old image if it exists and is different
                if (!string.IsNullOrEmpty(oldImagePath) && oldImagePath != newImagePath)
                {
                    await _fileUploadService.DeleteFileAsync(oldImagePath);
                }
            }
        }

        try
        {
            await _productRepository.UpdateProductAsync(product);
            _logger.LogInformation("Product updated successfully: {ProductName} by user: {User}", 
                product.Name, User.Identity?.Name);
            
            TempData["SuccessMessage"] = "Product updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product: {ProductName}", model.Name);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the product. Please try again.");
            return View(model);
        }
    }

    // POST: Products/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productRepository.DeleteProductAsync(id);
            _logger.LogInformation("Product deleted successfully: {ProductName} by user: {User}", 
                product.Name, User.Identity?.Name);
            
            TempData["SuccessMessage"] = "Product deleted successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the product. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }
}
