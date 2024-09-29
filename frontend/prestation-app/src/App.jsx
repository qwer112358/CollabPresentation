// App.js
import { useState } from 'react';
import ApplicationUserForm from './components/ApplicationUserForm';
import PresentationList from './components/PresentationList';
import Whiteboard from './components/Whiteboard';

function App() {
  const [nickname, setNickname] = useState('');
  const [currentPresentation, setCurrentPresentation] = useState(null);

  const handleUserSubmit = (userData) => {
    setNickname(userData.nickname);
  };

  const handleJoinPresentation = (presentation) => {
    setCurrentPresentation(presentation);
  };

  const handleCreatePresentation = (presentation) => {
    setCurrentPresentation(presentation);
  };

  const handleAddSlide = (newSlide) => {
    setCurrentPresentation((prev) => ({
      ...prev,
      slides: [...prev.slides, newSlide],
    }));
  };

  if (!nickname) {
    return <ApplicationUserForm onSubmit={handleUserSubmit} />;
  }

  if (!currentPresentation) {
    return (
      <PresentationList
        onJoin={handleJoinPresentation}
        onCreate={handleCreatePresentation}
      />
    );
  }

  return (
    <Whiteboard
      presentationId={currentPresentation.id}
      slides={currentPresentation.slides}
      onSlideAdded={handleAddSlide}
      users={[{ nickname }]} // Подключенные пользователи
    />
  );
}

export default App;
