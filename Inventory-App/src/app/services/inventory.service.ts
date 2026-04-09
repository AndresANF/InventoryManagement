import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product, Warehouse, Category, StockSummary, MovementRequest } from '../models/inventory.models';

@Injectable({
  providedIn: 'root'
})
export class InventoryService {
  private http = inject(HttpClient);

  // URL base sincronizada con tu puerto de Visual Studio
  private apiUrl = 'https://localhost:7084/api';

  // --- CATEGORÍAS ---
  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/Categories`);
  }

  createCategory(category: Category): Observable<Category> {
    return this.http.post<Category>(`${this.apiUrl}/Categories`, category);
  }

  updateCategory(category: Category): Observable<any> {
    return this.http.put(`${this.apiUrl}/Categories/${category.id}`, category);
  }

  deleteCategory(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/Categories/${id}`);
  }

  // --- PRODUCTOS ---
  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/Products`);
  }

  createProduct(product: Product): Observable<Product> {
    return this.http.post<Product>(`${this.apiUrl}/Products`, product);
  }

  updateProduct(product: Product): Observable<any> {
    return this.http.put(`${this.apiUrl}/Products/${product.id}`, product);
  }

  deleteProduct(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/Products/${id}`);
  }

  // --- ALMACENES ---
  getWarehouses(): Observable<Warehouse[]> {
    return this.http.get<Warehouse[]>(`${this.apiUrl}/Warehouses`);
  }

  createWarehouse(warehouse: Warehouse): Observable<Warehouse> {
    return this.http.post<Warehouse>(`${this.apiUrl}/Warehouses`, warehouse);
  }

  updateWarehouse(warehouse: Warehouse): Observable<any> {
    return this.http.put(`${this.apiUrl}/Warehouses/${warehouse.id}`, warehouse);
  }

  deleteWarehouse(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/Warehouses/${id}`);
  }

  // --- MOVIMIENTOS Y REPORTES ---
  getStockSummary(): Observable<StockSummary[]> {
    return this.http.get<StockSummary[]>(`${this.apiUrl}/InventoryMovements/stock-summary`);
  }

  registerMovement(movement: MovementRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/InventoryMovements/movement`, movement);
  }
}