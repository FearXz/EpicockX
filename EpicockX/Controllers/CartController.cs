using EpicockX.Models;
using EpicockX.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

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



            var domain = "https://localhost:7231/";

            var Options = new SessionCreateOptions
            {
                SuccessUrl = domain + "Cart/Success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = domain + "Cart/Fail",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in cart)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)item.ProductPrice * 100,
                        Currency = "eur",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ProductName,
                            Description = item.ProductDescription,
                        }
                    },
                    Quantity = 1
                };
                Options.LineItems.Add(sessionLineItem);
            }
            // creo la sessione di Stripe e la invio al client
            var service = new SessionService();
            Session session = service.Create(Options);

            order.SessionId = session.Id;
            var orderId = _cartSvc.SubmitOrder(order, userId, cart);

            Response.Headers.Add("Location", session.Url);

            return Redirect(session.Url);



            //var lastOrder = _cartSvc.GetResultOrder(orderId, userId);
            //RedirectToAction("Result", lastOrder);
        }

        public IActionResult Result()
        {
            //var order = _cartSvc.
            return View();
        }

        public IActionResult AddToCart(int id, string returnUrl)
        {
            List<Product> cart = _cartSvc.GetCart();
            Product product = _productSvc.GetProductById(id);
            cart.Add(product);
            _cartSvc.SaveCart(cart);

            // Utilizza returnUrl per reindirizzare l'utente, se è fornito e valido
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Altrimenti, reindirizza alla pagina di default se returnUrl non è fornito o non è valido
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int productId, string returnUrl)
        {
            List<Product> cart = _cartSvc.GetCart();
            Product productToRemove = cart.FirstOrDefault(p => p.ProductId == productId);
            if (productToRemove != null)
            {
                cart.Remove(productToRemove);
                _cartSvc.SaveCart(cart);
            }
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return View();
        }
    }
}
