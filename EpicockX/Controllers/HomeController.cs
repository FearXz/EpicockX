using EpicockX.Models;
using EpicockX.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EpicockX.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductService _productSvc;
        private readonly ImageService _imageSvc;
        private readonly OrderService _orderSvc;
        private readonly CartService _cartSvc;

        public HomeController(
            ProductService productService,
            ImageService imageService,
            OrderService orderService,
            CartService cartSVC
        )
        {
            _productSvc = productService;
            _imageSvc = imageService;
            _orderSvc = orderService;
            _cartSvc = cartSVC;
        }

        // Visualizza la pagina di amministrazione con la lista di tutti i brand
        public IActionResult Admin()
        {
            return View();
        }

        // Creazione pagina Home
        public IActionResult Index()
        {
            var products = _productSvc.GetProducts();
            var images = _imageSvc.GetImages();
            ViewBag.Images = images;
            return View(products);
        }

        // Creazione pagina Dettaglio Prodotto
        public IActionResult Details(int id)
        {
            var product = _productSvc.GetProductById(id);
            var images = _imageSvc.GetImages().Where(img => img.ProductId == id).ToList();
            ViewBag.Images = images;
            return View(product);
        }

        // Creazione pagina Carrello
        public IActionResult Cart()
        {
            var cart = _cartSvc.GetCart();
            return View(cart);
        }

        // Creazione pagina Informazioni
        public IActionResult AboutUs()
        {
            return View();
        }

        // Creazione pagina FAQ
        public IActionResult FAQ()
        {
            return View();
        }

        // Modifica prodotto da pagina admin
        public IActionResult UpdateProduct(int id)
        {
            return View();
        }

        public IActionResult Catalog(string category = "all")
        {
            var products = _productSvc.GetProducts();

            if (category != "all")
            {
                products = products.Where(p => p.ProductCategory == category).ToList();
            }

            var images = _imageSvc.GetImages();
            ViewBag.Images = images;

            return View(products);
        }

        // Creazione pagina Errore404
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Page404()
        {
            return View(
                new Page404Dto { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
            );
        }
    }
}
