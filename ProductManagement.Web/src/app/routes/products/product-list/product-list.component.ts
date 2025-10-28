import { Component, OnInit, ViewChild } from '@angular/core';
import { ProductService } from '../../../core/services/product.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  displayedColumns = ['id','sku','name','price','actions'];
  data: any[] = [];
  total = 0;
  page = 1;
  pageSize = 10;
  search = '';
  includeDeleted = false;
  loading = false;

  private searchChanged$ = new Subject<string>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(private productService: ProductService, private router: Router, private snack: MatSnackBar) {}

  ngOnInit(): void {
    this.searchChanged$.pipe(
      debounceTime(350),
      distinctUntilChanged()
    ).subscribe(() => {
      this.page = 1;
      this.load();
    });
    this.load();
  }

  load() {
    this.loading = true;
    this.productService.getAll(this.search, this.page, this.pageSize, this.includeDeleted)
      .subscribe({
        next: res => {
          this.data = res.items;
          this.total = res.total;
          this.loading = false;
        },
        error: _ => {
          this.loading = false;
          this.snack.open('Failed to load data', undefined, { duration: 3000, panelClass: 'snack-error', horizontalPosition: 'right', verticalPosition: 'top' });
        }
      });
  }

  onSearchChange(value: string) {
    this.search = value;
    this.searchChanged$.next(this.search);
  }

  onPage(event: PageEvent) {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.load();
  }

  onToggleDeleted(checked: boolean) {
    this.includeDeleted = checked;
    this.page = 1;
    this.load();
  }

  add() {
    this.router.navigate(['products/add']);
  }

  edit(row: any) {
    this.router.navigate(['products/edit', row.id]);
  }

  delete(row: any) {
    this.productService.softDelete(row.id).subscribe({
      next: () => {
        this.snack.open('Deleted successfully', undefined, { duration: 2000, panelClass: 'snack-success', horizontalPosition: 'right', verticalPosition: 'top' });
        this.load();
      },
      error: _ => this.snack.open('Delete failed', undefined, { duration: 3000, panelClass: 'snack-error', horizontalPosition: 'right', verticalPosition: 'top' })
    });
  }

  exportCsv() {
    const headers = ['Id','SKU','Name','Price'];
    const rows = this.data.map(x => [x.id, x.sku, x.name, x.price]);
    const csv = [headers, ...rows].map(r => r.join(',')).join('\n');
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'products.csv';
    a.click();
    URL.revokeObjectURL(url);
  }
}
