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
        public IActionResult AddProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                var viewModelValid = new BackOfficeIndexViewModel
                {
                    Products = _productSvc.GetProducts(),
                    NewProduct = product
                };
                return View("Index", viewModelValid);
            }

            _productSvc.AddProduct(product);
            return RedirectToAction("Index");
        }

        [HttpPut]
        public IActionResult UpdateProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                var viewModelValid = new BackOfficeIndexViewModel
                {
                    Products = _productSvc.GetProducts(),
                    NewProduct = product
                };
                return View("Index", viewModelValid);
            }

            _productSvc.UpdateProduct(product);
            return RedirectToAction("Index");
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
