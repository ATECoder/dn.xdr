namespace cc.isr.XDR.Codecs;

/// <summary>
/// Instances of the class <see cref="DoubleXdrCodec"/> represent serializable <see cref="double"/>s, which are
/// especially useful in cases where a result with only a single <see cref="double"/> is expected from a remote
/// function call or only a single <see cref="double"/> parameter needs to be supplied.
/// </summary>
/// <remarks>
/// The XDR data type wrapper classes wrap their value as read only properties, which are set
/// upon construction and decoding. <para>
/// 
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public class DoubleXdrCodec : IXdrCodec
{
    /// <summary>   Constructs and initializes a new <see cref="DoubleXdrCodec"/> object. </summary>
    /// <summary>   Constructs and initializes a new <see cref="DoubleXdrCodec"/> object. </summary>
    /// <param name="value">    Double value. </param>
    public DoubleXdrCodec( double value )
    {
        this.Value = value;
    }

    /// <summary>   Constructs and initializes a new <see cref="DoubleXdrCodec"/> object. </summary>
    public DoubleXdrCodec()
    {
        this.Value = 0;
    }

    /// <summary>   Gets or sets the value of this <see cref="DoubleXdrCodec"/> object as a <see cref="double"/> primitive.. </summary>
    /// <value> The primitive <see cref="double"/> value of this object. </value>
    public double Value { get; private set; }

    /// <summary>
    /// Encodes -- that is: serializes -- a XDR double into an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="encoder">  XDR stream to which information is sent for encoding. </param>
    public virtual void Encode( XdrEncodingStreamBase encoder )
    {
        encoder.EncodeDouble( this.Value );
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- a XDR double from an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="decoder">  XDR stream from which decoded information is retrieved. </param>
    public virtual void Decode( XdrDecodingStreamBase decoder )
    {
        this.Value = decoder.DecodeDouble();
    }

}
