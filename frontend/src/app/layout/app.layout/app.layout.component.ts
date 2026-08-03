import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AppSidebarComponent } from '../app.sidebar/app.sidebar.component';
import { AppTopbarComponent } from '../app.topbar/app.topbar.component';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, AppSidebarComponent, AppTopbarComponent],
  templateUrl: './app.layout.component.html'
})
export class AppLayoutComponent {
}
