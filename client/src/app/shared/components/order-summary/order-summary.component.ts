import { Component, inject,OnInit } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { RouterLink } from '@angular/router';
import { CartService } from '../../../core/services/cart.service';
import { CurrencyPipe, Location } from '@angular/common';
import { MatIcon } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-order-summary',
  standalone:true,
  imports: [MatButton,RouterLink,MatFormField,MatLabel,MatInput,MatIcon,CurrencyPipe,FormsModule],
  templateUrl: './order-summary.component.html',
  styleUrl: './order-summary.component.scss'
})
export class OrderSummaryComponent implements OnInit {
 cartService = inject(CartService);
 location = inject(Location);
 code="";

   applyCouponCode(){
      console.log("apply ",this.code);
      this.cartService.applyDiscount(this.code);   
   }

   removeCouponCode(){
    this.code="";
      this.cartService.removeCoupon();
   }

   ngOnInit(): void {
     
   }   
}
