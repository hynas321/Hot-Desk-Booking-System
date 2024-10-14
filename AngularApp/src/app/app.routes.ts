import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginViewComponent } from './views/login-view/login-view.component';
import { DesksViewComponent } from './views/desks-view/desks-view.component';
import { LocationsViewComponent } from './views/locations-view/locations-view.component';
import { NotFoundViewComponent } from './views/not-found-view/not-found-view.component';
import { BrowserModule } from '@angular/platform-browser';
import { AuthGuard } from './guards/auth.guard';
import { LocationResolver } from './resolvers/location.resolver';
import { DesksResolver } from './resolvers/desk.resolver';

export const routes: Routes = [
  { path: '', component: LoginViewComponent },
  {
    path: 'locations/:locationName/desks',
    component: DesksViewComponent,
    canActivate: [AuthGuard],
    resolve: { desks: DesksResolver },
  },
  {
    path: 'locations',
    component: LocationsViewComponent,
    canActivate: [AuthGuard],
    resolve: { locations: LocationResolver },
  },
  { path: '**', component: NotFoundViewComponent },
];

@NgModule({
  declarations: [],
  imports: [BrowserModule, RouterModule.forRoot(routes)],
  providers: [AuthGuard, LocationResolver],
  bootstrap: [],
})
export class AppRoutingModule {}
