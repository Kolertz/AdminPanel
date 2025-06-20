import { useEffect, useState } from 'react';
import { getPayments } from '../api';

export default function PaymentHistory() {
    const [payments, setPayments] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchPayments = async () => {
            try {
                const data = await getPayments(5);
                setPayments(data);
                setLoading(false);
            } catch (error) {
                console.error('Error fetching payments:', error);
                setLoading(false);
            }
        };
        fetchPayments();
    }, []);

    if (loading) return <div>Loading...</div>;

    return (
        <div className="mt-6">
            <h2 className="text-xl font-semibold mb-4">Payment History</h2>
            <div className="overflow-x-auto">
                <table className="min-w-full bg-white">
                    <thead>
                        <tr>
                            <th className="py-2 px-4 border-b">Date</th>
                            <th className="py-2 px-4 border-b">Client</th>
                            <th className="py-2 px-4 border-b">Amount</th>
                            <th className="py-2 px-4 border-b">Description</th>
                        </tr>
                    </thead>
                    <tbody>
                        {payments.map((payment) => (
                            <tr key={payment.id}>
                                <td className="py-2 px-4 border-b">
                                    {new Date(payment.date).toLocaleDateString()}
                                </td>
                                <td className="py-2 px-4 border-b">{payment.clientName}</td>
                                <td className="py-2 px-4 border-b">{payment.amount}</td>
                                <td className="py-2 px-4 border-b">{payment.description}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}