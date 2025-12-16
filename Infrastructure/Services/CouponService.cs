using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Stripe;


namespace Infrastructure.Services;

public class PaymentCouponService(IConfiguration config) : ICouponService
{
    public async Task<AppCoupon?> GetCouponFromPromoCode(string code)
    {
        StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
        
     
       var promotionService = new PromotionCodeService();

        var options = new PromotionCodeListOptions
        {
           Code = code
           
        }; 
    
       var promotionCodes = await promotionService.ListAsync(options);      
        var promotionCode = promotionCodes.FirstOrDefault();
        
        if (promotionCode != null )
        {
             var service = new Stripe.CouponService();
             var coupon = service.Get(promotionCode?.Promotion.CouponId);

            if(coupon==null) return null;

            return new AppCoupon
            {
                Name =  coupon.Name,
                AmountOff = coupon.AmountOff??0,
                PercentOff = (long)(coupon.PercentOff??0),
                CouponId = coupon.Id,
                PromotionCode = promotionCode?.Code
            };
        }

        return null;
        
    }
}