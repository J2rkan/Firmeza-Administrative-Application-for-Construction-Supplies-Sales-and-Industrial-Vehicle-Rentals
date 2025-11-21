import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { User, AuthResponse } from '../types';
import { authApi } from '../services/api';

interface AuthState {
    user: User | null;
    isAuthenticated: boolean;
    login: (email: string, password: string) => Promise<void>;
    register: (email: string, password: string, confirmPassword: string) => Promise<void>;
    logout: () => void;
    initializeAuth: () => void;
}

export const useAuthStore = create<AuthState>()(
    persist(
        (set) => ({
            user: null,
            isAuthenticated: false,

            login: async (email: string, password: string) => {
                try {
                    const response: AuthResponse = await authApi.login({ email, password });

                    const user: User = {
                        email: response.email,
                        roles: response.roles,
                        token: response.token,
                    };

                    localStorage.setItem('token', response.token);
                    localStorage.setItem('user', JSON.stringify(user));

                    set({ user, isAuthenticated: true });
                } catch (error) {
                    console.error('Login error:', error);
                    throw error;
                }
            },

            register: async (email: string, password: string, confirmPassword: string) => {
                try {
                    const response: AuthResponse = await authApi.register({
                        email,
                        password,
                        confirmPassword,
                    });

                    const user: User = {
                        email: response.email,
                        roles: response.roles,
                        token: response.token,
                    };

                    localStorage.setItem('token', response.token);
                    localStorage.setItem('user', JSON.stringify(user));

                    set({ user, isAuthenticated: true });
                } catch (error) {
                    console.error('Register error:', error);
                    throw error;
                }
            },

            logout: () => {
                localStorage.removeItem('token');
                localStorage.removeItem('user');
                set({ user: null, isAuthenticated: false });
            },

            initializeAuth: () => {
                const token = localStorage.getItem('token');
                const userStr = localStorage.getItem('user');

                if (token && userStr) {
                    try {
                        const user = JSON.parse(userStr) as User;
                        set({ user, isAuthenticated: true });
                    } catch (error) {
                        console.error('Error parsing user from localStorage:', error);
                        localStorage.removeItem('token');
                        localStorage.removeItem('user');
                    }
                }
            },
        }),
        {
            name: 'auth-storage',
        }
    )
);
