import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-create-room-success',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './create-room-success.component.html',
  styleUrls: ['./create-room-success.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreateRoomSuccessComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly joinCode = this.route.snapshot.queryParamMap.get('joinCode') ?? '';
  readonly roomName = this.route.snapshot.queryParamMap.get('name') ?? '';

  goToJoinRoom(): void {
    this.router.navigate(['/join-room'], {
      queryParams: {
        code: this.joinCode,
      },
    });
  }
}