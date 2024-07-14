using EpicockX.Services;
using Microsoft.AspNetCore.Mvc;

namespace EpicockX.Component.Cart
{
    public class CartViewComponent : ViewComponent
    {
        private readonly CartService _cartService;

        public CartViewComponent(CartService cartService)
        {
            _cartService = cartService;
        }

        public IViewComponentResult Invoke()
        {
            var cart = _cartService.GetCart();
            return View(cart); // Passa il carrello alla vista del View Component
        }
    }
}
