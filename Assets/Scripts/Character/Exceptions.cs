using System;

[Serializable]
public class MovementException : Exception {
    public MovementException() : base() { }
    public MovementException(string message) : base(message) { }

    // A constructor is needed for serialization when an
    // exception propagates from a remoting server to the client.
    protected MovementException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}