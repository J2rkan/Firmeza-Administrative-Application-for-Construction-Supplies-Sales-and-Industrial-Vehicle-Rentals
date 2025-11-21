import { useEffect, useState } from 'react';
import { productsApi } from '../services/api';
import { useCartStore } from '../store/cartStore';
import type { Product } from '../types';
import ProductCard from '../components/ProductCard';
import Navbar from '../components/Navbar';

export default function Products() {
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(true);
    const [search, setSearch] = useState('');
    const [error, setError] = useState('');

    const addItem = useCartStore((state) => state.addItem);

    useEffect(() => {
        loadProducts();
    }, [search]);

    const loadProducts = async () => {
        try {
            setLoading(true);
            const data = await productsApi.getAll(search);
            setProducts(data);
            setError('');
        } catch (err) {
            setError('Error al cargar productos');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handleAddToCart = (product: Product) => {
        addItem(product, 1);
    };

    return (
        <div className="min-h-screen bg-gray-50">
            <Navbar />

            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                <div className="mb-8">
                    <h1 className="text-3xl font-bold text-gray-900 mb-4">
                        Catálogo de Productos
                    </h1>

                    <div className="flex gap-4">
                        <input
                            type="text"
                            placeholder="Buscar productos..."
                            className="input flex-1"
                            value={search}
                            onChange={(e) => setSearch(e.target.value)}
                        />
                    </div>
                </div>

                {error && (
                    <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg mb-6">
                        {error}
                    </div>
                )}

                {loading ? (
                    <div className="text-center py-12">
                        <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
                        <p className="mt-4 text-gray-600">Cargando productos...</p>
                    </div>
                ) : products.length === 0 ? (
                    <div className="text-center py-12">
                        <p className="text-gray-600 text-lg">No se encontraron productos</p>
                    </div>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                        {products.map((product) => (
                            <ProductCard
                                key={product.id}
                                product={product}
                                onAddToCart={handleAddToCart}
                            />
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
}
