using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using VendingMachine.Models;
using VendingMachine.Properties;
using VendingMachine.Utilities;

namespace VendingMachine.Tests
{
    public class VendingMachineTests
    {
        private List<Product> _expectedProducts;
        private VendingMachine _machine;

        [SetUp]
        public void Setup()
        {
            _expectedProducts = new List<Product>()
            {
                new Product("Cola", 1),
                new Product("Chips", new decimal(0.50)),
                new Product("Candy", new decimal(0.65))
            };

            _machine = new VendingMachine(_expectedProducts);
        }

        //EG1
        [Test]
        public void Given_AVendingMachine_WhenIReadTheProductList_ItReturnsExpectedProducts()
        {
            _expectedProducts.SequenceEqual(_machine.Products).Should().BeTrue();
        }

        [Test]
        public void Given_EnoughCoinsInserted_AndProductButtonPressed_ProductDispensedAndThankyouShown()
        {
            MakeProductDispense();
            _machine.CheckDisplay().ShouldBeEquivalentTo(Resources.Thanks);
            _machine.CollectDispensedProducts().Contains(_machine.Products[1]).Should().BeTrue();
        }

        [Test]
        public void Given_ProductDispensed_And_DisplayChecked_WhenDisplayCheckedAgain_InsertCoinShownAndBalanceZero()
        {
            MakeProductDispense();

            _machine.CheckDisplay(); //thank you
            _machine.CheckDisplay().ShouldBeEquivalentTo(Resources.InsertCoin);

            //check balance was reset to zero
            _machine.InsertCoin(CoinFactory.CreateCoin((decimal)0.20));
            _machine.CheckDisplay().ShouldBeEquivalentTo($"{Resources.CurrencySign}0.20");
        }

        [Test]
        public void Given_NotEnoughCoinsInserted_AndProductButtonPressed_DisplayIsProductPrice()
        {
            _machine.InsertCoin(CoinFactory.CreateCoin((decimal)0.10));

            _machine.PressProductButton(1);

            _machine.CheckDisplay().ShouldBeEquivalentTo($"{Resources.Price} {Resources.CurrencySign}{_expectedProducts[1].Value:F}");
        }

        [Test]
        public void Given_NotEnoughCoinsInserted_AndProductButtonPressed__WhenDisplayCheckedAgain_DisplayIsBalance()
        {
            _machine.InsertCoin(CoinFactory.CreateCoin((decimal)0.20));
            _machine.PressProductButton(1);

            _machine.CheckDisplay(); //Price

            _machine.CheckDisplay().ShouldBeEquivalentTo($"{Resources.CurrencySign}0.20");
        }

        [Test]
        public void Given_NoCoinsInserted_AndProductPriceShown__WhenDisplayCheckedAgain_DisplayIsInsertCoin()
        {
            _machine.PressProductButton(1);

            _machine.CheckDisplay(); //Price

            _machine.CheckDisplay().ShouldBeEquivalentTo(Resources.InsertCoin);
        }

        private void MakeProductDispense()
        {
            _machine.InsertCoin(CoinFactory.CreateCoin((decimal)0.50));
            _machine.InsertCoin(CoinFactory.CreateCoin((decimal)0.50));

            _machine.PressProductButton(1);
        }

        //EG2
        [Test]
        public void Given_ValueInsertedOverProductPrice_WhenProductDispensed_CorrectChangePlacedInCoinReturn()
        {
            _machine.InsertCoin(CoinFactory.CreateCoin((decimal)0.50));
            _machine.InsertCoin(CoinFactory.CreateCoin((decimal)0.50));
            _machine.InsertCoin(CoinFactory.CreateCoin((decimal)0.50));

            _machine.PressProductButton(2);

            var change = _machine.CollectChange();
            
            var valueReceived = change.Sum(x => CoinFactory.ValueForCoin(x));
            
            valueReceived.ShouldBeEquivalentTo((decimal) 1.50 - _machine.Products[2].Value);
        }

        //EG3
        [Test]
        public void Given_CoinsInserted_WhenCoinReturnPressed_CoinsAreReturned()
        {
            _machine.InsertCoin(CoinFactory.CreateCoin((decimal)0.50));
            _machine.InsertCoin(CoinFactory.CreateCoin((decimal)0.50));
            _machine.InsertCoin(CoinFactory.CreateCoin((decimal)0.50));

            _machine.PressCoinReturn();

            var change = _machine.CollectChange();

            var valueReceived = change.Sum(x => CoinFactory.ValueForCoin(x));
            
            valueReceived.ShouldBeEquivalentTo((decimal)1.50);
        }

        [Test]
        public void Given_NoCoinsInserted_WhenCoinReturnPressed_NoCoinsAreReturned()
        {
            _machine.PressCoinReturn();

            var change = _machine.CollectChange();

            change.Count.Should().Be(0);
        }
    }
}