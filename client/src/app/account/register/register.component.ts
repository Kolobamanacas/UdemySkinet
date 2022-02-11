import { Component, OnInit } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { of, timer } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  // TODO: How to remove code duplicate?
  registerForm: FormGroup = this.formBuilder.group({
    displayName: [undefined, [Validators.required]],
    email: [undefined, [Validators.required, Validators.pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')]],
    password: [undefined, [Validators.required]]
  });
  errors: string[] = [];

  constructor(private formBuilder: FormBuilder, private accountService: AccountService, private router: Router) { }

  ngOnInit(): void {
    this.createRegisterForm();
  }

  createRegisterForm() {
    // TODO: How to remove code duplicate?
    this.registerForm = this.formBuilder.group({
      displayName: [undefined, [Validators.required]],
      email: [undefined,
        [Validators.required, Validators.pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')],
        [this.validateIsEmailInUse()]],
      password: [undefined, [Validators.required]]
    });
  }

  onSubmit() {
    this.accountService.register(this.registerForm.value)
      .subscribe(() => {
        this.router.navigateByUrl('/shop');
      }, (error) => {
        console.log(`Something went wrong during registration. Here is an error:\n${error}`);
        this.errors = [...error.errors];
      });
  }

  validateIsEmailInUse(): AsyncValidatorFn {
    return (control: AbstractControl) => {
      return timer(500).pipe(switchMap(() => {
        if (!control.value) {
          return of(null);
        }

        return this.accountService.doesEmailExist(control.value).pipe(map((result) => {
          return result ? { isEmailInUse: true } : null;
        }));
      }));
    }
  }
}
