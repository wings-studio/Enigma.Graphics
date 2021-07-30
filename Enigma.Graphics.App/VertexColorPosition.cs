namespace Enigma.Graphics.App
{
    public struct VertexColorPosition
    {
        public float X;
        public float Y;
        public float Z;

        public float R;
        public float G;
        public float B;
        public float A;

        public VertexColorPosition(float x, float y, float z, float r, float g, float b, float a)
        {
            X = x;
            Y = y;
            Z = z;
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}
