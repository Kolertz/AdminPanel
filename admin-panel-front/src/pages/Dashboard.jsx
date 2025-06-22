import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getTags } from '../api/api';
import ClientTable from '../components/ClientTable';
import RateDisplay from '../components/RateDisplay';
import PaymentHistory from '../components/PaymentHistory';
import TagManager from '../components/TagManager';
import LogoutButton from '../components/LogoutButton';

export default function Dashboard() {
    const navigate = useNavigate();
    const [tags, setTags] = useState([]);

    useEffect(() => {
        const token = localStorage.getItem('token');
        const refreshToken = localStorage.getItem('refreshToken');

        if (!token || !refreshToken) {
            navigate('/login');
        } else {
            fetchTags();
        }
    }, [navigate]);

    const fetchTags = async () => {
        try {
            const data = await getTags();
            setTags(data);
        } catch (error) {
            console.error('Error fetching tags:', error);
        }
    };

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-2xl font-bold mb-6">Admin Dashboard</h1>
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <div className="lg:col-span-2">
                    <TagManager tags={tags} onTagChange={fetchTags} />
                    <ClientTable tags={tags} onTagChange={fetchTags} />
                    <PaymentHistory />
                </div>
                <div>
                    <RateDisplay />
                    <LogoutButton />
                </div>
            </div>
        </div>
    );
}