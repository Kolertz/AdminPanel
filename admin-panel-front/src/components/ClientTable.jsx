import { useEffect, useState } from 'react';
import { getClients, deleteClient } from '../api';
import ClientForm from './ClientForm';

export default function ClientTable() {
    const [clients, setClients] = useState([]);
    const [editingClient, setEditingClient] = useState(null);
    const [showForm, setShowForm] = useState(false);

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

    const handleDelete = async (id) => {
        try {
            await deleteClient(id);
            fetchClients();
        } catch (error) {
            console.error('Error deleting client:', error);
        }
    };

    const handleEdit = (client) => {
        setEditingClient(client);
        setShowForm(true);
    };

    const handleCreate = () => {
        setEditingClient(null);
        setShowForm(true);
    };

    const handleFormSuccess = () => {
        setShowForm(false);
        fetchClients();
    };

    return (
        <div>
            <div className="flex justify-between items-center mb-4">
                <h2 className="text-xl font-semibold">Clients</h2>
                <button
                    onClick={handleCreate}
                    className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600"
                >
                    Add Client
                </button>
            </div>

            {showForm ? (
                <ClientForm
                    client={editingClient}
                    onSuccess={handleFormSuccess}
                    onCancel={() => setShowForm(false)}
                />
            ) : (
                <div className="overflow-x-auto">
                    <table className="min-w-full bg-white">
                        <thead>
                            <tr>
                                <th className="py-2 px-4 border-b">Name</th>
                                <th className="py-2 px-4 border-b">Email</th>
                                <th className="py-2 px-4 border-b">Balance (T)</th>
                                <th className="py-2 px-4 border-b">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {clients.map((client) => (
                                <tr key={client.id}>
                                    <td className="py-2 px-4 border-b">{client.name}</td>
                                    <td className="py-2 px-4 border-b">{client.email}</td>
                                    <td className="py-2 px-4 border-b">{client.balance}</td>
                                    <td className="py-2 px-4 border-b">
                                        <button
                                            onClick={() => handleEdit(client)}
                                            className="mr-2 text-blue-500 hover:text-blue-700"
                                        >
                                            Edit
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
            )}
        </div>
    );
}