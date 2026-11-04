import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy, HostListener, AfterViewInit, OnDestroy, ElementRef, inject } from '@angular/core';
import { DishListResponse } from '@app/models/dishes/dish-list-response.model';

@Component({
  selector: 'app-dish-modal',
  standalone: true,
  imports: [],
  templateUrl: './dish-modal.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './dish-modal.component.scss',
})
export class DishModalComponent implements AfterViewInit, OnDestroy {
  private readonly element: ElementRef<HTMLElement> = inject(ElementRef);
  private previousFocus: HTMLElement | null = null;
  @Input() dish!: DishListResponse;
  @Output() closed = new EventEmitter<void>();

  ngAfterViewInit(): void {
    this.previousFocus = document.activeElement instanceof HTMLElement ? document.activeElement : null;
    this.element.nativeElement.querySelector<HTMLButtonElement>('.close-button')?.focus();
  }

  ngOnDestroy(): void { this.previousFocus?.focus(); }

  @HostListener('document:keydown.escape')
  onEscape(): void { this.onClose(); }

  @HostListener('keydown.tab', ['$event'])
  onTab(event: Event): void {
    // The close button is the only interactive control in this read-only dialog.
    event.preventDefault();
    this.element.nativeElement.querySelector<HTMLButtonElement>('.close-button')?.focus();
  }

  onClose(): void {
    this.closed.emit();
  }
}
