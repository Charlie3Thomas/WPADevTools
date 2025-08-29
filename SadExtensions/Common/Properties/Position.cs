namespace WPADevTools.SadExtensions.Common.Components
{
    public readonly struct Position
    {
        private readonly int _x;
        private readonly int _y;

        public Position(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public readonly int X => _x;
        public readonly int Y => _y;
    }
}
