import { useState, useEffect } from 'react';
import axios from 'axios';

function PresentationList({ onJoin, onCreate }) {
  const [presentations, setPresentations] = useState([]);
  const [newTitle, setNewTitle] = useState('');

  useEffect(() => {
    // Получение списка презентаций с сервера при загрузке компонента
    const fetchPresentations = async () => {
      try {
        const response = await axios.get(
          'http://localhost:5000/api/presentations'
        );
        setPresentations(response.data);
      } catch (error) {
        console.error('Error fetching presentations:', error);
      }
    };

    fetchPresentations();
  }, []);

  const handleCreate = async (e) => {
    e.preventDefault();
    try {
      // Создание новой презентации на сервере
      const response = await axios.post(
        'http://localhost:5000/api/presentations',
        {
          title: newTitle,
        }
      );
      setPresentations([...presentations, response.data]);
      onCreate(response.data);
    } catch (error) {
      console.error('Error creating presentation:', error);
    }
  };

  return (
    <div>
      <h2>Existing Presentations</h2>
      <ul>
        {presentations.map((presentation, index) => (
          <li key={index}>
            <span>
              {presentation.title} (Owner: {presentation.ownerName})
            </span>
            <button onClick={() => onJoin(presentation)}>Join</button>
          </li>
        ))}
      </ul>

      <h2>Create a New Presentation</h2>
      <form onSubmit={handleCreate}>
        <label htmlFor="newTitle">Presentation Title:</label>
        <input
          id="newTitle"
          type="text"
          value={newTitle}
          onChange={(e) => setNewTitle(e.target.value)}
          required
        />
        <button type="submit">Create</button>
      </form>
    </div>
  );
}

export default PresentationList;
