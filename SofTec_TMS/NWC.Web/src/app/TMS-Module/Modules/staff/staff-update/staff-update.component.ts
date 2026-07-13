import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-staff-update',
  templateUrl: './staff-update.component.html',
  styleUrls: ['./staff-update.component.scss']
})
export class StaffUpdateComponent implements OnInit {
  id: string = ''
  constructor(
    private activatedsrRoute: ActivatedRoute,

  ) {
    this.id = this.activatedsrRoute.snapshot.paramMap.get('id');
  }

  ngOnInit() {

  }

}
