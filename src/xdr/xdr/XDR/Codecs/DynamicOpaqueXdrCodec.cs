namespace cc.isr.XDR.Codecs;

/// <summary>
/// Instances of the class <see cref="DynamicOpaqueXdrCodec"/> represent serializable dynamic-size
/// opaque values, which are especially useful in cases where a result with only a single opaque
/// value is expected from a remote function call or only a single opaque value parameter needs
/// to be supplied.
/// </summary>
/// <remarks>
/// The XDR data type wrapper classes wrap their value as read only properties, which are set
/// upon construction and decoding. <para>
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public class DynamicOpaqueXdrCodec : IXdrCodec
{
    /// <summary>   Constructs and initializes a new <see cref="DynamicOpaqueXdrCodec"/> object. </summary>
    public DynamicOpaqueXdrCodec()
    {
        this._value = null;
    }

    /// <summary>   Constructs and initializes a new <see cref="DynamicOpaqueXdrCodec"/> object. </summary>
    /// <param name="value">    The encapsulated opaque value itself. </param>
    public DynamicOpaqueXdrCodec( byte[] value )
    {
        this._value = value;
    }

    /// <summary>The encapsulated opaque value itself.</summary>
    private byte[] _value;

    /// <summary>
    /// Returns the value of this <see cref="DynamicOpaqueXdrCodec"/> object as a <see cref="byte"/> vector.
    /// </summary>
    /// <returns>   The primitive <see cref="byte"/> array value of this object. </returns>
    public virtual byte[] GetValue()
    {
        return this._value;
    }

    /// <summary>
    /// Encodes -- that is: serializes -- a XDR opaque into an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="xdr">  XDR stream to which information is sent for encoding. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Encode( XdrEncodingStreamBase xdr )
    {
        xdr.EncodeDynamicOpaque( this._value );
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- a XDR opaque from an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="xdr">  XDR stream from which decoded information is retrieved. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Decode( XdrDecodingStreamBase xdr )
    {
        this._value = xdr.DecodeDynamicOpaque();
    }

}
