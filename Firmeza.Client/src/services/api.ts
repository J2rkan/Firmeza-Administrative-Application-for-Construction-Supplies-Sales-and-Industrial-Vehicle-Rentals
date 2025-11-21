import axios, { AxiosError } from 'axios';
import type {
    Product,
    LoginDto,
    RegisterDto,
    AuthResponse,
    CreateSaleDto,
    Sale,
    Client,
} from '../types';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5001/api';

// Create axios instance
const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Request interceptor to add JWT token
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// Response interceptor to handle errors
api.interceptors.response.use(
    (response) => response,
    (error: AxiosError) => {
        if (error.response?.status === 401) {
            // Token expired or invalid
            localStorage.removeItem('token');
            localStorage.removeItem('user');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

// Auth API
export const authApi = {
    login: async (data: LoginDto): Promise<AuthResponse> => {
        const response = await api.post<AuthResponse>('/auth/login', data);
        return response.data;
    },

    register: async (data: RegisterDto): Promise<AuthResponse> => {
        const response = await api.post<AuthResponse>('/auth/register', data);
        return response.data;
    },
};

// Products API
export const productsApi = {
    getAll: async (search?: string): Promise<Product[]> => {
        const response = await api.get<Product[]>('/products', {
            params: { search },
        });
        return response.data;
    },

    getById: async (id: number): Promise<Product> => {
        const response = await api.get<Product>(`/products/${id}`);
        return response.data;
    },
};

// Sales API
export const salesApi = {
    create: async (data: CreateSaleDto): Promise<Sale> => {
        const response = await api.post<Sale>('/sales', data);
        return response.data;
    },

    getById: async (id: number): Promise<Sale> => {
        const response = await api.get<Sale>(`/sales/${id}`);
        return response.data;
    },

    getByClient: async (clientId: number): Promise<Sale[]> => {
        const response = await api.get<Sale[]>(`/sales/by-client/${clientId}`);
        return response.data;
    },
};

// Clients API
export const clientsApi = {
    getById: async (id: number): Promise<Client> => {
        const response = await api.get<Client>(`/clients/${id}`);
        return response.data;
    },
};

export default api;
