namespace ChibiKnight.Gameplay
{
    public struct AnimationData
    {
        public AnimationData(int id, string name, bool loop) : this()
        {
            this.id = id;
            this.name = name;
            this.loop = loop;
            mixDuration = -1f;
        }

        public AnimationData(int id, string name, bool loop, float mixDuration) : this()
        {
            this.id = id;
            this.name = name;
            this.loop = loop;
            this.mixDuration = mixDuration;
        }

        public int id { get; }
        public string name { get; }
        public bool loop { get; }
        public float mixDuration { get; }
    }

}