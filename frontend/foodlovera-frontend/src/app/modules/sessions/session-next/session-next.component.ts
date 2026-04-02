import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-session-next',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './session-next.component.html',
  styleUrls: ['./session-next.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SessionNextComponent {}