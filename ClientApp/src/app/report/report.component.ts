import { Component, OnInit, Inject } from '@angular/core';
import { Router } from "@angular/router";
import { DOCUMENT } from '@angular/common';

import {ReportService} from '../service/report.service';


@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.css'],
  providers:[ReportService]
})
export class ReportComponent implements OnInit {

  providerId:number;

  reportFile:File;

  constructor(@Inject(DOCUMENT)private document: Document, private router: Router,private reportServ:ReportService) { }

  ngOnInit(): void {
  }

  getReportProvider(){
   
    this.reportServ.getReportProvider(this.providerId).subscribe(res => {
      const fileURL = URL.createObjectURL(res);
      window.open(fileURL, '_blank');
    });

   // (window as any).open("https://localhost:44342/api/report/provider/" + this.providerId, "_blank");
  }

}
