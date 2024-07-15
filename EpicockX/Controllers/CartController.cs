using System.Security.Claims;
using EpicockX.Models;
using EpicockX.Services;
using Microsoft.AspNetCore.Mvc;

namespace EpicockX.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductService _productSvc;
        private readonly CartService _cartSvc;

        public CartController(ProductService produtctSvc, CartService cartSvc)
        {
            _productSvc = produtctSvc;
            _cartSvc = cartSvc;
        }

        public IActionResult Index()
        {
            var cart = _cartSvc.GetCart();
            return View(cart);
        }

        public IActionResult Checkout()
        {
            ViewBag.Cart = _cartSvc.GetCart();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(
            [Bind("Name, Surname, Address, City, ZipCode, Country")] Order order
        )
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Si è verificato un errore durante la convalida del modulo.";
                return View();
            }
            var cart = _cartSvc.GetCart();
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var session = _cartSvc.CreateCheckoutSession(cart);
            order.SessionId = session.Id;
            var orderId = _cartSvc.SubmitOrder(order, userId, cart);

            Response.Headers.Add("Location", session.Url);

            return Redirect(session.Url);
        }

        public IActionResult AddToCart(int id, string returnUrl, int quantity)
        {
            List<Product> cart = _cartSvc.GetCart();
            Product product = _productSvc.GetProductById(id);
            
            var existingProduct = cart.FirstOrDefault(p => p.ProductId == id);
            if (existingProduct != null)
            {
                existingProduct.ProductQuantity += quantity;
            }
            else   
            {
                product.ProductQuantity = quantity;
                cart.Add(product);
            }
            _cartSvc.SaveCart(cart);

            // Utilizza returnUrl per reindirizzare l'utente, se è fornito e valido
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Altrimenti, reindirizza alla pagina di default se returnUrl non è fornito o non è valido
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int productId, string returnUrl, int quantity)
        {
            List<Product> cart = _cartSvc.GetCart();
            Product productToRemove = cart.FirstOrDefault(p => p.ProductId == productId);
            if (productToRemove != null)
            {
               productToRemove.ProductQuantity -= quantity;
               if(productToRemove.ProductQuantity <= 0)
               {
                   cart.Remove(productToRemove);
               }
               _cartSvc.SaveCart(cart);
            }
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return View();
        }

        public IActionResult Success(string session_id)
        {
            var result = _cartSvc.GetResultOrder(session_id);
            _cartSvc.ClearCart();
            return View(result);
        }
    }
}
