namespace cc.isr.XDR.Codecs;

/// <summary>
/// Instances of the class <see cref="LongXdrCodec"/> represent serializable longs (64 bit), which
/// are especially useful in cases where a result with only a single long is expected from a
/// remote function call or only a single long parameter needs to be supplied.
/// </summary>
/// <remarks>
/// The XDR data type wrapper classes wrap their value as read only properties, which are set
/// upon construction and decoding. <para>
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public class LongXdrCodec : IXdrCodec
{
    /// <summary>   Constructs and initializes a new <see cref="LongXdrCodec"/> object. </summary>
    /// <param name="value">    Long value. </param>
    public LongXdrCodec( long value )
    {
        this.Value = value;
    }

    /// <summary>   Constructs and initializes a new <see cref="LongXdrCodec"/> object. </summary>
    public LongXdrCodec()
    {
        this.Value = 0;
    }

    /// <summary>   Gets or sets the value of this <see cref="LongXdrCodec"/> object as a <see cref="long"/> primitive. </summary>
    /// <value> The primitive <see cref="long"/> value of this object. </value>
    public long Value { get; private set; }

    /// <summary>
    /// Encodes -- that is: serializes -- a XDR long into an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="xdr">  XDR stream to which information is sent for encoding. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Encode( XdrEncodingStreamBase xdr )
    {
        xdr.EncodeLong( this.Value );
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- a XDR long from an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="xdr">  XDR stream from which decoded information is retrieved. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Decode( XdrDecodingStreamBase xdr )
    {
        this.Value = xdr.DecodeLong();
    }

}
