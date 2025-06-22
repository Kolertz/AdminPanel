import { useState } from 'react';
import { createTag, deleteTag } from '../api/api';

export default function TagManager({ tags, onTagChange }) {
    const [newTagName, setNewTagName] = useState('');

    const handleCreateTag = async () => {
        if (!newTagName.trim()) return;
        try {
            await createTag({ name: newTagName });
            setNewTagName('');
            onTagChange();
        } catch (error) {
            console.error('Error creating tag:', error);
        }
    };

    const handleDeleteTag = async (id) => {
        try {
            await deleteTag(id);
            onTagChange();
        } catch (error) {
            console.error('Error deleting tag:', error);
        }
    };

    return (
        <div className="bg-white p-4 rounded-lg shadow mb-6">
            <h2 className="text-xl font-semibold mb-4">Manage Tags</h2>
            <div className="flex mb-4">
                <input
                    type="text"
                    value={newTagName}
                    onChange={(e) => setNewTagName(e.target.value)}
                    placeholder="New tag name"
                    className="border p-2 rounded-l flex-grow"
                />
                <button
                    onClick={handleCreateTag}
                    className="bg-blue-500 text-white px-4 py-2 rounded-r hover:bg-blue-600"
                >
                    Add Tag
                </button>
            </div>
            <ul>
                {tags.map((tag) => (
                    <li key={tag.id} className="flex justify-between items-center p-2 border-b">
                        <span>{tag.name}</span>
                        <button
                            onClick={() => handleDeleteTag(tag.id)}
                            className="text-red-500 hover:text-red-700"
                        >
                            Delete
                        </button>
                    </li>
                ))}
            </ul>
        </div>
    );
}