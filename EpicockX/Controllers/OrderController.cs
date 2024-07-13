using EpicockX.Services;
using Microsoft.AspNetCore.Mvc;

namespace EpicockX.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService _orderSvc;

        public OrderController(OrderService service)
        {
            _orderSvc = service;
        }

        public IActionResult Index()
        {
            var orders = _orderSvc.GetOrders();
            return View("Home/Admin", orders);
        }

        public IActionResult Details(int id)
        {
            var order = _orderSvc.GetOrderById(id);
            return View("Home/Admin", order);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            var order = _orderSvc.GetOrderById(id);
            return View("Home/Admin", order);
        }

        public IActionResult Delete(int id)
        {
            var order = _orderSvc.GetOrderById(id);
            return View("Home/Admin", order);
        }
    }
}
