import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.css']
})
export class ProductFormComponent implements OnInit {
  isEdit = false;
  id?: number;

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    sku: ['', [Validators.required]],
    description: [''],
    price: [0, [Validators.required, Validators.min(0.01)]]
  });

  constructor(private fb: FormBuilder, private route: ActivatedRoute, private router: Router, private productService: ProductService, private snack: MatSnackBar) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEdit = true;
      this.id = +idParam;
      this.productService.getById(this.id).subscribe(p => {
        this.form.patchValue({
          name: p.name,
          sku: p.sku,
          description: p.description,
          price: p.price
        });
      });
    }
  }

  save() {
    if (this.form.invalid) return;
    const value = this.form.value;
    if (this.isEdit && this.id) {
      this.productService.update(this.id, value).subscribe({
        next: _ => { this.snack.open('Saved successfully', undefined, { duration: 2000, panelClass: 'snack-success', horizontalPosition: 'right', verticalPosition: 'top' }); this.router.navigate(['products']); },
        error: _ => this.snack.open('Update failed', undefined, { duration: 3000, panelClass: 'snack-error', horizontalPosition: 'right', verticalPosition: 'top' })
      });
    } else {
      this.productService.create(value).subscribe({
        next: _ => { this.snack.open('Created successfully', undefined, { duration: 2000, panelClass: 'snack-success', horizontalPosition: 'right', verticalPosition: 'top' }); this.router.navigate(['products']); },
        error: _ => this.snack.open('Create failed', undefined, { duration: 3000, panelClass: 'snack-error', horizontalPosition: 'right', verticalPosition: 'top' })
      });
    }
  }

  cancel() {
    this.router.navigate(['products']);
  }
}
