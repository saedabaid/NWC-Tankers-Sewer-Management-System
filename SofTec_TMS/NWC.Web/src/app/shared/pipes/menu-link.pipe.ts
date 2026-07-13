import { Pipe, PipeTransform } from '@angular/core';
import { MenuLinkService } from '../Services/menu/menu-link.service';

@Pipe({ name: 'MenuLink' })
export class MenuLinkPipe implements PipeTransform {
    constructor(private MenuLink: MenuLinkService) {

    }
    transform(value: any, system: string): any {
        return this.MenuLink.getLink(value, system);
    }
}