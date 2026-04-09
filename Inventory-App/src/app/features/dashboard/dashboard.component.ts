import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InventoryService } from '../../services/inventory.service';
import { StockSummary } from '../../models/inventory.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  private invService = inject(InventoryService);
  stockData = signal<StockSummary[]>([]);

  ngOnInit() { this.load(); }

  load() {
    this.invService.getStockSummary().subscribe(data => this.stockData.set(data));
  }
}