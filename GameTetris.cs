using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoTetris.Logic;
using MonoTetris.Hardware;
using System;


namespace MonoTetris;

public class GameTetris : Game
{
    // Game settings
    private static readonly (int Width, int Height, bool IsFullScreen) SScreenResolution = (320, 640, false);
    private static readonly (int Width, int Height) SMapSize = (10, 20);
    

    private readonly InputManager _inputManager;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    private Texture2D _block;
    
    private ShapeManager _shapeManager;
    
    private Map _map;

    private int _spriteWidth;
    private int _spriteHeight;
    private float _spriteScale;

    private readonly double _gameSpeedDelta = 0.5;
    
    private double _gameTimer = 0;
    private double _gameTotalTime = 0;

    private FallingShape _fallingShape;


    public GameTetris()
    {
        _graphics = new GraphicsDeviceManager(this);

        // Setup graphics.
        _graphics.PreferredBackBufferWidth = SScreenResolution.Width;
        _graphics.PreferredBackBufferHeight = SScreenResolution.Height;
        _graphics.IsFullScreen = SScreenResolution.IsFullScreen;
        _graphics.ApplyChanges();

        // Setup controllers
        IsMouseVisible = true;
        _inputManager = new InputManager();

        // Setup content
        Content.RootDirectory = "Content";
    }

    protected override void Initialize()
    {
        // Setup game logic
        _shapeManager = new ShapeManager(Content);
        _map = new Map(SMapSize.Width, SMapSize.Height);
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Init SpriteBatch
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // Load block image
        _block = Content.Load<Texture2D>("images/block03");

        // Calculating sprite size
        _spriteWidth = Window.ClientBounds.Width / _map.Width;
        _spriteHeight = _spriteWidth;

        // Calculating sprite scale
        _spriteScale = (float)Window.ClientBounds.Width / _map.Width / 16;
    }

    protected override void Update(GameTime gameTime)
    {
        _gameTimer += gameTime.ElapsedGameTime.TotalSeconds;
        _gameTotalTime += gameTime.ElapsedGameTime.TotalSeconds;
        
        // inputManager = new InputManager()
        _inputManager.UpdateState(_gameTotalTime);
        // Debug.WriteLine("TOTAL TIME: " + _gameTotalTime);
        
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        
            Exit();

        if (_fallingShape is not null 
            && (_inputManager.IsKeyDownPressedWithSpeed(Keys.Space, 0.25D) 
                || _inputManager.IsKeyDownPressedWithSpeed(Keys.Up, 0.25D)))
        {
            _fallingShape.NextState();
            bool checkResult = _map.CheckCollision(       
                _fallingShape.Left,                       
                _fallingShape.Top,                        
                _fallingShape.GetCurrentStateMatrix()     
            );
            if (checkResult == true)
            {
                _fallingShape.PrevState();
            }   
        }

        if (_fallingShape is not null && _inputManager.IsKeyDownPressedWithSpeed(Keys.Left, 0.25D)) 
        {                            
            bool checkResult = _map.CheckCollision(       
                _fallingShape.Left - 1,                       
                _fallingShape.Top,                        
                _fallingShape.GetCurrentStateMatrix()     
            );
            if (checkResult == false)
            {
                _fallingShape.Left--;
            }            
        }                    
        
        if (_fallingShape is not null && _inputManager.IsKeyDownPressedWithSpeed(Keys.Right, 0.25D)) 
        {                            
            bool checkResult = _map.CheckCollision(       
                _fallingShape.Left + 1,                       
                _fallingShape.Top,                        
                _fallingShape.GetCurrentStateMatrix()     
            );
            if (checkResult == false)
            {
                _fallingShape.Left++;
            }            
        }
        
        if (_fallingShape is not null && _inputManager.IsKeyDownPressedWithSpeed(Keys.Down, 0.08D)) 
        {                            
            bool checkResult = _map.CheckCollision(       
                _fallingShape.Left,                       
                _fallingShape.Top + 1,                        
                _fallingShape.GetCurrentStateMatrix()     
            );
            if (checkResult == false)
            {
                _fallingShape.Top++;
            }            
        }    

        if (_fallingShape is not null && _inputManager.IsKeyJustPressed(Keys.Z))
        {
             _map.OffsetDownToLine(19);
        }      
        
        
        // TODO: Add your update logic here
        if (_gameTimer >= _gameSpeedDelta)
        {
            UpdateGameState();
            _gameTimer = 0;
            
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);


        // TODO: Add your drawing code here
        // Begin the sprite batch to prepare for rendering.
        _spriteBatch.Begin();


        // Draw the texture.
        DrawMap();
        if (_fallingShape is not null)
        {
            DrawFallenShape();
        }

        // Always end the sprite batch when finished.
        _spriteBatch.End();


        base.Draw(gameTime);
    }

    private void UpdateGameState()
    {
        CreateNewFallingShapeIfNull();
    }
    
    private void CreateNewFallingShapeIfNull()
    {
        if (_fallingShape is null)
        {
            var random = new Random();
            var randomColorIndex = ColorManager.GetRandomColorIndex();

            var randomShape = _shapeManager.GetRandomShape();

            _fallingShape = new FallingShape(
                (_map.Width - randomShape.Width) / 2,
                0,
                randomShape,
                randomColorIndex + 1
            );
        }
        else
        {
            _fallingShape.Top++;
        }

        bool checkResult = _map.CheckCollision(
            _fallingShape.Left,
            _fallingShape.Top,
            _fallingShape.GetCurrentStateMatrix()
        );
        
        if (checkResult == true)
        {
            _fallingShape.Top--;
            if (_fallingShape.Top < 0)
            {
                _fallingShape.Top = 0;
            }
            _map.SaveMatrix(                
                _fallingShape.Left,                                
                _fallingShape.Top,                                 
                _fallingShape.GetCurrentStateMatrix(),
                _fallingShape.ColorId
            );
            
            if (_fallingShape.Top == 0)
            {
                Exit();
            }
            _fallingShape = null;

            for (int y = _map.Height - 1; y >= 0; y--)
            {
                if (_map.IsFilledLine(y))
                {
                    _map.RemoveLine(y);
                }
            }
            
            _map.GravitateLines();
        }
    }
    
    
    private void DrawMap()
    {
        for (int y = 0; y < _map.Height; y++)
        {
            for (int x = 0; x < _map.Width; x++)
            {
                if (_map.Get(x, y) != 0)
                {
                    _spriteBatch.Draw(
                        _block,
                        new Vector2(
                            _spriteWidth * x, _spriteHeight * y   
                        ),
                        null,
                        ColorManager.GetFadeColor(_map.Get(x, y) - 1),
                        0.0f,
                        Vector2.Zero,
                        _spriteScale,
                        SpriteEffects.None,
                        0.0f
                    );
                }
            }
        }
    }
    
    private void DrawFallenShape()
    {
        
        int[,] shapeMatrix = _fallingShape.GetCurrentStateMatrix();
        for (int y = 0; y < _fallingShape.Height; y++)                                             
        {                                                                                  
            for (int x = 0; x < _fallingShape.Width; x++)                                           
            {                                                                              
                if ( shapeMatrix[x, y] != 0)                                                   
                {                                                                          
                    _spriteBatch.Draw(                                                     
                        _block,                                                            
                        new Vector2(                                                       
                            _spriteWidth * (x + _fallingShape.Left), _spriteHeight * (y + _fallingShape.Top)       
                        ),                                                                 
                        null,                             
                        ColorManager.GetColor(_fallingShape.ColorId - 1),                                          
                        0.0f,                                    
                        Vector2.Zero,                                      
                        _spriteScale,                               
                        SpriteEffects.None,                                     
                        0.0f                                  
                    );                                                                     
                }                                                                          
            }                                                                              
        }                                                                                  
    }
}