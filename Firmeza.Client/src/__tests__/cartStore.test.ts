import { describe, it, expect } from 'vitest';
import { useCartStore } from '../store/cartStore';
import type { Product } from '../types';

describe('Cart Store', () => {
    it('should calculate total correctly', () => {
        const store = useCartStore.getState();

        // Clear cart first
        store.clearCart();

        // Create test products
        const product1: Product = {
            id: 1,
            name: 'Cemento',
            description: 'Cemento Portland',
            price: 25.50,
            stock: 100
        };

        const product2: Product = {
            id: 2,
            name: 'Arena',
            description: 'Arena fina',
            price: 15.00,
            stock: 200
        };

        // Add items to cart
        store.addItem(product1, 2); // 2 x 25.50 = 51.00
        store.addItem(product2, 3); // 3 x 15.00 = 45.00

        // Calculate expected total
        const expectedTotal = (2 * 25.50) + (3 * 15.00); // 96.00

        // Get actual total
        const actualTotal = store.getTotal();

        // Assert
        expect(actualTotal).toBe(expectedTotal);
    });

    it('should update quantity correctly', () => {
        const store = useCartStore.getState();
        store.clearCart();

        const product: Product = {
            id: 1,
            name: 'Test Product',
            description: 'Test',
            price: 10.00,
            stock: 50
        };

        store.addItem(product, 1);
        store.updateQuantity(1, 5);

        const items = store.items;
        expect(items[0].quantity).toBe(5);
    });

    it('should remove item from cart', () => {
        const store = useCartStore.getState();
        store.clearCart();

        const product: Product = {
            id: 1,
            name: 'Test Product',
            description: 'Test',
            price: 10.00,
            stock: 50
        };

        store.addItem(product, 1);
        expect(store.items.length).toBe(1);

        store.removeItem(1);
        expect(store.items.length).toBe(0);
    });

    it('should get item count correctly', () => {
        const store = useCartStore.getState();
        store.clearCart();

        const product1: Product = {
            id: 1,
            name: 'Product 1',
            description: 'Test',
            price: 10.00,
            stock: 50
        };

        const product2: Product = {
            id: 2,
            name: 'Product 2',
            description: 'Test',
            price: 20.00,
            stock: 30
        };

        store.addItem(product1, 2);
        store.addItem(product2, 3);

        expect(store.getItemCount()).toBe(5); // 2 + 3
    });
});
