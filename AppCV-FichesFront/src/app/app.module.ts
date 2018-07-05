import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { CvDetailsComponent } from './components/cv-details-component/cv-details.component';
import { HttpClientModule } from '@angular/common/http';
import { NavbarComponent } from './components/navbar/navbar.component';

import { ServiceRoutingModule } from './Routes.services';
import { HomeComponent } from './components/home/home.component';
import { RegisterComponent } from './components/register/register.component';
import { LoginComponent } from './components/login/login.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { FormsModule } from '@angular/forms';
import { ValidatorService } from './validator.services';


@NgModule({
  declarations: [
    AppComponent,
    CvDetailsComponent,
    NavbarComponent,
    HomeComponent,
    RegisterComponent,
    LoginComponent,
    PageNotFoundComponent,
    ForgotPasswordComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ServiceRoutingModule,

   ],
  providers: [ValidatorService],
  bootstrap: [AppComponent]
})
export class AppModule { }
