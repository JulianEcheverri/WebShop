using System.Web.Mvc;
using WebShop.Models;

namespace WebShop.Controllers
{
    public class ProductController : Controller
    {
        public ActionResult Products() => View(new ProductViewModel());

        public ActionResult CreateProduct() => PartialView("_CreateProduct");

        public ActionResult EditProduct(ProductViewModel product)
        {
            ModelState.Clear();
            product.GetProduct();
            return PartialView("_EditProduct", product);
        }

        public JsonResult UpdateProduct(ProductViewModel product) => Json(product.CreateOrEditProduct(), JsonRequestBehavior.AllowGet);
    }
}