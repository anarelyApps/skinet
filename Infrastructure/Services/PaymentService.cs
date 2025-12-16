using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class PaymentService(IConfiguration config, ICartService cartService, 
    IUnitOfWork unit) : IPaymentService
{

private async Task<long?> GetShippingPriceAsync(ShoppingCart cart)
    {

        if (cart.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync((int)cart.DeliveryMethodId);

            if (deliveryMethod == null) return null;
            
            return (long)deliveryMethod.Price*100;
        }
       
       return null;
    }
    
    private async Task ValidateCartItemsInCartAsync(ShoppingCart cart)
    {        
          foreach (var item in cart.Items)
        {
            var productItem = await unit.Repository<Core.Entities.Product>().GetByIdAsync(item.ProductId);

            if (productItem == null) continue;

            if (item.Price != productItem.Price)
            {
                item.Price = productItem.Price;
            }
        }
        
    }

    private long CalculateSubtotal(ShoppingCart cart)
    {       
        if (cart.Items.Count==0) return 0;
        return (long) cart.Items.Sum(i => i.Quantity * (i.Price * 100));        
    }
    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {
        StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];

        var cart = await cartService.GetCartAsync(cartId)?? throw new Exception("Cart unavaliable");

        if (cart == null) return null;

        var shippingPrice = await GetShippingPriceAsync(cart)?? throw new Exception("No shipping price");

        
      //  if (cart.DeliveryMethodId.HasValue)
     //   {
     //       var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync((int)cart.DeliveryMethodId);

     //       if (deliveryMethod == null) return null;
            
     //       shippingPrice = deliveryMethod.Price;
     //   }

     //   foreach (var item in cart.Items)
     //   {
     //       var productItem = await unit.Repository<Core.Entities.Product>().GetByIdAsync(item.ProductId);

      //      if (productItem == null) return null;

      //      if (item.Price != productItem.Price)
       //     {
        //        item.Price = productItem.Price;
       //     }
      //  }
       var subtotal = CalculateSubtotal(cart);
     
     //  var service = new PaymentIntentService();

      // PaymentIntent? intent = null;

     //   if (string.IsNullOrEmpty(cart.PaymentIntentId))
     //  {
     //       var options = new PaymentIntentCreateOptions
     //       {
      //          Amount = (long)cart.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)shippingPrice * 100 ,
      //          Currency = "usd",
     //           PaymentMethodTypes = ["card"]
      //      }
      //             intent = await service.CreateAsync(options);
       //     cart.PaymentIntentId = intent.Id;
      //      cart.ClientSecret = intent.ClientSecret;
     //   }
      //  else
      //  {
      //      var options = new PaymentIntentUpdateOptions
      //      {
       //         Amount = (long)cart.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)shippingPrice * 100 ,
       //     };
       //     intent = await service.UpdateAsync(cart.PaymentIntentId, options);
        //}

        if(cart.couponId!=null)
        {
           subtotal = await ApplyDiscountAsync(cart,subtotal);
        }

        var total = shippingPrice +  subtotal;

        await CreateOrUpdatePaymentIntentAsync(cart, total);

        await cartService.SetCartAsync(cart);

        return cart;
    }

    private async Task<long> ApplyDiscountAsync(ShoppingCart cart,long amount)
    {  
        var service = new PromotionCodeService();
       Stripe.PromotionCode? promotion = await service.GetAsync(cart.couponId);    
       Stripe.Coupon? coupon = promotion?.Promotion.Coupon;  
               
        var discount = amount * (coupon?.PercentOff/100)??0;
        amount -= (long) discount;
              
        return amount;
    }

    private async Task CreateOrUpdatePaymentIntentAsync(ShoppingCart cart,decimal total)
    {
        var service = new PaymentIntentService();

        PaymentIntent? intent = null;

        if (string.IsNullOrEmpty(cart.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)total ,
                Currency = "usd",
                PaymentMethodTypes = ["card"]
            };
            intent = await service.CreateAsync(options);
            cart.PaymentIntentId = intent.Id;
            cart.ClientSecret = intent.ClientSecret;
        }
        else
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long)total ,
            };
            intent = await service.UpdateAsync(cart.PaymentIntentId, options);
        }
    }
}
