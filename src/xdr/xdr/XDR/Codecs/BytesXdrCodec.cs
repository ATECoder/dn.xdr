namespace cc.isr.XDR.Codecs;

/// <summary>
/// Instances of the class <see cref="BytesXdrCodec"/> represent serializable bytes values, which
/// are especially useful in cases where a result with only a single bytes value is expected from
/// a remote function call or only a single bytes value parameter needs to be supplied.
/// </summary>
/// <remarks>
/// The XDR data type wrapper classes wrap their value as read only properties, which are set
/// upon construction and decoding. <para>
/// 
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public class BytesXdrCodec : IXdrCodec
{
    /// <summary>   Constructs and initializes a new <see cref="BytesXdrCodec"/> object. </summary>
    /// <param name="value">    bytes vector. </param>
    public BytesXdrCodec( byte[] value )
    {
        this._value = value;
    }

    /// <summary>   Constructs and initializes a new <see cref="BytesXdrCodec"/> object. </summary>
    public BytesXdrCodec() : this( Array.Empty<byte>() )
    {
    }

    /// <summary>The encapsulated bytes value itself.</summary>
    private byte[] _value;

    /// <summary>   Returns the value of this <see cref="BytesXdrCodec"/> object as a <see cref="byte"/> vector. </summary>
    /// <returns>   The primitive <see cref="byte"/>[] value of this object. </returns>
    public virtual byte[] GetValue()
    {
        return this._value;
    }

    /// <summary>
    /// Encodes -- that is: serializes -- a XDR bytes value into an XDR stream in compliance to RFC
    /// 1832.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="encoder">  XDR stream to which information is sent for encoding. </param>
    public virtual void Encode( XdrEncodingStreamBase encoder )
    {
        encoder.EncodeByteVector( this._value );
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- a XDR bytes value from an XDR stream in compliance to RFC
    /// 1832.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="decoder">  XDR stream from which decoded information is retrieved. </param>
    public virtual void Decode( XdrDecodingStreamBase decoder )
    {
        this._value = decoder.DecodeByteVector();
    }

}
