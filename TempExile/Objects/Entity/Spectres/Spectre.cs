using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sonar
{
    public class Spectre : Entity
    {

        #region Fields
        public object unityGameObject;


        public List<MapUnit> patrolPaths;
        private List<MapUnit> foundPath;
        public List<Door> doors;
        private List<bool> walkability;
        protected MapUnit[,] map;
        public Boolean isChasing;
        protected Player player;
        protected float Volume = 100;
        float randX, randY, searchRange;
        public bool inPlayer;
        public bool possessing;
        public bool finishedSearching;
        public bool playerVisible;
        public bool playerBeingHeard;
        public bool objectHeard;
        public bool brokeADoor;
        public bool nextToDoor;
        public bool neutralized;
        public GameVector2 locationOfNoise;
        public GameVector2 lastLocationOfNoise;
        public GameVector2 homePosition;
        public Sound soundHeard, lastSoundHeard;
        protected object/*BoundingSphere*/ hearingSphere;
        bool lastSoundIsSpectre = false;

        private float randDist;
        public MapUnit targetPosition;
        public Door door;
        public List<MapUnit> tempPath;
        public MapUnit investigateSpot;
        public MapUnit theDoor;
        public bool openThatDoor;
        public bool goingToInvestigate;
        public bool active;

        public float dmgCooldown;
        public int randomAction;
        public float fleeTimer;
        public float lastSoundTimer;
        public float neutralizedTimer;
        public bool behindDoor;
        public bool returningFromOpenDoor;
        public float abilityCooldown;
        protected float hearingRange;
        protected float scale; // Scale for stretching sprite texture
        public enum Collidable : sbyte { none, player, spectre, throwable, civilian };

        public int alertedCount;

        #region Audio
        protected AudioListener listener = new AudioListener();

        protected int gruntTimer;

        protected Sound grunt;
        protected Sound roar;
        protected Sound walk;
        protected Sound InvestigateCue;
        protected Sound excorcismCue;

        public float Dampening;

        #endregion

        #region Behaviors
        public StateMachine behaviorMachine;
        #endregion

        #region Pathfinding

        private MapUnit target;
        private MapUnit immediate;
        private MapUnit currentUnit;
        int currentPathIndex;

        #endregion

        #endregion

        public Spectre() { }


        public Spectre(MapUnit[,] map, List<MapUnit> path, List<Door> doors, Player p)
        {
            this.player = p;
            this.map = map;
            patrolPaths = new List<MapUnit>();
            this.doors = doors;
            walkability = new List<bool>();
            //foundPath = new List<MapUnit>();
            foreach (MapUnit m in path)
            {
                patrolPaths.Add(m);
            }
            currentUnit = patrolPaths[0];
            //currentUnit.isWalkable = false;
            position.Y = currentUnit.y * MapUnit.MAX_SIZE;
            position.X = currentUnit.x * MapUnit.MAX_SIZE;
            //patrolPaths[0].isWalkable = false;

            baseSpeed = speed = 100;
            if (patrolPaths.Count > 1)
                if (patrolPaths[1] != null)
                {
                    target = patrolPaths[1];
                    currentPathIndex = 1;
                }
            BuildBehaviorMachine();
            //texture = Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Player/temp");
            //boundingBox = new GameRectangle((int)position.X, (int)position.Y, MapUnit.MAX_SIZE, MapUnit.MAX_SIZE);
            //color = GameColor.White;
            inPlayer = false;
            possessing = false;
            finishedSearching = false;
            neutralized = false;
            dmgCooldown = 0;
            abilityCooldown = 0;
            fleeTimer = 0;
            neutralized = false;
            lastSoundTimer = 0;
            hearingRange = 400;
            Dampening = 0;
            alertedCount = 0;
            animationInterval = 200;
            scale = 1.0f;
            active = true;
        }

        protected override void InitializeAnimations()
        {
            //animation = new AnimationCollection(texture, new GameRectangle(0, 0, spriteWidth, spriteHeight));
            //animation.add("DownWalk", 0, spriteColNum, GameVector2.Zero, animationInterval, true);
            //animation.add("UpWalk", 0, spriteColNum, new GameVector2(0, spriteHeight * 1), animationInterval, true);
            //animation.add("LeftWalk", 0, spriteColNum, new GameVector2(0, spriteHeight * 2), animationInterval, true);
            //animation.add("RightWalk", 0, spriteColNum, new GameVector2(0, spriteHeight * 3), animationInterval, true);
            //animation.RUN("RightWalk");
        }

        public virtual void BuildBehaviorMachine()
        {
            PatrolState tempPatrol = new PatrolState();
            InvestigateState tempInvestigate = new InvestigateState();
            ChaseState tempChase = new ChaseState();
            tempPatrol.addTransition(new ToInvestigateFromPatrolTransition(tempInvestigate));
            tempInvestigate.addTransition(new ToPatrolTransition(tempPatrol));
            tempInvestigate.addTransition(new ToChaseTransition(tempChase));
            tempChase.addTransition(new ToInvestigateFromChaseTransition(tempInvestigate));
            behaviorMachine = new StateMachine(tempPatrol);
            behaviorMachine.AddState(tempInvestigate);
            behaviorMachine.AddState(tempChase);
        }

        #region Pathfinding
        /// <summary>
        /// Chris Peterson / Derrick Huey - 1/23/12
        /// </summary>
        public void FindPath()
        {
            AStar_FindPath();
        }

        /// <summary>
        /// Chris Peterson / Derrick Huey - 1/23/12
        /// A* pathfinding implementation
        /// </summary>
        private void AStar_FindPath()
        {
            List<MapUnit> open = new List<MapUnit>();
            List<MapUnit> closed = new List<MapUnit>();
            List<MapUnit> walkableNeighbors;
            MapUnit current = getCurrentUnit();
            int breakCounter = 0;
            foundPath = new List<MapUnit>();
            open.Add(current);

            while (!closed.Contains(target) && current != target && current != null)//open.Count != 0)
            {
                walkableNeighbors = FindWalkableNeighbors(current);
                CheckNeighbors(current, walkableNeighbors, open, closed);
                current = FindLowest(open);
                breakCounter++;

                if (breakCounter > 400)
                {
                    target = null;
                    current = null;
                    break;
                }
            }

            // if a path has been found, builds the found path traversal list
            if (current == target && target != null)
            {
                foundPath.Add(current);
                while (current.parent != null)
                {
                    foundPath.Add(current.parent);
                    current = current.parent;
                }
                foundPath.Reverse();
                //if (foundPath.Count != 1)
                    foundPath.Remove(foundPath[0]);
            }
            else
            {
                foundPath = null;
                immediate = null;
            }

            resetFGH(open);
            resetFGH(closed);
        }

        /// <summary>
        /// Derrick
        /// Resets the cost/heuristic values of the nodes in the given list
        /// </summary>
        /// <param name="mList"></param>
        private void resetFGH(List<MapUnit> mList)
        {
            foreach (MapUnit m in mList)
            {
                m.resetFGH();
            }
        }

        /// <summary>
        /// Derrick
        /// Determines the neighbors of the currently expanded node that are walkable
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private List<MapUnit> FindWalkableNeighbors(MapUnit current)
        {
            List<MapUnit> walkableNeighbors = new List<MapUnit>();

            for (int i = 0; i < 8; i++)
            {
                if (i % 2 == 0)
                {
                    if (i == 0)
                    {
                        if (current.neighbors[7].isWalkable && current.neighbors[1].isWalkable && current.neighbors[i].objType.GetType() != typeof(Exit))
                            walkableNeighbors.Add(current.neighbors[i]);
                    }
                    else
                    {
                        if (current.neighbors[i - 1].isWalkable && current.neighbors[i + 1].isWalkable && current.neighbors[i].objType.GetType() != typeof(Exit))
                            walkableNeighbors.Add(current.neighbors[i]);
                    }
                }
                else
                {
                    if ((current.neighbors[i].isWalkable/* || current.neighbors[i].objType.GetType() == typeof(Siren)*/) && current.neighbors[i].objType.GetType() != typeof(Exit))
                        walkableNeighbors.Add(current.neighbors[i]);
                }
            }
            return walkableNeighbors;
        }

        /// <summary>
        /// Derrick
        /// Determines the cost/heuristic values of the neighboring nodes and updates
        /// the open and closed lists as necessary
        /// </summary>
        /// <param name="current">node to be expanded</param>
        /// <param name="walkableNeighbors"></param>
        /// <param name="open">open list</param>
        /// <param name="closed">closed list</param>
        private void CheckNeighbors(MapUnit current, List<MapUnit> walkableNeighbors, List<MapUnit> open, List<MapUnit> closed)
        {
            // move currently expanded node from open to closed list
            open.Remove(current);
            closed.Add(current);

            foreach (MapUnit m in walkableNeighbors)
            {
                if (m != null && (m.isWalkable/* || m.objType.GetType() == typeof(Siren)*/) && !closed.Contains(m))
                {
                    // if neighbor node is not on the open list, add it and calculate cost
                    if (!open.Contains(m))// && !closed.Contains(m))
                    {
                        open.Add(m);
                        m.parent = current;
                        setFGH(current, m);
                    }
                    else
                    {
                        // if neighbor node is on the open list, add if has a lower cost
                        if (current.g + getG(current, m) < m.g)
                        {
                            m.parent = current;
                            setFGH(current, m);
                        }
                    }
                }
                else
                {
                    closed.Add(m);
                }
            }
        }

        /// <summary>
        /// Derrick
        /// Calculates the cost of moving from the expanded node to a neighbor
        /// </summary>
        /// <param name="current">currently expanded node</param>
        /// <param name="m">neighbor node</param>
        /// <returns></returns>
        private int getG(MapUnit current, MapUnit m)//, int n)
        {
            int g;
            /*if (m.parent != null)
                g = m.parent.g;*/
            if (m.x - current.x == 0 || m.y - current.y == 0)//n == 1 || n == 3 || n == 5 || n == 7) //not diagonal
            {
                g = 10 * m.weight;
            }

            else //diagonal
            {
                g = 14 * m.weight;
            }

            return g;
        }

        /// <summary>
        /// Derrick
        /// Calculates and sets the costs/heuristic of the neighboring node
        /// </summary>
        /// <param name="current">current expanded node</param>
        /// <param name="m">neighbor node</param>
        private void setFGH(MapUnit current, MapUnit m)//, int n)
        {
            m.g = m.parent.g;
            m.g += getG(current, m);//, n);
            if (target == null)
                return;
            m.h = (Math.Abs(m.x - target.x) * 10) + (Math.Abs(m.y - target.y) * 10); // manhattan distance heuristic

            m.f = m.g + m.h;
        }

        /// <summary>
        /// Derrick
        /// Determines which node on the open list has lowest cost to expand
        /// </summary>
        /// <param name="open"> open list</param>
        /// <returns></returns>
        private MapUnit FindLowest(List<MapUnit> open)
        {
            if (open.Count == 0)
            {
                return null;
            }

            int lowestF = open[0].f;
            MapUnit lowest = open[0];

            foreach (MapUnit m in open)
            {
                if (m.f < lowestF)
                {
                    lowestF = m.f;
                    lowest = m;
                }
            }

            return lowest;
        }

        /// <summary>
        /// Chris Peterson / Derrick Huey - 1/23/12
        /// </summary>
        /// <returns></returns>
        public MapUnit getCurrentUnit()
        {
            //return map[((int)position.X + boundingBox.Width / 2) / MapUnit.MAX_SIZE, ((int)position.Y + boundingBox.Height / 2) / MapUnit.MAX_SIZE];
            return map[((int)position.X) / MapUnit.MAX_SIZE, ((int)position.Y) / MapUnit.MAX_SIZE];
        }

        /// <summary>
        /// Uses Breadth First Search to find a door from the location the spectre is in.
        /// </summary>
        /// <returns></returns>
        public MapUnit findDoor() {
            clearMarks();
            List<MapUnit> queue = new List<MapUnit>();
            MapUnit goal = null;
            
            queue.Add(getCurrentUnit());
            queue[0].marked = true;
            while (queue.Count != 0) {
                MapUnit t = queue[0];
                queue.Remove(t);
                if (t.getObject().GetType() == typeof(Door)) {
                    goal = t;
                    return goal;
                }
                foreach (MapUnit m in t.neighbors) {
                    if (m.isWalkable || m.getObject().GetType() == typeof(Door)) {
                        MapUnit p = m;
                        if (!p.marked) {
                            p.marked = true;
                            queue.Add(p);
                        }
                    }
                }
            }
            return goal;
        }

        /// <summary>
        /// Makes all nodes unmarked for the find doors function.
        /// </summary>
        private void clearMarks() {
            foreach (MapUnit m in map) {
                m.marked = false;
            }
        }

        public void holdTempPath() {
            if (foundPath != null) {
                tempPath = foundPath.ToList<MapUnit>();
            }
            else {
                tempPath = null;
            }
        }

        #endregion

        #region Behaviors

        #region Patrol
        /// <summary>
        /// Chris Peterson - 1/25/12
        /// For patroling in a circle
        /// </summary>
        public void CircularPatrol()
        {
            if (getCurrentUnit() != target)
            {
                FindPath();
            }
            else
            {
                if (currentPathIndex == patrolPaths.Count - 1)
                {
                    currentPathIndex = 0;
                }
                else
                {
                    currentPathIndex++;
                }
                //foundPath.Clear();
                foundPath = null;
                target = patrolPaths[currentPathIndex];
            }
        }

        public MapUnit GetTarget()
        {
            return target;
        }

        public void SetTarget(MapUnit t)
        {
            target = t;
        }

        public int GetCurrentPathIndex()
        {
            return currentPathIndex;
        }

        public void SetCurrentPathIndex(int i)
        {
            currentPathIndex = i;
        }

        public List<MapUnit> GetPatrolPath()
        {
            return patrolPaths;
        }

        public void ClearPath()
        {
            foundPath = null;
        }

        public MapUnit[,] GetMap()
        {
            return map;
        }

        /// <summary>
        /// Chris Peterson - 1/25/12
        /// For patroling in a line 
        /// </summary>
        public void Patrol()
        {

        }
        #endregion

        #region Movement
        /// <summary>
        /// Chris Peterson / Derrick Huey - 1/25/12
        /// Moves the spectres according to the current state
        /// </summary>
        /// <param name="time"></param>
        public void move(GameTime time)
        {
            
            
            if (foundPath == null || foundPath.Count == 0)
            {
                FindPath();
            }
            currentUnit = getCurrentUnit();
            immediate = FindNext();
            //if (foundPath != null && foundPath.Count != 0 && immediate != null)
            //{
                //immediate = FindNext();

                if (immediate != null && !immediate.isWalkable && immediate.objType != this)
                {
                    immediate = null;
                    foundPath = null;
                    if (!target.isWalkable)
                    {
                        target = null;
                    }
                    return;
                }
            //}
            float timeInSec = (float)time.ElapsedGameTime.TotalSeconds;
            // check if in the immediate square, and if so update current and path list
            //UpdatePosition(timeInSec);
            if (immediate != null && GameVector2.Distance(immediate.GetPosition(), position) <= speed * timeInSec)/*(Math.Abs(position.X - immediate.GetPosition().X) < timeInSec * speed)
                && (Math.Abs(position.Y - immediate.GetPosition().Y) < timeInSec * speed))*/
            {
                
                RemoveNext();
                //immediate = FindNext();
                }
            //if (immediate != null)
            //if (currentUnit != target)
                UpdatePosition(timeInSec);


        }

        public MapUnit FindNext()
        {
            if (foundPath != null && foundPath.Count != 0)
                return foundPath[0];
            return null;
        }

        private void RemoveNext()
        {
            if (foundPath != null && foundPath.Count != 0)
                foundPath.RemoveAt(0);
        }

        private void UpdatePosition(float time)
        {
            if (immediate != null)
            {
                orientation = immediate.GetPosition() - position;// getCurrentUnit().GetPosition();
                if (orientation == GameVector2.Zero)
                    return;
                orientation.Normalize();
                positionPrevious = position;
                position += orientation * speed * time;
            }
        }
        #endregion

        /// <summary>
        /// Checks to see if the player is heard and sets the Object heard or Player heard flag.
        /// Created By Joshua Ray
        /// 2/15/2012
        /// </summary>
        /// <param name="playerPos"> Player Position</param>
        public void checkHearing(Sonar.Player.loudness playerNoise) {
            playerBeingHeard = false;
            color = GameColor.White;
            float hearingMod = 0;
            foreach (Sound sound in SoundManager.soundList())
            {
                if (sound.SOURCE == this || (lastSoundTimer <= 30 && lastSoundHeard != null && lastSoundHeard.SOURCE != null &&
                        /*(lastSoundHeard.SOURCE.GetType() == typeof(Wrath) || lastSoundHeard.SOURCE.GetType() == typeof(Dumb) || lastSoundHeard.SOURCE.GetType() == typeof(Siren))*/
                        lastSoundIsSpectre && sound.SOURCE == lastSoundHeard.SOURCE))
                    continue;
                
                if (sound.SUSPICIOUS) // I might need to change this line to check the Spectres hearing range as apposed to the sound radius of the object********
                {
                    // Heard an Object
                    hearingMod = 0;
                    if (sound.CueName() == SoundType.PLAYER.PLAYER_FOOTSTEP_CARPET.ToString() ||
                        sound.CueName() == SoundType.PLAYER.PLAYER_FOOTSTEP_CONCRETE.ToString() ||
                        sound.CueName() == SoundType.PLAYER.PLAYER_FOOTSTEP_HARDWOOD.ToString() ||
                        sound.CueName() == SoundType.PLAYER.PLAYER_FOOTSTEP_TILE.ToString())
                    {
                        if (Player.getInstance().isStealthing) {
                            hearingMod += hearingRange*.80f;
                        }
                    }
                    if (sound.CueName() == SoundType.THROWABLE.THROWABLE_GLASS_BREAKING.ToString())
                    {
                            hearingMod += -hearingRange/2;
                    }
                    //if (hearingSphere.Contains(sound.SPHERE) == ContainmentType.Contains || hearingSphere.Contains(sound.SPHERE) == ContainmentType.Intersects)
                    objectHeard = soundAttentuation(GameVector2.Distance(sound.POSITION, position) + hearingMod, sound.RADIUS);
                    if (objectHeard && sound.SOURCE != null && sound.SOURCE.GetType() == typeof(Radio))
                    {
                        Radio temp = (Radio)sound.SOURCE;
                        if (temp.isPlaying() == temp.lastHeardState)
                            objectHeard = false;
                        else
                        {
                            temp.lastHeardState = temp.isPlaying();
                        }
                    }
                    if (objectHeard)
                    {
                        color = GameColor.Red;
                        
                        lastLocationOfNoise = locationOfNoise;
                        locationOfNoise = sound.POSITION;
                        if (sound.SOURCE != null && sound.SOURCE.GetType() == typeof(Radio))
                        {
                            MapUnit temp = map[((int)sound.POSITION.X) / MapUnit.MAX_SIZE, ((int)sound.POSITION.Y) / MapUnit.MAX_SIZE];
                            MapUnit radioUnit = temp;
                            do
                            {
                                temp = radioUnit.neighbors[Game1.random.Next(0, 8)];
                            } while (!temp.isWalkable);
                            locationOfNoise = temp.GetPosition();
                        }
                        lastSoundHeard = soundHeard;
                        if (lastSoundHeard != null && lastSoundHeard.SOURCE != null && lastSoundHeard.SOURCE.GetType() != typeof(Radio))
                            lastSoundIsSpectre = true;
                        else
                            lastSoundIsSpectre = false;
                        soundHeard = sound;
                        lastSoundTimer = 0;
                        break;
                    }
                }
                else
                {
                    objectHeard = false;
                }
            }
            float distToPlayer = Util.getInstance().DistanceToPlayer(position);
            if (player.isHiding)
            {
                
                if (distToPlayer <= player.radius)
                {
                    //Console.Out.WriteLine("Within range");
                    if (playerNoise != Sonar.Player.loudness.silent/*Game1.voiceEngine.VOLUME > .1f*/)
                    {
                        // Heard The Player
                        playerBeingHeard = soundAttentuation(Util.getInstance().DistanceToPlayer(position));//, player.radius);
                        if (playerBeingHeard)
                        {
                            color = GameColor.Black;

                            searchRange = player.radius / 5;
                            GameVector2 temp = findValidRandomVal(Player.getInstance().position);

                            lastLocationOfNoise = locationOfNoise;
                            locationOfNoise = temp;
                        }
                    }
                }
            }
            else
            {
                if (distToPlayer < player.radius)
                {
                    playerBeingHeard = soundAttentuation(distToPlayer);//, player.radius);
                    if (playerBeingHeard)
                    {
                        color = GameColor.Black;

                        searchRange = player.radius / 5;
                        GameVector2 temp = findValidRandomVal(Player.getInstance().position);

                        lastLocationOfNoise = locationOfNoise;
                        locationOfNoise = temp;
                    }
                }
            }
        }

        /// <summary>
        /// Steven Ekejiuba - 2/17/'12
        /// Ensures that the random values chosen are on the map and on walkable terrain
        /// </summary>
        /// <param name="playerPos"></param>
        public GameVector2 findValidRandomVal (GameVector2 playerPos) {
            do
            {
                do
                {
                    randX = Game1.random.Next((int)(playerPos.X - searchRange), (int)(playerPos.X + searchRange));
                    randY = Game1.random.Next((int)(playerPos.Y - searchRange), (int)(playerPos.Y + searchRange));
                } while ((randX < 0) || (randY < 0) ||
                    (randX >= map.GetUpperBound(0) * MapUnit.MAX_SIZE) || (randY >= map.GetUpperBound(1) * MapUnit.MAX_SIZE));
            } while (!map[(int)randX / MapUnit.MAX_SIZE, (int)randY / MapUnit.MAX_SIZE].isWalkable);
            return new GameVector2(randX, randY);
        }

        /// <summary>
        /// Derrick Huey
        /// Ensures that the random values chosen are on the map and on walkable terrain, overload for specifiable search range
        /// </summary>
        /// <param name="playerPos"></param>
        /// <param name="range">specified range for random val</param>
        public GameVector2 findValidRandomVal(GameVector2 playerPos, int range)
        {
            do
            {
                do
                {
                    randX = Game1.random.Next((int)(playerPos.X - range), (int)(playerPos.X + range));
                    randY = Game1.random.Next((int)(playerPos.Y - range), (int)(playerPos.Y + range));
                } while ((randX < 0) || (randY < 0) ||
                    (randX >= map.GetUpperBound(0) * MapUnit.MAX_SIZE) || (randY >= map.GetUpperBound(1) * MapUnit.MAX_SIZE));
            } while (!map[(int)randX / MapUnit.MAX_SIZE, (int)randY / MapUnit.MAX_SIZE].isWalkable);
            return new GameVector2(randX, randY);
        }

        /// <summary>
        /// Attentuates the sound of the player over a distance and returns true if the oject is heard.
        /// The further away an object the less likely it will be that the spectre heard the player
        /// Created By Joshua Ray
        /// 2/15/2012
        /// </summary>
        /// <param name="distancefromObject"> The current distance of the Spectre to the object</param>
        /// /// <param name="maxHearingDistance"> the maximum hearing distance a Spectre can hear</param>
        public bool soundAttentuation(float distancefromObject, float soundRadius) {
            float ratio;
            ratio = distancefromObject / hearingRange;
            /*if (distancefromObject <= hearingRange)
                ratio = distancefromObject / hearingRange;
            else
                ratio = Math.Abs(distancefromObject - soundRadius) / hearingRange;*/
            float odds = (float)Math.Pow((1 - ratio), 3);
            if (ratio <= 1)
            {
                if (Game1.random.NextDouble() < odds)
                {
                    return true;
                }
            }
            return false;
        }

        public bool soundAttentuation(float distancefromObject)
        {
            float ratio;
            ratio = distancefromObject / hearingRange;
            float odds = (float)Math.Pow((1 - ratio), 3);
            if (ratio <= 1)
            {
                if (Game1.random.NextDouble() < odds)
                {
                    return true;
                }
            }
            return false;
        }


        #endregion

        /// <summary>
        /// Travis Carlson
        /// 2/15/12
        /// When the player dies, resets the spectres position and sets it to its initial state
        /// </summary>
        /// <param name="player"></param>

        public void resetPosition()
        {
            currentUnit = patrolPaths[0];
            //currentUnit.isWalkable = false;
            position.Y = currentUnit.y * MapUnit.MAX_SIZE;
            position.X = currentUnit.x * MapUnit.MAX_SIZE;
            //updateBoundingBox(position);
            nextToDoor = false;
            if (patrolPaths.Count > 1)
                if (patrolPaths[1] != null)
                {
                    target = patrolPaths[1];
                    currentPathIndex = 1;
                }
            ClearPath();
            isChasing = false;
            targetPosition = null;
            door = null;
            tempPath = null;
            investigateSpot = null;
            theDoor = null;
            openThatDoor = false;
            goingToInvestigate = false;
            locationOfNoise = GameVector2.Zero;
            behindDoor = false;
            neutralized = false;
            brokeADoor = false;
            nextToDoor = false;
            behaviorMachine.toInitialState(this, Player.getInstance());
            objectHeard = false;
            playerBeingHeard = false;
            InitializeAnimations();
        }

        public void UpdateHearingSphere()
        {
            //hearingSphere.Center.X = position.X;
            //hearingSphere.Center.Y = position.Y;
        }

        public virtual void Update(object gameTime, object player)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            dmgCooldown += elapsedTime;
            abilityCooldown += elapsedTime;
            fleeTimer += elapsedTime;
            neutralizedTimer += elapsedTime;
            lastSoundTimer += elapsedTime;
            Collide(player.getBox(), Collidable.player);
            checkHearing(player.GetNoise());
            setWalkable();
            behaviorMachine.Update(this, player);
            if (this.GetType() != typeof(Siren) && active)
                move(gameTime);
            updateBoundingBox(position);
            UpdateHearingSphere();
            animation.Update(gameTime, new GameVector2((int)position.X - spriteWidth / 2, (int)position.Y - spriteHeight / 2));
            if (!possessing && this.GetType() != typeof(Stalker) && this.GetType() != typeof(Siren))
            {
                FootstepLightNoise(gameTime);
            }
        }

        /// <summary>
        /// Corey - 3/5
        /// Refactored code from above.
        /// </summary>
        protected void FootstepLightNoise(GameTime time) {
            if (isMoving)
            {
                gruntTimer += time.ElapsedGameTime.Milliseconds;
                if (gruntTimer > 4000)
                {
                    gruntTimer = 0;
                    SoundManager.createSound(position, 400, 400, 1f, SoundManager.WRATH.GROWL, false);
                }
                int timer = updateFootstepTimer();
                if (timer == 1)
                {
                    SoundManager.createSound(position + new GameVector2(orientation.Y * 10f, orientation.X * 10f), 150, 150, 1, SoundManager.WRATH.FOOTSTEP, false);
                }
                else if (timer == 2)
                {
                    SoundManager.createSound(position - new GameVector2(orientation.Y * 10f, orientation.X * 10f), 150, 150, 1, SoundManager.WRATH.FOOTSTEP, false);
                }
            }
        }

        /// <summary>
        /// Corey - 3/5
        /// This is the changed footstep when they hallucinate.
        /// </summary>
        public void HallucinateLightNoise(Player player) {
            int timer = updateFootstepTimer();
            if (timer == 1) {
                randDist = Game1.random.Next(150, 200);
                SoundManager.createSound(
                    new GameVector2(player.position.X + (-player.facing.X * randDist), player.position.Y + (-player.facing.Y * randDist)),
                    200,
                    200,1,
                    SoundManager.WRATH.FOOTSTEP, 
                    false);
            }
            else if (timer == 2) {
                randDist += 10f;
                SoundManager.createSound(new GameVector2(player.position.X + (-player.facing.X * randDist), player.position.Y + (-player.facing.Y * randDist)),
                    200, 200, 1, SoundManager.WRATH.FOOTSTEP, false);
            }
        }

        public virtual void Draw(object batch, object graphics)
        {

            SetSprite();
            if ((!inPlayer || !possessing || GetType() == typeof(Sonar.Siren)) && behaviorMachine.getCurrenState().GetType() != typeof(Sonar.FleeState)) { 
               animation.Draw(batch, scale);
               //batch.Draw(texture, boundingBox, GameColor.Red);
               //batch.Draw(Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Player/temp"), boundingBox, GameColor.White); // bounding box debug
               //batch.Draw(Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Player/temp"), new GameRectangle((int)position.X, (int)position.Y, 5, 5), GameColor.Red); //Debugging for spectre positon
                //if (target != null)
               //batch.Draw(Game1.contentManager.Load<GameTexture>(@"Textures/Objects/Entity/Player/temp"), new GameRectangle((int)target.GetPosition().X - 24, (int)target.GetPosition().Y - 24, MapUnit.MAX_SIZE, MapUnit.MAX_SIZE), GameColor.Red);
            }
        }

        /// <summary>
        /// Finds the X and Y grid intercepts of the line created by the Position and the Destination
        /// This function takes directly from the MapUnit.MaxSize Value
        /// Created By Joshua Ray
        /// 2/12/2012
        /// </summary>
        /// <param name="Position"> The Starting Position of the Line </param>
        /// <param name="Destination">The Destination of the Line</param>
        public List<GameVector2> componentIntercept(GameVector2 Position, GameVector2 Destination)
        {
            List<GameVector2> tempList = new List<GameVector2>();
            GameVector2 t = Util.getInstance().Slope(Position, Destination);
            float slope = t.Y / t.X;
            float min = Math.Min(Position.X, Destination.X);
            float max = Math.Max(Position.X, Destination.X);

            int I = (int)Math.Ceiling(min / MapUnit.MAX_SIZE);
            I = I * MapUnit.MAX_SIZE;


            // Finding the X intercepts
            // Increments X, Plugs in X value to find the Y Points
            // I = X
            while (I <= max)
            {
                GameVector2 temp = new GameVector2(0, 0);
                temp.Y = (slope) * (I - Position.X) + position.Y;
                temp.X = I;
                tempList.Add(temp);
                I += MapUnit.MAX_SIZE;
            }

            min = Math.Min(Position.Y, Destination.Y);
            max = Math.Max(Position.Y, Destination.Y);

            // Finding the Y intercepts
            // Increments Y, Plugs in Y value to find the XPoints
            // I = Y
            I = (int)Math.Ceiling(min / MapUnit.MAX_SIZE);
            I = I * MapUnit.MAX_SIZE;
            while (I <= max)
            {
                GameVector2 temp = new GameVector2(0, 0);
                temp.X = (I - position.Y) / (slope) + position.X;
                temp.Y = I;
                tempList.Add(temp);
                I += MapUnit.MAX_SIZE;
            }
            Util.getInstance().GridCoordinates(ref tempList);
            return tempList;
        }



        /// <summary>
        /// Getter for foundPath.
        /// </summary>
        /// <returns>foundPath</returns>
        public List<MapUnit> GetPath() {
            return foundPath;
        }

        public virtual bool Collide(GameRectangle rect, Collidable collidingWith)
        {
            if (possessing)
            {
                return true;
            }
            //Console.WriteLine(rect.Bottom + " " + boundingBox.Top + " " + boundingBox.Bottom);
            if ((((rect.Bottom > boundingBox.Top) && (rect.Bottom < boundingBox.Bottom)) || ((rect.Top > boundingBox.Top) && (rect.Top < boundingBox.Bottom))) &&
                (((rect.Left > boundingBox.Left) && (rect.Left < boundingBox.Right)) || ((rect.Right > boundingBox.Left) && (rect.Right < boundingBox.Right))))
            {
                //put whatever collision shit is supposed to happen here.
                //Console.WriteLine("Colliding with Spectre");
                if (collidingWith == Collidable.player && (inPlayer || isChasing || playerBeingHeard || playerVisible))
                {
                    inPlayer = true;
                }
                return true;
            }
            inPlayer = false;
            return false;
        }

        public int getID()
        {
            return this.id;
        }

        /// <summary>
        /// Sets all the doors walkable for a second, in order to plan a path and find the right door.
        /// </summary>
        public void setWalkable() {
            foreach (Door d in doors) {
                if (d.locked) {
                    map[(int)d.getIndex().X, (int)d.getIndex().Y].isWalkable = false;
                    map[(int)d.getOtherHalfOfDoorIndex().X, (int)d.getOtherHalfOfDoorIndex().Y].isWalkable = false;
                }
                else {
                    map[(int)d.getIndex().X, (int)d.getIndex().Y].isWalkable = true;
                    map[(int)d.getOtherHalfOfDoorIndex().X, (int)d.getOtherHalfOfDoorIndex().Y].isWalkable = true;
                }
                if (d.isOpen || d.isBroken) {
                    map[(int)d.getIndex().X, (int)d.getIndex().Y].isSeeThrough = true;
                    map[(int)d.getOtherHalfOfDoorIndex().X, (int)d.getOtherHalfOfDoorIndex().Y].isSeeThrough = true;
                }
                else {
                    map[(int)d.getIndex().X, (int)d.getIndex().Y].isSeeThrough = false;
                    map[(int)d.getOtherHalfOfDoorIndex().X, (int)d.getOtherHalfOfDoorIndex().Y].isSeeThrough = false;
                }
            }
        }

        /// <summary>
        /// Sets all the doors back to unwalkable.
        /// </summary>
        public void unSetWalkable() {
            foreach (Door d in doors) {
                map[(int) d.getIndex().X, (int) d.getIndex().Y].isWalkable = false;
                map[(int) d.getOtherHalfOfDoorIndex().X, (int) d.getOtherHalfOfDoorIndex().Y].isWalkable = false;
            }
        }


        #region AudioCommands
        public virtual void stopAudio()
        {
            if (grunt != null)
                SoundManager.Stop(ref grunt);
            if (roar != null)
                SoundManager.Stop(ref roar);
            if (walk != null)
                SoundManager.Stop(ref walk);
        }

        public virtual void pauseAudio()
        {
            if (grunt != null)
                SoundManager.Puase(ref grunt);
            if (roar != null)
                SoundManager.Puase(ref roar);
            if (walk != null)
                SoundManager.Puase(ref walk);
        }

        public virtual void unpauseAudio()
        {
            if (grunt != null)
            {
                if (grunt.IsPaused)
                    SoundManager.Play(ref grunt, SoundType.DUMB.DUMB_GROWL);
            }
            if (roar != null)
            {
                if (roar.IsPaused)
                    SoundManager.Play(ref roar, SoundType.WRATH.WRATH_ROAR);
            }
            if (walk != null)
            {
                if (walk.IsPaused)
                    SoundManager.Play(ref walk, SoundType.WRATH.WRATH_FOOTSTEP);
            }
        }
        #endregion AudioCommands

        #region AudioCues

        public virtual void playCue()
        {
            // Used only by the Siren
        }

        /// <summary>
        /// The Alert Sound
        /// </summary>
        public virtual void playAlertCue()
        {
            //Game1.camera.Shake(2f, 1f);
            this.storeSound(500f);
        }

        /// <summary>
        /// Played when Excorcised
        /// </summary>
        public virtual void playExcorcismCue() {
            //SoundManager.createSound(position, 500, 500, 1, excorcismCue, true, this);
        }
        
        /// <summary>
        /// Played when transitioning into the investigation state
        /// </summary>
        public virtual void playInvestigationCue()
        {
            if (playerBeingHeard)
            {
                //SoundManager.createSound(position, 500, 500, 1, SoundManager.WRATH.HEAR_PLAYER, true, this);
            }
            //SoundManager.createSound(position, 500, 500, 1, InvestigateCue, true, this);
        }

        /// <summary>
        /// Played when Player is Heard
        /// </summary>
        public virtual void playHeardCue()
        {
                //SoundManager.createSound(position, 500, 500, 1, SoundManager.WRATH.HEAR_PLAYER, true, this);
        }

        /// <summary>
        /// Returns the sound heard by the spectre
        /// </summary>
        /// <returns></returns>
        public Sound getSoundHeard()
        {
            return soundHeard;
        }

        /// <summary>
        /// returns the previous sound heard by the spectre
        /// </summary>
        /// <returns>The last sound.</returns>
        public Sound getLastSoundHeard()
        {
            return lastSoundHeard;
        }

        #endregion AudioCues

        #region Testing
        public string toString()
        {
            string s = "";
            foreach (MapUnit m in patrolPaths)
            {
                s += m.toString();
                s += "\n";
            }
            return "";
        }
        #endregion
    }
}
