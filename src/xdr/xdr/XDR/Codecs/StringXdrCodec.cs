namespace cc.isr.XDR.Codecs;

/// <summary>
/// Instances of the class <see cref="StringXdrCodec"/> represent serializable strings, which are
/// especially useful in cases where a result with only a single string is expected from a remote
/// function call or only a single string parameter needs to be supplied.
/// </summary>
/// <remarks>
/// The XDR data type wrapper classes wrap their value as read only properties, which are set
/// upon construction and decoding. <para>
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public class StringXdrCodec : IXdrCodec
{
    /// <summary>   Constructs and initializes a new <see cref="StringXdrCodec"/> object. </summary>
    /// <param name="value">    String value. </param>
    public StringXdrCodec( string value )
    {
        this.Value = value;
    }

    /// <summary>   Constructs and initializes a new <see cref="StringXdrCodec"/> object. </summary>
    public StringXdrCodec()
    {
        this.Value = null;
    }

    /// <summary>   Gets or sets the value of this <see cref="StringXdrCodec"/> object as a <see cref="string"/> primitive. </summary>
    /// <value> The primitive <see cref="string"/> value of this object.. </value>
    public string Value { get; private set; }

    /// <summary>
    /// Encodes -- that is: serializes -- a XDR string into an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="encoder">  XDR stream to which information is sent for encoding. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Encode( XdrEncodingStreamBase encoder )
    {
        encoder.EncodeString( this.Value );
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- a XDR string from an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="decoder">  XDR stream from which decoded information is retrieved. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Decode( XdrDecodingStreamBase decoder )
    {
        this.Value = decoder.DecodeString();
    }

}
