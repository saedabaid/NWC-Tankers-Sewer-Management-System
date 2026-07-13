import { Injectable, inject } from '@angular/core';
import { Configuration } from '../../configurations/shared.config';
import { MenuLinks } from '../../Resources/menu.res'

@Injectable()
export class MenuLinkService {

    public getLink(key: string, system: string) {
        //debugger;
        //let url = "https://vpn.nwc.com.sa/,DanaInfo=tms.nwc.com.sa,SSL+NWClogin.aspx";
        //let isUrlVpn = window.location.href.includes(Configuration.keys.UrlVpnDomain);
        //let isUrlVpn = (window.location.hostname == Configuration.keys.hostName); 

        // let x1 = window.location as any;
        // let isUrlVpn = (x1.type && (x1.type == "DanaLocation"));
        // isUrlVpn = false;
debugger;
        let port: string = '';
        if (system.toLowerCase() == 'Altair'.toLowerCase()) {
            //port = isUrlVpn ? Configuration.Ports.AltairPortUrlVpn : Configuration.Ports.AltairPort;
            port = Configuration.Ports.AltairPort;
        }
        else if (system.toLowerCase() == 'GPS'.toLowerCase())
            port = Configuration.Ports.GpsPort;
        else if (system.toLowerCase() == 'CarRent'.toLowerCase())
            port = Configuration.Ports.RenCarsPort;

        if (MenuLinks[key])
            return port + MenuLinks[key];
        else
            return '';
    }
}