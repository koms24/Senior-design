import { Component } from '@angular/core';
import { IHLSConfig, VgCoreModule } from '@videogular/ngx-videogular/core'
import { VgControlsModule } from '@videogular/ngx-videogular/controls'
import { VgOverlayPlayModule } from '@videogular/ngx-videogular/overlay-play'
import { VgBufferingModule } from '@videogular/ngx-videogular/buffering'
import { VgStreamingModule } from '@videogular/ngx-videogular/streaming'

@Component({
  selector: 'app-livefeed-view',
  standalone: true,
  imports: [VgCoreModule, VgControlsModule, VgOverlayPlayModule, VgBufferingModule, VgStreamingModule],
  templateUrl: './livefeed-view.component.html',
  styleUrl: './livefeed-view.component.css'
})
export class LivefeedViewComponent {

  public hlsConfig: IHLSConfig = {
    enableWorker : true,
    maxBufferLength: 1,
    liveSyncDurationCount: 0,
    liveMaxLatencyDurationCount: 2,

  } as IHLSConfig;


}
