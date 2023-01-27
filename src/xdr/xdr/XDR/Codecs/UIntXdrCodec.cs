namespace cc.isr.XDR.Codecs;

/// <summary>
/// Instances of the class <see cref="UIntXdrCodec"/> represent serializable <see cref="uint"/>s, which are
/// especially useful in cases where a result with only a single <see cref="uint"/> is expected from a remote
/// function call or only a single <see cref="uint"/> parameter needs to be supplied.
/// </summary>
/// <remarks>
/// The XDR data type wrapper classes wrap their value as read only properties, which are set
/// upon construction and decoding.
/// </remarks>
public class UIntXdrCodec : IXdrCodec
{
    /// <summary>   Constructs and initializes a new <see cref="UIntXdrCodec"/> object. </summary>
    /// <param name="value">    Int value. </param>
    public UIntXdrCodec( uint value )
    {
        this.Value = value;
    }

    /// <summary>   Constructs and initializes a new <see cref="UIntXdrCodec"/> object. </summary>
    public UIntXdrCodec()
    {
        this.Value = 0;
    }

    /// <summary>   Gets or sets the value of this <see cref="UIntXdrCodec"/> object as an <see cref="uint"/> primitive. </summary>
    /// <value> The primitive <see cref="uint"/> value of this object. </value>
    public uint Value { get; private set; }

    /// <summary>
    /// Encodes -- that is: serializes -- a XDR <see cref="uint"/> into an XDR stream in compliance
    /// to RFC 1832.
    /// </summary>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="encoder">  XDR stream to which information is sent for encoding. </param>
    public virtual void Encode( XdrEncodingStreamBase encoder )
    {
        encoder.EncodeUInt( this.Value );
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- a XDR <see cref="uint"/> from an XDR stream in compliance
    /// to RFC 1832.
    /// </summary>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="decoder">  XDR stream from which decoded information is retrieved. </param>
    public virtual void Decode( XdrDecodingStreamBase decoder )
    {
        this.Value = decoder.DecodeUInt();
    }

}
