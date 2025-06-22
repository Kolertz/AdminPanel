import { useState, useEffect } from 'react';
import { getClients, deleteClient, getClientTags, addTagToClient, removeTagFromClient } from '../api/api';
import ClientForm from './ClientForm';

export default function ClientTable({ tags, onTagChange }) {
    const [clients, setClients] = useState([]);
    const [selectedClient, setSelectedClient] = useState(null);
    const [clientTags, setClientTags] = useState([]);
    const [newTagId, setNewTagId] = useState('');
    const [showCreateForm, setShowCreateForm] = useState(false);

    useEffect(() => {
        fetchClients();
    }, []);

    const fetchClients = async () => {
        try {
            const data = await getClients();
            setClients(data);
        } catch (error) {
            console.error('Error fetching clients:', error);
        }
    };

    const fetchClientTags = async (clientId) => {
        try {
            const data = await getClientTags(clientId);
            setClientTags(data);
        } catch (error) {
            console.error('Error fetching client tags:', error);
        }
    };

    const handleDelete = async (id) => {
        try {
            await deleteClient(id);
            fetchClients();
        } catch (error) {
            console.error('Error deleting client:', error);
        }
    };

    const handleShowTags = async (client) => {
        setSelectedClient(client);
        await fetchClientTags(client.id);
    };

    const handleAddTag = async () => {
        if (!newTagId || !selectedClient) return;
        try {
            await addTagToClient(selectedClient.id, parseInt(newTagId));
            await fetchClientTags(selectedClient.id);
            setNewTagId('');
            onTagChange();
        } catch (error) {
            console.error('Error adding tag to client:', error);
        }
    };

    const handleRemoveTag = async (tagId) => {
        if (!selectedClient) return;
        try {
            await removeTagFromClient(selectedClient.id, tagId);
            await fetchClientTags(selectedClient.id);
            onTagChange();
        } catch (error) {
            console.error('Error removing tag from client:', error);
        }
    };

    const handleCreateSuccess = () => {
        setShowCreateForm(false);
        fetchClients();
    };

    return (
        <div className="bg-white p-4 rounded-lg shadow mb-6">
            <div className="flex justify-between items-center mb-4">
                <h2 className="text-xl font-semibold">Clients</h2>
                <button
                    onClick={() => setShowCreateForm(true)}
                    className="px-4 py-2 bg-green-500 text-white rounded hover:bg-green-600"
                >
                    Create Client
                </button>
            </div>

            {showCreateForm && (
                <div className="mb-6">
                    <ClientForm
                        onSuccess={handleCreateSuccess}
                        onCancel={() => setShowCreateForm(false)}
                    />
                </div>
            )}

            <div className="overflow-x-auto">
                <table className="min-w-full bg-white">
                    <thead>
                        <tr>
                            <th className="py-2 px-4 border-b">ID</th>
                            <th className="py-2 px-4 border-b">Name</th>
                            <th className="py-2 px-4 border-b">Email</th>
                            <th className="py-2 px-4 border-b">Balance</th>
                            <th className="py-2 px-4 border-b">Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {clients.map((client) => (
                            <tr key={client.id}>
                                <td className="py-2 px-4 border-b">{client.id}</td>
                                <td className="py-2 px-4 border-b">{client.name}</td>
                                <td className="py-2 px-4 border-b">{client.email}</td>
                                <td className="py-2 px-4 border-b">{client.balance}</td>
                                <td className="py-2 px-4 border-b">
                                    <button
                                        onClick={() => handleShowTags(client)}
                                        className="text-blue-500 hover:text-blue-700 mr-2"
                                    >
                                        Tags
                                    </button>
                                    <button
                                        onClick={() => handleDelete(client.id)}
                                        className="text-red-500 hover:text-red-700"
                                    >
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            {selectedClient && (
                <div className="mt-6 p-4 border rounded-lg">
                    <h3 className="text-lg font-medium mb-2">
                        Tags for {selectedClient.name}
                    </h3>
                    <div className="flex mb-4">
                        <select
                            value={newTagId}
                            onChange={(e) => setNewTagId(e.target.value)}
                            className="border p-2 rounded-l flex-grow"
                        >
                            <option value="">Select a tag</option>
                            {tags.map((tag) => (
                                <option key={tag.id} value={tag.id}>
                                    {tag.name}
                                </option>
                            ))}
                        </select>
                        <button
                            onClick={handleAddTag}
                            className="bg-green-500 text-white px-4 py-2 rounded-r hover:bg-green-600"
                        >
                            Add Tag
                        </button>
                    </div>
                    <ul>
                        {clientTags.map((tag) => (
                            <li key={tag.id} className="flex justify-between items-center p-2 border-b">
                                <span>{tag.name}</span>
                                <button
                                    onClick={() => handleRemoveTag(tag.id)}
                                    className="text-red-500 hover:text-red-700"
                                >
                                    Remove
                                </button>
                            </li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
}