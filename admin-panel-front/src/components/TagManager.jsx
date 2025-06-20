import { useState, useEffect } from 'react';
import { getTags, createTag, deleteTag } from '../api';

export default function TagManager() {
    const [tags, setTags] = useState([]);
    const [newTagName, setNewTagName] = useState('');

    useEffect(() => {
        fetchTags();
    }, []);

    const fetchTags = async () => {
        const data = await getTags();
        setTags(data);
    };

    const handleCreateTag = async () => {
        if (!newTagName.trim()) return;
        await createTag({ name: newTagName });
        setNewTagName('');
        fetchTags();
    };

    const handleDeleteTag = async (id) => {
        await deleteTag(id);
        fetchTags();
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