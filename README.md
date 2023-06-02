# TrafficLight

### Enum: 
The "LightColor" enum defines three values representing the possible colors of a traffic light: Red, Yellow, and Green.
### Fields:
_redTime: Stores the duration (in seconds) for which the red light remains on.

  _maxGreenTime: Represents the maximum duration (in seconds) for which the green light can stay on.
  
  _greenTime: Holds a random duration (in seconds) within the range of minimum and maximum green time.
  
  YellowTime: A constant value (5 seconds) representing the duration for which the yellow light remains on.
  
  TempGreenLight: Stores the temporary extended duration (in seconds) for the green light.
  
  GreenStartTime: Represents the timestamp when the green light was last turned on.
### Properties:
  _stateTransitions: A list that holds tuples representing state transitions of the traffic light. Each tuple contains a dictionary of light colors and   a duration (in seconds).
  
  _currentStateIndex: Keeps track of the current state transition index.
  
  _currentState: Stores the current state of the traffic light represented by a dictionary of light colors.
### Methods:
  SetState(): Initializes the _stateTransitions list with the predefined state transitions and sets the initial state.
  
  StartAsync(): The main loop that controls the traffic light's behavior. It displays the current color, delays execution based on the current state     duration, and triggers state changes.
  
  DisplayCurrentColor(): Clears the console and displays the current state of the traffic light.
  
  GetCurrentStateDuration(): Retrieves the duration of the current state.
  
  ChangeState(): Updates the current state by moving to the next state transition in the _stateTransitions list.
  
  ExtendGreenTime(): Extends the current green light duration by calculating the remaining time and adding extra time if available.
  
  ListenForPedestrianButtonAsync(): Asynchronously starts listening for a key press to extend the green light duration.
  
  ListenForPedestrianButton(): Monitors keypress events and, when the spacebar is pressed, checks if the current state is green and extends or not the   green light duration.
