import { Component, OnInit } from '@angular/core';

@Component({
  selector: ' attachments',
  templateUrl: './attachments.component.html',
  styleUrls: ['./attachments.component.scss']
})
export class AttachmentsComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  listForAttachments: any[] = [];
  getFileDetails(event) {
    for (var i = 0; i < event.target.files.length; i++) {
      let file= {
          name : event.target.files[i].name,
          type : event.target.files[i].type,
          size : event.target.files[i].size,
          modifiedDate : event.target.files[i].lastModifiedDate
      }
     this.listForAttachments.push(file);
    }
  }

  deleteAttachment(){
    this.listForAttachments.splice(0, 1)
  }


}
