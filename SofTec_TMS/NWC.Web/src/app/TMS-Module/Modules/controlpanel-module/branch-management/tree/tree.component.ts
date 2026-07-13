import { string } from '@amcharts/amcharts4/core';
import { Component, ElementRef, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { AreaModel } from '@tms-models/area-model';
import { FilterModel } from '@tms-models/common/filter-model';
import { Lookup } from '@tms-models/common/lookup';
import { PageFilter } from '@tms-models/common/page-fillter-model';
import { SearchResult } from '@tms-models/common/search-result';
import { TreeNode } from '@tms-models/common/tree-node';
import { StaffSearchCriteria } from '@tms-models/search-criteria/staff-search-criteria';
import { ControlPanelService } from '@tms-services/control-panel.service';
import { LookupService } from '@tms-services/lookup.service';
import { Configuration } from '@tms-shared/configurations/shared.config';
import { alertService } from '@tms-shared/Services/alert/alert.service';
import { AuthenticationService } from '@tms-shared/Services/authentication/authentication.service';
import { TranslateService } from '@ngx-translate/core';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-tree',
  templateUrl: './tree.component.html',
  styleUrls: ['./tree.component.scss']
})
export class TreeComponent implements OnInit {

  @Input() treeData: TreeNode[];
  @ViewChild('modaleTemplate', { static: true }) public templateref: TemplateRef<any>;

  pagePermission = '';
  SearchCriteria: StaffSearchCriteria;
  tableLoading = false;
  modalRef: BsModalRef;
  showTypes = false;

  currLang!: string;

  selectMenuOptions = {
    singleSelect: true,
  };
  landmarkList: Lookup<string>[] = [];
  Customer_Loading = false;
  dropdownSettings: IDropdownSettings = {};
  advancedDiv = <boolean>false;

  searchResult = new SearchResult<any>();
  areaModel: AreaModel = new AreaModel();
  
  action = {
    action: string,
    name: string,
    id: string
  };

  //@Input('templateRef') templateRef: TemplateRef<any>;
  //@ViewChild('templateRef', { static: true }) templateRef: ElementRef;
  constructor(
    private authServer: AuthenticationService,
    private lookupservice: LookupService,
    private _translate: TranslateService,
    private router: Router,
    private titleService: Title,
    private cpService: ControlPanelService,
    private modalService: BsModalService,
    private _alert: alertService,
  ) {
    this.pagePermission = this.authServer.getCurrentUserPermissionByRoleName('orderlist');
    this.authServer.checkViewPrivilege(this.pagePermission, true);
  }
  
  ngOnInit() {
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'Id',
      textField: 'Name',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 1,
      allowSearchFilter: true,
    };

    this.setDefaultSearchValues();

    this.loadDDLsGV();
    this._translate.onLangChange.subscribe((res) => {
      this.loadDDLsGV();
    });
  }

  getLandmarkType(e) {
    this.lookupservice.getLandMarkTypes().subscribe(res => this.landmarkList = res.Value.Value);
  }

  toggleChild(node) {
    node.isExpanded = !node.isExpanded;
  }

  onLandmarkDDLChanged(e) {
    console.log(e);
  }

  editCity(id) {
    console.log(id);
  }

  setDefaultSearchValues() {
    this.SearchCriteria = new StaffSearchCriteria();
    this.SearchCriteria.FilterModel = new FilterModel<string>();
    this.SearchCriteria.FilterModel.PageFilter = new PageFilter();
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = 1;
    this.SearchCriteria.FilterModel.PageFilter.PageSize = Configuration.GridSetting.Pagesize;
  }

  onClick(node: any, template: TemplateRef<any>){
    console.log(node.level, node.id);
    let param = {name: '', action: 'add', PID: node.id};
    if(node.level == 1)
    param.name = 'city';
    else if(node.level == 2)
    param.name = 'landmark';

    this.showModal(template, param);
  }

  onEdit(node: any, template: TemplateRef<any>){
    console.log(node.level, node.id);
    let param = {name: '', action: 'edit', PID: node.id};
    if(node.level == 1)
      param.name = 'area';
    else if (node.level == 2)
      param.name = 'area'
      else
      param.name = 'landmark';
      debugger;
    this.showModal(template, param)
  }

  onDelete(node: any){
    console.log(node.level, node.Id);
    let name = '';
    if(node.level == 1)
      name = 'area';
    else if (node.level == 2)
      name = 'area'
      else
      name = 'landmark';
      this.delete(name, node.id);
  }

  loadDDLsGV() {
    this.titleService.setTitle(this._translate.instant('BranchManagement'));
    this.currLang = this._translate.currentLang;
    console.log(this.currLang);
  }

  searchCaller() {
    this.tableLoading = true;
    this.lookupservice
      .searchStaff(this.SearchCriteria)
      .subscribe((res) => {
        this.searchResult.TotalCount = res.Value.TotalCount;
        this.searchResult.Result = res.Value.Result;
        this.tableLoading = false;

        console.log(typeof this.searchResult.Result);
        console.log(this.searchResult.Result);
      });
  }

  clearSearch() {
    this.setDefaultSearchValues();
    this.searchCaller();
  }

  onSearchSubmit() {
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = 1;
    this.searchCaller();
  }

  onPageIndexChanged(evt) {
    this.SearchCriteria.FilterModel.PageFilter.PageIndex = evt;
    this.searchCaller();
  }

  onPageSizeChanged(evt) {
    this.SearchCriteria.FilterModel.PageFilter.PageSize = evt;
    this.searchCaller();
  }

  editItem(id) {
    this.router.navigate([`create/${id}`]);
  }
  deleteItem(id) {
    this.cpService.deleteBranch(id);
  }

  showModal(template: TemplateRef<any>, props: any) {
    if(template == null){
      template = this.templateref;
    }
    console.log(props, template);
    this.action = props;
    this.areaModel.parentBranchId = props.PID;
    if(props.action === "edit") {
      switch (props.name) {
        case 'area':
          console.log(props.action, props.name);
          this.cpService.getAreaById(props.PID).subscribe(res => {this.areaModel = res.Value});
          break;
        case 'city':
          console.log(props.action, props.name);
          this.cpService.getCityById(props.PID).subscribe(res => this.areaModel = res.Value);
          break;
        case 'landmark':
          console.log(props.action, props.name);
          this.cpService.getLandmarkById(props.PID).subscribe(res => this.areaModel = res.Value);
          break;
      }
    }
    else{
      this.areaModel = new AreaModel();
      this.areaModel.parentBranchId = props.PID;
    }

    if(props.name === "landmark") {
      this.showTypes = true;
    } else {
      this.showTypes = false;
    }

    this.modalRef = this.modalService.show(
      template,
      Object.assign({}, { class: 'modal-lg' })
    );
  }



  delete(name, id) {

    if (!this.authServer.checkFullControlPrivilege(this.pagePermission)) {
      return;
    }
    this._alert.confirmationMessage("Delete").subscribe((confirm) => {
      if (confirm === true) {
        switch (name) {
          case 'area':
            this.cpService.deleteArea(id).subscribe(res =>{
              console.log(res)
              {console.log(res); this.showAlertAndColseModal(res, 'delete')}
            })
            break;
          case 'city':
            this.cpService.deleteCity(id).subscribe(res => {
              console.log(res)
              this.showAlertAndColseModal(res, 'delete')
            })
            break;
          case 'landmark':
            this.cpService.deleteLandmark(id).subscribe(res => {
              console.log(res)
              this.showAlertAndColseModal(res, 'delete')
            })
            break;
        }
      }
    });
  }

  showAlertAndColseModal(res: any, action: string){
    if (!res.IsErrorState) {
      if(action === 'edit')
        this._alert.success("Updated Successfully!");
        else if(action === 'add')
        this._alert.success("Added Successfully!");
        else
        this._alert.success("Deleted Successfully!");
    } else {
      this._alert.errorList(res.Errors);
    }
    this.modalRef.hide();
    window.location.reload();
  }

  save() {
    const currentAction = this.action.action as any;
    switch (this.action.name as any) {
      case 'area':
        this.areaModel.IsSubBranch = false;
        if (currentAction === 'edit') {
          this.cpService.editArea(this.areaModel).subscribe(res => {console.log(res); this.showAlertAndColseModal(res, currentAction)});
        } else {
          this.cpService.addArea(this.areaModel).subscribe(res => {console.log(res); this.showAlertAndColseModal(res, currentAction)});
        }
        break;
      case 'city':
        this.areaModel.IsSubBranch = true;
        if (currentAction === 'edit') {
          this.cpService.editCity(this.areaModel).subscribe(res => {console.log(res); this.showAlertAndColseModal(res, currentAction)});
        } else {
          this.cpService.addCity(this.areaModel).subscribe(res => {console.log(res); this.showAlertAndColseModal(res, currentAction)});
        }
        break;
      case 'landmark':
        if (currentAction === 'edit') {
          this.cpService.editLandmark(this.areaModel).subscribe(res => {console.log(res); this.showAlertAndColseModal(res, currentAction)});
        } else {
          this.cpService.addLandmark(this.areaModel).subscribe(res => {console.log(res); this.showAlertAndColseModal(res, currentAction)});
        }
        break;
    }
  }
}
