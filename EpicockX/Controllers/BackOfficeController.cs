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
        public IActionResult AddProduct(BackOfficeIndexViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Products = _productSvc.GetProducts();
                return View("Index", viewModel);
            }

            _productSvc.AddProduct(viewModel.NewProduct);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateProduct(BackOfficeIndexViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Products = _productSvc.GetProducts();
                return View("Index", viewModel);
            }

            _productSvc.UpdateProduct(viewModel.NewProduct);
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
