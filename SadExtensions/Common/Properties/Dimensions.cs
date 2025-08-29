namespace WPADevTools.SadExtensions.Common.Components
{
    public readonly struct Dimensions
    {
        private readonly int _width;
        private readonly int _height;

        public Dimensions(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public readonly int Width => _width;
        public readonly int Height => _height;
    }
}
