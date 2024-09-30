import { Button, VStack } from '@chakra-ui/react';
import { FaPlus } from 'react-icons/fa';

const SlidePanel = ({ slides, setCurrentSlide, handleAddSlide }) => {
  return (
    <VStack
      spacing={4}
      bg="gray.100"
      p={4}
      w="200px"
      mr={4}
      borderRadius="md"
      border="1px solid"
      borderColor="gray.300"
    >
      {slides.map((slide, index) => (
        <Button
          key={index}
          onClick={() => setCurrentSlide(slide)}
          colorScheme="teal"
          variant="outline"
        >
          Slide {index + 1}
        </Button>
      ))}
      <Button onClick={handleAddSlide} colorScheme="green">
        <FaPlus style={{ marginRight: '8px' }} />
        Add Slide
      </Button>
    </VStack>
  );
};

export default SlidePanel;
