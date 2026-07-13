import { Injectable, OnDestroy } from '@angular/core';
import * as XLSX from 'xlsx';
import { Subject, BehaviorSubject } from 'rxjs';
import { alertService } from '../alert/alert.service';

type AOA = any[][];

@Injectable()
export class ImportExcelService implements OnDestroy {

  constructor(
    private _alert: alertService
  ) { }
  
  data: AOA = [];
  _data: Subject<any>;
  validationResultText: string = "";

  importXLSX(selectedFile) {
    //debugger;
    this.data = []
    this._data = new BehaviorSubject<any>([]);
    this.validationResultText = "";
    /* wire up file reader */
    const target: DataTransfer = <DataTransfer>(selectedFile.target);
    if (target.files.length !== 1) 
    {
      this._alert.error('Cannot use multiple files');
      throw new Error('Cannot use multiple files');
      return;
    }
    
    const reader: FileReader = new FileReader();
    reader.onload = (e: any) => {
      /* read workbook */
      const bstr: string = e.target.result;
      const wb: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary', raw: false, cellDates: true, dateNF: 'mm/dd/yyyy;@' });

      /* grab first sheet */
      const wsname: string = wb.SheetNames[0];
      const ws: XLSX.WorkSheet = wb.Sheets[wsname];

      /* save data */
      this.data = <AOA>(XLSX.utils.sheet_to_json(ws, { header: 1, blankrows: false }));
      let result = {
        headers: this.data[0],
        rows: this.getDataWithoutRows(this.data)
      }
      this._data.next(result);
    };
    reader.readAsBinaryString(target.files[0]);
    return this._data;
  }

  private getDataWithoutRows(data){
    let results = [];
    for(var i = 1; i < data.length; i++){
      results.push(data[i]);
    }
    return results;
  }

  matchObjects(source, target, key){
    let keyIndex = source.headers.findIndex(x => x == key);
    source.rows.forEach(r => {
      target.push(r[keyIndex])
    });
  }

  ngOnDestroy() {
    this._data.unsubscribe();
  }
  
}
