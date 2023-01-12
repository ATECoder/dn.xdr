namespace cc.isr.XDR.Codecs;

/// <summary>
/// Instances of the class <see cref="OpaqueXdrCodec"/> represent serializable fixed-size opaque
/// values, which are especially useful in cases where a result with only a single opaque value
/// is expected from a remote function call or only a single opaque value parameter needs to be
/// supplied.
/// </summary>
/// <remarks>
/// The XDR data type wrapper classes wrap their value as read only properties, which are set
/// upon construction and decoding. <para>
///  
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public class OpaqueXdrCodec : IXdrCodec
{
    /// <summary>   Constructs and initializes a new <see cref="OpaqueXdrCodec"/> object. </summary>
    /// <param name="value">    The encapsulated opaque value itself. </param>
    public OpaqueXdrCodec( byte[] value )
    {
        this._value = value;
    }

    /// <summary>
    /// Constructs and initializes a new <see cref="OpaqueXdrCodec"/> object given only the size of the
    /// opaque value.
    /// </summary>
    /// <param name="length">   size of opaque value. </param>
    public OpaqueXdrCodec( int length )
    {
        this._value = new byte[length];
    }

    /// <summary>The encapsulated opaque value itself.</summary>
    private readonly byte[] _value;

    /// <summary>   Returns the value of this <see cref="OpaqueXdrCodec"/> object as a byte vector. </summary>
    /// <returns>   The primitive <see cref="byte"/> array value of this object. </returns>
    public virtual byte[] GetValue()
    {
        return this._value;
    }

    /// <summary>
    /// Encodes -- that is: serializes -- a XDR opaque into an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="encoder">  XDR stream to which information is sent for encoding. </param>
    public virtual void Encode( XdrEncodingStreamBase encoder )
    {
        encoder.EncodeOpaque( this._value );
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- a XDR opaque from an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="decoder">  XDR stream from which decoded information is retrieved. </param>
    public virtual void Decode( XdrDecodingStreamBase decoder )
    {
        decoder.DecodeOpaque( this._value );
    }

}
