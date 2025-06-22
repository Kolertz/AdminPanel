import axios from 'axios';

const API_URL = 'http://localhost:5000';

// Создаем экземпляр axios
const api = axios.create({
    baseURL: API_URL,
});

// Интерцептор для добавления токена к запросам
api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// Интерцептор для обработки 401 ошибки и обновления токена
api.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;

        if (error.response.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;

            try {
                const refreshToken = localStorage.getItem('refreshToken');
                const response = await axios.post(`${API_URL}/auth/refresh`, { refreshToken });

                const { token, refreshToken: newRefreshToken } = response.data;
                localStorage.setItem('token', token);
                localStorage.setItem('refreshToken', newRefreshToken);

                originalRequest.headers.Authorization = `Bearer ${token}`;
                return api(originalRequest);
            } catch (refreshError) {
                // Если обновление токена не удалось, делаем логаут
                localStorage.removeItem('token');
                localStorage.removeItem('refreshToken');
                window.location.href = '/login';
                return Promise.reject(refreshError);
            }
        }

        return Promise.reject(error);
    }
);

export const login = async (email, password) => {
    const response = await api.post('/auth/login', { email, password });
    return response.data;
};

export const refreshToken = async (refreshToken) => {
    const response = await api.post('/auth/refresh', { refreshToken });
    return response.data;
};

export default api;