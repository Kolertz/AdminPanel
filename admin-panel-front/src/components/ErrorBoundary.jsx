import { Component } from 'react';

class ErrorBoundary extends Component {
    state = { hasError: false, error: null };

    static getDerivedStateFromError(error) {
        return { hasError: true, error };
    }

    componentDidCatch(error, errorInfo) {
        console.error("Error caught by ErrorBoundary:", error, errorInfo);
    }

    render() {
        if (this.state.hasError) {
            return (
                <div className="p-4 bg-red-100 border border-red-400 text-red-700 rounded">
                    <h2>Что-то пошло не так 😕</h2>
                    <p>{this.state.error.message}</p>
                    <button
                        onClick={() => window.location.reload()}
                        className="mt-2 bg-red-500 text-white px-3 py-1 rounded"
                    >
                        Перезагрузить страницу
                    </button>
                </div>
            );
        }
        return this.props.children;
    }
}

export default ErrorBoundary;