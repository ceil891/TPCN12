using SNKRS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace SNKRS.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext db;
        public CartController()
        {
            db = new ApplicationDbContext();
        }
        public List<Cart> Get()
        {
            var cart = Session["Cart"] as List<Cart>;
            if (cart == null)
            {
                cart = new List<Cart>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        public void Add(int Id, int Quantity)
        {
            var cart = Get();
            Cart item = cart.FirstOrDefault(c => c.ProductSizeId == Id);
            if (item == null)
            {
                item = new Cart(Id, Quantity);
                cart.Add(item);
            }
            else
            {
                item.Quantity += Quantity;
            }
        }

        public int GetCartAmount()
        {
            var cart = Get();
            return cart.Count;
        }

        public decimal Amount()
        {
            decimal amount = 0;
            var cart = Get();
            if (cart != null)
            {
                amount = cart.Sum(c => c.Amount);
            }
            return amount;
        }

        public int IncreaseQuantity(int Id)
        {
            var cart = Get();
            var item = cart.First(x => x.ProductSizeId == Id);
            if (item != null)
            {
                item.Quantity = item.Quantity+1;
            }
            return item.Quantity;
        }

        public int DecreaseQuantity(int Id)
        {
            var cart = Get();
            var item = cart.First(x => x.ProductSizeId == Id);
            if (item != null && item.Quantity > 0)
            {
                item.Quantity = item.Quantity - 1;
            }
            return item.Quantity;
        }

        public ActionResult Index()
        {
            var cart = Get();
            ViewBag.Amount = Amount();
            return View(cart);
        }

        public void Delete(int Id)
        {
            var cart = Get();
            var item = cart.First(x => x.ProductSizeId == Id);
            if (item != null)
            {
                cart.Remove(item);
            }
        }

        public ActionResult Checkout()
        {
            var cart = Get();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index");
            }
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.FirstOrDefault(x => x.Id == userId);
                ViewBag.User = user;
            }
            ViewBag.Cart = cart;
            ViewBag.Amount = Amount();
            return View();
        }

        [HttpPost]
        public ActionResult Checkout(Order order)
        {
            var cart = Get();
            if (!ModelState.IsValid)
            {
                if (cart.Count == 0)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.Cart = cart;
                ViewBag.Amount = Amount();
                return View(order);
            }
            order.Created_At = DateTime.Now;
            order.Amount = Amount();
            order.StatusId = 1;
            if (User.Identity.IsAuthenticated)
            {
                order.CustomerId = User.Identity.GetUserId();
            }
            db.Orders.Add(order);
            cart = Get();
            foreach (var item in cart)
            {
                var orderDetail = new OrderDetail()
                {
                    OrderId = order.Id,
                    ProductSizeId = item.ProductSizeId,
                    Quantity = item.Quantity,
                };
                var productSize = db.ProductSizes.First(x => x.Id == item.ProductSizeId);
                productSize.Quantity -= item.Quantity;
                db.OrderDetails.Add(orderDetail);
            }
            db.SaveChanges();
            return RedirectToAction("Success", new { order.Id });
        }

        public ActionResult Success(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }
    }
}