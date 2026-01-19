import { Directive } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator } from '@angular/forms';

/**
 * Alphanumeric only validator directive (for Model Code: exactly 10 chars, alphanumeric)
 * Usage: <input appAlphanumeric minlength="10" maxlength="10">
 */
@Directive({
  selector: '[appAlphanumeric]',
  providers: [{ provide: NG_VALIDATORS, useExisting: AlphanumericDirective, multi: true }],
  standalone: true
})
export class AlphanumericDirective implements Validator {
  validate(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    
    const value = control.value.toString();
    const alphanumericRegex = /^[a-zA-Z0-9]+$/;
    
    if (!alphanumericRegex.test(value)) {
      return { alphanumeric: true };
    }
    
    // Validate exactly 10 characters for model code
    if (value.length !== 10) {
      return { modelCodeLength: { requiredLength: 10, actualLength: value.length } };
    }
    
    return null;
  }
}

/**
 * Email validator directive
 * Usage: <input appEmailValidator>
 */
@Directive({
  selector: '[appEmailValidator]',
  providers: [{ provide: NG_VALIDATORS, useExisting: EmailValidatorDirective, multi: true }],
  standalone: true
})
export class EmailValidatorDirective implements Validator {
  validate(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(control.value) ? null : { invalidEmail: true };
  }
}

/**
 * Phone number validator directive
 * Usage: <input appPhoneValidator>
 */
@Directive({
  selector: '[appPhoneValidator]',
  providers: [{ provide: NG_VALIDATORS, useExisting: PhoneValidatorDirective, multi: true }],
  standalone: true
})
export class PhoneValidatorDirective implements Validator {
  validate(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    
    const phoneRegex = /^[\d\s\-\+\(\)]{10,}$/;
    return phoneRegex.test(control.value) ? null : { invalidPhone: true };
  }
}
