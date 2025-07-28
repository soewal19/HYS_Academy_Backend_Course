import { Routes } from '@angular/router';
import { MeetingsListComponent } from './components/meetings-list/meetings-list.component';
import { SignalsDemoComponent } from './signals-demo';

export const routes: Routes = [
    { path: '', component: MeetingsListComponent },
    { path: 'signals-demo', component: SignalsDemoComponent }
];
