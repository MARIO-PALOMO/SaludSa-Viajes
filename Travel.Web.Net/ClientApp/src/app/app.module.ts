import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CUSTOM_ELEMENTS_SCHEMA, LOCALE_ID, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { HeaderComponent } from './view/client/componets/header/header.component';
import { IndexComponent } from './view/client/reservation/index/index.component';
import { ReservationListComponent } from './view/client/reservation-list/reservation-list.component';
import { UserService } from './services/user/user.service';
import { ApiService } from './services/api/api.service';

import { ApproverComponent } from './view/client/reservation/approver/approver.component';
import { LodgingComponent } from './view/client/reservation/lodging/lodging.component';
import { RegisterComponent } from './view/client/reservation/register/register.component';
import { TransportComponent } from './view/client/reservation/transport/transport.component';
import { TravelComponent } from './view/client/reservation/travel/travel.component';
import { TravelExpensesComponent } from './view/client/reservation/travel-expenses/travel-expenses.component';
import { ExternalDataService } from './controllers/external/data/data.service';
import { SessionExternalService } from './services/session-external/session-external.service';
import { SessionInternalService } from './services/session-internal/session-internal.service';
import { GlobalsService } from './methods/globals/globals.service';
import { ValidationsService } from './methods/validations/validations.service';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { CalendarModule } from '@progress/kendo-angular-dateinputs';
import { IntlModule } from '@progress/kendo-angular-intl';
import { GridModule } from '@progress/kendo-angular-grid';

import '@progress/kendo-angular-intl/locales/es/all';
import { InternalDataService } from './controllers/internal/data/data.service';
import { DashboardComponent } from './view/admistrator/dashboard/dashboard.component';
import { TopBarComponent } from './view/admistrator/component/top-bar/top-bar.component';
import { SideBarComponent } from './view/admistrator/component/side-bar/side-bar.component';
import { HotelsComponent } from './view/admistrator/hotels/hotels.component';
import { RutesComponent } from './view/admistrator/rutes/rutes.component';
import { ErrorsComponent } from './view/admistrator/errors/errors.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    HeaderComponent,
    IndexComponent,
    ReservationListComponent,
    ApproverComponent,
    LodgingComponent,
    RegisterComponent,
    TransportComponent,
    TravelComponent,
    TravelExpensesComponent,
    DashboardComponent,
    TopBarComponent,
    SideBarComponent,
    HotelsComponent,
    RutesComponent,
    ErrorsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    DropDownsModule,
    DateInputsModule,
    CalendarModule,
    IntlModule,
    GridModule,
    RouterModule.forRoot([
      { path: 'client/reservation', component: IndexComponent, pathMatch: 'full' },
      { path: 'client/reservation/list/:data', component: ReservationListComponent },
      { path: 'admistrator/dashboard', component: DashboardComponent },
      { path: 'admistrator/hotels', component: HotelsComponent },
      { path: 'admistrator/rutes', component: RutesComponent },
      { path: 'admistrator/errors', component: ErrorsComponent },
    ])
  ],
  providers: [{ provide: LOCALE_ID, useValue: 'es-ES' },UserService, ValidationsService, ExternalDataService, ApiService, SessionExternalService, SessionInternalService, GlobalsService, InternalDataService],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }
