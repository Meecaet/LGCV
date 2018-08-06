import { InlineEditorModule } from "./qontu/ngx-inline-editor";
import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import {  ReactiveFormsModule,  FormsModule,  FormBuilder} from "@angular/forms";
import {  HttpClientModule,  HTTP_INTERCEPTORS} from "@angular/common/http";
import {
  MatButtonModule,
  MatCheckboxModule,
  MatChipsModule,
  MatIconModule,
  MatFormFieldModule,
  MatAutocompleteModule,
  MatInputModule,
  MatSelectModule,
  MatDialogModule,
  MatDialog
} from "@angular/material";

import { AppComponent } from "./app.component";
//Components
import { ResumeInterventionsComponent } from "./components/CV/resume-interventions/resume-interventions.component";
import { CvEditComponent } from './components/cv-edit/cv-edit.component';
import { RoleAdminComponent} from "./components/role-admin/role-admin.component";
import { RoleCreateComponent} from "./components/role-create/role-create.component";
import { RoleDetailComponent } from './components/role-detail/role-detail.component';
import { RoleEditComponent } from './components/role-edit/role-edit.component';
import { AccessDeniedComponent } from './components/access-denied/access-denied.component';
import { NavbarComponent } from "./components/navbar/navbar.component";
import { HomeComponent } from "./components/home/home.component";
import { RegisterComponent } from "./components/register/register.component";
import { LoginComponent } from "./components/login/login.component";
import { PageNotFoundComponent } from "./components/page-not-found/page-not-found.component";
import { ForgotPasswordComponent } from "./components/forgot-password/forgot-password.component";
import { ResetPasswordComponent } from "./components/reset-password/reset-password.component";
//Shared
import { TableLangueComponent } from "./shared/table-langue/table-langue.component";
import { TablePerfectionnementsComponent } from "./shared/table-perfectionnements/table-perfectionnements.component";
import { TableFormationAcademiqueComponent } from "./shared/table-formation-academique/table-formation-academique.component";
import { TableCertificationsComponent } from "./shared/table-certifications/table-certifications.component";
import { ChipDomainComponent } from "./shared/chip-domain/chip-domain.component";
import { ModalOldInformationComponent } from './shared/modal-old-information/modal-old-information.component';
import { TableTechnologieComponent } from "./shared/table-technologie/table-technologie.component";
import { TableMandatComponent } from "./shared/table-mandat/table-mandat.component";
import { TablePublicationComponent } from './shared/table-publication/table-publication.component';
import { TableConferenceComponent } from './shared/table-conference/table-conference.component';
import { BioComponent } from "./shared/bio/bio.component";
import { CarouselComponent } from './shared/carousel/carousel.component';
import { DropDownFonctionComponent } from './shared/drop-down-fonction/drop-down-fonction.component';
//Services
import { ServiceRoutingModule } from "./Routes.services";
import { ValidatorService } from "./validator.services";
import { JwtInterceptor } from "./Services/jwt-interceptor.service";
import { CVService } from "./Services/cv.service";
import { AdminService } from "./Services/admin.service";
import { LangueService } from "./Services/langue.service";
import { FonctionPipe } from "./Services/fonction.pipe";

@NgModule({
  declarations: [
    AppComponent,

    NavbarComponent,
    HomeComponent,
    RegisterComponent,
    LoginComponent,
    PageNotFoundComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent,
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
    RoleEditComponent,
    AccessDeniedComponent,
    TablePublicationComponent,
    TableConferenceComponent,
    ModalOldInformationComponent
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
    MatInputModule,
    MatSelectModule,
    MatDialogModule
  ],
  providers: [
    MatDialog,
    LangueService,
    FonctionPipe,
    ValidatorService,
    FormBuilder,

    CVService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    },
    AdminService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    }
  ],
  entryComponents:[
    ModalOldInformationComponent
  ],

  exports: [LangueService ,FonctionPipe],

  bootstrap: [AppComponent]
})
export class AppModule {}
