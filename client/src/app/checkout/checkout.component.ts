import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../account/account.service';
import { IAddress } from '../shared/models/address';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent implements OnInit {
  checkoutForm: FormGroup = new FormGroup({});

  constructor(private formBuilder: FormBuilder, private accountService: AccountService) { }

  ngOnInit(): void {
    this.createCheckoutForm();
    this.getAddressFormValues();
  }

  createCheckoutForm(): void {
    this.checkoutForm = this.formBuilder.group({
      addressForm: this.formBuilder.group({
        firstName: [undefined, Validators.required],
        lastName: [undefined, Validators.required],
        street: [undefined, Validators.required],
        city: [undefined, Validators.required],
        state: [undefined, Validators.required],
        zipCode: [undefined, Validators.required]
      }),
      deliveryForm: this.formBuilder.group({
        deliveryMethod: [undefined, Validators.required]
      }),
      payment: this.formBuilder.group({
        cardHolderName: [undefined, Validators.required]
      })
    });
  }

  getAddressFormValues(): void {
    this.accountService.getUserAddress()
      .subscribe((address: IAddress) => {
        if (!address) {
          return;
        }

        this.checkoutForm.get('addressForm')?.patchValue(address);
      }, (error) => {
        console.log(`Something went wrong during getting user address. Here is the error.\n${error}`);
      });
  }
}
