using System;
using SadConsole;
using SadConsole.Components;
using SadRogue.Primitives;

namespace OingoBoingoConsole.ScreenExtensions
{
    /// <summary>Animated ASCII donut, renders directly into its ScreenSurface.</summary>
    public sealed class DonutPanel : ScreenSurface
    {
        private int _innerW, _innerH;
        private char[] _buf = Array.Empty<char>();
        private double[] _zbuf = Array.Empty<double>();
        private float[] _hueBuf = Array.Empty<float>();

        private double _A, _B;
        private double _hueScroll;

        public DonutPanel(int width, int height) : base(width, height)
        {
            Realloc(width, height);

            // Drive animation every frame
            SadComponents.Add(new DonutTick(this));
        }

        /// <summary>Resize the surface + internal buffers (safe to call on layout changes).</summary>
        public void ResizeTo(int width, int height)
        {
            var cs = (CellSurface)Surface;
            if (cs.Width != width || cs.Height != height || cs.ViewWidth != width || cs.ViewHeight != height)
                cs.Resize(width, height, width, height, clear: false);

            Realloc(width, height);
        }

        private void Realloc(int width, int height)
        {
            _innerW = Math.Max(2, width - 2);
            _innerH = Math.Max(2, height - 2);

            int n = _innerW * _innerH;
            _buf = n == _buf.Length ? _buf : new char[n];
            _zbuf = n == _zbuf.Length ? _zbuf : new double[n];
            _hueBuf = n == _hueBuf.Length ? _hueBuf : new float[n];
        }

        private void RenderFrame()
        {
            Array.Fill(_buf, ' ');
            Array.Fill(_zbuf, 0.0);

            const string shades = ".,-~:;=!*#$@";

            double cosA = Math.Cos(_A), sinA = Math.Sin(_A);
            double cosB = Math.Cos(_B), sinB = Math.Sin(_B);

            for (double j = 0; j < 2 * Math.PI; j += 0.07)
            {
                double cosj = Math.Cos(j), sinj = Math.Sin(j);
                double h = cosj + 2;

                for (double i = 0; i < 2 * Math.PI; i += 0.02)
                {
                    double cosi = Math.Cos(i), sini = Math.Sin(i);

                    double D = 1.0 / (sini * h * sinA + sinj * cosA + 5.0);
                    double t = sini * h * cosA - sinj * sinA;

                    double x3 = cosi * h * cosB - t * sinB;
                    double y3 = cosi * h * sinB + t * cosB;

                    int x = (int)(_innerW / 2.0 + (_innerW * 0.60) * D * x3);
                    int y = (int)(_innerH / 2.0 + (_innerH * 0.60) * D * y3);
                    if ((uint)x >= (uint)_innerW || (uint)y >= (uint)_innerH)
                        continue;

                    int o = x + _innerW * y;

                    double L = ((sinj * sinA - sini * cosj * cosA) * cosB)
                               - sini * cosj * sinA
                               - sinj * cosA
                               - cosi * cosj * sinB;

                    int n = (int)(8 * L);
                    if (n < 0) n = 0;
                    if (n >= shades.Length) n = shades.Length - 1;

                    if (D > _zbuf[o])
                    {
                        _zbuf[o] = D;
                        _buf[o] = shades[n];

                        float hue = Repeat01(Math.Atan2(y3, x3) / (2 * Math.PI) + 0.5 + _hueScroll);
                        _hueBuf[o] = hue;
                    }
                }
            }

            var surf = Surface;
            int idx = 0;
            for (int row = 0; row < _innerH; row++)
            {
                for (int col = 0; col < _innerW; col++)
                {
                    char ch = _buf[idx + col];
                    if (ch == ' ')
                    {
                        surf.SetCellAppearance(1 + col, 1 + row, new ColoredGlyph(Color.Black, Color.Black, ' '));
                    }
                    else
                    {
                        const string shadesLocal = ".,-~:;=!*#$@";
                        int shadeIndex = shadesLocal.IndexOf(ch);
                        float lit = 1f - shadeIndex / (float)(shadesLocal.Length - 1);
                        float sat = MathF.Pow(1f - lit, 0.8f);
                        float light = 0.30f + 0.45f * lit;

                        var fg = Color.FromHSL(_hueBuf[idx + col] * 360f, sat, light);
                        surf.SetCellAppearance(1 + col, 1 + row, new ColoredGlyph(fg, Color.Black, ch));
                    }
                }
                idx += _innerW;
            }

            _A += 0.04;
            _B += 0.02;
            _hueScroll = Repeat01(_hueScroll + 0.003);
        }

        private static float Repeat01(double x) => (float)(x - Math.Floor(x));

        private sealed class DonutTick : UpdateComponent
        {
            private readonly DonutPanel _owner;
            public DonutTick(DonutPanel owner) => _owner = owner;
            public override void Update(IScreenObject host, TimeSpan delta) => _owner.RenderFrame();
        }
    }
}
