namespace VendingMachine.Models
{
    public class Coin
    {
        public int Weight { get; }
        public int Diameter { get;}

        public Coin(int weight, int diameter)
        {
            Weight = weight;
            Diameter = diameter;
        }

        protected bool Equals(Coin other)
        {
            return Weight == other.Weight && Diameter == other.Diameter;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Coin) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Weight*397) ^ Diameter;
            }
        }
    }
}
