import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../Services/admin.service';
import { HttpErrorResponse } from '../../../../node_modules/@angular/common/http';
import { RoleViewModel } from '../../Models/Admin/Role-model';
import { ErrorService } from '../../Services/error.service';

@Component({
  selector: 'app-role-admin',
  templateUrl: './role-admin.component.html',
  styleUrls: ['./role-admin.component.css']
})
export class RoleAdminComponent implements OnInit {

  constructor(
    private adminService: AdminService,
    private errorService: ErrorService
  ) {}

  roles: Array<RoleViewModel>;

  ngOnInit() {
    this.adminService.GetRole().subscribe(
      (data: Array<RoleViewModel>) => {
        this.roles = data;
      }, 
      (error: HttpErrorResponse) => {
        this.errorService.ErrorHandle(error.status);
      }
    );
  }

  delete(roleId: string): void {
    this.adminService.DeleteRol(roleId).subscribe(
      (data: Array<RoleViewModel>) => {
        this.roles = data;
      },
      (error: HttpErrorResponse) => {
        this.errorService.ErrorHandle(error.status);
      }
    );
  }

}
