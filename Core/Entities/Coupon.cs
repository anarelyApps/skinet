using System;

namespace Core.Entities;

public class AppCoupon
{
    public required string CouponId { get; set; }
    public object? Object { get; set; } 
    public long AmountOff { get; set; }
    public long PercentOff { get; set; }
    public string? Duration { get; set; }
    public bool Valid { get; set; }
    public string? Name {get;set;}
        public string? PromotionCode {get;set;}
}