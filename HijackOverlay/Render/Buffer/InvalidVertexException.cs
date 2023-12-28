using System;

namespace HijackOverlay.Render.Buffer
{
    public class InvalidVertexException : Exception
    {
        public InvalidVertexException(VertexModes vertexModes) : base($"Invalid Vertex Construct for Mode: {vertexModes}")
        {
        }
    }
}