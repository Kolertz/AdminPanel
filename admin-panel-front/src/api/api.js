import api from './auth';

// Client functions
export const getClients = () => api.get('/clients').then(res => res.data);

export const getClient = (id) => api.get(`/clients/${id}`).then(res => res.data);

export const createClient = (client) => api.post('/clients', client).then(res => res.data);

export const updateClient = (id, client) => api.put(`/clients/${id}`, client).then(res => res.data);

export const deleteClient = (id) => api.delete(`/clients/${id}`).then(res => res.data);

// Payment functions
export const getPayments = (take = 5) => api.get(`/payments?take=${take}`).then(res => res.data);

// Rate functions
export const getRate = () => api.get('/rate').then(res => res.data);

export const updateRate = (value) => api.post('/rate', { value }).then(res => res.data);

// Tag functions
export const getTags = () => api.get('/tags').then(res => res.data);

export const createTag = (tag) => api.post('/tags', tag).then(res => res.data);

export const deleteTag = (id) => api.delete(`/tags/${id}`).then(res => res.data);

// Client Tag functions
export const getClientTags = (clientId) => api.get(`/clients/${clientId}/tags`).then(res => res.data);

export const addTagToClient = (clientId, tagId) => api.post(`/clients/${clientId}/tags`, tagId).then(res => res.data);

export const removeTagFromClient = (clientId, tagId) => api.delete(`/clients/${clientId}/tags/${tagId}`).then(res => res.data);