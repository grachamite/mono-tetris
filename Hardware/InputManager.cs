namespace MonoTetris.Hardware;

using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class InputManager
{
    private KeyboardState _previousKeyboardState;
    private KeyboardState _currentKeyboardState;
    private double _currentTotalSeconds;

    private readonly Dictionary<Keys, double> _keyValues = new Dictionary<Keys, double>();
    
    public void UpdateState(double totalSeconds)
    {
        _currentTotalSeconds = totalSeconds;
        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();
    }
    
    public bool IsKeyJustPressed(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key) && 
               _previousKeyboardState.IsKeyUp(key);
    }
    
    public bool IsKeyDownPressed(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key);
    }
    
    public bool IsKeyDownPressedWithSpeed(Keys key, double speed = 0.25)
    {
        if (!_currentKeyboardState.IsKeyDown(key))
            return false;

        var previousTotalSeconds = _keyValues.GetValueOrDefault(key, 0.0D);
        
        var isPressed = _currentTotalSeconds - previousTotalSeconds >= speed;
        if (isPressed == true)
        {
            _keyValues[key] = _currentTotalSeconds;    
        }
        return isPressed;
    }
}