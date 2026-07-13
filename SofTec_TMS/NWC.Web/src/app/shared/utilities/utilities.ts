
export function isNullOrUndefined(value: any) {
    return value === null || value === undefined;
}

export function minutesToHHMM(value: number) {
    if (!value) return '';

    let hours: number | string = Math.floor(value / 60);
    let minutes: number | string = value % 60;
    if (minutes <= 9 && minutes >= 0) {
        minutes = "0" + minutes;
    }
    if (hours <= 9 && hours >= 0) {
        hours = "0" + hours;
    }
    return `${hours}:${minutes}`;
}

export function ConvertDateToExcel(input: Date | string, includeTime: boolean): string {
    if (input) {
        let date = input.toString().substring(0, 10);
        if (!includeTime) {
            return date;
        }
        let time = input.toString().substring(11, 16);
        return date + ' ' + time;
    }
    return '';
}
