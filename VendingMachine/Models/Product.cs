using System;

namespace VendingMachine.Models
{
    public class Product
    {
        public string Name { get;}
        public decimal Value { get;}

        public Product(string name, decimal value)
        {
            Name = name;
            Value = value;
        }

        public Product(Product product)
        {
            Name = product.Name;
            Value = product.Value;
        }

        protected bool Equals(Product other)
        {
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Product) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0)*397) ^ Value.GetHashCode();
            }
        }
    }
}
