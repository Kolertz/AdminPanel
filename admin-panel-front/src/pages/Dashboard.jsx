import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import ClientTable from '../components/ClientTable';
import RateDisplay from '../components/RateDisplay';
import PaymentHistory from '../components/PaymentHistory';
import TagManager from '../components/TagManager';

export default function Dashboard() {
    const navigate = useNavigate();

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (!token) {
            navigate('/login');
        }
    }, [navigate]);

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-2xl font-bold mb-6">Admin Dashboard</h1>
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <div className="lg:col-span-2">
                    <TagManager />
                    <ClientTable />
                    <PaymentHistory />
                </div>
                <div>
                    <RateDisplay />
                </div>
            </div>
        </div>
    );
}