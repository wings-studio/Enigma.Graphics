using System;
using System.Numerics;

namespace Enigma.Graphics
{
    public class FloatResource : IResource
    {
        public float Value;

        public FloatResource(float value)
        {
            Value = value;
        }

        public void SetResources(IGraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetUniform1(Value);
        }
    }
    public class IntResource : IResource
    {
        public int Value;

        public IntResource(int value)
        {
            Value = value;
        }

        public void SetResources(IGraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetUniform1(Value);
        }
    }
    public class DoubleResource : IResource
    {
        public double Value;

        public DoubleResource(double value)
        {
            Value = value;
        }

        public void SetResources(IGraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetUniform1(Value);
        }
    }
    public class Matrix4x4Resource : IResource
    {
        public Matrix4x4 Matrix;

        public Matrix4x4Resource(Matrix4x4 matrix)
        {
            Matrix = matrix;
        }

        public void SetResources(IGraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetUniformMatrix4x4(Matrix);
        }
    }
}
