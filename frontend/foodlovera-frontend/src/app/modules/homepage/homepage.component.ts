import { Component, ChangeDetectionStrategy } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { Observable } from 'rxjs';

import { ImageConfigService } from '../../core/services/imageConfig.service';
import { BdButtonComponent } from '../shared/bd-button/bd-button.component';


@Component({
  selector: 'app-homepage',
  standalone: true,
  imports: [AsyncPipe, TranslateModule, BdButtonComponent],
  templateUrl: './homepage.component.html',
  styleUrls: ['./homepage.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomepageComponent {
  readonly heroBg$: Observable<string>;
  readonly step1Img$: Observable<string>;
  readonly step2Img$: Observable<string>;
  readonly step3Img$: Observable<string>;

  constructor(private readonly images: ImageConfigService) {
    this.heroBg$ = this.images.get$('home', 'heroBackground1');
    this.step1Img$ = this.images.get$('home', 'step1');
    this.step2Img$ = this.images.get$('home', 'step2');
    this.step3Img$ = this.images.get$('home', 'step3');
    
  }

  createRoom(): void {
    console.log('create room');
  }

  joinRoom(): void {
    console.log('join room');
  }
}