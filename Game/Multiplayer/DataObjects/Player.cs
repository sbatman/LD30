namespace LD30.Multiplayer.DataObjects
{
    class Player
    {
        private long _ID;
        private string _Name;

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
