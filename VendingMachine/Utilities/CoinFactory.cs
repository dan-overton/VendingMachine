using System.Collections.Generic;
using System.Linq;
using VendingMachine.Models;

namespace VendingMachine.Utilities
{
    public static class CoinFactory
    {
        public static readonly List<CoinValue> KnownCoins = new List<CoinValue>()
        {
            new CoinValue() {Weight = 5, Diameter = 5, Value = (decimal) 0.05},
            new CoinValue() {Weight = 5, Diameter = 10, Value = (decimal) 0.10},
            new CoinValue() {Weight = 10, Diameter = 20, Value = (decimal) 0.20},
            new CoinValue() {Weight = 20, Diameter = 20, Value = (decimal) 0.50}
        };

        public static Coin CreateCoin(decimal value)
        {
            var coinVal = KnownCoins.FirstOrDefault(x => x.Value == value);

            return coinVal == null ? null : new Coin(coinVal.Weight, coinVal.Diameter);
        }

        public static decimal ValueForCoin(Coin coin)
        {
            var coinVal = KnownCoins.FirstOrDefault(x => x.Weight == coin.Weight && x.Diameter == coin.Diameter);
            return coinVal?.Value ?? 0;
        }
    }
}
