import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-signals-demo',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './signals-demo.component.html',
  styleUrls: ['./signals-demo.component.css']
})
export class SignalsDemoComponent {
  counter = signal(0);

  increment() {
    this.counter.update(v => v + 1);
  }

  decrement() {
    this.counter.update(v => v - 1);
  }

  reset() {
    this.counter.set(0);
  }
}
