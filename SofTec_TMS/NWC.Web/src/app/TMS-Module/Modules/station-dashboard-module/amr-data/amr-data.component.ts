import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-amr-data',
  templateUrl: './amr-data.component.html',
  styleUrls: ['./amr-data.component.scss']
})
export class AmrDataComponent implements OnInit {

  ImageUrl= "assets/TMSBranding/styles/img/loader.gif";
  dropImageUrl= "assets/TMSBranding/styles/img/drop.svg";

  loading_amr1 = false;
  count_amr1 = 0;

  loading_amr2 = false;
  count_amr2 = 0;

  loading_exitTankers = false;
  count_exitTankers = 0;

  loading_totalTankers = false;
  count_totalTankers = 0;

  loading_lossTankers = false;
  count_lossTankers = 0;

  constructor() { }

  ngOnInit() {
  }

}
