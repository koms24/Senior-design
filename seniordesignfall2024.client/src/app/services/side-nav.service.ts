import { Injectable } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';

@Injectable({
  providedIn: 'root'
})
export class SideNavService {

  private _opened = false;
  private _drawer?: MatDrawer;

  constructor() { }

  public registerDrawer(d: MatDrawer) {
    this._drawer = d;
  }

  public toggle() {
    this._drawer?.toggle();
    this._opened = this._drawer?.opened ?? false;
  }
  public open() {
    this._opened = true;
    this._drawer?.open();
  }
  public close() {
    this._opened = false;
    this._drawer?.close();
  }
}
