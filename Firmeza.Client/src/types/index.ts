// API Types
export interface Product {
    id: number;
    name: string;
    description: string;
    price: number;
    stock: number;
}

export interface Client {
    id: number;
    name: string;
    document: string;
    email: string;
    phone: string;
    address: string;
}

export interface SaleDetail {
    productId: number;
    quantity: number;
}

export interface CreateSaleDto {
    clientId: number;
    saleDetails: SaleDetail[];
}

export interface Sale {
    id: number;
    date: string;
    total: number;
    clientId: number;
    clientName: string;
    saleDetails: SaleDetailResponse[];
}

export interface SaleDetailResponse {
    id: number;
    productId: number;
    productName: string;
    quantity: number;
    unitPrice: number;
    subtotal: number;
}

// Auth Types
export interface LoginDto {
    email: string;
    password: string;
}

export interface RegisterDto {
    email: string;
    password: string;
    confirmPassword: string;
}

export interface AuthResponse {
    token: string;
    email: string;
    roles: string[];
    expiration: string;
}

// Cart Types
export interface CartItem {
    product: Product;
    quantity: number;
}

export interface Cart {
    items: CartItem[];
    total: number;
    itemCount: number;
}

// User State
export interface User {
    email: string;
    roles: string[];
    token: string;
}
