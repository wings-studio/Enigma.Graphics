using System;
using System.IO;
using System.Numerics;
using System.Collections.Generic;
using Veldrid;
using Veldrid.Utilities;
using Unsafe = System.Runtime.CompilerServices.Unsafe;

namespace Enigma.Graphics
{
    public static class Util
    {
        public static uint SizeInBytes<T>(this T[] array)
        {
            return (uint)(array.Length * Unsafe.SizeOf<T>());
        }

        public static uint SizeOf<T>() => (uint)Unsafe.SizeOf<T>();

        // Code adapted from https://bitbucket.org/sinbad/ogre/src/9db75e3ba05c/OgreMain/include/OgreVector3.h
        public static Quaternion FromToRotation(Vector3 from, Vector3 to, Vector3 fallbackAxis = default)
        {
            // Based on Stan Melax's article in Game Programming Gems
            Quaternion q;
            // Copy, since cannot modify local
            Vector3 v0 = from;
            Vector3 v1 = to;
            v0 = Vector3.Normalize(v0);
            v1 = Vector3.Normalize(v1);

            float d = Vector3.Dot(v0, v1);
            // If dot == 1, vectors are the same
            if (d >= 1.0f)
            {
                return Quaternion.Identity;
            }
            if (d < (1e-6f - 1.0f))
            {
                if (fallbackAxis != Vector3.Zero)
                {
                    // rotate 180 degrees about the fallback axis
                    q = Quaternion.CreateFromAxisAngle(fallbackAxis, (float)Math.PI);
                }
                else
                {
                    // Generate an axis
                    Vector3 axis = Vector3.Cross(Vector3.UnitX, from);
                    if (axis.LengthSquared() == 0) // pick another if colinear
                    {
                        axis = Vector3.Cross(Vector3.UnitY, from);
                    }

                    axis = Vector3.Normalize(axis);
                    q = Quaternion.CreateFromAxisAngle(axis, (float)Math.PI);
                }
            }
            else
            {
                float s = (float)Math.Sqrt((1 + d) * 2);
                float invs = 1.0f / s;

                Vector3 c = Vector3.Cross(v0, v1);

                q.X = c.X * invs;
                q.Y = c.Y * invs;
                q.Z = c.Z * invs;
                q.W = s * 0.5f;
                q = Quaternion.Normalize(q);
            }
            return q;
        }

        // modifies projection matrix in place
        // clipPlane is in camera space
        public static void CalculateObliqueMatrixPerspective(ref Matrix4x4 projection, Matrix4x4 view, Plane clipPlane)
        {
            Matrix4x4 invTransposeView = VdUtilities.CalculateInverseTranspose(view);
            Vector4 clipV4 = new Vector4(clipPlane.Normal, clipPlane.D);
            clipV4 = Vector4.Transform(clipV4, invTransposeView);

            Vector4 q = new Vector4(
                (Math.Sign(clipV4.X) + projection.M13) / projection.M11,
                (Math.Sign(clipV4.Y) + projection.M23) / projection.M22,
                -1f,
                (1 + projection.M33) / projection.M34);

            Vector4 c = clipV4 * (1f / Vector4.Dot(clipV4, q));

            projection.M31 = c.X;
            projection.M32 = c.Y;
            projection.M33 = c.Z;
            projection.M34 = c.W;
        }

        public static Matrix4x4 Inverse(this Matrix4x4 src)
        {
            Matrix4x4.Invert(src, out Matrix4x4 result);
            return result;
        }

        public static Matrix4x4 CreatePerspective(
            this GraphicsDevice gd,
            bool useReverseDepth,
            float fov,
            float aspectRatio,
            float near, float far)
        {
            Matrix4x4 persp;
            if (useReverseDepth)
            {
                persp = CreatePerspective(fov, aspectRatio, far, near);
            }
            else
            {
                persp = CreatePerspective(fov, aspectRatio, near, far);
            }
            if (gd.IsClipSpaceYInverted)
            {
                persp *= new Matrix4x4(
                    1, 0, 0, 0,
                    0, -1, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1);
            }

            return persp;
        }

        public static Matrix4x4 CreatePerspective(float fov, float aspectRatio, float near, float far)
        {
            if (fov <= 0.0f || fov >= MathF.PI)
                throw new ArgumentOutOfRangeException(nameof(fov));

            if (near <= 0.0f)
                throw new ArgumentOutOfRangeException(nameof(near));

            if (far <= 0.0f)
                throw new ArgumentOutOfRangeException(nameof(far));

            float yScale = 1.0f / MathF.Tan(fov * 0.5f);
            float xScale = yScale / aspectRatio;

            Matrix4x4 result;

            result.M11 = xScale;
            result.M12 = result.M13 = result.M14 = 0.0f;

            result.M22 = yScale;
            result.M21 = result.M23 = result.M24 = 0.0f;

            result.M31 = result.M32 = 0.0f;
            var negFarRange = float.IsPositiveInfinity(far) ? -1.0f : far / (near - far);
            result.M33 = negFarRange;
            result.M34 = -1.0f;

            result.M41 = result.M42 = result.M44 = 0.0f;
            result.M43 = near * negFarRange;

            return result;
        }

        public static unsafe Texture GetColorTexture(this GraphicsDevice gd, ResourceFactory factory, RgbaByte color)
        {
            Texture _colorTex = factory.CreateTexture(TextureDescription.Texture2D(1, 1, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled));
            gd.UpdateTexture(_colorTex, (IntPtr)(&color), SizeOf<RgbaByte>(), 0, 0, 0, 1, 1, 1, 0, 0);
            return _colorTex;
        }

        public static Matrix4x4 CreateOrtho(
            this GraphicsDevice gd,
            bool useReverseDepth,
            float left, float right,
            float bottom, float top,
            float near, float far)
        {
            Matrix4x4 ortho;
            if (useReverseDepth)
            {
                ortho = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, far, near);
            }
            else
            {
                ortho = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, near, far);
            }
            if (gd.IsClipSpaceYInverted)
            {
                ortho *= new Matrix4x4(
                    1, 0, 0, 0,
                    0, -1, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1);
            }

            return ortho;
        }

        public static float[] GetFullScreenQuadVerts(this GraphicsDevice gd)
        {
            if (gd.IsClipSpaceYInverted)
            {
                return new float[]
                {
                        -1, -1, 0, 0,
                        1, -1, 1, 0,
                        1, 1, 1, 1,
                        -1, 1, 0, 1
                };
            }
            else
            {
                return new float[]
                {
                        -1, 1, 0, 0,
                        1, 1, 1, 0,
                        1, -1, 1, 1,
                        -1, -1, 0, 1
                };
            }
        }

        internal static Stream OpenResourcesStream() 
            => typeof(Util).Assembly.GetManifestResourceStream(typeof(Util).Assembly.GetManifestResourceNames()[0]);

        internal static byte[] ReadBytesFromResources(string name)
        {
            return (byte[])Properties.Resources.ResourceManager.GetObject(name);
        }

        public static Vector3 ToNumerics(this Assimp.Vector3D vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static Vector3[] ConvertToVectorArray(this List<Assimp.Vector3D> vectors)
        {
            Vector3[] v = new Vector3[vectors.Count];
            for (int i = 0; i < vectors.Count; i++)
            {
                v[i] = vectors[i].ToNumerics();
            }
            return v;
        }
    }
}
