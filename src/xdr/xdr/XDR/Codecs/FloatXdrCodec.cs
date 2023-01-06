namespace cc.isr.XDR.Codecs;

/// <summary>
/// Instances of the class <see cref="FloatXdrCodec"/> represent serializable floats, which are
/// especially useful in cases where a result with only a single float is expected from a remote
/// function call or only a single float parameter needs to be supplied.
/// </summary>
/// <remarks>
/// The XDR data type wrapper classes wrap their value as read only properties, which are set
/// upon construction and decoding. <para>
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public class FloatXdrCodec : IXdrCodec
{
    /// <summary>   Constructs and initializes a new <see cref="FloatXdrCodec"/> object. </summary>
    /// <param name="value">    Float value. </param>
    public FloatXdrCodec( float value )
    {
        this.Value = value;
    }

    /// <summary>   Constructs and initializes a new <see cref="FloatXdrCodec"/> object. </summary>
    public FloatXdrCodec()
    {
        this.Value = 0;
    }

    /// <summary>   Gets or sets the value of this <see cref="FloatXdrCodec"/> object as a <see cref="float"/> primitive. </summary>
    /// <value> The primitive <see cref="float"/> value of this object. </value>
    public float Value { get; private set; }

    /// <summary>
    /// Encodes -- that is: serializes -- a XDR float into an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="encoder">  XDR stream to which information is sent for encoding. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Encode( XdrEncodingStreamBase encoder )
    {
        encoder.EncodeFloat( this.Value );
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- a XDR float from an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="decoder">  XDR stream from which decoded information is retrieved. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Decode( XdrDecodingStreamBase decoder )
    {
        this.Value = decoder.DecodeFloat();
    }

}