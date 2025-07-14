import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { SideNavDirective } from './directives/side-nav.directive';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, NavBarComponent, MatSidenavModule, MatIconModule, FormsModule, SideNavDirective],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  constructor() {}

  ngOnInit() {
  }

  title = 'seniordesignfall2024.client';
  imperialChecked: boolean = true;
  metricChecked: boolean = false;
  onCheckboxClick(selectedUnit: string) {
    //if (selectedUnit === 'imperial') {
    //  const metricCheckbox = document.getElementById('metric') as HTMLInputElement;
    //  metricCheckbox.checked = false;
    //} else if (selectedUnit === 'metric') {
    //  const imperialCheckbox = document.getElementById('imperial') as HTMLInputElement;
    //  imperialCheckbox.checked = false;
    //}
  }
}
