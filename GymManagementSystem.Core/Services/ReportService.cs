using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GymManagementSystem.Core.Entities;

namespace GymManagementSystem.Core.Services
{
    public class ReportService
    {
        private readonly PaymentService _paymentService;
        private readonly MaintenanceService _maintenanceService;

        public ReportService(PaymentService paymentService,
                             MaintenanceService maintenanceService)
        {
            _paymentService = paymentService;
            _maintenanceService = maintenanceService;
        }

        // Aylık bütçe raporu (PDF FR-10)
        public BudgetReport GetMonthlyReport(int month, int year)
        {
            var payments = _paymentService.GetPaymentsByDateRange(
                new DateTime(year, month, 1),
                new DateTime(year, month, DateTime.DaysInMonth(year, month)));

            var totalIncome = _paymentService.GetTotalIncomeByMonth(month, year);
            var totalExpense = _maintenanceService
                .GetTotalMaintenanceCostByMonth(month, year);

            return new BudgetReport
            {
                Month = month,
                Year = year,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Payments = payments
            };
        }

        // Yıllık bütçe raporu (PDF FR-10)
        public List<BudgetReport> GetYearlyReport(int year)
        {
            var reports = new List<BudgetReport>();

            for (int month = 1; month <= 12; month++)
            {
                reports.Add(GetMonthlyReport(month, year));
            }

            return reports;
        }

        // Yıllık toplam gelir
        public decimal GetYearlyTotalIncome(int year)
        {
            return _paymentService.GetTotalIncomeByYear(year);
        }

        // Yıllık toplam gider
        public decimal GetYearlyTotalExpense(int year)
        {
            decimal total = 0;
            for (int month = 1; month <= 12; month++)
            {
                total += _maintenanceService
                    .GetTotalMaintenanceCostByMonth(month, year);
            }
            return total;
        }

        // Yıllık net bakiye
        public decimal GetYearlyNetBalance(int year)
        {
            return GetYearlyTotalIncome(year) - GetYearlyTotalExpense(year);
        }
    }
}
