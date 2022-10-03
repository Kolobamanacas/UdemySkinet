export interface IChangeEvent {
  elementType: 'cardNumber' | 'cardExpiry' | 'cardCvc';
  complete: boolean;
  error?: {
    message?: string;
  }
}
