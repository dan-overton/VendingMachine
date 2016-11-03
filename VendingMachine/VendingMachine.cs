using System;
using System.Collections.Generic;
using System.Linq;
using VendingMachine.Models;
using VendingMachine.Properties;
using VendingMachine.Utilities;

namespace VendingMachine
{
    public class VendingMachine
    {
        private string _display;
        private readonly List<Product> _productsAwaitingCollection;
        private readonly List<Coin> _coinBalance;
        private readonly List<Coin> _changeCoins;

        public VendingMachine(IList<Product> products)
        {
            Products = products ?? new List<Product>();
            _display = Resources.InsertCoin;
            _productsAwaitingCollection = new List<Product>();
            _coinBalance = new List<Coin>();
            _changeCoins = new List<Coin>();
        }

        public IList<Product> Products { get; }

        private decimal Balance()
        {
            return _coinBalance.Sum(x => CoinFactory.ValueForCoin(x));
        }

        public string CheckDisplay()
        {
            var oldDisplay = _display;

            if (_display == Resources.Thanks)
            {
                _display = Resources.InsertCoin;
            }
            else
            {
                _display = _coinBalance.Any() ? $"{Resources.CurrencySign}{Balance():F}" : Resources.InsertCoin;
            }

            return oldDisplay;
        }

        public void PressProductButton(int i)
        {
            if(i < 0 || i > Products.Count-1)
                throw new ArgumentOutOfRangeException(nameof(i));
            
            if (Balance() >= Products[i].Value)
            {
                _productsAwaitingCollection.Add(new Product(Products[i]));
                _display = Resources.Thanks;

                var changeToGive = Balance();
                changeToGive -= Products[i].Value;
                _coinBalance.Clear();

                while (changeToGive > 0)
                {
                    //For now assume we have infinite coins to give change with
                    bool coinAdded = false;
                    foreach (var coin in CoinFactory.KnownCoins.OrderByDescending(x => x.Value))
                    {
                        if (changeToGive >= coin.Value)
                        {
                            _changeCoins.Add(new Coin(coin.Weight, coin.Diameter));
                            changeToGive -= coin.Value;
                            coinAdded = true;
                            break;
                        }
                    }

                    if (!coinAdded) //no more change can be given
                        break;
                }
            }
            else
            {
                _display = $"{Resources.Price} {Resources.CurrencySign}{Products[i].Value:F}";
            }
        }

        public void PressCoinReturn()
        {
            foreach (var coin in _coinBalance)
            {
                _changeCoins.Add(coin);
            }
            _coinBalance.Clear();
        }

        public IList<Product> CollectDispensedProducts()
        {
            var productsToReturn = new List<Product>();

            foreach (var product in _productsAwaitingCollection)
            {
                productsToReturn.Add(product);
            }

            _productsAwaitingCollection.Clear();
            return productsToReturn;
        }
            
        public void InsertCoin(Coin coin)
        {
            var coinValue = CoinFactory.ValueForCoin(coin);
            if (coinValue != 0)
            {
                _coinBalance.Add(new Coin(coin.Weight, coin.Diameter));
            }
            
            _display = $"{Resources.CurrencySign}{Balance():F}";
        }

        public IList<Coin> CollectChange()
        {
            var retval = new List<Coin>();
            foreach (var coin in _changeCoins)
            {
                retval.Add(new Coin(coin.Weight, coin.Diameter));
            }

            _changeCoins.Clear();
            return retval;
        }
    }
}