import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs'

@Injectable({
  providedIn: 'root'
})
export class LoaderService {

  loaderCount: number = 0;
  loaderCount$ = new BehaviorSubject<number>(this.loaderCount);

  constructor() { }


  PreloaderIcreaseCount() {
    this.loaderCount = this.loaderCount + 1;
    this.loaderCount$.next(this.loaderCount);
  
}

PreloaderDecreaseCount() {
    this.loaderCount = this.loaderCount - 1;
    this.loaderCount$.next(this.loaderCount);
}

hidePreloader() {
    document.getElementsByClassName("loading").item(0).setAttribute("style", "display:none;");
}
}
