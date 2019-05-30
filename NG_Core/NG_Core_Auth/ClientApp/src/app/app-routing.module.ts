import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ProductsComponent } from './products/products.component';

const routes: Routes = [];

@NgModule({
    imports: [RouterModule.forRoot([
    {path: "home", component: HomeComponent},
    {path: "", component: HomeComponent, pathMatch: 'full'},
    {path: "**", redirectTo: '/home'},
    {path: "login", component: LoginComponent, pathMatch: 'full'},
    {path: "register", component: RegisterComponent, pathMatch: 'full'},
    {path: "products", component: ProductsComponent, pathMatch: 'full'}
        ])],
  exports: [RouterModule]
})
export class AppRoutingModule { }
