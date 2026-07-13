import { Component, OnInit, ElementRef, Input, ViewEncapsulation, ViewChild } from '@angular/core';
import { FileUploadService } from '../../Services/file-upload.service';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { AttachmentDTO } from '../../datamodels/attachment-dto';
import { Configuration } from '../../configurations/shared.config';
import { alertService } from '../../Services/alert/alert.service';

@Component({
  selector: 'uploud-files',
  templateUrl: './uploader.component.html',
  styleUrls: ['./uploader.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class UploudFilesComponent implements OnInit {
  extensions: string[];

  constructor(
    private fileUploadService: FileUploadService,
    private alert: alertService
  ) {
    this.extensions = Configuration.keys.UploadExtension.split(',').map(x => x.toLowerCase())
    this.maxFileSize = Configuration.keys.maxFileSize;
  }
  @ViewChild('fileUpload', { static: true }) myFileControl: ElementRef;
  FilesUpload: AttachmentDTO;

  @Input() IsAllowPdf: boolean;
  @Input() maxFileSize: number;
  @Input() uploadType: number = 0;
  @Input() IsDisabled: boolean = false;
  @Input('ListFiles') ListFiles: AttachmentDTO[] = [];
  _fileName: string = "";
  @Input() AttachmentType: number; //1: contract, 2: Workorder, 3: Contractor, 4: ContractViolation

  Upload_Loading = false;
  
  filesToUpload: File[] = [];
  ngOnInit() {
  }


  fileChangeEvent(fileInput: any) {
    this.filesToUpload = <File[]>fileInput.target.files;
    let file = this.filesToUpload[0]
    let fileextension = file.name.split('.').pop()
    this._fileName = file.name;

    // if (this.IsAllowPdf && !this.extensions.includes('pdf'))
    //   this.extensions.push('pdf');
    if (this.extensions.findIndex(x => x === fileextension.toLowerCase()) == -1 ) {
      //this.alert.error(`wrong format, you should upload ${this.extensions.join(',')} only.`);
      this.alert.error("NotSupportedMediaType");
      this.myFileControl.nativeElement.value = "";
      this.filesToUpload = [];
      this._fileName = "";
      return;
    }
    if (this.maxFileSize && file.size > this.maxFileSize) {
      //this.alert.error(`Maximum file size allowed is ${file.size / 1024 / 1024} MB`);
      this.alert.error("ViolateMediaSize");
      this.myFileControl.nativeElement.value = "";
      this.filesToUpload = [];
      this._fileName = "";
      return;
    }

  }

  upload() {
    if (this.filesToUpload && this.filesToUpload.length > 0) {
      let files: File[] = this.filesToUpload;

      this.Upload_Loading = true;
      this.fileUploadService.uploadFiles(files).subscribe(res => {
        if (!res.IsErrorState && !isNullOrUndefined(res.Value)) {
          if (!this.ListFiles)
            this.ListFiles = [];
          res.Value[0].DocumentName = this._fileName;
          this.ListFiles.push(res.Value[0]);

          this.filesToUpload = [];
          this._fileName = "";
        }
      }
      , err => {
        this.Upload_Loading = false;
      }
      , () => {
        this.Upload_Loading = false;
      });
    }
  }

  // remove(objectToDeleteIndex: any) {
  //   if (this.ListFiles == null || objectToDeleteIndex >= this.ListFiles.length) return;
  //   this.alert.confirmationMessage("ConfirmDelete").subscribe(confirm => {
  //     if (confirm == true) {
  //       let objectToDelete = this.ListFiles[objectToDeleteIndex];
  //       if (objectToDelete.ID && objectToDelete.ID !== 0) {
  //         objectToDelete.IsDeleted = true;
  //         return;
  //       }
  //       else {
  //         this.fileUploadService.deleteFiles(objectToDelete.RelativePath).subscribe(res => {
  //           if (!res.IsErrorState && !isNullOrUndefined(res.Value)) {
  //             this.ListFiles.splice(objectToDeleteIndex, 1);
  //           }
  //         });
  //       }
  //     }
  //   })
  // }

  remove(myAttach: AttachmentDTO) {
    if (this.ListFiles == null || myAttach == null) return;
    this.alert.confirmationMessage("ConfirmDelete").subscribe(confirm => {
      if (confirm == true) {
        if (myAttach.ID ) {
          myAttach.IsDeleted = true;
          return;
        }
        else {
          this.fileUploadService.deleteFiles(myAttach.RelativePath).subscribe(res => {
            if (!res.IsErrorState && !isNullOrUndefined(res.Value)) {
              let index = this.ListFiles.indexOf(myAttach);
              this.ListFiles.splice(index, 1);
            }
          });
        }
      }
    })
  }

  filterDeleted(files: AttachmentDTO[]) {
    return files.filter(x => !x.IsDeleted);
  }

  download(DocumentName: string, relativePath: string, Id: number) {
    this.fileUploadService.download(this.AttachmentType, relativePath, Id).subscribe(res => {
      var blob = new Blob([res]);
      let downloadName = relativePath.substring(relativePath.lastIndexOf('/'));
      this.saveFile(blob, downloadName);
    },
      err => {
        if (err.status == 404) {
          //this.alert.error('File Not Found ');
          return;
        }
      });
  }

  saveFile(blob: Blob, fileName: string) {
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
  }


}
