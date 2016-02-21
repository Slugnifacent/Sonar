using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;








namespace Sonar {
    public class Throwable : Entity {

        private int flightTime;
        public bool landed;
        public bool collideWithWall;
        public bool readyForPickUp;
        protected int weight;
        private GameTexture spriteVile;

        protected Throwable()
        {
        }

        public Throwable(ContentManager Content, GameVector2 Position, float health, int weight, GameTexture sprite) {
            if (sprite == null) {
                string vial = "Textures/Objects/Entity/Glass/vial"; // Variation of glass vial to display
                Random rand = Game1.random;
                int vialNum = rand.Next(1, 3);
                vial += vialNum;
                spriteVile = Content.Load<GameTexture>(@"" + vial);

                sprite = Content.Load<GameTexture>(@"Textures/Objects/Environment/Lab/desk_lab");
            }
            else { texture = sprite; }

            rootPos = position;
            position = Position;
            //orientation = Orientation;
            orientation.Normalize();
            boundingBox = new GameRectangle((int)(position.X - MapUnit.MAX_SIZE / 2-1), (int)(position.Y - MapUnit.MAX_SIZE / 2 - 1), (int)(MapUnit.MAX_SIZE), (int)(MapUnit.MAX_SIZE));
            InitializeAnimations();
            speed = 45/weight;
            this.health = health;
            this.weight = weight;
            texture = sprite;
            readyForPickUp = true;
            color = GameColor.Blue;
            color.A = 0;
        }

        protected override void InitializeAnimations()
        {
            animationInterval = 200;
            animation = new AnimationCollection(texture, boundingBox);
            animation.add("Intact", 0, spriteColNum, GameVector2.Zero, animationInterval, false);
            animation.add("Break", 0, spriteColNum, new GameVector2(0, spriteHeight), animationInterval, false);
            animation.RUN("Intact");
        }

        /// <summary>
        /// Corey - 2/6
        /// Called when the player presses the throw button. Will set the throw object at the player and how long it will be flying for.
        /// </summary>
        /// <param name="player"></param>
        public void throwObject(Player player) {
            color.A = 255;
            position = player.position;
            orientation = player.facing;
            updateBoundingBox(position);
            speed = 45 / weight;
            flightTime = 50;
            player.throwCooldown = true;
            readyForPickUp = false;
            texture = spriteVile;
        }

        public void setPos(GameVector2 v) {
            position = v;
            updateBoundingBox(position);
        }

        public void setSpeed(float f)
        {
            speed = f;
        }

        /// <summary>
        /// Corey - 2/6
        /// Returns just the flight time. No setter for you.
        /// </summary>
        /// <returns></returns>
        public int getFlightTime() {
            return flightTime;
        }

        /// <summary>
        /// Corey - 2/6
        /// Returns just the weight. No setter for you.
        /// </summary>
        /// <returns></returns>
        public int getWeight() {
            return weight;
        }

        /// <summary>
        /// Corey - 2/7
        /// Returns the health. No setter for you.
        /// </summary>
        /// <returns></returns>
        public float getHealth() {
            return health;
        }

        /// <summary>
        /// Corey - 2/6
        /// Collision code taken from the spectre class. 
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool Collide(GameRectangle rect) {
            //if ((((rect.Bottom > boundingBox.Top) && (rect.Bottom < boundingBox.Bottom)) || ((rect.Top > boundingBox.Top) && (rect.Top < boundingBox.Bottom))) &&
            //    (((rect.Left > boundingBox.Left) && (rect.Left < boundingBox.Right)) || ((rect.Right > boundingBox.Left) && (rect.Right < boundingBox.Right)))) {
                if (boundingBox.Intersects(new GameRectangle(rect.X - rect.Width / 4,
                                                         rect.Y - rect.Height / 4,
                                                   (int)(rect.Width + rect.Width / 2),
                                                   (int)(rect.Height + rect.Height / 2)))) {
                //put whatever collision shit is supposed to happen here.
                //Console.WriteLine("Colliding with Throwing");
                return true;
            }
            return false;
        }

        public GameTexture getTexture()
        {
            return texture;
        }

        /// <summary>
        /// Corey - 2/6
        /// Basic Draw override. Just shows the bounding box and the texture. Colored blue to differentiate for now.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(object spriteBatch) {
            spriteBatch.Draw(texture, boundingBox, color);
            //animation.Draw(spriteBatch);
        }

        /// <summary>
        /// Devon - 3/6
        /// Override to fix collision detection for throwables. No longer goes through walls or gets stuck.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void updateBoundingBox(GameVector2 position)
        {
            if (texture != null)
            {
                boundingBox.X = (int)position.X - texture.Width / 2 - 1;
                boundingBox.Y = (int)position.Y - texture.Height / 2 - 1;
            }
        }

        public void update() {
            if (Player.getInstance().GetNoise() >= Player.loudness.whispering)
            {
                if (color.A + 25 < 255) color.A += 20;
            }
            else
            {
                if (color.A - 10 > 0) color.A--;
            }

            if (color.A < 100)
            {
                boundingBox.Height = 0;
                boundingBox.Width = 0;
            }
            else
            {
                boundingBox.Height = MapUnit.MAX_SIZE;
                boundingBox.Width = MapUnit.MAX_SIZE;
            }
        }

        /// <summary>
        /// Corey - 2/6
        /// The update function. Updates as long as it is in flight. If not then it is as if it isn't there, just stored in the player list.
        /// If it collides into a wall then it sets the time to just 1 more frame to make it go away right then.
        /// TODO: Only works with colliding into walls going right and down, not up or left.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {


            if (collideWithWall) {
                flightTime = 1;
                collideWithWall = false;
            }
            if (flightTime > 0) {
                updateBoundingBox(position);
                flightTime--;
                if (speed > 0) {
                    speed -= .08f;
                }
                positionPrevious = position;
                position.X += (orientation.X * speed);
                position.Y += (orientation.Y * speed);
                if (flightTime == 0)
                {
                    landed = true;
                }
                else
                {
                    if (flightTime % 3 == 0)
                        VisionManager.addVisionPoint(position, 100, false);
                }
            }
            //animation.Update(gameTime, new GameVector2(boundingBox.Center.X, boundingBox.Center.Y));
        }
    }
}
