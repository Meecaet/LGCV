import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
//
import { HomeComponent } from "./components/home/home.component";
import { RegisterComponent } from "./components/register/register.component";
import { LoginComponent } from "./components/login/login.component";
import { CvDetailsComponent } from "./components/cv-details-component/cv-details.component";
import { AuthGuardService as AuthGuard } from "./auth-guard.service";
import { PageNotFoundComponent } from "./components/page-not-found/page-not-found.component";
import { ForgotPasswordComponent } from "./components/forgot-password/forgot-password.component";


const routes: Routes = [
  { path: "", component: HomeComponent },
  { path: "Home", component: HomeComponent },

  {
    path: "Account",
    children: [
        { path: "Login", component: LoginComponent },
        { path: "Register", component: RegisterComponent },
        { path: "ForgotPassword", component: ForgotPasswordComponent },
      ]
  },
  { path: "Details", component: CvDetailsComponent, canActivate: [AuthGuard], },

  { path: "notfound", component: PageNotFoundComponent },
  { path: "**", redirectTo: "notfound" }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class ServiceRoutingModule {}

export const routedComponents = [
  HomeComponent,
  RegisterComponent,
  LoginComponent,
  PageNotFoundComponent,
  ForgotPasswordComponent
];
