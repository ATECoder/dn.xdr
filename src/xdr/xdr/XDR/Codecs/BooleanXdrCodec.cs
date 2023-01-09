namespace cc.isr.XDR.Codecs;

/// <summary>
/// Instances of the class <see cref="BooleanXdrCodec"/> represent a <see cref="bool"/> <see cref="IXdrCodec"/>
/// , which are especially useful in cases where a result with only a single boolean is expected
/// from a remote function call or only a single boolean parameter needs to be supplied.
/// </summary>
/// <remarks>
/// The XDR data type wrapper classes wrap their value as read only properties, which are set
/// upon construction and decoding. <para>
/// Remote Tea authors: Harald Albrecht, Jay Walters. </para>
/// </remarks>
public class BooleanXdrCodec : IXdrCodec
{
    /// <summary>   Constructs and initializes a new <see cref="BooleanXdrCodec"/> object. </summary>
    /// <param name="value">    Boolean value. </param>
    public BooleanXdrCodec( bool value )
    {
        this.Value = value;
    }

    /// <summary>   Constructs and initializes a new <see cref="BooleanXdrCodec"/> object. </summary>
    public BooleanXdrCodec()
    {
        this.Value = false;
    }

    /// <summary>
    /// Gets or sets the value of this <see cref="BooleanXdrCodec"/> object as a <see cref="bool"/> primitive.
    /// </summary>
    /// <value> The primitive <see cref="bool"/> value of this object. </value>
    public bool Value { get; private set; }

    /// <summary>
    /// Encodes -- that is: serializes -- a XDR boolean into an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="encoder">  XDR stream to which information is sent for encoding. </param>
    public virtual void Encode( XdrEncodingStreamBase encoder )
    {
        encoder.EcodeBoolean( this.Value );
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- a XDR boolean from an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="decoder">  XDR stream from which decoded information is retrieved. </param>
    public virtual void Decode( XdrDecodingStreamBase decoder )
    {
        this.Value = decoder.DecodeBoolean();
    }

}
