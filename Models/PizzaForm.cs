namespace Pizza_Shop.Models
{
    public class PizzaForm
    {
        public int ID { get; set; }
        public string PizzaName { get; set; }
        public ICollection<Topping>? AvailableToppings {get; set;}
        public List<int> ToppingsSelected { get; set; }
    }
}
