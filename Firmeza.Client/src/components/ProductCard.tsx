import type { Product } from '../types';

interface ProductCardProps {
    product: Product;
    onAddToCart: (product: Product) => void;
}

export default function ProductCard({ product, onAddToCart }: ProductCardProps) {
    return (
        <div className="card hover:shadow-lg transition-shadow duration-200">
            <div className="mb-4">
                <h3 className="text-lg font-semibold text-gray-900 mb-2">
                    {product.name}
                </h3>
                <p className="text-gray-600 text-sm line-clamp-2">
                    {product.description}
                </p>
            </div>

            <div className="flex items-center justify-between mb-4">
                <div>
                    <p className="text-2xl font-bold text-primary-600">
                        ${product.price.toFixed(2)}
                    </p>
                </div>
                <div>
                    {product.stock > 0 ? (
                        <span className="badge badge-success">
                            Stock: {product.stock}
                        </span>
                    ) : (
                        <span className="badge badge-danger">
                            Agotado
                        </span>
                    )}
                </div>
            </div>

            <button
                onClick={() => onAddToCart(product)}
                disabled={product.stock === 0}
                className="w-full btn btn-primary disabled:opacity-50 disabled:cursor-not-allowed"
            >
                {product.stock > 0 ? '🛒 Agregar al Carrito' : 'No Disponible'}
            </button>
        </div>
    );
}
