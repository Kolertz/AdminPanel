import { useState } from 'react';
import { createClient, updateClient } from '../api';

export default function ClientForm({ client, onSuccess, onCancel }) {
    const [formData, setFormData] = useState({
        name: client?.name || '',
        email: client?.email || '',
        balance: client?.balance || 0
    });

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (client?.id) {
                await updateClient(client.id, formData);
            } else {
                await createClient(formData);
            }
            onSuccess();
        } catch (error) {
            console.error('Error saving client:', error);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="bg-white p-4 rounded shadow">
            <div className="mb-4">
                <label className="block text-gray-700 mb-2">Name</label>
                <input
                    type="text"
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    className="w-full p-2 border rounded"
                    required
                />
            </div>
            <div className="mb-4">
                <label className="block text-gray-700 mb-2">Email</label>
                <input
                    type="email"
                    value={formData.email}
                    onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                    className="w-full p-2 border rounded"
                    required
                />
            </div>
            <div className="mb-4">
                <label className="block text-gray-700 mb-2">Balance</label>
                <input
                    type="number"
                    step="0.01"
                    value={formData.balance}
                    onChange={(e) => setFormData({ ...formData, balance: parseFloat(e.target.value) })}
                    className="w-full p-2 border rounded"
                    required
                />
            </div>
            <div className="flex justify-end gap-2">
                <button
                    type="button"
                    onClick={onCancel}
                    className="px-4 py-2 bg-gray-300 rounded hover:bg-gray-400"
                >
                    Cancel
                </button>
                <button
                    type="submit"
                    className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
                >
                    Save
                </button>
            </div>
        </form>
    );
}