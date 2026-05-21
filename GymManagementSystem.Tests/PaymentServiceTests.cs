using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GymManagementSystem.Core.Entities;
using GymManagementSystem.Core.Services;

namespace GymManagementSystem.Tests
{
    [TestClass]
    public class PaymentServiceTests
    {
        private PaymentService _paymentService = new();

        [TestInitialize]
        public void Setup()
        {
            _paymentService = new PaymentService();
        }

        [TestMethod]
        public void AddPayment_ValidPayment_ReturnsTrue()
        {
            // Arrange
            var payment = new Payment
            {
                MemberId = 1,
                SubscriptionId = 1,
                Amount = 500,
                PaymentMethod = PaymentMethod.Cash
            };

            // Act
            var result = _paymentService.AddPayment(payment);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddPayment_InvalidAmount_ReturnsFalse()
        {
            // Arrange
            var payment = new Payment
            {
                MemberId = 1,
                SubscriptionId = 1,
                Amount = -100, // Geçersiz tutar
                PaymentMethod = PaymentMethod.Cash
            };

            // Act
            var result = _paymentService.AddPayment(payment);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetTotalIncomeByMonth_ReturnsCorrectAmount()
        {
            // Arrange
            _paymentService.AddPayment(new Payment
            {
                MemberId = 1,
                SubscriptionId = 1,
                Amount = 500,
                PaymentMethod = PaymentMethod.Cash,
                Status = PaymentStatus.Completed
            });

            _paymentService.AddPayment(new Payment
            {
                MemberId = 2,
                SubscriptionId = 2,
                Amount = 300,
                PaymentMethod = PaymentMethod.CreditCard,
                Status = PaymentStatus.Completed
            });

            // Act
            var total = _paymentService.GetTotalIncomeByMonth(
                DateTime.Now.Month, DateTime.Now.Year);

            // Assert
            Assert.AreEqual(800, total);
        }

        [TestMethod]
        public void GetPaymentsByMember_ReturnsCorrectPayments()
        {
            // Arrange
            _paymentService.AddPayment(new Payment
            {
                MemberId = 1,
                SubscriptionId = 1,
                Amount = 500,
                PaymentMethod = PaymentMethod.Cash
            });

            _paymentService.AddPayment(new Payment
            {
                MemberId = 2,
                SubscriptionId = 2,
                Amount = 300,
                PaymentMethod = PaymentMethod.Cash
            });

            // Act
            var result = _paymentService.GetPaymentsByMember(1);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(500, result[0].Amount);
        }
    }
}