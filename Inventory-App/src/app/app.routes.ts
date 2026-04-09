import { Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { InventoryFormComponent } from './features/inventory-form/inventory-form.component';
import { CatalogManagerComponent } from './features/catalog-manager/catalog-manager.component';

export const routes: Routes = [
  { 
    path: 'dashboard', 
    component: DashboardComponent,
    title: 'Inventario - Dashboard'
  },
  { 
    path: 'movimientos', 
    component: InventoryFormComponent,
    title: 'Inventario - Registrar Movimiento'
  },
  { 
    path: 'catalogos', 
    component: CatalogManagerComponent,
    title: 'Inventario - Gestión de Catálogos'
  },
  { 
    path: '', 
    redirectTo: 'dashboard', 
    pathMatch: 'full' 
  },
  { 
    path: '**', 
    redirectTo: 'dashboard' 
  }
];