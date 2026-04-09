export interface Category {
  id: number;
  name: string;
  description: string;
}

export interface Product {
  id?: number;
  code: string;
  name: string;
  description: string;
  categoryId: number;
  minimumStock: number;
  category?: Category;
}

export interface Warehouse {
  id?: number;
  name: string;
  location: string;
}

export interface StockSummary {
  productCode: string;
  productName: string;
  warehouseName: string;
  currentStock: number;
  minimumStock: number;
}

export interface MovementRequest {
  productId: number;
  warehouseId: number;
  quantity: number;
  type: 'Entrada' | 'Salida';
}