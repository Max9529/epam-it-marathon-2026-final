import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { AccountRecoveryService } from '../../services/account-recovery.service';
import { AuthenticationService } from '../../services/authentication.service';

@Component({
  selector: 'app-account-recovery', standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './account-recovery.component.html',
  styleUrl: './account-recovery.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AccountRecoveryComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly builder = inject(FormBuilder);
  private readonly service = inject(AccountRecoveryService);
  private readonly auth = inject(AuthenticationService);
  readonly verifying = this.route.snapshot.queryParamMap.get('verify') === 'true';
  readonly busy = signal(false);
  readonly codeSent = signal(false);
  readonly complete = signal(false);
  readonly message = signal('');
  readonly error = signal('');
  readonly hasSession = !!this.auth.getTokens()?.accessToken;
  readonly form = this.builder.nonNullable.group({
    email: [this.auth.getUser()?.email ?? '', [Validators.required, Validators.email, Validators.maxLength(254)]],
    code: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]],
    password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(16),
      Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$/)]],
    confirmation: [''],
  });

  sendCode(): void {
    if (this.busy() || (!this.verifying && this.form.controls.email.invalid)) return;
    const token = this.auth.getTokens()?.accessToken;
    if (this.verifying && !token) { this.error.set('Please sign in first.'); return; }
    this.busy.set(true); this.error.set(''); this.message.set('');
    const request = this.verifying ? this.service.requestVerification(token!) : this.service.requestRecovery(this.form.controls.email.value.trim());
    request.pipe(finalize(() => this.busy.set(false))).subscribe({
      next: response => { this.message.set(response.message); this.codeSent.set(true); },
      error: response => this.showError(response),
    });
  }

  confirm(): void {
    const fields = this.form.getRawValue();
    this.error.set('');
    if (this.busy() || this.form.controls.code.invalid) { this.error.set('Enter the six-digit code from your email.'); return; }
    if (!this.verifying && (this.form.controls.password.invalid || fields.password !== fields.confirmation)) {
      this.error.set('Use 8–16 characters with uppercase, lowercase, a number and a symbol. Both passwords must match.'); return;
    }
    const token = this.auth.getTokens()?.accessToken;
    if (this.verifying && !token) { this.error.set('Please sign in again.'); return; }
    this.busy.set(true);
    const request = this.verifying ? this.service.confirmVerification(token!, fields.code)
      : this.service.confirmRecovery(fields.email.trim(), fields.code, fields.password);
    request.pipe(finalize(() => this.busy.set(false))).subscribe({
      next: response => { this.message.set(response.message); this.complete.set(true); this.form.controls.password.reset(); this.form.controls.confirmation.reset(); },
      error: response => this.showError(response),
    });
  }

  private showError(response: HttpErrorResponse): void {
    this.error.set(typeof response.error?.error === 'string' ? response.error.error : 'The request could not be completed. Please try again.');
  }
}
