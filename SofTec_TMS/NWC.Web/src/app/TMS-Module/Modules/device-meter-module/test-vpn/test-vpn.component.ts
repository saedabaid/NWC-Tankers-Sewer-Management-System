import { Component, OnInit, Inject } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { StorageService, LOCAL_STORAGE } from 'ngx-webstorage-service';

@Component({
  selector: 'app-test-vpn',
  templateUrl: './test-vpn.component.html',
  styleUrls: ['./test-vpn.component.scss']
})
export class TestVPNComponent implements OnInit {

  constructor(private CookieSrvc: CookieService,
    @Inject(LOCAL_STORAGE) private storage: StorageService
    ) { }

  ngOnInit() {
  }

  userText: string;
  userDom: string;

  cookieReturn: string;
  localReturn: string;


  saveC(){
    this.CookieSrvc.set('testVPN', this.userText, null, '/', this.userDom);
  }

  saveL(){
    this.storage.set('testVPN', this.userText);
  }

  getC(){
    this.cookieReturn = this.CookieSrvc.get('testVPN');
  }

  getL() {
    this.localReturn = this.storage.get('testVPN');
  }
  

}
