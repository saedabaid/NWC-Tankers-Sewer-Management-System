import { Pipe, PipeTransform } from '@angular/core';
import { minutesToHHMM } from '../utilities/utilities';
@Pipe({
    name: 'm2HHMM'
})
export class MinutesToHHMMPipe implements PipeTransform {
    transform(value: number): string {
        
        return minutesToHHMM(value);
        
        // if (!value) return '';

        // let hours: number | string = Math.floor(value / 60);
        // let minutes: number | string = Math.floor(value % 60);
        // if (minutes <= 9 && minutes >= 0) {
        //     minutes = "0" + minutes;
        // }
        // if (hours <= 9 && hours >= 0) {
        //     hours = "0" + hours;
        // }
        // return `${hours}:${minutes}`;
    }
}