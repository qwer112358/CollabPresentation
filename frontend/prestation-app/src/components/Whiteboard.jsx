import { Stage, Layer, Rect } from 'react-konva';
import { useState } from 'react';
import axios from 'axios';

function Whiteboard({ presentationId, slides, onSlideAdded, users }) {
  const [selectedTool, setSelectedTool] = useState('brush');
  const [currentSlide, setCurrentSlide] = useState(slides[0]);

  const handleAddSlide = async () => {
    try {
      // Добавление слайда на сервер
      const response = await axios.post(
        `/api/presentations/${presentationId}/slides`,
        {}
      );
      onSlideAdded(response.data);
    } catch (error) {
      console.error('Error adding slide:', error);
    }
  };

  return (
    <div className="whiteboard-container">
      <div className="toolbar">
        <button onClick={() => setSelectedTool('brush')}>Brush</button>
        <button onClick={() => setSelectedTool('eraser')}>Eraser</button>
        <button onClick={handleAddSlide}>Add Slide</button>
      </div>
      <div className="slides-panel">
        {slides.map((slide, index) => (
          <button key={index} onClick={() => setCurrentSlide(slide)}>
            Slide {index + 1}
          </button>
        ))}
      </div>
      <div className="whiteboard">
        <Stage width={800} height={600}>
          <Layer>
            <Rect width={800} height={600} fill="white" />
            {/* Здесь отображаются элементы слайда */}
          </Layer>
        </Stage>
      </div>
      <div className="users-panel">
        <h3>Connected Users</h3>
        <ul>
          {users.map((user, index) => (
            <li key={index}>{user.nickname}</li>
          ))}
        </ul>
      </div>
    </div>
  );
}

export default Whiteboard;
