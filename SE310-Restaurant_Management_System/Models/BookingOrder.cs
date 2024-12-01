using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SE310_Restaurant_Management_System.Models;

public partial class BookingOrder
{
    public int BookingOrderId { get; set; }
    [Required(ErrorMessage = "Tên khách hàng không được để trống.")]
    public string CustomerName { get; set; } = null!;
    [Required(ErrorMessage = "Số điện thoại không được để trống.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    public string CustomerPhone { get; set; } = null!;
    [Required(ErrorMessage = "Thời gian đặt bàn không được để trống.")]
    [DataType(DataType.DateTime, ErrorMessage = "Vui lòng nhập thời gian hợp lệ.")]
    [FutureDate(ErrorMessage = "Thời gian đặt bàn phải lớn hơn hoặc bằng ngày hiện tại.")]
    public DateTime BookingTime { get; set; }
    [Required(ErrorMessage = "Số người không được để trống.")]
    [Range(1, int.MaxValue, ErrorMessage = "Số người phải lớn hơn 0.")]
    public int NumberOfPeople { get; set; }
    [Required(ErrorMessage = "Tiền cọc không được để trống.")]
    [Range(0, double.MaxValue, ErrorMessage = "Tiền cọc phải là số dương.")]
    public decimal? DepositAmount { get; set; }

    public int? TableId { get; set; }

    public virtual RestaurantTable? Table { get; set; }
}
public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        DateTime dateTime = (DateTime)value;
        return dateTime >= DateTime.Now;
    }
}