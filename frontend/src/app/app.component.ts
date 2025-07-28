import { Component } from '@angular/core';
import { MeetingsListComponent } from './components/meetings-list/meetings-list.component';
import { SignalsDemoComponent } from './signals-demo';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, MeetingsListComponent, SignalsDemoComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'frontend';
}
