using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sync.Experiments.FireFly;
using Sync.Experiments.Termite;
using Sync.GameSpace;
using System.Threading;

namespace Sync
{
    /**
     * 
     * Start point for some of my experiments with synchrony and emergent behavior.
     * 
     * */
    public class Sync : Game
    {
        public static Sync instance { get; private set; }
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        WorldBase world;

        public Sync()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 800;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            //Pick one!
            //The firefly world is an experiment involving synchrony. The termite world is an experiment on
            //cohesive behavior by a collective without central control.
            //world = new FireFlyWorld(100);
            world = new TermiteWorld(50);
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            world.update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            world.draw(spriteBatch, gameTime, new Vector2(), 8);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
