﻿using System;
using System.Collections.Generic;
using System.Numerics;
using Veldrid;

namespace Enigma.Graphics.Objects
{
    public sealed class Camera
    {
        public Vector3 Position { get => pos; set { pos = value; UpdateViewMatrix(); } }
        public Vector3 LookDirection { get; private set; }
        public Matrix4x4 ViewMatrix { get; private set; }
        public Matrix4x4 ProjectionMatrix { get; private set; }
        public float FieldOfView { get; set; } = 1f;
        public float Near { get; private set; } = 1f;
        public float Far { get; private set; } = 1000f;
        public float Yaw { get => yaw; set { yaw = value; UpdateViewMatrix(); } }
        public float Pitch { get => pitch; set { pitch = value; UpdateViewMatrix(); } }
        public float AspectRatio => windowWidth / windowHeight;
        public GraphicsDevice GraphicsDevice { get; set; }
        public CommandList CommandList { get; set; }

        private bool UseReverseDepth => GraphicsDevice.IsDepthRangeZeroToOne;

        private Vector3 pos = Vector3.Zero;
        private float yaw, pitch, windowHeight, windowWidth;

        /// <summary>
        /// Inits camera
        /// </summary>
        /// <param name="height">Height of rendering window</param>
        /// <param name="width">Width of rendering window</param>
        public Camera(float width, float height)
        {
            windowHeight = height;
            windowWidth = width;
        }

        public void WindowResized(float width, float height)
        {
            windowWidth = width;
            windowHeight = height;
            UpdatePerspectiveMatrix();
        }

        public void UpdatePerspectiveMatrix()
        {
            ProjectionMatrix = Util.CreatePerspective(
                GraphicsDevice,
                UseReverseDepth,
                FieldOfView,
                AspectRatio,
                Near,
                Far);
        }

        public void UpdateViewMatrix()
        {
            Quaternion lookRotation = Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, 0f);
            LookDirection = Vector3.Transform(-Vector3.UnitZ, lookRotation);
            ViewMatrix = Matrix4x4.CreateLookAt(pos, pos + LookDirection, Vector3.UnitY);
        }
    }
}