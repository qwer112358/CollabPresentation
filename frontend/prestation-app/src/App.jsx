// App.js
import { useState, useEffect } from 'react';
import ApplicationUserForm from './components/ApplicationUserForm';
import PresentationList from './components/PresentationList';
import Whiteboard from './components/Whiteboard';
import * as signalR from '@microsoft/signalr';

function App() {
  const [nickname, setNickname] = useState('');
  const [currentPresentation, setCurrentPresentation] = useState(null);
  const [connection, setConnection] = useState(null);
  const port = 'http://localhost:5000/api/';

  useEffect(() => {
    if (nickname) {
      const newConnection = new signalR.HubConnectionBuilder()
        .withUrl(`http://localhost:5000/whiteboardHub`)
        .withAutomaticReconnect()
        .build();

      setConnection(newConnection);
    }
  }, [nickname]);

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => {
          console.log('Connected to SignalR hub');
        })
        .catch((error) => console.error('SignalR connection error:', error));

      connection.on('UserJoined', (nickname) => {
        console.log(`${nickname} has joined the presentation.`);
      });

      connection.on('UserLeft', (nickname) => {
        console.log(`${nickname} has left the presentation.`);
      });

      connection.on('ReceiveDrawing', (drawingData) => {
        console.log('Received drawing data:', drawingData);
        // Здесь можно обработать полученные данные рисунка
      });
    }
  }, [connection]);

  const handleUserSubmit = (userData) => {
    setNickname(userData.nickname);
  };

  const handleJoinPresentation = async (presentation) => {
    if (connection) {
      await connection.invoke('JoinPresentation', presentation.id, nickname);
      setCurrentPresentation(presentation);
    }
  };

  const handleLeavePresentation = async () => {
    if (connection && currentPresentation) {
      await connection.invoke(
        'LeavePresentation',
        currentPresentation.id,
        nickname
      );
      setCurrentPresentation(null);
    }
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
  console.log(currentPresentation);

  return (
    <Whiteboard
      presentationId={currentPresentation.id}
      slides={currentPresentation.slides}
      onSlideAdded={handleAddSlide}
      users={[{ nickname }]}
      connection={connection}
    />
  );
}

export default App;
