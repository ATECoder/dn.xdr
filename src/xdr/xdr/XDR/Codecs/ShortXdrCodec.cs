namespace cc.isr.XDR.Codecs;

/// <summary>
/// Instances of the class <see cref="ShortXdrCodec"/> represent serializable shorts, which are
/// especially useful in cases where a result with only a single short is expected from a remote
/// function call or only a single short parameter needs to be supplied.
/// </summary>
/// <remarks>
/// The XDR data type wrapper classes wrap their value as read only properties, which are set
/// upon construction and decoding. <para>
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public class ShortXdrCodec : IXdrCodec
{
    /// <summary>   Constructs and initializes a new <see cref="ShortXdrCodec"/> object. </summary>
    /// <param name="value">    Short value. </param>
    public ShortXdrCodec( short value )
    {
        this.Value = value;
    }

    /// <summary>   Constructs and initializes a new <see cref="ShortXdrCodec"/> object. </summary>
    public ShortXdrCodec()
    {
        this.Value = 0;
    }

    /// <summary>   Gets or sets the value of this <see cref="ShortXdrCodec"/> object as a <see cref="short"/> primitive. </summary>
    /// <value> The primitive <see cref="short"/> value of this object. </value>
    public short Value { get; private set; }

    /// <summary>
    /// Encodes -- that is: serializes -- a XDR short into an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="xdr">  XDR stream to which information is sent for encoding. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Encode( XdrEncodingStreamBase xdr )
    {
        xdr.EncodeShort( this.Value );
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- a XDR short from an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="xdr">  XDR stream from which decoded information is retrieved. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Decode( XdrDecodingStreamBase xdr )
    {
        this.Value = xdr.DecodeShort();
    }

}
