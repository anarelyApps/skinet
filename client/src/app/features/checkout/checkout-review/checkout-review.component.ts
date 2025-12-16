import { Component, inject, Input, input } from '@angular/core';
import { CartService } from '../../../core/services/cart.service';
import { CurrencyPipe } from '@angular/common';
import { ConfirmationToken } from '@stripe/stripe-js';
import { AddressPipe } from '../../../shared/pipes/address-pipe';
import { paymentCardPipe } from '../../../shared/pipes/payment-card-pipe';

@Component({
  selector: 'app-checkout-review',
  standalone: true,
  imports: [CurrencyPipe,AddressPipe,paymentCardPipe],
  templateUrl: './checkout-review.component.html',
  styleUrl: './checkout-review.component.scss'
})
export class CheckoutReviewComponent {
cartService = inject(CartService);
@Input() confirmationToken?:ConfirmationToken;

}
