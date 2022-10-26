namespace Pizza_Shop.Models
{
    public class PizzaTopping
    {
        public int PizzaToppingID { get; set; }
        public int PizzaID { get; set; }
        public int ToppingID { get; set; }
        public virtual Topping? Topping { get; set; }
    }
}
