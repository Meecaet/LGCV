import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { CvDetailsComponent } from './components/cv-details-component/cv-details.component';
import { MandatComponent } from './components/mandat/mandat.component';

import { HttpClientModule } from '@angular/common/http';
import { NavbarComponent } from './components/navbar/navbar.component';

import { ServiceRoutingModule } from './Routes.services';
import { HomeComponent } from './components/home/home.component';
import { RegisterComponent } from './components/register/register.component';
import { LoginComponent } from './components/login/login.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { FormsModule, FormBuilder, FormControl } from '@angular/forms';
import { ValidatorService } from './validator.services';

import {InlineEditorModule} from './qontu/ngx-inline-editor';
import {MatButtonModule, MatCheckboxModule, MatChipsModule, MatIconModule, MatFormFieldModule, MatAutocompleteModule, MatInputModule} from '@angular/material';
import { CvCreateComponent } from './components/cv-create-component/cv-create.component';


@NgModule({
  declarations: [
    AppComponent,
    CvDetailsComponent,
    MandatComponent,
    NavbarComponent,
    HomeComponent,
    RegisterComponent,
    LoginComponent,
    PageNotFoundComponent,
    ForgotPasswordComponent,
    CvCreateComponent,
  ],
  imports: [
    MatAutocompleteModule,
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    ServiceRoutingModule,
    InlineEditorModule,
    MatButtonModule,
    MatCheckboxModule,
    MatChipsModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
   ],
  providers: [ValidatorService,FormBuilder],
  bootstrap: [AppComponent]
})
export class AppModule { }
