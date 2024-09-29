import { Stage, Layer, Rect } from 'react-konva';
import { useState, useEffect, useRef } from 'react';
import axios from 'axios';
import ToolBar from './ToolBar';
import Shapes from './Shapes';
import drawingService from '../services/drawingService';
import signalRService from '../services/signalRService';
import lineService from '../services/lineService';

function Whiteboard({
  presentationId,
  slides,
  onSlideAdded,
  users,
  connection,
}) {
  const [selectedTool, setSelectedTool] = useState('brush');
  const [currentSlide, setCurrentSlide] = useState(slides[0]);
  const port = 'http://localhost:5000/api/';
  const [drawingData, setDrawingData] = useState([]);
  const stageRef = useRef(null);
  const [lines, setLines] = useState([]);
  const [color, setColor] = useState('#000000');
  const [currentShape, setCurrentShape] = useState(null);
  let currentSlideId = slides[0].id;
  console.log('slideId', currentSlideId);

  useEffect(() => {
    if (connection) {
      connection.on('ReceiveSlide', (newSlide) => {
        onSlideAdded(newSlide);
      });

      connection.on('ReceiveDrawing', (data) => {
        setDrawingData((prev) => [...prev, data]);
      });

      signalRService.startConnection();
      signalRService.onReceiveDrawAction((user, newLines) => {
        drawingService.addExternalDraw(newLines);
        setLines([...drawingService.getLines()]);
      });

      signalRService.onLoadPreviousDrawings((savedLines) => {
        drawingService.addExternalDraw(
          savedLines.map((line) => ({
            points: JSON.parse(line.points),
            stroke: line.stroke || 'black',
          }))
        );
        setLines([...drawingService.getLines()]);
      });
    }

    return () => {
      if (connection) {
        connection.off('ReceiveSlide');
        connection.off('ReceiveDrawing');
      }
    };
  }, [connection, onSlideAdded]);

  const handleAddSlide = async () => {
    try {
      const response = await axios.post(`${port}Slide/${presentationId}`, {});
      const newSlide = response.data;

      if (connection) {
        await connection.invoke('BroadcastSlide', presentationId, newSlide);
      }

      onSlideAdded(newSlide);
    } catch (error) {
      console.error('Error adding slide:', error);
    }
  };

  const handleMouseDown = () => {
    const pos = stageRef.current.getPointerPosition();
    if (selectedTool === 'pencil') {
      drawingService.startDrawing(pos, color);
    } else if (selectedTool === 'eraser') {
      drawingService.startDrawing(pos, 'white', 20);
    } else if (['rect', 'circle', 'arrow'].includes(selectedTool)) {
      drawingService.startShapeDrawing(selectedTool, pos.x, pos.y, color);
    }
  };

  const handleMouseMove = () => {
    const pos = stageRef.current.getPointerPosition();

    if (drawingService.isDrawing) {
      if (['pencil', 'eraser'].includes(selectedTool)) {
        drawingService.continueDrawing(pos);
      } else if (['rect', 'circle', 'arrow'].includes(selectedTool)) {
        drawingService.continueShapeDrawing(pos.x, pos.y);
      }
      setLines([...drawingService.getLines()]);
    }

    if (
      ['rect', 'circle', 'arrow'].includes(selectedTool) &&
      drawingService.currentShape
    ) {
      setCurrentShape({ ...drawingService.currentShape });
    }
  };

  const handleMouseUp = () => {
    const pos = stageRef.current.getPointerPosition();

    if (['pencil', 'eraser'].includes(selectedTool)) {
      const newLine = drawingService.endDrawing();
      setLines([...drawingService.getLines()]);
      if (connection) {
        signalRService.sendDrawAction('user1', [newLine]);
      }
      lineService.saveLine(newLine);
    } else if (['rect', 'circle', 'arrow'].includes(selectedTool)) {
      const shape = drawingService.endShapeDrawing();
      setLines([...lines, shape]);
      if (connection) {
        signalRService.sendDrawAction('user1', [shape]);
      }

      const shapeToSave = {
        id: 0,
        points: null,
        stroke: shape.stroke,
        tool: shape.tool,
        startX: Number(shape.startX),
        startY: Number(shape.startY),
        endX: Number(shape.endX),
        endY: Number(shape.endY),
      };

      lineService.saveLine(shapeToSave).catch((err) => {
        console.error('Error saving data to DB: ', err);
      });
    }
  };

  return (
    <div className="whiteboard-container">
      <ToolBar setTool={setSelectedTool} setColor={setColor} />
      <div className="slides-panel">
        {slides.map((slide, index) => (
          <button key={index} onClick={() => setCurrentSlide(slide)}>
            Slide {index + 1}
          </button>
        ))}
        <button onClick={handleAddSlide}>Add Slide</button>
      </div>
      <div className="whiteboard">
        <Stage
          width={800}
          height={600}
          onMouseDown={handleMouseDown}
          onMousemove={handleMouseMove}
          onMouseup={handleMouseUp}
          ref={stageRef}
          style={{ border: '1px solid black' }}
        >
          <Layer>
            <Rect width={800} height={600} fill="white" />
            {drawingData.map((data, index) => (
              <Rect key={index} {...data} />
            ))}
            <Shapes lines={lines} currentShape={currentShape} />
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
