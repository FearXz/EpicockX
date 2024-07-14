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
        private readonly CartService _cartSvc;

        public HomeController(
            ProductService productService,
            ImageService imageService,
            CartService cartSVC
        )
        {
            _productSvc = productService;
            _imageSvc = imageService;
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
            return View(products);
        }

        // Creazione pagina Dettaglio Prodotto
        public IActionResult Details(int id)
        {
            var product = _productSvc.GetProductById(id); 
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
