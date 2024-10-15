import { Component } from '@angular/core';
import { TopBarComponent } from './components/top-bar/top-bar.component';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  standalone: true,
  imports: [TopBarComponent, RouterModule],
})
export class AppComponent {
  title = 'AngularApp';
}
