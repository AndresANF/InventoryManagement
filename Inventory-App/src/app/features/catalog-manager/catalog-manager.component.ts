import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../../services/inventory.service';
import { Product, Warehouse, Category } from '../../models/inventory.models';

@Component({
  selector: 'app-catalog-manager',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './catalog-manager.component.html'
})
export class CatalogManagerComponent implements OnInit {
  private invService = inject(InventoryService);

  // Señales para el estado de las listas
  products = signal<Product[]>([]);
  warehouses = signal<Warehouse[]>([]);
  categories = signal<Category[]>([]);

  // Formularios inicializados con los métodos reset
  productForm: Product = this.resetProduct();
  warehouseForm: Warehouse = this.resetWarehouse();
  categoryForm: Category = this.resetCategory();

  ngOnInit() {
    this.loadData();
  }

  // Método único para refrescar toda la información de la vista
  loadData() {
    this.invService.getProducts().subscribe(p => this.products.set(p));
    this.invService.getCategories().subscribe(c => this.categories.set(c));
    this.invService.getWarehouses().subscribe(w => this.warehouses.set(w));
  }

  // --- GESTIÓN DE PRODUCTOS ---
  saveProduct() {
    if (!this.isValidProduct()) return;
    const action = this.productForm.id
      ? this.invService.updateProduct(this.productForm)
      : this.invService.createProduct(this.productForm);

    action.subscribe({
      next: () => {
        this.productForm = this.resetProduct();
        this.loadData();
      },
      error: (err) => alert("Error al guardar Producto: " + (err.error?.message || err.message))
    });
  }

  deleteProduct(id: number | undefined) {
    if (!id) return;
    if (confirm('¿Deseas eliminar este producto del catálogo?')) {
      this.invService.deleteProduct(id).subscribe({
        next: () => {
          alert('Producto eliminado');
          this.loadData();
        },
        error: (err) => alert('Error: ' + (err.error?.message || 'No se pudo eliminar'))
      });
    }
  }

  // --- GESTIÓN DE ALMACENES ---
  saveWarehouse() {
    if (!this.isValidWarehouse()) return;
    const action = this.warehouseForm.id
      ? this.invService.updateWarehouse(this.warehouseForm)
      : this.invService.createWarehouse(this.warehouseForm);

    action.subscribe({
      next: () => {
        this.warehouseForm = this.resetWarehouse();
        this.loadData();
      },
      error: (err) => alert("Error al guardar Almacén: " + (err.error?.message || err.message))
    });
  }

  deleteWarehouse(id: number | undefined) {
    if (!id) return;
    if (confirm('¿Eliminar este almacén definitivamente?')) {
      this.invService.deleteWarehouse(id).subscribe({
        next: () => {
          alert('Almacén eliminado');
          this.loadData();
        },
        error: (err) => alert('Error: ' + (err.error?.message || 'No se pudo eliminar'))
      });
    }
  }

  // --- GESTIÓN DE CATEGORÍAS ---
  saveCategory() {
    if (!this.isValidCategory()) return;
    const action = (this.categoryForm.id && this.categoryForm.id > 0)
      ? this.invService.updateCategory(this.categoryForm)
      : this.invService.createCategory(this.categoryForm);

    action.subscribe({
      next: () => {
        this.categoryForm = this.resetCategory();
        this.loadData();
      },
      error: (err) => alert("Error al guardar Categoría: " + (err.error?.message || err.message))
    });
  }

  deleteCategory(id: number | undefined) {
    if (!id) return;
    if (confirm('¿Estás seguro de eliminar esta categoría?')) {
      this.invService.deleteCategory(id).subscribe({
        next: () => {
          alert('Categoría eliminada con éxito');
          this.loadData(); // Refresca las listas
        },
        error: (err) => alert('Error: ' + (err.error?.message || 'No se pudo eliminar'))
      });
    }
  }

  // --- FUNCIONES DE APOYO Y RESET ---
  editProduct(p: Product) { this.productForm = { ...p }; }
  editWarehouse(w: Warehouse) { this.warehouseForm = { ...w }; }
  editCategory(c: Category) { this.categoryForm = { ...c }; }

  private resetProduct(): Product {
    return { code: '', name: '', description: '', categoryId: 0, minimumStock: 0 };
  }
  private resetWarehouse(): Warehouse { return { name: '', location: '' }; }
  private resetCategory(): Category { return { id: 0, name: '', description: '' }; }

  // --- VALIDACIONES DE NEGOCIO ---
  private isValidProduct() {
    const p = this.productForm;
    return p.code.trim().length >= 3 && p.code.trim().length <= 15 &&
      p.name.trim().length >= 3 && p.name.trim().length <= 50 &&
      p.description.trim().length >= 10 && p.description.trim().length <= 200 &&
      p.categoryId > 0 &&
      p.minimumStock >= 0;
  }

  private isValidWarehouse() {
    const w = this.warehouseForm;
    return w.name.trim().length >= 3 && w.name.trim().length <= 30 &&
      w.location.trim().length >= 3 && w.location.trim().length <= 50;
  }

  private isValidCategory() {
    const c = this.categoryForm;
    return c.name.trim().length >= 3 && c.name.trim().length <= 20 &&
      c.description.trim().length >= 5 && c.description.trim().length <= 100;
  }
}