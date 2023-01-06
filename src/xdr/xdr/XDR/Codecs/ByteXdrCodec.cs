namespace cc.isr.XDR.Codecs;

/// <summary>
/// Instances of the class <see cref="ByteXdrCodec"/> represent a serializable byte, which are
/// especially useful in cases where a result with only a single byte is expected from a remote
/// function call or only a single byte parameter needs to be supplied.
/// </summary>
/// <remarks>
/// The XDR data type wrapper classes wrap their value as read only properties, which are set
/// upon construction and decoding. <para>
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public class ByteXdrCodec : IXdrCodec
{
    /// <summary>   Constructs and initializes a new <see cref="ByteXdrCodec"/> object. </summary>
    /// <param name="value">    Byte value. </param>
    public ByteXdrCodec( byte value )
    {
        this.Value = value;
    }

    /// <summary>   Constructs and initializes a new <see cref="ByteXdrCodec"/> object. </summary>
    public ByteXdrCodec()
    {
        this.Value = 0;
    }

    /// <summary>   Gets or sets the value of this <see cref="ByteXdrCodec"/> object as a <see cref="byte"/> primitive. </summary>
    /// <value> The primitive <see cref="byte"/> value of this object. </value>
    public byte Value { get; private set; }

    /// <summary>
    /// Encodes -- that is: serializes -- a XDR byte into an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="encoder">  XDR stream to which information is sent for encoding. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Encode( XdrEncodingStreamBase encoder )
    {
        encoder.EncodeByte( this.Value );
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- a XDR byte from an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="decoder">  XDR stream from which decoded information is retrieved. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Decode( XdrDecodingStreamBase decoder )
    {
        this.Value = decoder.DecodeByte();
    }

}