import { Directive, ElementRef } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';
import { SideNavService } from '../services/side-nav.service';

@Directive({
  selector: 'mat-drawer[sideNavService]',
  standalone: true
})
export class SideNavDirective {

  constructor(
    private drawer: MatDrawer,
    private nService: SideNavService
  ) {
    nService.registerDrawer(drawer);
  }

}
