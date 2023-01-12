namespace cc.isr.XDR.Codecs;

/// <summary>
/// Instances of the class <see cref="CharXdrCodec"/> represent serializable chars, which are
/// especially useful in cases where a result with only a single char is expected from a remote
/// function call or only a single char parameter needs to be supplied.
/// </summary>
/// <remarks>
/// The XDR data type wrapper classes wrap their value as read only properties, which are set
/// upon construction and decoding. <para>
/// 
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public class CharXdrCodec : IXdrCodec
{
    /// <summary>   Constructs and initializes a new <see cref="CharXdrCodec"/> object. </summary>
    /// <param name="value">    Char value. </param>
    public CharXdrCodec( char value )
    {
        this.Value = value;
    }

    /// <summary>   Constructs and initializes a new <see cref="CharXdrCodec"/> object. </summary>
    public CharXdrCodec()
    {
        this.Value = ( char ) 0;
    }

    /// <summary>   Gets or sets the value of this <see cref="CharXdrCodec"/> object as a <see cref="char"/> primitive. </summary>
    /// <value> The primitive <see cref="char"/> value of this object. </value>
    public char Value { get; private set; }

    /// <summary>
    /// Encodes -- that is: serializes -- a XDR char into an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="encoder">  XDR stream to which information is sent for encoding. </param>
    public virtual void Encode( XdrEncodingStreamBase encoder )
    {
        encoder.EncodeByte( ( byte ) this.Value );
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- a XDR char from an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="decoder">  XDR stream from which decoded information is retrieved. </param>
    public virtual void Decode( XdrDecodingStreamBase decoder )
    {
        this.Value = ( char ) decoder.DecodeByte();
    }

}
