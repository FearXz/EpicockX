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
            // Utilizza il modello Order per raccogliere i dati del modulo
            [Bind("Name, Surname, Address, City, ZipCode, Country")] Order order
        )
        {
            // Convalida il modulo
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Si è verificato un errore durante la convalida del modulo.";
                return View();
            }
            // Ottieni il carrello dell'utente
            var cart = _cartSvc.GetCart();
            // Ottieni l'ID dell'utente attualmente connesso
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            // Crea una sessione di checkout con Stripe e ottieni l'ID della sessione
            var session = _cartSvc.CreateCheckoutSession(cart);
            // Salva l'ID della sessione nel modello Order
            order.SessionId = session.Id;
            // Invia l'ordine al servizio OrderService per salvare l'ordine nel database
            var orderId = _cartSvc.SubmitOrder(order, userId, cart);
            // Reindirizza l'utente alla pagina di successo con l'ID dell'ordine
            Response.Headers.Add("Location", session.Url);
            // Reindirizza l'utente alla pagina di successo con l'ID dell'ordine
            return Redirect(session.Url);
        }

        public IActionResult AddToCart(int id, string returnUrl, int quantity)
        {
            // Ottieni il carrello dell'utente
            List<Product> cart = _cartSvc.GetCart();
            // Ottieni il prodotto dal database
            Product product = _productSvc.GetProductById(id);
            // Verifica se il prodotto è già presente nel carrello
            var existingProduct = cart.FirstOrDefault(p => p.ProductId == id);
            // Aggiungi il prodotto al carrello o aumenta la quantità se il prodotto è già presente
            if (existingProduct != null)
            {
                existingProduct.ProductQuantity += quantity;
            }
            // Altrimenti, aggiungi il prodotto al carrello
            else   
            {
                product.ProductQuantity = quantity;
                cart.Add(product);
            }
            // Salva il carrello aggiornato
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
            // Ottieni il carrello dell'utente
            List<Product> cart = _cartSvc.GetCart();
            // Cerca il prodotto da rimuovere dal carrello
            Product productToRemove = cart.FirstOrDefault(p => p.ProductId == productId);
            // Rimuovi il prodotto dal carrello o diminuisci la quantità se il prodotto è presente
            if (productToRemove != null)
            {
               productToRemove.ProductQuantity -= quantity;
               if(productToRemove.ProductQuantity <= 0)
               {
                   cart.Remove(productToRemove);
               }
               _cartSvc.SaveCart(cart);
            }
            // Utilizza returnUrl per reindirizzare l'utente, se è fornito e valido
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            // Altrimenti, reindirizza alla pagina di default se returnUrl non è fornito o non è valido
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
