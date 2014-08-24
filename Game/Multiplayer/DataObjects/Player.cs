using LD30.Multiplayer.Ghosts;

namespace LD30.Multiplayer.DataObjects
{
    class Player
    {
        private long _ID;
        private string _Name;
        private int _WorldOffset;
        private Ghosts.Level _Level;

        public long ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public int WorldOffset
        {
            get { return _WorldOffset; }
            set { _WorldOffset = value; }
        }

        

        public Level Level
        {
            get { return _Level; }
            set { _Level = value; }
        }

        public Player()
        {

        }

        public Player(long id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}
