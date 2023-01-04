namespace cc.isr.XDR.Codecs;

/// <summary>
/// Instances of the class <see cref="VoidXdrCodec"/> represent serializable voids, which are
/// especially useful in cases where no result is expected from a remote function call or no
/// parameters are supplied.
/// </summary>
/// <remarks>
/// The XDR data type wrapper classes wrap their value as read only properties, which are set
/// upon construction and decoding. <para>
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public class VoidXdrCodec : IXdrCodec
{
    /// <summary>
    /// Encodes -- that is: serializes -- a void into an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="xdr">  XDR stream to which information is sent for encoding. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Encode( XdrEncodingStreamBase xdr )
    {
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- a void from an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="xdr">  XDR stream from which decoded information is retrieved. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Decode( XdrDecodingStreamBase xdr )
    {
    }

    /// <summary>
    /// (Immutable)
    /// Static <see cref="VoidXdrCodec"/> instance, which can be used in cases where no data is to be
    /// serialized or deserialized but some ONC/RPC function expects a reference to an XDR Codec object.
    /// </summary>
    public static readonly VoidXdrCodec VoidXdrCodecInstance = new();
}
