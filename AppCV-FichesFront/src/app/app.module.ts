import { InlineEditorModule } from "./qontu/ngx-inline-editor";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import {
  MatButtonModule,
  MatCheckboxModule,
  MatChipsModule,
  MatIconModule,
  MatFormFieldModule,
  MatAutocompleteModule,
  MatInputModule
} from "@angular/material";
import { CvCreateComponent } from "./components/cv-create-component/cv-create.component";
import { ResumeInterventionsComponent } from "./components/CV/resume-interventions/resume-interventions.component";
import { AppComponent } from "./app.component";
import { CvDetailsComponent } from "./components/cv-details-component/cv-details.component";
import { NavbarComponent } from "./components/navbar/navbar.component";
import { HomeComponent } from "./components/home/home.component";
import { RegisterComponent } from "./components/register/register.component";
import { LoginComponent } from "./components/login/login.component";
import { PageNotFoundComponent } from "./components/page-not-found/page-not-found.component";
import { ForgotPasswordComponent } from "./components/forgot-password/forgot-password.component";
import { LangueService } from "./Services/langue.service";
import { TableLangueComponent } from "./shared/table-langue/table-langue.component";
import { TablePerfectionnementsComponent } from "./shared/table-perfectionnements/table-perfectionnements.component";
import { TableFormationAcademiqueComponent } from "./shared/table-formation-academique/table-formation-academique.component";
import { TableCertificationsComponent } from "./shared/table-certifications/table-certifications.component";
import { ChipDomainComponent } from "./shared/chip-domain/chip-domain.component";
import { TableTechnologieComponent } from "./shared/table-technologie/table-technologie.component";
import { TableMandatComponent } from "./shared/table-mandat/table-mandat.component";
import { BrowserModule } from "@angular/platform-browser";
import {  HttpClientModule,  HTTP_INTERCEPTORS} from "@angular/common/http";
import { ServiceRoutingModule } from "./Routes.services";
import {  ReactiveFormsModule,  FormsModule,  FormBuilder} from "@angular/forms";
import { ValidatorService } from "./validator.services";
import { JwtInterceptor } from "./Services/jwt-interceptor.service";
import { CVService } from "./Services/cv.service";
import { NgModule } from "@angular/core";
import { CvEditComponent } from './components/cv-edit/cv-edit.component';
import { BioComponent } from "./shared/bio/bio.component";

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
    CvCreateComponent,
    ResumeInterventionsComponent,
    LangueService,
    TableLangueComponent,
    TablePerfectionnementsComponent,
    TableFormationAcademiqueComponent,
    TableCertificationsComponent,
    ChipDomainComponent,
    TableTechnologieComponent,
    TableMandatComponent,
    CvEditComponent,
    BioComponent
  ],
  imports: [
    MatAutocompleteModule,
    BrowserModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    HttpClientModule,
    FormsModule,
    ServiceRoutingModule,
    InlineEditorModule,
    MatButtonModule,
    MatCheckboxModule,
    MatChipsModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule
  ],
  providers: [
    LangueService,
    ValidatorService,
    FormBuilder,
    CVService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    }
  ],
  exports: [LangueService],

  bootstrap: [AppComponent]
})
export class AppModule {}
