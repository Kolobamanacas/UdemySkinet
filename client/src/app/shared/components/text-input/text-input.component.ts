import { Component, ElementRef, Input, OnInit, Self, ViewChild } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.scss']
})
export class TextInputComponent implements OnInit, ControlValueAccessor {
  @ViewChild('input', { static: true }) input?: ElementRef;
  @Input() type = 'text';
  @Input() label?: string;

  constructor(@Self() public controlDir: NgControl) {
    this.controlDir.valueAccessor = this;
  }

  ngOnInit(): void {
    const control = this.controlDir.control;

    if (!control) {
      return;
    }

    const validators = control.validator ?? [];
    const asyncValidators = control.asyncValidator ?? [];
    control.setValidators(validators);
    control.setAsyncValidators(asyncValidators);
    control.updateValueAndValidity();
  }

  // TODO: Get rid of any!
  onChange(event: any) { }

  // A workaround to pass value from $event.target to onChange() in a strict template mode.
  onChangeKludge(target: EventTarget | null) {
    this.onChange((target as HTMLInputElement).value);
  }

  // TODO: Get rid of any!
  onTouched(event: any) { }

  onTouchedKludge() {
    this.onTouched(undefined);
  }

  // TODO: Get rid of any!
  writeValue(obj: any): void {
    if (!this.input) {
      return;
    }

    /*
      TODO: Consider official docs: Use this API ("nativeElement" property) as the last resort when direct access to DOM is needed. Use templating and data-binding provided by Angular instead. Alternatively you can take a look at Renderer2 which provides API that can safely be used even when direct access to native elements is not supported.

      Relying on direct DOM access creates tight coupling between your application and rendering layers which will make it impossible to separate the two and deploy your application into a web worker.
    */
    this.input.nativeElement.value = obj || '';
  }

  // TODO: Get rid of any!
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  // TODO: Get rid of any!
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
}
