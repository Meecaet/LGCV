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

import { ResumeInterventionsComponent } from "./components/CV/resume-interventions/resume-interventions.component";
import { AppComponent } from "./app.component";

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
import { CarouselComponent } from './shared/carousel/carousel.component';
import { DropDownFonctionComponent } from './shared/drop-down-fonction/drop-down-fonction.component';
import { FonctionPipe } from "./Services/fonction.pipe";

import { RoleAdminComponent} from "./components/role-admin/role-admin.component";
import { RoleCreateComponent} from "./components/role-create/role-create.component";
import { RoleDetailComponent } from './components/role-detail/role-detail.component';
import { RoleEditComponent } from './components/role-edit/role-edit.component';

@NgModule({
  declarations: [
    AppComponent,

    NavbarComponent,
    HomeComponent,
    RegisterComponent,
    LoginComponent,
    PageNotFoundComponent,
    ForgotPasswordComponent,
    FonctionPipe,
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
    BioComponent,
    CarouselComponent,
    DropDownFonctionComponent,


    RoleAdminComponent,
    RoleCreateComponent,
    RoleDetailComponent,
    RoleEditComponent
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
    FonctionPipe,
    ValidatorService,
    FormBuilder,
    CVService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    }
  ],
  exports: [LangueService ,FonctionPipe],

  bootstrap: [AppComponent]
})
export class AppModule {}
