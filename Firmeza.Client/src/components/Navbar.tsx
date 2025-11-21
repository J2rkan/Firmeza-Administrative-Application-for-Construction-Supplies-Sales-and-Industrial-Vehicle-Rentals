import { Link } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';
import { useCartStore } from '../store/cartStore';

export default function Navbar() {
    const user = useAuthStore((state) => state.user);
    const logout = useAuthStore((state) => state.logout);
    const itemCount = useCartStore((state) => state.getItemCount());

    return (
        <nav className="bg-white shadow-md">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="flex justify-between h-16">
                    <div className="flex items-center">
                        <Link to="/products" className="flex items-center space-x-2">
                            <span className="text-2xl">🏗️</span>
                            <span className="text-xl font-bold text-primary-900">Firmeza</span>
                        </Link>
                    </div>

                    <div className="flex items-center space-x-4">
                        <Link to="/products" className="text-gray-700 hover:text-primary-600 font-medium">
                            Productos
                        </Link>

                        <Link to="/cart" className="relative">
                            <button className="btn btn-outline flex items-center space-x-2">
                                <span>🛒</span>
                                <span>Carrito</span>
                                {itemCount > 0 && (
                                    <span className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full w-6 h-6 flex items-center justify-center text-xs font-bold">
                                        {itemCount}
                                    </span>
                                )}
                            </button>
                        </Link>

                        <div className="flex items-center space-x-3">
                            <span className="text-gray-700">
                                👤 {user?.email}
                            </span>
                            <button
                                onClick={logout}
                                className="btn btn-secondary"
                            >
                                Cerrar Sesión
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </nav>
    );
}
