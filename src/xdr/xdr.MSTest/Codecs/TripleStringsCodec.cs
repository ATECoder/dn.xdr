#nullable disable
namespace cc.isr.XDR.MSTest.Codecs;

/// <summary>   (Serializable) a triple strings XBR encoder/decoder. </summary>
/// <remarks>   2022-12-30. </remarks>
[Serializable]
internal class TripleStringsCodec : IXdrCodec
{
    /// <summary>   Gets or sets the one. </summary>
    /// <value> The one. </value>
    public string One { get; set; }

    /// <summary>   Gets or sets the two. </summary>
    /// <value> The two. </value>
    public string Two { get; set; }

    /// <summary>   Gets or sets the three. </summary>
    /// <value> The three. </value>
    public string Three { get; set; }

    /// <summary>
    /// Encodes -- that is: serializes -- an object into a XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <remarks>
    /// Encodes -- that is: serializes -- an object into a XDR stream in compliance to RFC 1832.
    /// </remarks>
    /// <param name="xdr">  XDR stream to which information is sent for encoding. </param>
    public void Encode( XdrEncodingStreamBase xdr )
    {
        xdr.EncodeString( this.One );
        xdr.EncodeString( this.Two );
        xdr.EncodeString( this.Three );
    }
    /// <summary>
    /// Decodes -- that is: deserializes -- an object from a XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <remarks>
    /// Decodes -- that is: deserializes -- an object from a XDR stream in compliance to RFC 1832.
    /// </remarks>
    /// <param name="xdr">  XDR stream from which decoded information is retrieved. </param>
    public void Decode( XdrDecodingStreamBase xdr )
    {
        this.One = xdr.DecodeString();
        this.Two = xdr.DecodeString();
        this.Three = xdr.DecodeString();
    }
};
