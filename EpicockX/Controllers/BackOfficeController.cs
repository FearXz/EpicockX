﻿using EpicockX.Models;
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
        public async Task<IActionResult> AddProduct(BackOfficeIndexViewModel viewModel, [FromForm] List<IFormFile> productImages)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Products = _productSvc.GetProducts();
                return View("Index", viewModel);
            }

            _productSvc.AddProduct(viewModel.NewProduct);

            if (productImages != null && productImages.Count > 0)
            {
                foreach (var imageFile in productImages)
                {
                    if (imageFile.Length > 0)
                    {
                        // Crea un oggetto ProductImage per ogni immagine caricata
                        var productImage = new ProductImage
                        {
                            ProductId = viewModel.NewProduct.ProductId,
                            ImageFile = imageFile // Imposta il campo ImageFile con l'oggetto IFormFile corrente
                        };
                        // Utilizza ImageService per caricare l'immagine e salvare l'URL nel database
                        _imageSvc.AddImage(productImage);
                    }
                }
            }

            return RedirectToAction("Index");
        }



        [HttpPost]
        public IActionResult UpdateProduct(BackOfficeIndexViewModel viewModel, [FromForm] List<IFormFile> productImages)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Products = _productSvc.GetProducts();
                return View("Index", viewModel);
            }

            _productSvc.UpdateProduct(viewModel.NewProduct);
            _imageSvc.UpdateImage(viewModel.NewProduct.ProductId, productImages);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            // Ottieni tutte le immagini associate al prodotto
            var images = _imageSvc.GetImages().Where(img => img.ProductId == id).ToList();

            // Elimina tutte le immagini trovate
            foreach (var image in images)
            {
                _imageSvc.DeleteImage(image.ProductImageId);
            }

            // Elimina il prodotto
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

        [HttpGet]
        public IActionResult GetProductImages(int productId)
        {
            var images = _imageSvc.GetImages().Where(img => img.ProductId == productId).ToList();
            return Json(images);
        }

        [HttpPost]
        public IActionResult DeleteProductImage(int id)
        {
            _imageSvc.DeleteImage(id);
            return RedirectToAction("Index");
        }


    }
}
