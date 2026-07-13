import { Injectable, Inject } from '@angular/core';
import { TRANSLATIONS } from './translations';
import { AuthenticationService } from '../../Services/authentication/authentication.service';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';

@Injectable()
export class TranslateService {
  // private _currentLang: string;

  public get currentLang() {
    if (this.auth.getCurrentculture()) {
      return this.auth.getCurrentculture();
    }
    else
      return 'en';
  }

  // inject our translations
  constructor(@Inject(TRANSLATIONS) private _translations: any, private auth: AuthenticationService, private localeService: BsLocaleService) {
  }

  public use(lang: string): void {
    // set current language
    // this._currentLang = lang;
    // localStorage.setItem('culture', lang)
    this.auth.setCulture(lang);
    this.localeService.use(this.auth.getCurrentculture());
    this._translations = Object.assign({}, this._translations)
  }

  private translate(key: string): string {
    // private perform translation
    let translation = key;
    if (this._translations[this.currentLang] && this._translations[this.currentLang][key]) {
      return this._translations[this.currentLang][key];
    }

    return translation;
  }

  public instant(key: string) {
    // public perform translation
    return this.translate(key);
  }
}
