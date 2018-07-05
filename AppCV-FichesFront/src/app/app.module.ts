import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { CvDetailsComponent } from './components/cv-details-component/cv-details.component';
import { HttpClientModule } from '@angular/common/http';
import { NavbarComponent } from './components/navbar/navbar.component';
import { MandatComponent } from './components/mandat/mandat.component';

@NgModule({
  declarations: [
    AppComponent,
    CvDetailsComponent,
    NavbarComponent,
    MandatComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
