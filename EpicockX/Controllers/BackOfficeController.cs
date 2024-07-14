using EpicockX.Models;
using EpicockX.Services;
using Microsoft.AspNetCore.Mvc;

namespace EpicockX.Controllers
{
    public class BackOfficeController : Controller
    {
        private readonly ProductService _productSvc;
        private readonly ImageService _imageSvc;

        public BackOfficeController(ProductService productService, ImageService imageService)
        {
            _productSvc = productService;
            _imageSvc = imageService;
        }

        public IActionResult Index()
        {
            var products = _productSvc.GetProducts();
            var viewModel = new BackOfficeIndexViewModel
            {
                Products = products,
                NewProduct = new Product(),
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddOrUpdateProduct(Product newProduct)
        {
            if (ModelState.IsValid)
            {
                if (newProduct.ProductId == 0)
                {
                    _productSvc.AddProduct(newProduct);
                }
                else
                {
                    _productSvc.UpdateProduct(newProduct);
                }
                return RedirectToAction("Index");
            }

            var viewModel = new BackOfficeIndexViewModel
            {
                Products = _productSvc.GetProducts(),
                NewProduct = newProduct
            };
            return View("Index", viewModel);
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            _productSvc.DeleteProduct(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddProductImage(ProductImage image)
        {
            if (ModelState.IsValid)
            {
                _imageSvc.AddImage(image);
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteProductImage(int id)
        {
            _imageSvc.DeleteImage(id);
            return RedirectToAction("Index");
        }
    }
}
