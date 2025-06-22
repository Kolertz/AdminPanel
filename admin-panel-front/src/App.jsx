import { Suspense, lazy } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import ErrorBoundary from './components/ErrorBoundary'; // Создайте этот компонент (см. предыдущий ответ)
import LoadingSpinner from './components/LoadingSpinner'; // Компонент для загрузки

// Ленивая загрузка страниц (оптимизация)
const Login = lazy(() => import('./pages/Login'));
const Dashboard = lazy(() => import('./pages/Dashboard'));

// Защищённый маршрут (проверяет авторизацию)
const ProtectedRoute = ({ children }) => {
    const token = localStorage.getItem('token');
    return token ? children : <Navigate to="/login" replace />;
};

export default function App() {
    return (
        <BrowserRouter>
            <ErrorBoundary>
                <Suspense fallback={<LoadingSpinner />}>
                    <Routes>
                        <Route path="/login" element={<Login />} />
                        <Route
                            path="/dashboard"
                            element={
                                <ProtectedRoute>
                                    <Dashboard />
                                </ProtectedRoute>
                            }
                        />
                        <Route path="/" element={<Navigate to="/dashboard" replace />} />
                        <Route path="*" element={<Navigate to="/login" replace />} /> {/* 404 */}
                    </Routes>
                </Suspense>
            </ErrorBoundary>
        </BrowserRouter>
    );
}