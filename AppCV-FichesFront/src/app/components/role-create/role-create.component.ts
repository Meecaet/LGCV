import { Component, OnInit } from '@angular/core';
import { ValidatorService} from '../../validator.services'
import { RoleViewModel} from '../../Models/Admin/Role-model'
import { AdminService} from '../../Services/admin.service'
import { HttpErrorResponse } from '../../../../node_modules/@angular/common/http';
import { Router } from '@angular/router';
import { Location } from "@angular/common";

@Component({
  selector: 'app-role-create',
  templateUrl: './role-create.component.html',
  styleUrls: ['./role-create.component.css']
})
export class RoleCreateComponent implements OnInit {

  constructor(
    private validator: ValidatorService,
    private adminService: AdminService,
    private router: Router,
    private location: Location
  ) {}
  
  model: RoleViewModel = new RoleViewModel();

  ngOnInit() {}
  
  IsValid(value, errorEmpty) {
    errorEmpty.presse = true;
    this.validator.ValidateEmpty(value, errorEmpty);
  }
  
  Create(): void {
    this.adminService.CreateRole(this.model).subscribe(
      (data: RoleViewModel) => {
        this.location.replaceState('/');
        this.router.navigate(['role/admin']);
      }, 
      (error: HttpErrorResponse) => {
        // TODO
        debugger;
      }
    );
  }
}
