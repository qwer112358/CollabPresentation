import { useState } from 'react';
import axios from 'axios';

function ApplicationUserForm({ onSubmit }) {
  const [nickname, setNickname] = useState('');
  const port = 'http://localhost:5000/api/';

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      // Отправка никнейма на сервер
      const response = await axios.post(
        `${port}ApplicationUser?nickname=${nickname}`,
        { nickname }
      );
      onSubmit(response.data); // Передача данных обратно в App
    } catch (error) {
      console.error('Error submitting nickname:', error);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <label htmlFor="nickname">Enter your nickname:</label>
      <input
        id="nickname"
        type="text"
        value={nickname}
        onChange={(e) => setNickname(e.target.value)}
        required
      />
      <button type="submit">Submit</button>
    </form>
  );
}

export default ApplicationUserForm;
