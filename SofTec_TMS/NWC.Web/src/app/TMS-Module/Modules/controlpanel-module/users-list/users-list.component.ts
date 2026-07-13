import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { userListService } from '../../../Services/user-list.service';
import { LookupService } from '../../../Services/lookup.service';
import { Lookup } from '../../../Models/common/lookup';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { UserSearchCriteriaDTO } from '../../../Models/search-criteria/user-search-criteria';
import { UserListDTO } from '../../../Models/user-listDTO';
import { UserInfo } from '../../../Models/user-listDTO';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { Subject } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { Title } from '@angular/platform-browser';
import { SearchStream } from 'src/app/TMS-Module/Models/common/search-stream-object.model';
import { LoaderService } from 'src/app/shared/loader.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Router, ActivatedRoute } from "@angular/router";

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.scss']
})
export class UsersListComponent implements OnInit {
  UserSearchCriteriaDTO: UserSearchCriteriaDTO = new UserSearchCriteriaDTO();
  pagePermission: string = '';
  userList: UserListDTO[];
  TotalCount: number;
  Name:string;
  constructor(private _translate: TranslateService,
    private authServer: AuthenticationService,
    private LookupService: LookupService,
    private userListService: userListService,
    private _alert: alertService,
    private titleService: Title,
    private mainloading: LoaderService,
    private modalRef: BsModalRef,
    private modalService: BsModalService, private _router: Router
  ) {

    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName('UserList');
    this.authServer.checkViewPrivilege(this.pagePermission, true);
  }

  ngOnInit() {
    this.UserSearchCriteriaDTO.IsDeleted = false;
    this.UserSearchCriteriaDTO.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
    this.UserSearchCriteriaDTO.PageFilter.PageIndex = 1;

    this.load();
    this._translate.onLangChange.subscribe(res => {
      this.load();
    });

  }
  load() {

    this.titleService.setTitle(this._translate.instant('Users'));

    this.Search();
  }
  searchUserByName($event) {
    debugger;
    return this.LookupService.SearchUserByName($event)

  }
  customRoute(routeTo: string,value: string) {
    if (routeTo && routeTo !== "") {
      const targetLink = `/tms/${routeTo}`;
      const url = this._router.serializeUrl(
        this._router.createUrlTree([targetLink])
      );
      console.log(routeTo, url);
      this._router.navigate([url], { queryParams: { key: value } });
      // window.open(url, '_blank');
    }
  }
  Clear() {
    this.UserSearchCriteriaDTO = new UserSearchCriteriaDTO();
    this.UserSearchCriteriaDTO.IsDeleted = false;
    this.UserSearchCriteriaDTO.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
    this.UserSearchCriteriaDTO.PageFilter.PageIndex = 1;
    this.Search();
  }
  unlockUser(Name: string) {
    this._alert.confirmationMessage("UnlockMsgUser").subscribe(confirm => {
      if (confirm == true) {
        this.userListService.unlockUser(Name).subscribe((res) => {
          if (!res.IsErrorState) {
            this._alert.success("unlockUser");
            this.Search();
          } else {
            this._alert.errorList(res.Errors);
          }
        });
      }
    })
  }
  lockUser(Name: string) {
    this._alert.confirmationMessage("lockMsgUser").subscribe(confirm => {
      if (confirm == true) {
        this.userListService.lockUser(Name).subscribe((res) => {
          if (!res.IsErrorState) {
            this._alert.success("lockUser");
            this.Search();
          } else {
            this._alert.errorList(res.Errors);
          }
        });
      }
    })
  }
  deleteUser(Name: string) {
    this._alert.confirmationMessage("DeleteMsgUser").subscribe(confirm => {
      if (confirm == true) {
        this.userListService.deleteUser(Name).subscribe((res) => {
          if (!res.IsErrorState) {
            this._alert.success("DeletedSuccessed");
            this.Search();
          } else {
            this._alert.errorList(res.Errors);
          }
        });
      }
    })
  }
  onUserDDLChanged(evt) {
    this.UserSearchCriteriaDTO.Name = isNullOrUndefined(evt.Name) ? evt : evt.Name;
  }
  onPageIndexChanged(evt) {
    this.UserSearchCriteriaDTO.PageFilter.PageIndex = evt;
    this.Search();
  }

  onPageSizeChanged(evt) {
    this.UserSearchCriteriaDTO.PageFilter.PageSize = evt;
    this.Search();
  }
  Search(Name:string="") {
    debugger;
    this.UserSearchCriteriaDTO.Name = Name;

    this.mainloading.PreloaderIcreaseCount();
    this.userListService.Search(this.UserSearchCriteriaDTO).subscribe(res => {
      if (res.Value != null) {
        this.userList = res.Value.Result;
        this.TotalCount = res.Value.TotalCount;
      }
      else {
        this.userList = [];
        this.TotalCount = 0;
      }
    }
      , err => {
        this.mainloading.PreloaderDecreaseCount();
      }
      , () => {
        //this.tableLoading= false;
        this.mainloading.PreloaderDecreaseCount();
      })
  }

}
