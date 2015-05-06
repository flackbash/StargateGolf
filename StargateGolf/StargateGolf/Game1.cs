using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StargateGolf
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /* 
     * Sources:
     * Background http://www.nasa.gov/vision/universe/solarsystem/hubble_pluto.html
     * Golf Ball http://www.onegolf.ga/wp-content/uploads/2015/01/golfball2.png
     * Stargate
     * Splash Sound http://soundbible.com/1463-Water-Balloon.html
     * Golf Sound http://soundbible.com/101-Golf-Club-Swing.html
    */

    public sealed class Game1 : Game
    {
        readonly GraphicsDeviceManager mGraphics;
        SpriteBatch mSpriteBatch;
        private Rectangle mFrame;
        private Texture2D mStargate, mBackground, mBall;
        private SoundEffect mSplash, mGolf;
        private Vector2 mSgPos, mBallPos, mBallInitPos;
        private Random mRandom;
        private int mScreenWidth, mScreenHeight, mRandXChange, mRandYChange, mTimer, mCounter,
            mStrokes, mHits;
        private MouseState mMouseState, mLastMouseState;
        private float mScale, mDiffX, mDiffY;
        private SpriteFont mFont;
        

        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;
            mGraphics.IsFullScreen = true;
            mGraphics.ApplyChanges();

            mFrame = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            mScreenHeight = GraphicsDevice.Viewport.Height;
            mScreenWidth = GraphicsDevice.Viewport.Width;
            mSgPos = new Vector2(mScreenWidth / 2f, mScreenHeight / 4f);
            mBallPos = new Vector2(mScreenWidth / 2f, mScreenHeight - 100);
            mBallInitPos = mBallPos;
            mRandom = new Random();
            mTimer = 0;
            mCounter = 0;
            mDiffX = 0;
            mDiffY = 0;
            mScale = 1;
            mHits = 0;
            mStrokes = 0;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            mBackground = Content.Load<Texture2D>("planet");
            mStargate = Content.Load<Texture2D>("Stargate");
            mBall = Content.Load<Texture2D>("ball");
            mSplash = Content.Load<SoundEffect>("Splash");
            mGolf = Content.Load<SoundEffect>("Golf");
            mFont = Content.Load<SpriteFont>("hud");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            // Process Mouse Click
            mMouseState = Mouse.GetState();
            if (mMouseState.LeftButton == ButtonState.Pressed &&
                mLastMouseState.LeftButton == ButtonState.Released && IsActive && mCounter == 0)
            {
                if (mMouseState.X >= mSgPos.X && mMouseState.X <= mSgPos.X + mStargate.Width &&
                    mMouseState.Y >= mSgPos.Y && mMouseState.Y <= mSgPos.Y + mStargate.Height)
                {
                    mSplash.Play();
                    mHits++;
                }
                else
                {
                    mGolf.Play();
                }
                mStrokes++;
                mCounter++;
                mDiffX = mBallInitPos.X - mMouseState.X;
                mDiffY = mBallInitPos.Y - mMouseState.Y;
            }
            mLastMouseState = mMouseState;

            // Update the position of the golf ball if needed
            if (mCounter != 0)
            {
                mBallPos -= new Vector2((mDiffX)/ 10f, (mDiffY)/ 10f);
                mScale -= 0.1f;
                mCounter++;
                if (mCounter == 11)
                {
                    mCounter = 0;
                    mScale = 1;
                    mBallPos = mBallInitPos;
                }
            }

            // Update the position of the gate
            if (mTimer % 20 == 0)
            {
                mRandXChange = mRandom.Next(-10, 11);
                mRandYChange = mRandom.Next(-10, 11);
            }
            mSgPos.X += mRandXChange;
            mSgPos.Y += mRandYChange;
            // If the gate is crossing the window border - get it outta there!
            if (mSgPos.X < 0 || mSgPos.X > mScreenWidth - mStargate.Width)
            {
                mSgPos.X -= 2 * mRandXChange;
                mTimer = 19;
            }
            if (mSgPos.Y < 0 || mSgPos.Y > mScreenHeight - mStargate.Height)
            {
                mSgPos.Y -= 2 * mRandYChange;
                mTimer = 19;
            }
            mTimer++;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            float hitRate;
            if (mStrokes == 0) hitRate = 100;
            else hitRate = (int)(mHits / (float)mStrokes * 100);


            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackground, mFrame, Color.White);
            mSpriteBatch.Draw(mStargate, mSgPos, Color.White);
            mSpriteBatch.Draw(mBall, mBallPos, null, Color.White, 0f, Vector2.Zero, mScale,
                SpriteEffects.None, 0f);
            mSpriteBatch.DrawString(mFont, "Hits: " + mHits, new Vector2(20, 20), Color.White);
            mSpriteBatch.DrawString(mFont, "Hit Rate: " + hitRate + "%",
                new Vector2(20, 50), Color.White);
            mSpriteBatch.DrawString(mFont, "Time: " + gameTime.TotalGameTime.Minutes.ToString("D2")
                + ":" + gameTime.TotalGameTime.Seconds.ToString("D2"),
                new Vector2(mScreenWidth - 120, 20), Color.White);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
