import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatCard } from '@angular/material/card';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { AccountService } from '../../../core/services/account.service';
import { ActivatedRoute, Router } from '@angular/router';
import { MatAnchor } from "@angular/material/button";

@Component({
  selector: 'app-login',
  standalone:true,
  imports: [
    ReactiveFormsModule,
    MatCard,
    MatFormField,
    MatInput,
    MatLabel,
    MatAnchor
],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
private fb=inject(FormBuilder);
private accountService =inject(AccountService);
private router =inject(Router);
private activedRoute=inject(ActivatedRoute);
returnUrl='/shop';

constructor() {
  const url = this.activedRoute.snapshot.queryParams['returnUrl'];
  if (url) this.returnUrl = url;
}

loginForm = this.fb.group({
  email:[''],
  password:['']
})

onSubmit(){
  this.accountService.login(this.loginForm.value).subscribe({
    next:() =>{
      this.accountService.getUserInfo().subscribe();
      this.router.navigateByUrl(this.returnUrl);
    }
  });
}

}
