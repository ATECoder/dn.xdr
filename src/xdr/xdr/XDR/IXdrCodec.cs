
namespace cc.isr.XDR;

/// <summary>
/// Defines the interface for all classes serialize and deserialize value(s)
/// into and from XDR streams.
/// </summary>
/// <remarks> <para>
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public interface IXdrCodec
{
    /// <summary>
    /// Encodes -- that is: serializes -- an object into an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    /// <param name="encoder">  XDR stream to which information is sent for encoding. </param>
    void Encode( XdrEncodingStreamBase encoder );

    /// <summary>
    /// Decodes -- that is: deserializes -- an object from an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    /// <param name="decoder">  XDR stream from which decoded information is retrieved. </param>
    void Decode( XdrDecodingStreamBase decoder );
}
