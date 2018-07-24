import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../Services/admin.service';
import { HttpErrorResponse } from '../../../../node_modules/@angular/common/http';
import { RoleViewModel } from '../../Models/Admin/Role-model';

@Component({
  selector: 'app-role-admin',
  templateUrl: './role-admin.component.html',
  styleUrls: ['./role-admin.component.css']
})
export class RoleAdminComponent implements OnInit {

  constructor(
    private adminService: AdminService
  ) {}

  roles: Array<RoleViewModel>;

  ngOnInit() {
    this.adminService.GetRole().subscribe(
      (data: Array<RoleViewModel>) => {
        this.roles = data;
      }, 
      (error: HttpErrorResponse) => {
        // TODO
        debugger;
      }
    );
  }

  delete(roleId: string): void {
    this.adminService.DeleteRol(roleId).subscribe(
      (data: Array<RoleViewModel>) => {
        this.roles = data;
      },
      (error: HttpErrorResponse) => {
        // TODO
        debugger;
      }
    );
  }

}
