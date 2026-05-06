import { Routes } from '@angular/router';
import { OverviewPage} from './pages/overview/overview-page/overview-page';
import { UserDetailsPage} from './pages/user-details/user-details-page/user-details-page';
import { UsersPage } from './pages/users/users-page/users-page';


export const routes: Routes = [
  { path: '', redirectTo: 'users', pathMatch: 'full' },
  { path: 'overview', component: OverviewPage },
  { path: 'users', component: UsersPage },
  { path: 'users/:userId', component: UserDetailsPage },
];
