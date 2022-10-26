namespace Pizza_Shop.Models
{
    public class Pizza
    {
        public int PizzaID { get; set; }
        public string PizzaName { get; set; }
        public virtual ICollection<PizzaTopping>? PizzaToppings { get; set; }
    }
}
