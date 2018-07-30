import { Component, OnInit } from '@angular/core';
import { RoleAdministratioViewModel } from '../../Models/Admin/RoleAdministration-model';
import { UserViewModel } from '../../Models/Admin/User-model';
import { AdminService } from '../../Services/admin.service';
import { ActivatedRoute, Router } from '../../../../node_modules/@angular/router';
import { HttpErrorResponse } from '../../../../node_modules/@angular/common/http';

@Component({
  selector: 'app-role-detail',
  templateUrl: './role-detail.component.html',
  styleUrls: ['./role-detail.component.css']
})
export class RoleDetailComponent implements OnInit {

  role: RoleAdministratioViewModel;
  applicationUsers: Array<UserViewModel>;

  constructor(
    private adminService: AdminService,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const roleId = params['id'];
      this.loadDetails(roleId);
    });
  }

  loadDetails(roleId: string) {
    this.adminService.GetDetail(roleId).subscribe(
      (data: RoleAdministratioViewModel) => {
        this.role = data;
      },
      (error: HttpErrorResponse) => {
        // TODO
        debugger;
      }
    );
  }

}
