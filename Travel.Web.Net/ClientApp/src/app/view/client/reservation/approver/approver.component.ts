import { Component, OnInit } from '@angular/core';
import { ExternalDataService } from '../../../../controllers/external/data/data.service';
import { SessionExternalService } from '../../../../services/session-external/session-external.service';

@Component({
  selector: 'app-approver',
  templateUrl: './approver.component.html',
  styleUrls: ['./approver.component.css']
})
export class ApproverComponent implements OnInit {

  lstBossGroup = [];
  lstBossGroupFilter: Array<{ nombreCompleto: string, usuario: string }>;

  constructor(private data_external: ExternalDataService,  private session_external: SessionExternalService) { }

  ngOnInit() {
    this.getBossGroup();
  }

  public getBossGroup() {
    var token = this.session_external.getKeyExternal();
    this.data_external.getBossGroup(token).then(res => {
       this.lstBossGroup = res;
       this.lstBossGroupFilter = this.lstBossGroup.slice();
     }).catch(err => {
       console.log(err);
     });
  }

  public filterBoss(value) {
    this.lstBossGroupFilter = this.lstBossGroup.filter((s) => s.NombreCompleto.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

}
