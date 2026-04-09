import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../../services/inventory.service';
import { Product, Warehouse, StockSummary } from '../../models/inventory.models';

@Component({
  selector: 'app-inventory-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './inventory-form.component.html'
})
export class InventoryFormComponent implements OnInit {
  private invService = inject(InventoryService);
  protected readonly Number = Number;

  // Estado del movimiento incluyendo el destino opcional
  movement = signal<any>({ 
    productId: 0, 
    warehouseId: 0, 
    destinationWarehouseId: 0,
    quantity: 1, 
    type: 'Entrada' 
  });

  products = signal<Product[]>([]);
  warehouses = signal<Warehouse[]>([]);
  stockSummary = signal<StockSummary[]>([]);

  // VALIDACIÓN: Stock insuficiente (Solo para Salidas)
  isInvalidStock = computed(() => {
    const m = this.movement();
    if (m.type === 'Entrada' || m.productId === 0 || m.warehouseId === 0) return false;

    const selectedProduct = this.products().find(p => p.id == m.productId);
    const selectedWarehouse = this.warehouses().find(w => w.id == m.warehouseId);

    if (!selectedProduct || !selectedWarehouse) return false;

    const currentEntry = this.stockSummary().find(s => 
      s.productName === selectedProduct.name &&
      s.warehouseName === selectedWarehouse.name
    );

    const stockDisponible = currentEntry ? currentEntry.currentStock : 0;
    return m.quantity > stockDisponible;
  });

  // VALIDACIÓN: Mismo almacén (Solo para Salidas/Transferencias)
  isSameWarehouse = computed(() => {
    const m = this.movement();
    return m.type === 'Salida' && m.warehouseId !== 0 && m.warehouseId === m.destinationWarehouseId;
  });

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.invService.getProducts().subscribe(d => this.products.set(d));
    this.invService.getWarehouses().subscribe(d => this.warehouses.set(d));
    this.invService.getStockSummary().subscribe(d => this.stockSummary.set(d));
  }

  updateField(field: string, value: any) {
    const finalValue = (field.includes('Id') || field === 'quantity') ? Number(value) : value;
    this.movement.update(m => ({ ...m, [field]: finalValue }));
  }

  save() {
    const m = this.movement();

    // Bloqueos de seguridad
    if (this.isInvalidStock()) return;
    if (m.type === 'Salida' && (this.isSameWarehouse() || m.destinationWarehouseId === 0)) return;

    this.invService.registerMovement(m).subscribe({
      next: (res: any) => {
        alert(res.message || 'Movimiento registrado con éxito');
        this.loadData();
        this.movement.set({ productId: 0, warehouseId: 0, destinationWarehouseId: 0, quantity: 1, type: 'Entrada' });
      },
      error: (err) => {
        const errorMsg = err.error?.message || 'Error en el servidor';
        alert('Error: ' + errorMsg);
      }
    });
  }
}