namespace Pizza_Shop.Models
{
    public class Topping
    {
        public int ToppingID { get; set; }
        public string ToppingName { get; set; }
        public virtual ICollection<PizzaTopping>? PizzaToppings { get; set; }
    }
}
