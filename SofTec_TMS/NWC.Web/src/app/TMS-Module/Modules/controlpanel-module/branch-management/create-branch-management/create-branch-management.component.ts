import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-create-branch-management',
  templateUrl: './create-branch-management.component.html',
  styleUrls: ['./create-branch-management.component.scss']
})
export class CreateBranchManagementComponent implements OnInit {

  id = '';

  constructor(
    private activRoute: ActivatedRoute
  ) {
    if (this.activRoute.snapshot.paramMap.has('id')) {
      this.id = this.activRoute.snapshot.paramMap.get('id');
    }
  }

  ngOnInit() {

  }

}
