using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GymManagementSystem.Core.Entities;

namespace GymManagementSystem.Core.Services
{
    public class PaymentService
    {
        private readonly List<Payment> _payments = new();

        public List<Payment> GetAllPayments()
        {
            return _payments;
        }

        public List<Payment> GetPaymentsByMember(int memberId)
        {
            return _payments.Where(p => p.MemberId == memberId).ToList();
        }

        public List<Payment> GetPaymentsByDateRange(DateTime start, DateTime end)
        {
            return _payments.Where(p =>
                p.PaymentDate >= start &&
                p.PaymentDate <= end).ToList();
        }

        public bool AddPayment(Payment payment)
        {
            // Tutar geçersizse işlem yapma (PDF UC-04)
            if (payment.Amount <= 0)
                return false;

            payment.Id = _payments.Count > 0 ? _payments.Max(p => p.Id) + 1 : 1;
            payment.PaymentDate = DateTime.Now;
            _payments.Add(payment);
            return true;
        }

        public decimal GetTotalIncomeByMonth(int month, int year)
        {
            return _payments
                .Where(p => p.PaymentDate.Month == month &&
                            p.PaymentDate.Year == year &&
                            p.Status == PaymentStatus.Completed)
                .Sum(p => p.Amount);
        }

        public decimal GetTotalIncomeByYear(int year)
        {
            return _payments
                .Where(p => p.PaymentDate.Year == year &&
                            p.Status == PaymentStatus.Completed)
                .Sum(p => p.Amount);
        }
    }
}