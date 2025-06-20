const API_URL = 'http://localhost:5000';

// Auth functions
export const login = async (email, password) => {
    const response = await fetch(`${API_URL}/auth/login`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, password }),
    });
    return response.json();
};

export const refreshToken = async (refreshToken) => {
    const response = await fetch(`${API_URL}/auth/refresh`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ refreshToken }),
    });
    return response.json();
};

// Client functions
export const getClients = async () => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_URL}/clients`, {
        headers: {
            'Authorization': `Bearer ${token}`,
        },
    });
    return response.json();
};

export const getClient = async (id) => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_URL}/clients/${id}`, {
        headers: {
            'Authorization': `Bearer ${token}`,
        },
    });
    return response.json();
};

export const createClient = async (client) => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_URL}/clients`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify(client),
    });
    return response.json();
};

export const updateClient = async (id, client) => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_URL}/clients/${id}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify(client),
    });
    return response.json();
};

export const deleteClient = async (id) => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_URL}/clients/${id}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${token}`,
        },
    });

    // Handle empty response (204 No Content)
    if (response.status === 204) {
        return;
    }

    return response.json();
};

// Payment functions
export const getPayments = async (take = 5) => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_URL}/payments?take=${take}`, {
        headers: {
            'Authorization': `Bearer ${token}`,
        },
    });
    return response.json();
};

// Rate functions
export const getRate = async () => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_URL}/rate`, {
        headers: {
            'Authorization': `Bearer ${token}`,
        },
    });
    return response.json();
};

export const updateRate = async (value) => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_URL}/rate`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify({ value }),
    });
    return response.json();
};