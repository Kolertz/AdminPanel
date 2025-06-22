import { useEffect, useState } from 'react';
import { getRate, updateRate } from '../api/api';

export default function RateDisplay() {
  const [rate, setRate] = useState(0);
  const [newRate, setNewRate] = useState('');

  useEffect(() => {
    const fetchRate = async () => {
      try {
        const data = await getRate();
        setRate(data.value);
      } catch (error) {
        console.error('Error fetching rate:', error);
      }
    };
    fetchRate();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const updatedRate = await updateRate(parseFloat(newRate));
      setRate(updatedRate.value);
      setNewRate('');
    } catch (error) {
      console.error('Error updating rate:', error);
    }
  };

  return (
    <div className="bg-gray-100 p-4 rounded-lg">
      <h2 className="text-lg font-semibold mb-2">Token Rate</h2>
      <p className="mb-4">Current rate: {rate} T per 1 USD</p>
      <form onSubmit={handleSubmit} className="flex gap-2">
        <input
          type="number"
          step="0.01"
          value={newRate}
          onChange={(e) => setNewRate(e.target.value)}
          placeholder="New rate"
          className="px-2 py-1 border rounded"
          required
        />
        <button
          type="submit"
          className="bg-blue-500 text-white px-4 py-1 rounded hover:bg-blue-600"
        >
          Update
        </button>
      </form>
    </div>
  );
}