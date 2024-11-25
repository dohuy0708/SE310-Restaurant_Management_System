namespace SE310_Restaurant_Management_System.Models
{
    public class StatisticViewModel
    {
        public List<RevenueData> RevenueByDay { get; set; }
        public List<RevenueData> RevenueByMonth { get; set; }
        public List<RevenueData> RevenueByYear { get; set; }
        public string ChartType { get; set; }  // "day", "month", "year"

        // Current period revenues
        public decimal CurrentDayRevenue { get; set; }
        public decimal CurrentMonthRevenue { get; set; }
        public decimal CurrentYearRevenue { get; set; }
    }

    public class RevenueData
    {
        public string Period { get; set; } // Ngày, Tháng, Năm
        public decimal TotalRevenue { get; set; } // Tổng doanh thu
    }

}
