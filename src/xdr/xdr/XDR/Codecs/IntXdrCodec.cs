namespace cc.isr.XDR.Codecs;

/// <summary>
/// Instances of the class <see cref="IntXdrCodec"/> represent serializable <see cref="int"/>s, which are
/// especially useful in cases where a result with only a single <see cref="int"/> is expected from a remote
/// function call or only a single <see cref="int"/> parameter needs to be supplied.
/// </summary>
/// <remarks>
/// The XDR data type wrapper classes wrap their value as read only properties, which are set
/// upon construction and decoding. <para>
/// 
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public class IntXdrCodec : IXdrCodec
{
    /// <summary>   Constructs and initializes a new <see cref="IntXdrCodec"/> object. </summary>
    /// <param name="value">    Int value. </param>
    public IntXdrCodec( int value )
    {
        this.Value = value;
    }

    /// <summary>   Constructs and initializes a new <see cref="IntXdrCodec"/> object. </summary>
    public IntXdrCodec()
    {
        this.Value = 0;
    }

    /// <summary>   Gets or sets the value of this <see cref="IntXdrCodec"/> object as an <see cref="int"/> primitive. </summary>
    /// <value> The primitive <see cref="int"/> value of this object. </value>
    public int Value { get; private set; }

    /// <summary>
    /// Encodes -- that is: serializes -- a XDR int into an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="encoder">  XDR stream to which information is sent for encoding. </param>
    public virtual void Encode( XdrEncodingStreamBase encoder )
    {
        encoder.EncodeInt( this.Value );
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- a XDR int from an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="decoder">  XDR stream from which decoded information is retrieved. </param>
    public virtual void Decode( XdrDecodingStreamBase decoder )
    {
        this.Value = decoder.DecodeInt();
    }

}
