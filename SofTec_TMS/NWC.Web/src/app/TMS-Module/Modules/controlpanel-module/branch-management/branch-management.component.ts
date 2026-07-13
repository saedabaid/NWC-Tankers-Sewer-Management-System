import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { LookupService } from '@tms-services/lookup.service';
import { TranslateService } from '@ngx-translate/core';


@Component({
  selector: 'app-branch-management',
  templateUrl: './branch-management.component.html',
  styleUrls: ['./branch-management.component.scss']
})
export class BranchManagementComponent implements OnInit {
  modaleTemplate: any;
  @ViewChild('modaleTemplate', { static: true }) public templateref: TemplateRef<any>;

  nodes = [];
  constructor(
    private lookupservice: LookupService,
    private _translate: TranslateService,
    private titleService: Title,
  ) {

  }

  ngOnInit() {
    this.getBranches();

    this.loadDDLsGV();
    this._translate.onLangChange.subscribe((res) => {
      this.loadDDLsGV();
    });
  }

  getBranches() {
    this.lookupservice.getBranches("")
      .subscribe(res =>
        res.Value.forEach(element => {
          this.nodes.push(
            {
              id: element.Id,
              name: element.Name,
              children: this.getSubBranches(element.Id),
              level: 1,
              color: 'success',
              btnName: 'Add SubBranch'
            }
          );
        })
      )
  }

  getSubBranches(Id: string) {
    let nodes = [];
    this.lookupservice.getSubBranches("", [Id])
      .subscribe(res => {
        res.Value.forEach(element => {
          nodes.push(
            {
              id: element.Id,
              name: element.Name,
              children: this.getLandmarks(element.Id),
              level: 2,
              color: 'info',
              btnName: 'Add Landmark'
            }
          )
        });
      });
    return nodes;
  }

  getLandmarks(Id: string) {
    let nodes = [];
    this.lookupservice.getLandmarks("", [Id])
      .subscribe(res => {
        res.Value.forEach(element => {
          nodes.push({
            id: element.Id,
            name: element.Name,
            level: 3,
            color: 'secondary',
          })
        });
      })
    return nodes;
  }
  loadDDLsGV() {
    this.titleService.setTitle(this._translate.instant('BranchManagement'));
  }
}
