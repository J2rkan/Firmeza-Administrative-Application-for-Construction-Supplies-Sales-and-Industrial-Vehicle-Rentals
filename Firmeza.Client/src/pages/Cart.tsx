import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Navbar from '../components/Navbar';
import { useCartStore } from '../store/cartStore';
import { salesApi } from '../services/api';

export default function Cart() {
    const items = useCartStore((state) => state.items);
    const updateQuantity = useCartStore((state) => state.updateQuantity);
    const removeItem = useCartStore((state) => state.removeItem);
    const clearCart = useCartStore((state) => state.clearCart);
    const getTotal = useCartStore((state) => state.getTotal());

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState(false);

    const navigate = useNavigate();

    const handleCheckout = async () => {
        if (items.length === 0) return;

        setLoading(true);
        setError('');

        try {
            // Create sale
            const saleData = {
                clientId: 1, // This should come from user profile
                saleDetails: items.map(item => ({
                    productId: item.product.id,
                    quantity: item.quantity
                }))
            };

            await salesApi.create(saleData);

            setSuccess(true);
            clearCart();

            setTimeout(() => {
                navigate('/products');
            }, 3000);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Error al procesar la compra');
        } finally {
            setLoading(false);
        }
    };

    if (success) {
        return (
            <div className="min-h-screen bg-gray-50">
                <Navbar />
                <div className="max-w-4xl mx-auto px-4 py-16 text-center">
                    <div className="card">
                        <div className="text-6xl mb-4">✅</div>
                        <h2 className="text-3xl font-bold text-green-600 mb-4">
                            ¡Compra Exitosa!
                        </h2>
                        <p className="text-gray-600 text-lg mb-4">
                            Tu compra ha sido procesada correctamente.
                        </p>
                        <p className="text-gray-600">
                            Recibirás un correo con el comprobante de compra.
                        </p>
                        <p className="text-sm text-gray-500 mt-4">
                            Redirigiendo al catálogo...
                        </p>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50">
            <Navbar />

            <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                <h1 className="text-3xl font-bold text-gray-900 mb-8">
                    Carrito de Compras
                </h1>

                {error && (
                    <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg mb-6">
                        {error}
                    </div>
                )}

                {items.length === 0 ? (
                    <div className="card text-center py-12">
                        <div className="text-6xl mb-4">🛒</div>
                        <h2 className="text-2xl font-semibold text-gray-700 mb-2">
                            Tu carrito está vacío
                        </h2>
                        <p className="text-gray-600 mb-6">
                            Agrega productos desde el catálogo
                        </p>
                        <button
                            onClick={() => navigate('/products')}
                            className="btn btn-primary"
                        >
                            Ver Productos
                        </button>
                    </div>
                ) : (
                    <div className="space-y-6">
                        <div className="card">
                            {items.map((item) => (
                                <div key={item.product.id} className="flex items-center justify-between py-4 border-b last:border-b-0">
                                    <div className="flex-1">
                                        <h3 className="font-semibold text-gray-900">{item.product.name}</h3>
                                        <p className="text-sm text-gray-600">${item.product.price.toFixed(2)} c/u</p>
                                    </div>

                                    <div className="flex items-center space-x-4">
                                        <div className="flex items-center space-x-2">
                                            <button
                                                onClick={() => updateQuantity(item.product.id, item.quantity - 1)}
                                                className="w-8 h-8 rounded-full bg-gray-200 hover:bg-gray-300"
                                            >
                                                -
                                            </button>
                                            <span className="w-12 text-center font-medium">{item.quantity}</span>
                                            <button
                                                onClick={() => updateQuantity(item.product.id, item.quantity + 1)}
                                                className="w-8 h-8 rounded-full bg-gray-200 hover:bg-gray-300"
                                                disabled={item.quantity >= item.product.stock}
                                            >
                                                +
                                            </button>
                                        </div>

                                        <div className="w-24 text-right font-semibold">
                                            ${(item.product.price * item.quantity).toFixed(2)}
                                        </div>

                                        <button
                                            onClick={() => removeItem(item.product.id)}
                                            className="text-red-600 hover:text-red-800"
                                        >
                                            🗑️
                                        </button>
                                    </div>
                                </div>
                            ))}
                        </div>

                        <div className="card">
                            <div className="space-y-2">
                                <div className="flex justify-between text-lg">
                                    <span>Subtotal:</span>
                                    <span>${getTotal.toFixed(2)}</span>
                                </div>
                                <div className="flex justify-between text-lg">
                                    <span>IVA (16%):</span>
                                    <span>${(getTotal * 0.16).toFixed(2)}</span>
                                </div>
                                <div className="border-t pt-2 flex justify-between text-2xl font-bold">
                                    <span>Total:</span>
                                    <span className="text-primary-600">${(getTotal * 1.16).toFixed(2)}</span>
                                </div>
                            </div>

                            <button
                                onClick={handleCheckout}
                                disabled={loading}
                                className="w-full btn btn-primary mt-6 py-3 text-lg disabled:opacity-50"
                            >
                                {loading ? 'Procesando...' : '✓ Finalizar Compra'}
                            </button>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}
