import { Component, ChangeDetectionStrategy, Input } from '@angular/core';

@Component({
  selector: 'bd-button',
  standalone: true,
  templateUrl: './bd-button.component.html',
  styleUrls: ['./bd-button.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BdButtonComponent {
  @Input() disabled = false;
  @Input() type: 'button' | 'submit' = 'button';
}